#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Unity.Services.Mediation.Platform
{
    class MockRewarded : MonoBehaviour, IPlatformRewardedAd
    {
        [Header("Properties")]
        public float AdTime = 5f;
        public bool DisplayConsoleMessages = false;
        public bool DisplayMockAd = true;
        public bool DisplaySlideAnimation = true;
        [Range(0f, 10f)]
        public float MockLoadTimeInSeconds = 0f;
        [Range(0f, 1f)]
        public float MockNoFillChance = 0f;

        [Header("References")]
        public Button CloseButton;
        public Button SkipButton;
        public Image ProgressBar;
        public Canvas Canvas;
        public RectTransform CanvasPanel;
        public EventSystem EventSystem;

        Coroutine m_AdCoroutine;
        bool m_Skipped;

        public AdState AdState { get; private set; }
        public string AdUnitId { get; internal set; }

        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        void Awake()
        {
            m_Skipped = false;

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
            OnLoaded += (sender, args) => Debug.unityLogger.Log("Mediation", "Rewarded Loaded");
            OnFailedLoad += (sender, args) => Debug.unityLogger.LogError("Mediation", "Rewarded Fail to Load: {args.Message}");

            OnUserRewarded += (sender, args) => Debug.unityLogger.Log("Mediation", "User Rewarded: (type: {args.Type}, amount: {args.Amount})");
            OnClicked += (sender, args) => Debug.unityLogger.Log("Mediation", "Rewarded Clicked");
            OnClosed += (sender, args) => Debug.unityLogger.Log("Mediation", "Rewarded Closed");

            OnShowed += (sender, args) => Debug.unityLogger.Log("Mediation", "Rewarded Showed");
            OnFailedShow += (sender, args) => Debug.unityLogger.LogError("Mediation", "Rewarded Fail to Show: {args.Message}");
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
                AdState = AdState.Loaded;
                OnLoaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Show(RewardedAdShowOptions showOptions = null)
        {
            if (AdState == AdState.Loaded)
            {
                if (DisplayMockAd)
                {
                    ShowAd();
                }

                OnShowed?.Invoke(this, EventArgs.Empty);
                AdState = AdState.Showing;
            }
            else
            {
                OnFailedShow?.Invoke(this, new ShowErrorEventArgs(ShowError.AdNotLoaded, "Ad Not Loaded."));
            }
        }

        void ShowAd()
        {
            Canvas.enabled = true;
            m_AdCoroutine = StartCoroutine(StartAd());
        }

        IEnumerator StartAd()
        {
            ProgressBar.fillAmount = 0;
            SkipButton.gameObject.SetActive(true);
            CloseButton.gameObject.SetActive(false);

            if (DisplaySlideAnimation)
            {
                CanvasPanel.offsetMax = new Vector2(CanvasPanel.offsetMax.x, -Screen.height);
                CanvasPanel.offsetMin = new Vector2(CanvasPanel.offsetMin.x, -Screen.height);
            }

            float timer = 0;
            while (timer < AdTime)
            {
                var interpolatedTopValue = Mathf.Lerp(CanvasPanel.offsetMax.y, 0, 0.075f);
                CanvasPanel.offsetMax = new Vector2(CanvasPanel.offsetMax.x, interpolatedTopValue);
                CanvasPanel.offsetMin = new Vector2(CanvasPanel.offsetMin.x, interpolatedTopValue);

                ProgressBar.fillAmount = timer / AdTime;

                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            EndAd();
        }

        public void SkipAd()
        {
            m_Skipped = true;
            EndAd();
        }

        public void EndAd()
        {
            StopCoroutine(m_AdCoroutine);
            ProgressBar.fillAmount = 1;
            SkipButton.gameObject.SetActive(false);
            CloseButton.gameObject.SetActive(true);
        }

        public void CloseAd()
        {
            EndAd();
            OnClosed?.Invoke(this, EventArgs.Empty);
            if (!m_Skipped)
            {
                OnUserRewarded?.Invoke(this, new RewardEventArgs("test", "1.0"));
            }
            else
            {
                m_Skipped = false;
            }

            InvokeImpressionEvent(this, new ImpressionEventArgs(AdUnitId, new ImpressionData()));
            Canvas.enabled = false;
        }

        public void ClickedAd()
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
