#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidImpressionEventPublisher : IImpressionEventPublisher, IAndroidImpressionListener, IDisposable
    {
        public event EventHandler<ImpressionEventArgs> OnImpression;

        AndroidJavaClass m_ImpressionEventPublisher;
        AndroidImpressionListener m_ImpressionListener;
        volatile bool m_Disposed;

        public AndroidImpressionEventPublisher()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_ImpressionEventPublisher = new AndroidJavaClass("com.unity3d.mediation.ImpressionEventPublisher");
                    m_ImpressionListener = new AndroidImpressionListener(this);
                    m_ImpressionEventPublisher.CallStatic("subscribe", m_ImpressionListener);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while loading ImpressionEventPublisher. ImpressionEventPublisher will not initialize. " +
                        "Please check your build settings, and make sure Mediation SDK is integrated properly.");
                    Debug.LogException(e);
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
                    try
                    {
                        m_ImpressionEventPublisher?.CallStatic("unsubscribe", m_ImpressionListener);
                        m_ImpressionEventPublisher?.Dispose();
                        m_ImpressionEventPublisher = null;
                        m_ImpressionListener = null;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidImpressionEventPublisher()
        {
            Dispose(false);
        }

        public void onImpression(string adUnitId, AndroidJavaObject impressionData)
        {
            ImpressionData impressData = null;
            if (impressionData != null)
            {
                impressData = new ImpressionData
                {
                    Timestamp = impressionData.Call<string>("getTimestamp"),
                    AdUnitName = impressionData.Call<string>("getAdUnitName"),
                    AdUnitId = impressionData.Call<string>("getAdUnitId"),
                    AdUnitFormat = impressionData.Call<string>("getAdUnitFormat"),
                    ImpressionId = impressionData.Call<string>("getImpressionId"),
                    Currency = impressionData.Call<string>("getCurrency"),
                    RevenueAccuracy = impressionData.Call<string>("getRevenueAccuracy"),
                    PublisherRevenuePerImpression = impressionData.Call<double>("getPublisherRevenuePerImpression"),
                    PublisherRevenuePerImpressionInMicros = impressionData.Call<Int64>("getPublisherRevenuePerImpressionInMicros"),
                    AdSourceName = impressionData.Call<string>("getAdSourceName"),
                    AdSourceInstance = impressionData.Call<string>("getAdSourceInstance"),
                    AppVersion = impressionData.Call<string>("getAppVersion"),
                    LineItemId = impressionData.Call<string>("getLineItemId"),
                    LineItemName = impressionData.Call<string>("getLineItemName"),
                    LineItemPriority = impressionData.Call<string>("getLineItemPriority"),
                    Country = impressionData.Call<string>("getCountry"),
                };
            }

            OnImpression?.Invoke(null, new ImpressionEventArgs(adUnitId, impressData));
        }
    }
}
#endif
