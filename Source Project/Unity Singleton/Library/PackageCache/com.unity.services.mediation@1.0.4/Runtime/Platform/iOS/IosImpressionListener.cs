#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosImpressionListener : IosNativeObject
    {
        public IosImpressionListener() : base(false)
        {
            NativePtr = ImpressionDelegateCreate(k_ImpressionCallback);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            ImpressionDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        ~IosImpressionListener()
        {
            Dispose();
        }

        delegate void ImpressionCallback(string adUnitId, IntPtr iosImpressionDataPtr);

        static readonly ImpressionCallback k_ImpressionCallback = Impression;

        [DllImport("__Internal", EntryPoint = "UMSPImpressionDelegateCreate")]
        static extern IntPtr ImpressionDelegateCreate(ImpressionCallback impressionCallback);

        [DllImport("__Internal", EntryPoint = "UMSPImpressionDelegateDestroy")]
        static extern void ImpressionDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(ImpressionCallback))]
        static void Impression(string adUnitId, IntPtr iosImpressionDataPtr)
        {
            if (IosImpressionEventPublisher.instance == null) return;
            ImpressionData impressionData = null;
            if (iosImpressionDataPtr != IntPtr.Zero)
            {
                var iosImpressionData = Marshal.PtrToStructure<IosImpressionData>(iosImpressionDataPtr);
                impressionData = iosImpressionData.ToImpressionData();
            }
            var args = new ImpressionEventArgs(adUnitId, impressionData);
            IosImpressionEventPublisher.instance.InvokeImpressionEvent(args);
        }
    }
}
#endif
