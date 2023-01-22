#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Unity.Services.Mediation.Platform
{
    class MockBanner : MonoBehaviour, IPlatformBannerAd, IPointerClickHandler
    {
        [Header("Properties")]
        public bool DisplayConsoleMessages = false;
        [Range(0f, 10f)]
        public float MockLoadTimeInSeconds = 0f;
        [Range(0f, 1f)]
        public float MockNoFillChance = 0f;
        [Range(0f, 30f)]
        public float MockRefreshDelayInSeconds = 5f;

        [Header("References")]
        public Canvas Canvas;
        public RectTransform CanvasPanel;
        public EventSystem EventSystem;

        Coroutine m_AdCoroutine;

        public AdState AdState { get; private set; }
        public string AdUnitId { get; internal set; }

        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnClicked;
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;

        BannerAdSize m_Size;

        public BannerAdSize Size
        {
            get => m_Size;

            internal set
            {
                m_Size = value;
                CanvasPanel.sizeDelta = new Vector2(value.Width, value.Height);
            }
        }


        void Awake()
        {
            Canvas.enabled = false;

            if (EventSystem.current == null)
            {
                EventSystem.gameObject.SetActive(true);
                EventSystem.current = EventSystem;
            }

            if (DisplayConsoleMessages)
            {
                InitializeConsoleCallbacks();
            }
        }

        void InitializeConsoleCallbacks()
        {
            OnLoaded += (sender, args) => Debug.unityLogger.Log("Mediation", "Banner Loaded");
            OnFailedLoad += (sender, args) => Debug.unityLogger.LogError("Mediation", "Banner Fail to Load: {args.Message}");
            OnClicked += (sender, args) => Debug.unityLogger.Log("Mediation", "Banner Clicked");
            OnRefreshed += (sender, args) => Debug.unityLogger.Log("Mediation", "Banner Refreshed");
        }

        public void Load()
        {
            if (MediationService.InitializationState != InitializationState.Initialized)
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.SdkNotInitialized, "Unity Mediation not Initialized."));
            }
            else if (string.IsNullOrEmpty(AdUnitId))
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, "Ad Unit Id is Empty."));
            }
            else if (m_Size == null)
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.Unknown, "Banner size has not been set."));
            }
            else
            {
                StartCoroutine(AttemptLoad());
            }
        }

        IEnumerator AttemptLoad()
        {
            yield return new WaitForSeconds(MockLoadTimeInSeconds);

            if (Random.value < MockNoFillChance)
            {
                OnFailedLoad?.Invoke(this, new LoadErrorEventArgs(LoadError.NoFill, "Ad Unit failed to fill."));
            }
            else
            {
                AdState = AdState.Showing;
                OnLoaded?.Invoke(this, EventArgs.Empty);
                Canvas.enabled = true;
                if (MockRefreshDelayInSeconds > 0f)
                {
                    StartCoroutine(RefreshSimulation());
                }
            }
        }

        IEnumerator RefreshSimulation()
        {
            while (this != null && gameObject != null)
            {
                yield return new WaitForSeconds(MockRefreshDelayInSeconds);
                if (Random.value < MockNoFillChance)
                {
                    OnRefreshed?.Invoke(this, new LoadErrorEventArgs(LoadError.NoFill, "Ad Unit failed to fill."));
                }
                else
                {
                    OnRefreshed?.Invoke(this, null);
                }
            }
        }

        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2())
        {
            CanvasPanel.anchoredPosition = positionOffset;
            switch (anchor)
            {
                case BannerAdAnchor.TopLeft:
                    CanvasPanel.pivot = new Vector2(0f, 1f);
                    CanvasPanel.anchorMin = new Vector2(0f, 1f);
                    CanvasPanel.anchorMax = new Vector2(0f, 1f);
                    break;

                case BannerAdAnchor.TopCenter:
                    CanvasPanel.pivot = new Vector2(0.5f, 1f);
                    CanvasPanel.anchorMin = new Vector2(0.5f, 1f);
                    CanvasPanel.anchorMax = new Vector2(0.5f, 1f);
                    break;

                case BannerAdAnchor.TopRight:
                    CanvasPanel.pivot = new Vector2(1f, 1f);
                    CanvasPanel.anchorMin = new Vector2(1f, 1f);
                    CanvasPanel.anchorMax = new Vector2(1f, 1f);
                    break;

                case BannerAdAnchor.MiddleLeft:
                    CanvasPanel.pivot = new Vector2(0f, 0.5f);
                    CanvasPanel.anchorMin = new Vector2(0f, 0.5f);
                    CanvasPanel.anchorMax = new Vector2(0f, 0.5f);
                    break;

                case BannerAdAnchor.Center:
                    CanvasPanel.pivot = new Vector2(0.5f, 0.5f);
                    CanvasPanel.anchorMin = new Vector2(0.5f, 0.5f);
                    CanvasPanel.anchorMax = new Vector2(0.5f, 0.5f);
                    break;

                case BannerAdAnchor.MiddleRight:
                    CanvasPanel.pivot = new Vector2(1f, 0.5f);
                    CanvasPanel.anchorMin = new Vector2(1f, 0.5f);
                    CanvasPanel.anchorMax = new Vector2(1f, 0.5f);
                    break;

                case BannerAdAnchor.BottomLeft:
                case BannerAdAnchor.None:
                    CanvasPanel.pivot = new Vector2(0f, 0f);
                    CanvasPanel.anchorMin = new Vector2(0f, 0f);
                    CanvasPanel.anchorMax = new Vector2(0f, 0f);
                    break;

                case BannerAdAnchor.BottomCenter:
                    CanvasPanel.pivot = new Vector2(0.5f, 0f);
                    CanvasPanel.anchorMin = new Vector2(0.5f, 0f);
                    CanvasPanel.anchorMax = new Vector2(0.5f, 0f);
                    break;

                case BannerAdAnchor.BottomRight:
                    CanvasPanel.pivot = new Vector2(1f, 0f);
                    CanvasPanel.anchorMin = new Vector2(1f, 0f);
                    CanvasPanel.anchorMax = new Vector2(1f, 0f);
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (this != null && gameObject != null)
                Destroy(gameObject);
        }

        void InvokeImpressionEvent(object sender, ImpressionEventArgs args)
        {
            ((EditorImpressionEventPublisher)MediationService.Instance.ImpressionEventPublisher).InvokeOnImpressionEvent(sender, args);
        }
    }
}
#endif
