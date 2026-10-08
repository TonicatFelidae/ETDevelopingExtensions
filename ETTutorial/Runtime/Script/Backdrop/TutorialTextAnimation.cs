using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ETEngine.TutorialSystem
{
    [RequireComponent(typeof(TMP_Text))]
    public class TutorialTextAnimation : MonoBehaviour
    {
        [SerializeField] private float _popDuration = 0.3f;
        [SerializeField] private float _bobHeight = 8f;
        [SerializeField] private float _bobDuration = 0.8f;

        // Off when the owner positions the text every frame and adds BobOffset itself.
        public bool ApplyBob { get; set; } = true;
        public float BobOffset { get; private set; }

        private TMP_Text _text;
        private RectTransform _rect;
        private Vector3 _baseScale = Vector3.one;
        private Vector2 _baseAnchoredPosition;
        private Tween _popTween;
        private Tween _bobTween;
        private bool _hasBaseScale;

        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            Play();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            CacheComponents();
            Stop();
            _baseAnchoredPosition = _rect.anchoredPosition;

            if (_popDuration > 0f)
            {
                _rect.localScale = _baseScale * 0.8f;
                _text.alpha = 0f;
                _popTween = DOTween.Sequence()
                    .Join(_rect.DOScale(_baseScale, _popDuration).SetEase(Ease.OutBack))
                    .Join(DOTween.To(() => _text.alpha, a => _text.alpha = a, 1f, _popDuration));
            }

            if (_bobHeight > 0f && _bobDuration > 0f)
            {
                _bobTween = DOTween.To(() => BobOffset, v => BobOffset = v, _bobHeight, _bobDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void Stop()
        {
            CacheComponents();
            _popTween?.Kill();
            _bobTween?.Kill();
            _popTween = null;
            _bobTween = null;
            if (ApplyBob && BobOffset != 0f)
            {
                _rect.anchoredPosition = _baseAnchoredPosition;
            }
            BobOffset = 0f;
            _rect.localScale = _baseScale;
            _text.alpha = 1f;
        }

        private void LateUpdate()
        {
            if (ApplyBob && _bobTween != null)
            {
                _rect.anchoredPosition = _baseAnchoredPosition + new Vector2(0f, BobOffset);
            }
        }

        private void CacheComponents()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
                _rect = _text.rectTransform;
            }

            if (!_hasBaseScale)
            {
                _baseScale = _rect.localScale;
                _hasBaseScale = true;
            }
        }
    }
}
