using System.Collections.Generic;
using UnityEngine;

namespace Loamhaven.World
{
    public class TorchGlowSystem : MonoBehaviour
    {
        public static TorchGlowSystem Instance { get; private set; }

        readonly Dictionary<long, GameObject> _glows = new();

        static Sprite   _glowSprite;
        static Material _additiveMat;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance     = this;
            _glowSprite  = BuildGlowSprite();
            _additiveMat = BuildAdditiveMat();
        }

        public void RegisterTorch(int bx, int by)
        {
            long key = Key(bx, by);
            if (_glows.ContainsKey(key)) return;

            var go = new GameObject($"Glow_{bx}_{by}");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(bx + 0.5f, by + 0.5f, 0f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = _glowSprite;
            sr.sharedMaterial   = _additiveMat;
            sr.sortingLayerName = "Glow";
            sr.sortingOrder     = 0;

            go.AddComponent<TorchFlicker>();
            _glows[key] = go;
        }

        public void UnregisterTorch(int bx, int by)
        {
            long key = Key(bx, by);
            if (_glows.TryGetValue(key, out var go))
            {
                Destroy(go);
                _glows.Remove(key);
            }
        }

        public void ClearAll()
        {
            foreach (var kv in _glows) if (kv.Value) Destroy(kv.Value);
            _glows.Clear();
        }

        static long Key(int x, int y) => ((long)(uint)x << 32) | (uint)y;

        static Sprite BuildGlowSprite()
        {
            const int S = 64;
            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode   = TextureWrapMode.Clamp,
                name       = "TorchGlow"
            };

            var px = new Color[S * S];
            float cx = (S - 1) / 2f, cy = (S - 1) / 2f, r = S / 2f;
            for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
            {
                float d = Mathf.Sqrt((x - cx)*(x - cx) + (y - cy)*(y - cy));
                float t = Mathf.Clamp01(1f - d / r);
                float a = t * t * t * 0.80f;
                px[y * S + x] = new Color(1f, 0.70f + t * 0.30f, t * 0.20f, a);
            }
            tex.SetPixels(px);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 16f);
        }

        static Material BuildAdditiveMat()
        {
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_ZWrite",   0);
            mat.renderQueue = 3000;
            return mat;
        }
    }

    [AddComponentMenu("")]
    public class TorchFlicker : MonoBehaviour
    {
        float _base;
        float _phase;

        void Start()
        {
            _base  = Random.Range(0.90f, 1.10f);
            _phase = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            _phase += Time.deltaTime * Random.Range(3f, 5f);
            float s = _base
                    + Mathf.Sin(_phase)         * 0.06f
                    + Mathf.Sin(_phase * 2.37f) * 0.03f;
            transform.localScale = new Vector3(s, s, 1f);
        }
    }
}