#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidRewardedAd : IPlatformRewardedAd, IAndroidRewardedLoadListener, IAndroidRewardedShowListener
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        /// <summary>
        /// Retrieves AdState from the Underlying Android SDK
        /// </summary>
        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdState")) return AdState.Unloaded;
                try
                {
                    using (var state = m_RewardedAd.Call<AndroidJavaObject>("getAdState"))
                    {
                        return state.ToEnum<AdState>();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return AdState.Unloaded;
                }
            }
        }

        /// <summary>
        /// Retrieves Ad Unit Id from the Underlying Android SDK
        /// </summary>
        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdUnitId")) return null;
                try
                {
                    return m_RewardedAd.Call<string>("getAdUnitId");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }
        }

        AndroidJavaObject m_RewardedAd;
        AndroidRewardedAdLoadListener m_RewardedAdLoadListener;
        AndroidRewardedAdShowListener m_RewardedAdShowListener;
        volatile bool m_Disposed;

        public AndroidRewardedAd(string adUnitId)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        m_RewardedAd = new AndroidJavaObject("com.unity3d.mediation.RewardedAd",
                            activity, adUnitId);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while creating Rewarded Ad. Rewarded Ad will not load. " +
                        "Please check your build settings, and make sure Mediation SDK is integrated properly.");
                    Debug.LogException(e);
                }
            });
        }

        public void Load()
        {
            if (CheckDisposedAndLogError("Cannot call Load()")) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_RewardedAdLoadListener == null)
                    {
                        m_RewardedAdLoadListener = new AndroidRewardedAdLoadListener(this);
                    }
                    m_RewardedAd.Call("load", m_RewardedAdLoadListener);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, "Failed to load - " + e.Message));
                }
            });
        }

        public void Show(RewardedAdShowOptions showOptions = null)
        {
            if (CheckDisposedAndLogError("Cannot call Show()")) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_RewardedAdShowListener == null)
                    {
                        m_RewardedAdShowListener = new AndroidRewardedAdShowListener(this);
                    }

                    AndroidJavaObject showOptionsJava = null;
                    if (showOptions != null && !string.IsNullOrEmpty(showOptions.S2SData.UserId))
                    {
                        showOptionsJava = new AndroidJavaObject("com.unity3d.mediation.RewardedAdShowOptions");
                        AndroidJavaObject s2sData = new AndroidJavaObject("com.unity3d.mediation.RewardedAdShowOptions$S2SRedeemData", showOptions.S2SData.UserId, showOptions.S2SData.CustomData);
                        showOptionsJava.Call("setS2SRedeemData", s2sData);
                    }

                    m_RewardedAd.Call("show", m_RewardedAdShowListener, showOptionsJava);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    OnFailedShow?.Invoke(this, new ShowErrorEventArgs(ShowError.Unknown, "Failed to show - " + e.Message));
                }
            });
        }

        void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                //AndroidJavaObjects are created and destroyed with JNI's NewGlobalRef and DeleteGlobalRef,
                //Therefore must be used on the same attached thread. In this case, it's Unity thread.
                ThreadUtil.Post(state =>
                {
                    m_RewardedAd?.Dispose();
                    m_RewardedAdLoadListener = null;
                    m_RewardedAdShowListener = null;
                    m_RewardedAd = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidRewardedAd()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            Debug.LogErrorFormat("Unity Mediation SDK: {0}: Instance of type {1} is disposed. Please create a new instance in order to call any method.", message, GetType().FullName);
            return true;
        }

        public void onRewardedLoaded(AndroidJavaObject rewardedAd)
        {
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedFailedLoad(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg));
        }

        public void onRewardedShowed(AndroidJavaObject rewardedAd)
        {
            OnShowed?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedClicked(AndroidJavaObject rewardedAd)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedClosed(AndroidJavaObject rewardedAd)
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        public void onRewardedFailedShow(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            OnFailedShow?.Invoke(this, new ShowErrorEventArgs(error.ToEnum<ShowError>(), msg));
        }

        public void onUserRewarded(AndroidJavaObject rewardedAd, AndroidJavaObject reward)
        {
            var type = reward.Call<string>("getType");
            var amount = reward.Call<string>("getAmount");
            OnUserRewarded?.Invoke(this, new RewardEventArgs(type, amount));
        }
    }
}
#endif
