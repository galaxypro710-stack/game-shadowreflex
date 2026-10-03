using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    /// <summary>
    /// Quản lý toàn bộ hiệu ứng hình ảnh (VFX) chiến đấu:
    /// - Vết chém kiếm (Slash VFX)
    /// - Vệt lướt né đòn (Dash Ghost Trail)
    /// - Lóe sáng cảnh báo địch ra đòn (Telegraph Flash)
    /// - Nổ tia sét/hào quang phản công hoàn hảo (Perfect Counter Burst)
    /// Tích hợp Object Pooling cơ bản để tránh Garbage Collection.
    /// </summary>
    public class CombatVFXController : MonoBehaviour
    {
        public static CombatVFXController Instance { get; private set; }

        [Header("Sprites")]
        [SerializeField] private Sprite _slashSprite;
        [SerializeField] private Sprite _dashSprite;
        [SerializeField] private Sprite _telegraphSprite;
        [SerializeField] private Sprite _burstSprite;

        private GameObject _telegraphIndicatorObj;
        private SpriteRenderer _telegraphIndicatorRenderer;

        private readonly List<VFXInstance> _activeVFX = new List<VFXInstance>(16);

        private class VFXInstance
        {
            public GameObject go;
            public SpriteRenderer sr;
            public float lifetime;
            public float maxLifetime;
            public Vector3 velocity;
            public bool isFading;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;

            // Khởi tạo Telegraph Indicator dùng chung
            _telegraphIndicatorObj = new GameObject("VFX_TelegraphIndicator");
            _telegraphIndicatorObj.transform.SetParent(transform);
            _telegraphIndicatorRenderer = _telegraphIndicatorObj.AddComponent<SpriteRenderer>();
            _telegraphIndicatorRenderer.sortingOrder = 10;
            _telegraphIndicatorObj.SetActive(false);

            // Nạp sprites nếu chưa gán qua Inspector
            LoadSpritesFromResources();
        }

        private void LoadSpritesFromResources()
        {
#if UNITY_EDITOR
            if (_slashSprite == null) _slashSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_Slash.png");
            if (_dashSprite == null) _dashSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/Hero_Dash.png");
            if (_telegraphSprite == null) _telegraphSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_TelegraphMark.png");
            if (_burstSprite == null) _burstSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Project/Art/Sprites/VFX_CounterBurst.png");
#endif
            if (_telegraphIndicatorRenderer != null && _telegraphSprite != null)
            {
                _telegraphIndicatorRenderer.sprite = _telegraphSprite;
            }
        }

        private void Update()
        {
            // Cập nhật hiệu ứng fading & recycling cho các VFX đang hoạt động
            for (int i = _activeVFX.Count - 1; i >= 0; i--)
            {
                var vfx = _activeVFX[i];
                vfx.lifetime -= Time.deltaTime;

                if (vfx.lifetime <= 0f)
                {
                    vfx.go.SetActive(false);
                    _activeVFX.RemoveAt(i);
                }
                else
                {
                    vfx.go.transform.position += vfx.velocity * Time.deltaTime;

                    if (vfx.isFading && vfx.sr != null)
                    {
                        float alpha = vfx.lifetime / vfx.maxLifetime;
                        Color c = vfx.sr.color;
                        c.a = alpha;
                        vfx.sr.color = c;
                    }
                }
            }
        }

        public void PlaySlash(Vector3 position, bool isCritical = false)
        {
            if (_slashSprite == null) LoadSpritesFromResources();
            var vfx = SpawnVFX("VFX_Slash", position, 0.18f);
            if (vfx.sr != null && _slashSprite != null)
            {
                vfx.sr.sprite = _slashSprite;
                vfx.sr.sortingOrder = 5;
                vfx.sr.color = isCritical ? new Color(1f, 0.9f, 0.2f, 1f) : new Color(0.2f, 0.9f, 1f, 1f);
                float angle = Random.Range(-35f, 35f);
                vfx.go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                float scale = isCritical ? 1.5f : 1.1f;
                vfx.go.transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        public void PlayDashTrail(Vector3 position)
        {
            if (_dashSprite == null) LoadSpritesFromResources();
            var vfx = SpawnVFX("VFX_DashTrail", position, 0.25f);
            if (vfx.sr != null && _dashSprite != null)
            {
                vfx.sr.sprite = _dashSprite;
                vfx.sr.sortingOrder = 2;
                vfx.sr.color = new Color(0.1f, 0.7f, 1f, 0.5f);
                vfx.go.transform.localScale = Vector3.one;
                vfx.go.transform.rotation = Quaternion.identity;
            }
        }

        public void PlayCounterBurst(Vector3 position)
        {
            if (_burstSprite == null) LoadSpritesFromResources();
            var vfx = SpawnVFX("VFX_CounterBurst", position, 0.35f);
            if (vfx.sr != null && _burstSprite != null)
            {
                vfx.sr.sprite = _burstSprite;
                vfx.sr.sortingOrder = 8;
                vfx.sr.color = new Color(1f, 0.95f, 0.3f, 1f);
                vfx.go.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
                vfx.go.transform.rotation = Quaternion.identity;
            }
        }

        public void ShowTelegraphMark(Vector3 enemyPosition)
        {
            if (_telegraphIndicatorObj == null) return;
            if (_telegraphSprite == null) LoadSpritesFromResources();

            _telegraphIndicatorObj.transform.position = enemyPosition + new Vector3(0f, 1.4f, 0f);
            _telegraphIndicatorObj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            _telegraphIndicatorObj.SetActive(true);
        }

        public void HideTelegraphMark()
        {
            if (_telegraphIndicatorObj != null)
            {
                _telegraphIndicatorObj.SetActive(false);
            }
        }

        private VFXInstance SpawnVFX(string name, Vector3 pos, float duration)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();

            var instance = new VFXInstance
            {
                go = go,
                sr = sr,
                lifetime = duration,
                maxLifetime = duration,
                velocity = Vector3.zero,
                isFading = true
            };

            _activeVFX.Add(instance);
            return instance;
        }
    }
}
