#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation.Platform
{
    class IosImpressionEventPublisher : IDisposable, IImpressionEventPublisher
    {
        internal static IosImpressionEventPublisher instance;
        public event EventHandler<ImpressionEventArgs> OnImpression;

        IosImpressionListener m_ImpressionListener;
        public IosImpressionEventPublisher()
        {
            instance = this;
            m_ImpressionListener = new IosImpressionListener();
            ImpressionEventPublisherSubscribe(m_ImpressionListener.NativePtr);
        }

        public void Dispose()
        {
            if (m_ImpressionListener != null)
            {
                ImpressionEventPublisherUnsubscribe(m_ImpressionListener.NativePtr);
                m_ImpressionListener.Dispose();
                m_ImpressionListener = null;
            }

            if (this == instance)
            {
                instance = null;
            }
        }

        ~IosImpressionEventPublisher()
        {
            Dispose();
        }

        internal void InvokeImpressionEvent(ImpressionEventArgs args)
        {
            ThreadUtil.Post(state => instance?.OnImpression?.Invoke(null, args));
        }

        [DllImport("__Internal", EntryPoint = "UMSPImpressionEventPublisherSubscribe")]
        static extern void ImpressionEventPublisherSubscribe(IntPtr listener);

        [DllImport("__Internal", EntryPoint = "UMSPImpressionEventPublisherUnsubscribe")]
        static extern void ImpressionEventPublisherUnsubscribe(IntPtr listener);
    }
}
#endif
