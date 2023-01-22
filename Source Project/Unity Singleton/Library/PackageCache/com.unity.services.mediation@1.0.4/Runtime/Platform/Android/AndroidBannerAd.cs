#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidBannerAd : IPlatformBannerAd, IAndroidBannerAdListener
    {
        const string k_BannerSizeClassName = "com.unity3d.mediation.BannerAdViewSize";
        const string k_BannerAdClassName   = "com.unity3d.mediation.BannerAd";

        const string k_FuncAdState     = "getAdState";
        const string k_FuncAdUnitId    = "getAdUnitId";
        const string k_FuncSetPosition = "setPosition";
        const string k_FuncLoad        = "load";
        const string k_FuncDestroy     = "destroy";

        const string k_ErrorCallingAdState     = "Cannot call AdState";
        const string k_ErrorCallingAdUnitId    = "Cannot call AdUnitId";
        const string k_ErrorCallingSetPosition = "Cannot call SetPosition()";
        const string k_ErrorCallingLoad        = "Cannot call Load()";
        const string k_ErrorFailedToLoad       = "Failed to load - ";
        const string k_ErrorCreatingBanner     = "Error while creating Banner Ad. Banner Ad will not load. Please check your build settings, and make sure Mediation SDK is integrated properly.";
        const string k_ErrorDisposed           = "Unity Mediation SDK: {0}: Instance of type {1} is disposed. Please create a new instance in order to call any method.";

        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnClicked;
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;

        public AdState AdState
        {
            get
            {
                var stateVal = AdState.Unloaded;
                if (!CheckDisposedAndLogError(k_ErrorCallingAdState))
                {
                    try
                    {
                        using (var state = m_BannerAd.Call<AndroidJavaObject>(k_FuncAdState))
                        {
                            stateVal =  state.ToEnum<AdState>();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return stateVal;
            }
        }

        public string AdUnitId
        {
            get
            {
                string adUnitId = null;
                if (!CheckDisposedAndLogError(k_ErrorCallingAdUnitId))
                {
                    try
                    {
                        adUnitId = m_BannerAd.Call<string>(k_FuncAdUnitId);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return adUnitId;
            }
        }

        public BannerAdSize Size => m_BannerAdSize;

        AndroidJavaObject m_BannerAd;
        BannerAdSize m_BannerAdSize;
        IAndroidBannerAdListener m_BannerAdListener;

        volatile bool m_Disposed;

        public AndroidBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2())
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    if (m_BannerAdListener == null)
                    {
                        m_BannerAdListener = new AndroidBannerAdListener(this);
                    }

                    m_BannerAdSize = size;

                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        AndroidJavaObject androidSize = new AndroidJavaObject(k_BannerSizeClassName, size.DpWidth, size.DpHeight);

                        m_BannerAd = new AndroidJavaObject(k_BannerAdClassName,
                            activity, adUnitId, androidSize, m_BannerAdListener);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(k_ErrorCreatingBanner);
                    Debug.LogException(e);
                }

                SetPosition(anchor, positionOffset);
            });
        }

        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2())
        {
            if (!CheckDisposedAndLogError(k_ErrorCallingSetPosition))
            {
                ThreadUtil.Post(state =>
                {
                    try
                    {
                        m_BannerAd.Call(k_FuncSetPosition, Convert.ToInt32(anchor), (int)positionOffset.x, (int)positionOffset.y);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            }
        }

        public void Load()
        {
            if (!CheckDisposedAndLogError(k_ErrorCallingLoad))
            {
                ThreadUtil.Post(state =>
                {
                    try
                    {
                        m_BannerAd.Call(k_FuncLoad);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, k_ErrorFailedToLoad + e.Message));
                    }
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void onBannerAdViewLoaded(AndroidJavaObject bannerAd)
        {
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void onBannerAdViewFailedLoad(AndroidJavaObject bannerAd, AndroidJavaObject error, string msg)
        {
            OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg));
        }

        public void onBannerAdViewClicked(AndroidJavaObject bannerAd)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void onBannerAdViewRefreshed(AndroidJavaObject bannerAd, AndroidJavaObject error, string msg)
        {
            LoadErrorEventArgs args = null;
            if (error != null)
            {
                args = new LoadErrorEventArgs(error.ToEnum<LoadError>(), msg);
            }
            OnRefreshed?.Invoke(this, args);
        }

        ~AndroidBannerAd()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                m_Disposed = true;
                if (disposing)
                {
                    //AndroidJavaObjects are created and destroyed with JNI's NewGlobalRef and DeleteGlobalRef,
                    //Therefore must be used on the same attached thread. In this case, it's Unity thread.
                    ThreadUtil.Post(state =>
                    {
                        m_BannerAd?.Call(k_FuncDestroy);
                        m_BannerAd?.Dispose();
                        m_BannerAdListener = null;
                        m_BannerAd = null;
                    });
                }
            }
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (m_Disposed)
            {
                Debug.LogErrorFormat(k_ErrorDisposed, message, GetType().FullName);
            }
            return m_Disposed;
        }
    }
}
#endif
