#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidImpressionListener : AndroidJavaProxy, IAndroidImpressionListener
    {
        IAndroidImpressionListener m_Listener;
        public AndroidImpressionListener(IAndroidImpressionListener listener) : base("com.unity3d.mediation.IImpressionListener")
        {
            m_Listener = listener;
        }

        [UnityEngine.Scripting.Preserve]
        public void onImpression(string adUnitId, AndroidJavaObject impressionData)
        {
            ThreadUtil.Post(state => m_Listener.onImpression(adUnitId, impressionData));
        }
    }
}
#endif
