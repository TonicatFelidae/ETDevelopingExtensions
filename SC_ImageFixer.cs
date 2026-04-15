using UnityEngine;
using UnityEngine.UI;
namespace ET.UIKit
{
    [RequireComponent(typeof(Image))]
    public class SC_ImageFixer : MonoBehaviour
    {
        [SerializeField] private bool cropImageToImageSize = true;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            if (cropImageToImageSize)
                ApplyCrop();
        }

        private void ApplyCrop()
        {
            if (_image == null)
                _image = GetComponent<Image>();

            if (_image == null || _image.sprite == null) return;

            Texture2D original = _image.sprite.texture;
            RectTransform rt = GetComponent<RectTransform>();

            int targetW = Mathf.RoundToInt(rt.rect.width);
            int targetH = Mathf.RoundToInt(rt.rect.height);

            if (targetW <= 0 || targetH <= 0) return;

            int cropW = Mathf.Min(targetW, original.width);
            int cropH = Mathf.Min(targetH, original.height);
            int startX = (original.width - cropW) / 2;
            int startY = (original.height - cropH) / 2;

            Texture2D cropped = new Texture2D(cropW, cropH, original.format, false);
            cropped.SetPixels(original.GetPixels(startX, startY, cropW, cropH));
            cropped.Apply();

            _image.sprite = Sprite.Create(
                cropped,
                new Rect(0, 0, cropW, cropH),
                new Vector2(0.5f, 0.5f),
                _image.sprite.pixelsPerUnit
            );
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!cropImageToImageSize) return;
            if (_image == null)
                _image = GetComponent<Image>();
            ApplyCrop();
        }
#endif
    }
}