using System;
using System.Collections.Generic;
using Unity.Services.Mediation.Dashboard.Editor;
using Unity.Services.Mediation.Settings.Editor.Layout;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    /// <summary>
    /// Window listing the available Ad Units defined in the dashboard and their infos for the user's convenience
    /// </summary>
    class MediationAdUnitsWindow : EditorWindow
    {
        public static List<AdUnitData> AdUnitData => FilteredAdUnitData ?? AdUnitDataSource;

        static List<AdUnitData> AdUnitDataSource = new List<AdUnitData>();
        static List<AdUnitData> FilteredAdUnitData;

        static SortMode[] SortModes = {SortMode.Ascending, SortMode.Ascending, SortMode.Ascending, SortMode.Ascending};

        const string k_AdUnitsTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsTemplate.uxml";
        const string k_AdUnitsStyle    = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsStyle.uss";

        const string k_AdUnitsWarningTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsWarningTemplate.uxml";
        const string k_AdUnitsErrorTemplate   = @"Packages/com.unity.services.mediation/Editor/Settings/Layout/AdUnitsErrorTemplate.uxml";

        [MenuItem("Services/" + MediationServiceIdentifier.k_PackageDisplayName + "/Ad Units", priority = 111)]
        public static void ShowWindow()
        {
            EditorGameServiceAnalyticsSender.SendTopMenuAdUnitsEvent();
            GetWindow<MediationAdUnitsWindow>($"{MediationServiceIdentifier.k_PackageDisplayName} - Ad Units", new Type[] { typeof(MediationCodeGeneratorWindow), typeof(SceneView), typeof(EditorWindow)});
        }

        void OnFocus()
        {
            if (rootVisualElement.Q(className: "list-view") != null)
            {
                RetrieveAdUnitInfo(rootVisualElement);
            }
            else
            {
                RefreshWindow();
            }
        }

        void RefreshWindow()
        {
            rootVisualElement.Clear();

            AdUnitVisualElement.Initialize();

            VisualElement root = rootVisualElement;
            AddStyleSheets(root);

            VisualTreeAsset adUnitTemplateAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsTemplate);
            adUnitTemplateAsset.CloneTree(root);

            RetrieveAdUnitInfo(root);
        }

        void RetrieveAdUnitInfo(VisualElement root)
        {
            DashboardClient.GetAdUnitsAsync(adUnits =>
            {
                // Remove Loading Element
                root.Q<Label>("loading")?.RemoveFromHierarchy();

                // Can't fetch ad unit data, display error message box
                if (adUnits == null)
                {
                    root.Q(className: "list-view")?.RemoveFromHierarchy();

                    VisualTreeAsset errorTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsErrorTemplate);
                    errorTemplate.CloneTree(root.Q(className: "table-box"));
                }
                else if (adUnits.Length == 0)
                {
                    root.Q(className: "list-view")?.RemoveFromHierarchy();

                    VisualTreeAsset warningTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdUnitsWarningTemplate);
                    warningTemplate.CloneTree(root.Q(className: "table-box"));
                }
                else
                {
                    // Received data construct ad units list.
                    List<AdUnitData> dashboardAdUnits = new List<AdUnitData>();
                    foreach (DashboardClient.AdUnitInfoJson adUnitInfo in adUnits)
                    {
                        if (!adUnitInfo.isArchived)
                        {
                            dashboardAdUnits.Add(new AdUnitData(adUnitInfo));
                        }
                    }
                    AdUnitDataSource = dashboardAdUnits;
                    ConstructListFromAdUnits(root);
                }
            });
        }

        static SortMode GetNextSortMode(SortMode sortMode)
        {
            return (SortMode)(((int)sortMode + 1) % 2);
        }

        static EventCallback<ChangeEvent<string>> FilterListBySearchField(ListView listView)
        {
            return evt =>
            {
                if (!string.IsNullOrEmpty(evt?.newValue))
                {
                    FilteredAdUnitData =
                        AdUnitDataSource.FindAll(data => data.AdUnit.ToLower().Contains(evt.newValue.ToLower()));
                    listView.itemsSource = FilteredAdUnitData;
                }
                else
                {
                    FilteredAdUnitData = null;
                    listView.itemsSource = AdUnitDataSource;
                }

#if UNITY_2022_1_OR_NEWER
                    //Prior to 2022, refresh was automatic.
                    listView.RefreshItems();
#endif

            };
        }

        static void SortColumn(ListView listView, int columnIndex, Comparison<AdUnitData> comparisonFunction)
        {
            EditorGameServiceAnalyticsSender.SendAdUnitsSortEvent();

            SortModes[columnIndex] = GetNextSortMode(SortModes[columnIndex]);
            switch (SortModes[0])
            {
                case SortMode.Descending:
                    FilteredAdUnitData = AdUnitData;
                    FilteredAdUnitData.Sort(comparisonFunction);
                    listView.itemsSource = FilteredAdUnitData;
                    break;
                case SortMode.Ascending:
                    FilteredAdUnitData = AdUnitData;
                    FilteredAdUnitData.Sort(comparisonFunction);
                    listView.itemsSource = FilteredAdUnitData;
                    break;
            }

#if UNITY_2022_1_OR_NEWER
                    //Prior to 2022, refresh was automatic.
                    listView.RefreshItems();
#endif

        }

        static void ConstructListFromAdUnits(VisualElement root)
        {
            ListView listView = root.Q<ListView>(className: "list-view");

            ToolbarSearchField searchField = root.Q<ToolbarSearchField>(className: "search-field");
            searchField.value = String.Empty;
            searchField.RegisterValueChangedCallback(FilterListBySearchField(listView));

            listView.makeItem = AdUnitVisualElement.CreateListItem;
            listView.bindItem = AdUnitVisualElement.BindListItem;
            listView.itemsSource = AdUnitDataSource;

            root.Q<Label>("list-header-adunit").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 0, (data, unitData) => Editor.AdUnitData.CompareByAdUnit(SortModes[0], data, unitData));
            });

            root.Q<Label>("list-header-platform").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 1, (data, unitData) => Editor.AdUnitData.CompareByPlatform(SortModes[1], data, unitData));
            });

            root.Q<Label>("list-header-adformat").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 2, (data, unitData) => Editor.AdUnitData.CompareByAdFormat(SortModes[2], data, unitData));
            });

            root.Q<Label>("list-header-id").RegisterCallback<MouseDownEvent>(evt =>
            {
                SortColumn(listView, 3, (data, unitData) => Editor.AdUnitData.CompareById(SortModes[3], data, unitData));
            });
        }

        void AddStyleSheets(VisualElement root)
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_AdUnitsStyle);
            root.styleSheets.Add(styleSheet);

            string k_SkinStyle = $@"Packages/com.unity.services.mediation/Editor/Settings/Layout/2019/SkinStyle{(EditorGUIUtility.isProSkin ? "Dark" : "Light")}.uss";
            styleSheet = EditorGUIUtility.Load(k_SkinStyle) as StyleSheet;
            root.styleSheets.Add(styleSheet);
        }
    }
}

