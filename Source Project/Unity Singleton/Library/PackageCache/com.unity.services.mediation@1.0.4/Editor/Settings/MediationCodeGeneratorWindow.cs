using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Services.Mediation.Dashboard.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    /// <summary>
    /// Window allowing to generate sample code with a given Adapter / Ad Unit Id
    /// to help the user implement a basic Ad Display functionality
    /// </summary>
    class MediationCodeGeneratorWindow : EditorWindow
    {
        const string k_CodeGeneratorWindowTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/CodeGenerationWindowTemplate.uxml";

        const string k_SettingsStyle               = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/SettingsStyle.uss";
        const string k_CodeGeneratorStyle          = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/CodeGeneratorStyle.uss";

        const string k_AdFormatBanner = "BANNER";
        const string k_AdFormatInterstitial = "INTERSTITIAL";
        const string k_AdFormatRewarded = "REWARDED";

        string[] m_SupportedAdFormats = new string[]
        {
            k_AdFormatBanner,
            k_AdFormatInterstitial,
            k_AdFormatRewarded
        };

        const string k_GeneratedCodeTemplate  =
@"using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

namespace Unity.Example
{
    public class {adType}Example : IDisposable
    {
        I{adType} ad;
        string adUnitId = ""{adUnitId}"";
        string gameId = ""{gameId}"";

        public async Task InitServices()
        {
            try
            {
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetGameId(gameId);
                await UnityServices.InitializeAsync(initializationOptions);

                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        public void SetupAd()
        {
            {setupCreateAndEvents}
            {rewardedCallback}
            // Impression Event
            MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
        }

        public void Dispose() => ad?.Dispose();

        {showAdFunction}
        void InitializationComplete()
        {
            SetupAd();
            LoadAd();
        }

        async Task LoadAd()
        {
            try
            {
                await ad.LoadAsync();
            }
            catch (LoadFailedException)
            {
                // We will handle the failure in the OnFailedLoad callback
            }
        }

        void InitializationFailed(Exception e)
        {
            Debug.Log(""Initialization Failed: "" + e.Message);
        }

        void AdLoaded(object sender, EventArgs e)
        {
            Debug.Log(""Ad loaded"");
        }

        void AdFailedLoad(object sender, LoadErrorEventArgs e)
        {
            Debug.Log(""Failed to load ad"");
            Debug.Log(e.Message);
        }
        {adShownFunction}
        {adRefreshedFunction}
        {adClosedFunction}
        void AdClicked(object sender, EventArgs e)
        {
            Debug.Log(""Ad has been clicked"");
            // Execute logic after an ad has been clicked.
        }
        {adFailedShowFunction}
        void ImpressionEvent(object sender, ImpressionEventArgs args)
        {
            var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : ""null"";
            Debug.Log(""Impression event from ad unit id "" + args.AdUnitId + "" "" + impressionData);
        }
        {rewardedFunction}
    }
}";

        const string k_SetupAdBannerEventsTemplate =
@"//Create
            ad = MediationService.Instance.Create{adType}(
                adUnitId,
                BannerAdPredefinedSize.Banner.ToBannerAdSize(),
                BannerAdAnchor.TopCenter,
                Vector2.zero);

            //Subscribe to events
            ad.OnRefreshed += AdRefreshed;
            ad.OnClicked += AdClicked;
            ad.OnLoaded += AdLoaded;
            ad.OnFailedLoad += AdFailedLoad;";

        const string k_AdRefreshedTemplate =
@"void AdRefreshed(object sender, LoadErrorEventArgs e)
        {
            Debug.Log(""Refreshed ad"");
            Debug.Log(e.Message);
        }";
        const string k_SetupAdTraditionalEventsTemplate =
@"//Create
            ad = MediationService.Instance.Create{adType}(adUnitId);

            //Subscribe to events
            ad.OnClosed += AdClosed;
            ad.OnClicked += AdClicked;
            ad.OnLoaded += AdLoaded;
            ad.OnFailedLoad += AdFailedLoad;";

        const string k_AdClosedTemplate =
@"void AdClosed(object sender, EventArgs e)
        {
            Debug.Log(""Ad has closed"");
            // Execute logic after an ad has been closed.
        }
";

        const string k_ShowAdTemplate =
@"
        public async void ShowAd()
        {
            if (ad.AdState == AdState.Loaded)
            {
                try
                {
                    {adType}ShowOptions showOptions = new {adType}ShowOptions();
                    showOptions.AutoReload = true;
                    await ad.ShowAsync(showOptions);
                    AdShown();
                }
                catch (ShowFailedException e)
                {
                    AdFailedShow(e);
                }
            }
        }
";
        const string k_AdShownTemplate =
@"
        void AdShown()
        {
            Debug.Log(""Ad shown!"");
        }";

        const string k_AdFailedShowTemplate =
@"
        void AdFailedShow(ShowFailedException e)
        {
            Debug.Log(e.Message);
        }
";
        const string k_OnRewardedTemplate =
@"
        void UserRewarded(object sender, RewardEventArgs e)
        {
            Debug.Log($""Received reward: type:{e.Type}; amount:{e.Amount}"");
        }
";

        const string k_LoadingText = "Loading...";
        const string k_ErrorText = "Error loading window. See console for details.";
        const string k_ErrorTextNoAds = "Error loading window. No Ad units were found.";


#if UNITY_2020_1_OR_NEWER
        const string k_ServiceBaseStyle    = @"StyleSheets/ServicesWindow/ServicesProjectSettingsCommon.uss";
        static readonly string k_SkinStyle = @"StyleSheets/ServicesWindow/ServicesProjectSettings{0}.uss";
#else
        const string k_ServiceBaseStyle    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/BaseStyle.uss";
        static readonly string k_SkinStyle = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/SkinStyle{0}.uss";
#endif

        Dictionary<(string, string), List<(string, string)>> m_AdUnitsPerGameId;
        VisualElement m_GameIdsDropdown;
        VisualElement m_AdUnitsDropdown;
        PopupField<(string, string)> m_GameIdsDropdownContent;
        PopupField<(string, string)> m_AdUnitsDropdownContent;
        TextField     m_CodeGenField;
        private Label m_CodeGenLineNumbers;

        [MenuItem("Services/" + MediationServiceIdentifier.k_PackageDisplayName + "/Code Generator", priority = 112)]
        public static void ShowWindow()
        {
            EditorGameServiceAnalyticsSender.SendTopMenuCodeGeneratorEvent();
            GetWindow<MediationCodeGeneratorWindow>($"{MediationServiceIdentifier.k_PackageDisplayName} - Code Generator", new Type[] { typeof(MediationAdUnitsWindow), typeof(SceneView), typeof(EditorWindow)});
        }

        void OnFocus()
        {
            // Refresh the window anytime it becomes in focus, fetching the latest ad units if the window was previously
            // loaded or loading the window normally otherwise.
            if (string.IsNullOrEmpty(m_CodeGenField?.value))
            {
                RefreshWindow();
            }
            else
            {
                PopulateGameIds();
            }
        }

        void RefreshWindow()
        {
            var skinTheme = EditorGUIUtility.isProSkin ? "Dark" : "Light";

            var styleSheets = new string[]
            {
                k_CodeGeneratorStyle,
                k_SettingsStyle,
                k_ServiceBaseStyle,
                string.Format(k_SkinStyle, skinTheme)
            };

            rootVisualElement.Clear();

            VisualTreeAsset codeGeneratorWindowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_CodeGeneratorWindowTemplate);
            codeGeneratorWindowTemplate.CloneTree(rootVisualElement);

            foreach (var styleSheet in styleSheets)
            {
                var sheet = EditorGUIUtility.Load(styleSheet) as StyleSheet;
                rootVisualElement.styleSheets.Add(sheet);
            }

            m_CodeGenLineNumbers = rootVisualElement.Q<Label>("codeGenLineNumbers");
            m_CodeGenLineNumbers.text = string.Empty;

            m_CodeGenField = rootVisualElement.Q<TextField>("codeGenField");
            m_CodeGenField.isReadOnly = true;
            m_CodeGenField.value = k_LoadingText;

            rootVisualElement.Q<Button>("copyToClipboard").clickable.clicked += CopyToClipboard;
            rootVisualElement.Q<Button>("goToAdUnits").clickable.clicked += GoToAdUnits;

            PopulateGameIds();
        }

        void CopyToClipboard()
        {
            EditorGameServiceAnalyticsSender.SendCodeGeneratorCopySnippetEvent();
            GUIUtility.systemCopyBuffer = m_CodeGenField.text;
            Debug.Log("Code Snippet copied to clipboard.");
        }

        void GoToAdUnits()
        {
            MediationAdUnitsWindow.ShowWindow();
        }

        void PopulateGameIds()
        {
            DashboardClient.GetAdUnitsAsync(adUnits =>
            {
                m_CodeGenLineNumbers.text = string.Empty;
                if (adUnits == null)
                {
                    // Project is not linked, no internet connexion or other issue; we could not retrieve the ad units
                    m_CodeGenField.value = k_ErrorText;
                    return;
                }

                if (adUnits.Length == 0)
                {
                    m_CodeGenField.value = k_ErrorTextNoAds;
                    return;
                }

                m_AdUnitsPerGameId = new Dictionary<(string, string), List<(string, string)>>();

                foreach (var adUnit in adUnits)
                {
                    //Do not consider archived ad units
                    if (adUnit.IsArchived) continue;

                    if (!m_SupportedAdFormats.Contains(adUnit.adFormat)) continue;

                    if (!m_AdUnitsPerGameId.ContainsKey((adUnit.Platform, adUnit.AppId)))
                    {
                        m_AdUnitsPerGameId.Add((adUnit.Platform, adUnit.AppId), new List<(string, string)>());
                    }

                    m_AdUnitsPerGameId[(adUnit.Platform, adUnit.AppId)].Add((adUnit.AdFormat, adUnit.AdUnitId));
                }

                foreach (var adUnitsList in m_AdUnitsPerGameId.Values)
                {
                    adUnitsList.Sort();
                }


                m_AdUnitsDropdown = rootVisualElement.Q<VisualElement>("adUnitDropdown");

                var selectedGameId = m_GameIdsDropdownContent?.value ?? ("", "");

                m_GameIdsDropdownContent = new PopupField<(string, string)>(m_AdUnitsPerGameId.Keys.ToList(), 0);
                m_GameIdsDropdown = rootVisualElement.Q<VisualElement>("gameIdDropdown");
                m_GameIdsDropdown.Clear();
                m_GameIdsDropdown.Add(m_GameIdsDropdownContent);
                m_GameIdsDropdown.RegisterCallback<ChangeEvent<(string, string)>>(evt =>
                {
                    PopulateAdUnits(evt.newValue);
                });

                if (m_AdUnitsPerGameId.Keys.Contains(selectedGameId))
                {
                    m_GameIdsDropdownContent.SetValueWithoutNotify(selectedGameId);
                }

                PopulateAdUnits(m_GameIdsDropdownContent.value);
            });
        }

        void PopulateAdUnits((string, string) selectedGameId)
        {
            var selectedAdUnit = m_AdUnitsDropdownContent?.value ?? ("", "");

            m_AdUnitsDropdownContent = new PopupField<(string, string)>(m_AdUnitsPerGameId[selectedGameId].ToList(), 0);
            m_AdUnitsDropdown.Clear();
            m_AdUnitsDropdown.Add(m_AdUnitsDropdownContent);
            m_AdUnitsDropdown.RegisterCallback<ChangeEvent<(string, string)>>(evt =>
            {
                RefreshCode();
            });

            if (m_AdUnitsPerGameId[selectedGameId].Contains(selectedAdUnit))
            {
                m_AdUnitsDropdownContent.SetValueWithoutNotify(selectedAdUnit);
            }

            RefreshCode();
        }

        void RefreshCode()
        {
            var gameId = m_GameIdsDropdownContent.value.Item2;
            var adUnitId = m_AdUnitsDropdownContent.value.Item2;

            var formattedAdType = m_AdUnitsDropdownContent.value.Item1[0] +
                m_AdUnitsDropdownContent.value.Item1.Substring(1).ToLower()
                + "Ad";

            var setupCreateAndEvents = "";
            var adRefreshedFunction = "";
            var adClosedFunction = "";
            var showAdFunction = "";
            var adShownFunction = "";
            var adFailedShowFunction = "";
            var rewardedCallback = "";
            var rewardedFunction = "";

            if (formattedAdType == "BannerAd")
            {
                setupCreateAndEvents = k_SetupAdBannerEventsTemplate;
                adRefreshedFunction = k_AdRefreshedTemplate;
            }
            else
            {
                if (formattedAdType == "RewardedAd")
                {
                    rewardedCallback = "ad.OnUserRewarded += UserRewarded;\n";
                    rewardedFunction = k_OnRewardedTemplate;
                }

                setupCreateAndEvents = k_SetupAdTraditionalEventsTemplate;
                adClosedFunction = k_AdClosedTemplate;
                showAdFunction = k_ShowAdTemplate;
                adShownFunction = k_AdShownTemplate;
                adFailedShowFunction = k_AdFailedShowTemplate;
            }


            var codeString = k_GeneratedCodeTemplate
                .Replace("{gameId}", gameId)
                .Replace("{adUnitId}", adUnitId)
                .Replace("{setupCreateAndEvents}", setupCreateAndEvents)
                .Replace("{showAdFunction}", showAdFunction)
                .Replace("{adShownFunction}", adShownFunction)
                .Replace("{adRefreshedFunction}", adRefreshedFunction)
                .Replace("{adClosedFunction}", adClosedFunction)
                .Replace("{adFailedShowFunction}", adFailedShowFunction)
                .Replace("{rewardedCallback}", rewardedCallback)
                .Replace("{rewardedFunction}", rewardedFunction)
                .Replace("{adType}", formattedAdType);

            m_CodeGenField.value = codeString;

            var lineCount = codeString.Count(x => x == '\n') + 1;
            StringBuilder lineNumbersTextBuilder = new StringBuilder("1");
            for (int i = 2; i <= lineCount; i++)
            {
                lineNumbersTextBuilder.Append($"\n {i}");
            }

            m_CodeGenLineNumbers.text = lineNumbersTextBuilder.ToString();
        }
    }
}
