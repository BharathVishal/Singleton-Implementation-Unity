using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity.Services.Mediation.UI
{
    /// <summary>
    /// Inserts a Banner Ad placement in Unity UI.
    /// Please note that Banner Ads are overlaid on top of the Unity activity.
    /// They do not exist in the UI hierarchy, and as such you should consider the following limitations:
    /// - The Banner Ad can not be masked, or displayed behind other UI elements, it will always render on top.
    /// - The Banner Ad can not be resized once created. You can disable this component, change the RectTransform's size
    /// and enable the component again to generate a new Banner Ad.
    /// - The banner Ad can not be hidden. Disabling this component will destroy the banner, and create a new one when
    /// it is enabled again.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIBannerAd : UIBehaviour
    {
        [Tooltip("The banner ad's Ad Unit ID for Android builds")]
        [SerializeField]
        string androidAdUnitId;
        [Tooltip("The banner ad's Ad Unit ID for iOS builds")]
        [SerializeField]
        string iosAdUnitId;

        [Tooltip("The Unity services will automatically be initialized using the linked project's game ID if they " +
            "have not been initialized when this component Awakens. " +
            "Turn this off if you want to initialize your services with a special configuration.")]
        [SerializeField]
        bool autoInitServices = true;

        [Tooltip("Draw a preview of the banner placement in the scene window")]
        [SerializeField]
        bool drawEditorPreview = true;

        /// <summary>
        /// Follow the UI component's movement to the best of the Banner Ad's ability.
        /// Turn this off if you want your banner to be stationary, or for better performances.
        /// </summary>
        [Tooltip("Follow the UI component's movement to the best of the Banner Ad's ability. " +
            "Turn this off if you want your banner to be stationary, or for better performances.")]
        public bool followComponentMovement = true;

        /// <summary>
        /// Returns the banner ad's Ad Unit ID
        /// </summary>
#if UNITY_ANDROID
        public string AdUnitId => androidAdUnitId;
#else
        public string AdUnitId => iosAdUnitId;
#endif

        const BannerAdAnchor k_OffsetBaseAnchor = BannerAdAnchor.None;

        RectTransform m_RectTransform;
        IBannerAd m_BannerAd;
        static Vector3[] s_Corners = new Vector3[4];

        bool m_Loading;

        /// <summary>
        /// Called once on object initialization
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_RectTransform = GetComponent<RectTransform>();
            if (autoInitServices)
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                {
                    var task = InitializeServices();
                }
            }
        }

        void LateUpdate()
        {
            if (!m_Loading && m_BannerAd == null)
            {
                StartCoroutine(LoadBannerAd());
            }
            else if (m_BannerAd != null && transform.hasChanged && followComponentMovement)
            {
                m_RectTransform.GetWorldCorners(s_Corners);
                var offset = s_Corners[0];
                m_BannerAd.SetPosition(k_OffsetBaseAnchor, offset);
                transform.hasChanged = false;
            }
        }

        IEnumerator LoadBannerAd()
        {
            m_Loading = true;
            yield return new WaitUntil(() => UnityServices.State == ServicesInitializationState.Initialized);

            // Make sure we did not disable in the meantime.
            if (enabled)
            {
                var task = CreateAndLoadBanner();
            }
            else
            {
                m_Loading = false;
            }
        }

        async Task CreateAndLoadBanner()
        {
            m_RectTransform.GetWorldCorners(s_Corners);

            var uiPixelSize = s_Corners[2] - s_Corners[0];
            var size = new BannerAdSize(uiPixelSize);
            var offset = s_Corners[0];

            try
            {
                m_BannerAd = MediationService.Instance.CreateBannerAd(AdUnitId, size, k_OffsetBaseAnchor, offset);
                await m_BannerAd.LoadAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }

            // Make sure we did not disable in the meantime.
            if (!enabled)
            {
                DestroyBanner();
            }
            m_Loading = false;
        }

        /// <summary>
        /// Called when the ui banner ad is disabled
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            DestroyBanner();
        }

        void DestroyBanner()
        {
            m_BannerAd?.Dispose();
            m_BannerAd = null;
        }

        async Task InitializeServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogError("BannerAdUI attempted to initialize UnityServices and failed: " + e.Message);
            }
        }

#if UNITY_EDITOR
        const string k_BannerTexturePath    = @"Packages/com.unity.services.mediation/Runtime/Platform/Editor/TestAds/testAds_background.png";
        const string k_BannerTopTexturePath = @"Packages/com.unity.services.mediation/Runtime/Platform/Editor/TestAds/testAds_unityads_logo.png";

        Texture txGizmoBg;
        Texture txGizmoTop;

        void OnDrawGizmos()
        {
            if (enabled && drawEditorPreview)
            {
                if (txGizmoBg == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                    txGizmoBg = AssetDatabase.LoadAssetAtPath<Texture>(k_BannerTexturePath);
                    txGizmoTop = AssetDatabase.LoadAssetAtPath<Texture>(k_BannerTopTexturePath);
                    txGizmoTop.wrapMode = TextureWrapMode.Clamp;
                }

                m_RectTransform.GetWorldCorners(s_Corners);
                var size = s_Corners[2] - s_Corners[0];
                var pos = s_Corners[0];

                // Flip the rect otherwise the texture is rendered upside down
                size.y *= -1;
                pos.y -= size.y;
                var rct = new Rect(pos, size);
                Gizmos.DrawGUITexture(rct, txGizmoBg);

                // Trim 10% each border
                rct.x += rct.size.x * 0.1f;
                rct.y += rct.size.y * 0.1f;
                rct.size *= 0.8f;
                Gizmos.DrawGUITexture(rct, txGizmoTop);
            }
        }

        [MenuItem("GameObject/UI/Mediation/Banner Ad", false, 0)]
        static void CreateBannerAdGameObject()
        {
            var go = new GameObject("Mediation Banner Ad", typeof(UIBannerAd));
            if (Selection.activeTransform != null)
            {
                go.transform.SetParent(Selection.activeTransform, false);
            }
        }

#endif
    }
}
