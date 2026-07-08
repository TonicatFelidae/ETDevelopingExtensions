using ET;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ETEngine.TutorialSystem
{
    public class TutorialBackdrop : Singleton<TutorialBackdrop>
    {

        [Header("Backdrop Settings")]
        [SerializeField] private int backdropSortingOrder = 900;

        [Header("UI Components")]
        [SerializeField] private Canvas backdropCanvas;
        [SerializeField] private Image backdropImage;
        [SerializeField] private RectTransform spotLightTransform;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private float defaultSpotLightRadius = 180f;
        [SerializeField] private float _fadeInTime = 0f;
        [SerializeField] private float _fadeOutTime = 0f;
        [SerializeField] private TextMeshProUGUI _tutorialText;

        private GameObject _currentTarget;
        public GameObject CurrentTarget => _currentTarget;
        private Canvas _targetCanvas;
        private GraphicRaycaster _targetRaycaster;
        private CanvasGroup _backdropCanvasGroup;
        private Tween _fadeTween;
        private bool _isSpotLightActive;
        private float _spotLightRadius;

        private bool _hadCanvas;
        private bool _hadRaycaster;
        private int _originalSortingOrder;
        private bool _originalOverrideSorting;
        private string _originalSortingLayerName;
        private int _originalSortingLayerID;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (backdropCanvas == null)
            {
                backdropCanvas = GetComponent<Canvas>();
            }

            if (backdropImage == null)
            {
                backdropImage = GetComponentInChildren<Image>(true);
            }

            EnsureBackdropCanvasGroup();

            EnsureBackdropSetup();
            ShowBackdrop(false);
        }
        public void ShowStandout(TutorialTarget target)
        {
            if (target == null) return;
            ShowStandout(target.gameObject);
        }

        public void ShowStandout(GameObject target)
        {
            if (target == null) return;
            if (_currentTarget != null && _currentTarget != target)
            {
                HideStandout();
            }
            _currentTarget = target;
            gameObject.SetActive(true);

            EnsureBackdropSetup();
            backdropCanvas.enabled = true;
            // Let's make sure the target has Canvas and GraphicRaycaster so it renders on top and receives clicks
            _targetCanvas = _currentTarget.GetComponent<Canvas>();
            if (_targetCanvas != null)
            {
                _hadCanvas = true;
                _originalSortingOrder = _targetCanvas.sortingOrder;
                _originalOverrideSorting = _targetCanvas.overrideSorting;
                _originalSortingLayerID = _targetCanvas.sortingLayerID;
                _originalSortingLayerName = _targetCanvas.sortingLayerName;
            }
            else
            {
                _hadCanvas = false;
                _targetCanvas = _currentTarget.AddComponent<Canvas>();
            }

            // Configure target canvas to render above the backdrop
            _targetRaycaster = _currentTarget.GetComponent<GraphicRaycaster>();

            if (_targetRaycaster != null)
            {
                _hadRaycaster = true;
            }
            else
            {
                _hadRaycaster = false;
                _targetRaycaster = _currentTarget.AddComponent<GraphicRaycaster>();
            }

            _isSpotLightActive = false;
            SetSpotLightActive(false);
            ShowBackdrop(true);
            RectTransform canvasRect = _targetCanvas.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRect);
            Canvas.ForceUpdateCanvases();
        }

        public void ShowSpotlight(TutorialTarget target, float radius)
        {
            if (target == null) return;
            ShowSpotlight(target.gameObject, radius);
        }

        public void ShowSpotlight(GameObject target, float radius)
        {
            if (target == null) return;
            if (_currentTarget != null && _currentTarget != target)
            {
                HideStandout();
            }

            _currentTarget = target;
            gameObject.SetActive(true);

            EnsureBackdropSetup();
            backdropCanvas.enabled = true;
            _isSpotLightActive = true;
            _spotLightRadius = radius > 0f ? radius : defaultSpotLightRadius;

            SetSpotLightRadius(_spotLightRadius);
            SetSpotLightActive(true);
            UpdateSpotLightPosition();
            ShowBackdrop(true);
        }

        public void HideStandout()
        {
            _isSpotLightActive = false;
            SetSpotLightActive(false);

            if (_currentTarget != null)
            {
                bool shouldRemoveCanvas = _targetCanvas != null && !_hadCanvas;

                // Remove GraphicRaycaster first when we plan to remove Canvas,
                // because GraphicRaycaster has a required dependency on Canvas.
                if (_targetRaycaster != null)
                {
                    if (shouldRemoveCanvas || !_hadRaycaster)
                    {
                        Destroy(_targetRaycaster);
                    }
                    _targetRaycaster = null;
                }

                // Restore or remove Canvas
                if (_targetCanvas != null)
                {
                    if (_hadCanvas)
                    {
                        _targetCanvas.overrideSorting = _originalOverrideSorting;
                        _targetCanvas.sortingOrder = _originalSortingOrder;
                        _targetCanvas.sortingLayerID = _originalSortingLayerID;
                        _targetCanvas.sortingLayerName = _originalSortingLayerName;
                    }
                    else
                    {
                        Destroy(_targetCanvas);
                    }
                    _targetCanvas = null;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_currentTarget.GetComponent<RectTransform>());
                _currentTarget = null;
            }
            ShowBackdrop(false);
        }
        void Update()
        {
            if (_currentTarget != null)
            {
                // Ensure the target is still active and valid
                if (_targetCanvas != null)
                {
                    _targetCanvas.enabled = true;
                    _targetCanvas.overrideSorting = true;
                    _targetCanvas.sortingOrder = backdropCanvas.sortingOrder + 1;
                }

                if (_isSpotLightActive)
                {
                    UpdateSpotLightPosition();
                }

            }
        }
        private void OnDisable()
        {
            // If the backdrop itself is disabled, make sure we clean up the standout target
            HideStandout();
        }

        private void EnsureBackdropSetup()
        {
            if (backdropCanvas == null)
            {
                backdropCanvas = gameObject.AddComponent<Canvas>();
            }

            // When this object is under another canvas, overrideSorting is required
            // or sortingOrder will not lift it above sibling UI roots.
            backdropCanvas.overrideSorting = true;
            backdropCanvas.sortingOrder = backdropSortingOrder;

            var raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            if (backdropImage != null)
            {
                backdropImage.raycastTarget = true;
            }

            RectTransform canvasRect = backdropCanvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                canvasRect.anchorMin = Vector2.zero;
                canvasRect.anchorMax = Vector2.one;
                canvasRect.offsetMin = Vector2.zero;
                canvasRect.offsetMax = Vector2.zero;
                LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRect);
            }

            Canvas.ForceUpdateCanvases();
        }

        private void EnsureBackdropCanvasGroup()
        {
            if (backdropImage == null)
            {
                return;
            }

            _backdropCanvasGroup = backdropImage.GetComponent<CanvasGroup>();
            if (_backdropCanvasGroup == null)
            {
                _backdropCanvasGroup = backdropImage.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void SetSpotLightActive(bool isActive)
        {
            if (spotLightTransform == null)
            {
                return;
            }

            spotLightTransform.gameObject.SetActive(isActive);
        }

        private void SetSpotLightRadius(float radius)
        {
            if (spotLightTransform == null)
            {
                return;
            }

            float clampedRadius = Mathf.Max(0f, radius);
            float diameter = clampedRadius * 2f;
            spotLightTransform.sizeDelta = new Vector2(diameter, diameter);
        }

        private void UpdateSpotLightPosition()
        {
            if (!_isSpotLightActive || _currentTarget == null || spotLightTransform == null || backdropCanvas == null)
            {
                return;
            }

            Camera cameraForProjection = worldCamera != null ? worldCamera : Camera.main;
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(cameraForProjection, _currentTarget.transform.position);

            if (screenPoint.z < 0f)
            {
                SetSpotLightActive(false);
                return;
            }

            RectTransform backdropRect = backdropCanvas.transform as RectTransform;
            if (backdropRect == null)
            {
                return;
            }

            Camera eventCamera = backdropCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cameraForProjection;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backdropRect, screenPoint, eventCamera, out var localPoint))
            {
                if (!spotLightTransform.gameObject.activeSelf)
                {
                    SetSpotLightActive(true);
                }

                spotLightTransform.anchoredPosition = localPoint;
            }
        }

        private void ShowBackdrop(bool show)
        {
            if (backdropImage == null)
            {
                return;
            }

            EnsureBackdropCanvasGroup();

            _fadeTween?.Kill();

            float fadeDuration = show ? _fadeInTime : _fadeOutTime;

            if (fadeDuration <= 0f || _backdropCanvasGroup == null)
            {
                backdropImage.gameObject.SetActive(show);
                if (_backdropCanvasGroup != null)
                {
                    _backdropCanvasGroup.alpha = show ? 1f : 0f;
                    _backdropCanvasGroup.blocksRaycasts = show;
                    _backdropCanvasGroup.interactable = show;
                }
                return;
            }

            if (show)
            {
                backdropImage.gameObject.SetActive(true);
                _backdropCanvasGroup.blocksRaycasts = true;
                _backdropCanvasGroup.interactable = true;
                _fadeTween = _backdropCanvasGroup.DOFade(1f, fadeDuration);
            }
            else
            {
                _backdropCanvasGroup.blocksRaycasts = false;
                _backdropCanvasGroup.interactable = false;
                _fadeTween = _backdropCanvasGroup.DOFade(0f, fadeDuration)
                    .OnComplete(() =>
                    {
                        if (backdropImage != null)
                        {
                            backdropImage.gameObject.SetActive(false);
                        }
                    });
            }
        }
        public void ForceSetup(float alpha, bool blocksRaycasts, bool interactable)
        {
            if (backdropImage == null)
            {
                return;
            }
            backdropImage.gameObject.SetActive(true);

            EnsureBackdropCanvasGroup();

            if (_backdropCanvasGroup != null)
            {
                _backdropCanvasGroup.alpha = Mathf.Clamp01(alpha);
                _backdropCanvasGroup.blocksRaycasts = blocksRaycasts;
                _backdropCanvasGroup.interactable = interactable;
            }
        }

        public void SetTutorialText(bool show, string text)
        {
            if (_tutorialText == null)
            {
                return;
            }

            if (!show || string.IsNullOrWhiteSpace(text))
            {
                _tutorialText.text = string.Empty;
                _tutorialText.gameObject.SetActive(false);
                return;
            }

            _tutorialText.text = text;
            _tutorialText.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
        }
    }
}
