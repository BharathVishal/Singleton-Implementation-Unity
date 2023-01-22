using System;
using System.Collections.Generic;
using UnityEditor;

namespace Unity.Services.Mediation.Dashboard.Editor
{
    class DashboardPoller
    {
        public bool FinishedPolling { get; private set; }

        readonly double PollingInterval;
        readonly int    PollingMaxAttempts;

        int    PollingAttempts;
        double LastPollTime;
        List<Action<bool>> Callbacks;
        Func<bool> PollingFunction;

        internal DashboardPoller(Func<bool> pollingFunction, double interval = 1, int attempts = 5)
        {
            EditorApplication.update += Update;

            PollingFunction = pollingFunction;
            PollingMaxAttempts = attempts;
            PollingInterval = interval;

            Callbacks = new List<Action<bool>>();
            LastPollTime = EditorApplication.timeSinceStartup;
        }

        void Update()
        {
            if (EditorApplication.timeSinceStartup - LastPollTime > PollingInterval)
            {
                bool result = PollingFunction.Invoke();

                if (result)
                {
                    Callbacks.ForEach(action => action?.Invoke(true));
                    Callbacks.Clear();
                    EditorApplication.update -= Update;
                    FinishedPolling = true;
                }
                else if (PollingAttempts > PollingMaxAttempts)
                {
                    Callbacks.ForEach(action => action?.Invoke(false));
                    Callbacks.Clear();
                    EditorApplication.update -= Update;
                    FinishedPolling = true;
                }

                PollingAttempts++;
                LastPollTime = EditorApplication.timeSinceStartup;
            }
        }

        public void AddCallback(Action<bool> callback)
        {
            Callbacks.Add(callback);
        }
    }
}
