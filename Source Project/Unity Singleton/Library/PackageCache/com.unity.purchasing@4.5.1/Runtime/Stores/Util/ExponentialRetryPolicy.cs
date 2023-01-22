#nullable enable

using System;
using System.Threading.Tasks;
using UnityEngine.Purchasing.Stores.Util;

namespace UnityEngine.Purchasing
{
    class ExponentialRetryPolicy : IRetryPolicy
    {
        readonly int m_BaseRetryDelay;
        readonly int m_MaxRetryDelay;
        readonly int m_ExponentialFactor;

        public ExponentialRetryPolicy(int baseRetryDelay = 1000, int maxRetryDelay = 30 * 1000, int exponentialFactor = 2)
        {
            m_BaseRetryDelay = baseRetryDelay;
            m_MaxRetryDelay = maxRetryDelay;
            m_ExponentialFactor = exponentialFactor;
        }

        public void Invoke(Action<Action> actionToTry, Action? onRetryAction)
        {
            var currentRetryDelay = m_BaseRetryDelay;
            actionToTry(Retry);

            async void Retry()
            {
                onRetryAction?.Invoke();
                await WaitAndRetry();
            }

            async Task WaitAndRetry()
            {
                await Task.Delay(currentRetryDelay);
                currentRetryDelay = AdjustDelay(currentRetryDelay);
                actionToTry(Retry);
            }
        }

        int AdjustDelay(int delay)
        {
            return Math.Min(m_MaxRetryDelay, delay * m_ExponentialFactor);
        }
    }
}
