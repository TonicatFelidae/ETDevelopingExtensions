using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ET
{
    [RequireComponent(typeof(Light2D))]
    public class ET2DLightEffect : MonoBehaviour
    {
        [SerializeField] private ET2DLightEffectData effectData;

        private Light2D light2D;
        private float baseIntensity;
        private Color baseColor;
        private float colorShiftTime;

        private void Awake()
        {
            light2D = GetComponent<Light2D>();
            if (light2D != null)
            {
                baseIntensity = light2D.intensity;
                baseColor = light2D.color;
            }
        }

        private void Start()
        {
            if (light2D != null)
            {
                baseIntensity = light2D.intensity;
                baseColor = light2D.color;
            }
        }

        private void Update()
        {
            if (light2D == null) return;

            UpdateFlicker();
            UpdateColorShift();
        }

        private void UpdateFlicker()
        {
            if (!effectData.enableFlicker)
            {
                light2D.intensity = baseIntensity;
                return;
            }

            float time = Time.time * effectData.flickerSpeed;
            float noise = Mathf.PerlinNoise(time, 0.0f);

            float randomNoise = 0f;
            if (effectData.flickerRandomness > 0f)
            {
                randomNoise = Random.Range(-1f, 1f) * effectData.flickerRandomness;
            }

            float fluctuation = (noise * 2f - 1f) * (1f - effectData.flickerRandomness) + randomNoise;
            float intensityMultiplier = Mathf.Max(0f, 1.0f + fluctuation * effectData.flickerIntensity);

            light2D.intensity = baseIntensity * intensityMultiplier;
        }

        private void UpdateColorShift()
        {
            if (!effectData.enableColorShift || effectData.colorGradient == null)
            {
                light2D.color = baseColor;
                return;
            }

            colorShiftTime += Time.deltaTime;
            float t = 0f;
            float speed = effectData.colorShiftSpeed;

            switch (effectData.colorShiftMode)
            {
                case ShiftMode.PingPong:
                    t = Mathf.PingPong(colorShiftTime * speed, 1f);
                    break;
                case ShiftMode.Loop:
                    t = Mathf.Repeat(colorShiftTime * speed, 1f);
                    break;
                case ShiftMode.Once:
                    t = Mathf.Clamp01(colorShiftTime * speed);
                    break;
            }

            light2D.color = effectData.colorGradient.Evaluate(t);
        }

        private void Reset()
        {
            light2D = GetComponent<Light2D>();

            Gradient defaultGradient = new Gradient();
            if (light2D != null)
            {
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(light2D.color, 0.0f);
                colorKeys[1] = new GradientColorKey(light2D.color, 1.0f);

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

                defaultGradient.SetKeys(colorKeys, alphaKeys);
            }

            effectData = new ET2DLightEffectData
            {
                enableFlicker = false,
                flickerSpeed = 5f,
                flickerIntensity = 0.2f,
                flickerRandomness = 0.1f,

                enableColorShift = false,
                colorShiftMode = ShiftMode.PingPong,
                colorShiftSpeed = 1f,
                colorGradient = defaultGradient
            };
        }
    }

    [System.Serializable]
    public struct ET2DLightEffectData
    {
        public bool enableFlicker;
        public float flickerSpeed;
        public float flickerIntensity;
        [Range(0f, 1f)] public float flickerRandomness;

        public bool enableColorShift;
        public ShiftMode colorShiftMode;
        public float colorShiftSpeed;
        public Gradient colorGradient;
    }

    public enum ShiftMode
    {
        PingPong,
        Loop,
        Once
    }
}

