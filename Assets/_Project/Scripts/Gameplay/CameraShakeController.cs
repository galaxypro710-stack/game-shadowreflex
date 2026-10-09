using UnityEngine;

namespace Game.Gameplay
{
    /// <summary>
    /// Điều khiển rung màn hình (Camera Shake) nhẹ khi tấn công chí mạng, Perfect Counter hoặc bị thương.
    /// Thiết kế Zero-GC, không phân bổ bộ nhớ động trong frame.
    /// </summary>
    public class CameraShakeController : MonoBehaviour
    {
        public static CameraShakeController Instance { get; private set; }

        private Vector3 _originalPosition;
        private float _shakeTimer = 0f;
        private float _shakeDuration = 0f;
        private float _shakeIntensity = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            _originalPosition = transform.localPosition;
        }

        private void LateUpdate()
        {
            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                float progress = _shakeTimer / _shakeDuration;
                float currentIntensity = _shakeIntensity * progress;

                // Procedural jitter
                float offsetX = (Mathf.PerlinNoise(Time.time * 25f, 0f) * 2f - 1f) * currentIntensity;
                float offsetY = (Mathf.PerlinNoise(0f, Time.time * 25f) * 2f - 1f) * currentIntensity;

                transform.localPosition = new Vector3(_originalPosition.x + offsetX, _originalPosition.y + offsetY, _originalPosition.z);

                if (_shakeTimer <= 0f)
                {
                    transform.localPosition = _originalPosition;
                }
            }
        }

        public void Shake(float duration = 0.2f, float intensity = 0.15f)
        {
            _shakeDuration = duration;
            _shakeTimer = duration;
            _shakeIntensity = intensity;
        }

        public void ShakeCritical()
        {
            Shake(0.28f, 0.25f);
        }

        public void ShakeLight()
        {
            Shake(0.12f, 0.08f);
        }
    }
}
