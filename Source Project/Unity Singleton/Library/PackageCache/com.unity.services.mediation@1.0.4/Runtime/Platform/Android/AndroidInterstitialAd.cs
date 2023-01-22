#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidInterstitialAd : IPlatformInterstitialAd, IAndroidInterstitialLoadListener, IAndroidInterstitialShowListener
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdState")) return AdState.Unloaded;
                try
                {
                    using (var state = m_InterstitialAd.Call<AndroidJavaObject>("getAdState"))
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

        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdUnitId")) return null;
                try
                {
                    return m_InterstitialAd.Call<string>("getAdUnitId");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }
        }

        AndroidJavaObject m_InterstitialAd;
        AndroidInterstitialAdLoadListener m_InterstitialAdLoadListener;
        AndroidInterstitialAdShowListener m_InterstitialAdShowListener;
        volatile bool m_Disposed;

        public AndroidInterstitialAd(string adUnitId)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        m_InterstitialAd = new AndroidJavaObject("com.unity3d.mediation.InterstitialAd",
                            activity, adUnitId);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while creating Interstitial Ad. Interstitial Ad will not load. " +
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
                    if (m_InterstitialAdLoadListener == null)
                    {
                        m_InterstitialAdLoadListener = new AndroidInterstitialAdLoadListener(this);
                    }
                    m_InterstitialAd.Call("load", m_InterstitialAdLoadListener);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, "Failed to load - " + e.Message));
                }
            });
        }

        public void Show()
        {
            if (CheckDisposedAndLogError("Cannot call Show()")) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    if (m_InterstitialAdShowListener == null)
                    {
                        m_InterstitialAdShowListener = new AndroidInterstitialAdShowListener(this);
                    }
                    m_InterstitialAd.Call("show", m_InterstitialAdShowListener);
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
                    m_InterstitialAd?.Dispose();
                    m_InterstitialAdLoadListener = null;
                    m_InterstitialAdShowListener = null;
                    m_InterstitialAd = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidInterstitialAd()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            Debug.LogErrorFormat("Unity Mediation SDK: {0}: Instance of type {1} is disposed. Please create a new instance in order to call any method.", message, GetType().FullName);
            return true;
        }

        public void onInterstitialLoaded(AndroidJavaObject interstitialAd)
        {
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialFailedLoad(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg));
        }

        public void onInterstitialShowed(AndroidJavaObject interstitialAd)
        {
            OnShowed?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialClicked(AndroidJavaObject interstitialAd)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialClosed(AndroidJavaObject interstitialAd)
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        public void onInterstitialFailedShow(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            OnFailedShow?.Invoke(this, new ShowErrorEventArgs(error.ToEnum<ShowError>(), msg));
        }
    }
}
#endif
