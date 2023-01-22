using System;
using Unity.Services.Core.Editor;
using UnityEngine;
using UnityEditor;

namespace Unity.Services.Mediation.Settings.Editor
{
    class EditorGameServiceAnalyticsSender
    {
        static class AnalyticsComponent
        {
            public const string TopMenu = "Top Menu";
            public const string ProjectSettings = "Project Settings";
            public const string CodeGenerator = "Code Generator";
            public const string AdUnits = "Ad Units";
        }

        static class AnalyticsAction
        {
            public const string Configure = "Configure";
            public const string AdUnits = "Ad Units";
            public const string CodeGenerator = "Code Generator";
            public const string InstallAdapter = "Install Adapter";
            public const string UninstallAdapter = "Uninstall Adapter";
            public const string CopySnippet = "Copy Snippet";
            public const string Sort = "Sort";
        }

        const int k_Version = 1;
        const string k_EventName = "editorgameserviceeditor";

        static IEditorGameServiceIdentifier s_Identifier;

        static IEditorGameServiceIdentifier Identifier
        {
            get
            {
                if (s_Identifier == null)
                {
                    s_Identifier = EditorGameServiceRegistry.Instance.GetEditorGameService<MediationServiceIdentifier>().Identifier;
                }

                return s_Identifier;
            }
        }


        static void SendEvent(string component, string action)
        {
            EditorAnalytics.SendEventWithLimit(k_EventName, new EditorGameServiceEvent
            {
                action = action,
                component = component,
                package = Identifier.GetKey()
            }, k_Version);
        }

        internal static void SendTopMenuConfigureEvent()
        {
            SendEvent(AnalyticsComponent.TopMenu, AnalyticsAction.Configure);
        }

        internal static void SendTopMenuAdUnitsEvent()
        {
            SendEvent(AnalyticsComponent.TopMenu, AnalyticsAction.AdUnits);
        }

        internal static void SendTopMenuCodeGeneratorEvent()
        {
            SendEvent(AnalyticsComponent.TopMenu, AnalyticsAction.CodeGenerator);
        }

        internal static void SendProjectSettingsAdapterInstallEvent(string network)
        {
            SendEvent(AnalyticsComponent.ProjectSettings, AnalyticsAction.InstallAdapter + "-" + network);
        }

        internal static void SendProjectSettingsAdapterUninstallEvent(string network)
        {
            SendEvent(AnalyticsComponent.ProjectSettings, AnalyticsAction.UninstallAdapter + "-" + network);
        }

        internal static void SendCodeGeneratorCopySnippetEvent()
        {
            SendEvent(AnalyticsComponent.CodeGenerator, AnalyticsAction.CopySnippet);
        }

        internal static void SendAdUnitsSortEvent()
        {
            SendEvent(AnalyticsComponent.AdUnits, AnalyticsAction.Sort);
        }

        /// <remarks>Lowercase is used here for compatibility with analytics.</remarks>
        [Serializable]
        internal struct EditorGameServiceEvent
        {
            public string action;
            public string component;
            public string package;
        }
    }
}
