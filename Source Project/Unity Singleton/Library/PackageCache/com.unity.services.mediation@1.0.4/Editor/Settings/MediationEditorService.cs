using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;
using Unity.Services.Mediation.Dashboard.Editor;
using System.Collections.Generic;
using UnityEditor.Advertisements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MediationEditorService : IEditorGameService
    {
        public string Name => Identifier.GetKey();

        const string k_DashboardUrl = @"https://dashboard.unity3d.com/organizations/{0}/projects/{1}/monetization/placements";

        public IEditorGameServiceIdentifier Identifier { get; } = new MediationServiceIdentifier();
        public bool RequiresCoppaCompliance => true;
        public bool HasDashboard => true;

        static bool s_RefreshingGameId = false;

        public string GetFormattedDashboardUrl()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            return string.Format(k_DashboardUrl, CloudProjectSettings.organizationKey, CloudProjectSettings.projectId);
#else
            var orgID = Core.Editor.OrganizationHandler.OrganizationProvider.Organization.Key;
            if (string.IsNullOrWhiteSpace(orgID))
            {
                return null;
            }
            else
            {
                return string.Format(k_DashboardUrl, orgID, CloudProjectSettings.projectId);
            }
#endif
        }

        internal static void RefreshGameId()
        {
            if (!s_RefreshingGameId)
            {
                s_RefreshingGameId = true;
                DashboardClient.GetGameIdAsyncOrWait(OnGameIdRetrieved);
            }
        }

        static void OnGameIdRetrieved(Dictionary<string, string> gameIdPerPlatform)
        {
            s_RefreshingGameId = false;
            if (gameIdPerPlatform == null)
            {
                Debug.LogWarning("Warning, failed to retrieve game id from Dashboard");
                return;
            }

            string gameId;
            if (gameIdPerPlatform.TryGetValue("IOS", out gameId))
            {
                AdvertisementSettings.SetGameId(RuntimePlatform.IPhonePlayer, gameId);
            }

            if (gameIdPerPlatform.TryGetValue("ANDROID", out gameId))
            {
                AdvertisementSettings.SetGameId(RuntimePlatform.Android, gameId);
            }
        }

        public IEditorGameServiceEnabler Enabler { get; } = null;
    }
}
