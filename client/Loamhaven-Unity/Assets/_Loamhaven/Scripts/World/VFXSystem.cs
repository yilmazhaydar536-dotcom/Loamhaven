using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loamhaven.World
{
    public class VFXSystem : MonoBehaviour
    {
        public static VFXSystem Instance { get; private set; }

        [SerializeField] int _chipPoolSize = 64;
        [SerializeField] int _leafPoolSize = 32;

        readonly Queue<ChipParticle> _chips  = new();
        readonly Queue<ChipParticle> _leaves = new();

        Texture2D _chipTex;
        Texture2D _leafTex;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _chipTex = BuildSolid(2, 2);
            _leafTex = BuildLeaf();

            for (int i = 0; i < _chipPoolSize; i++) _chips.Enqueue(MakeParticle("Chip", _chipTex));
            for (int i = 0; i < _leafPoolSize; i++) _leaves.Enqueue(MakeParticle("Leaf", _leafTex));
        }

        public void SpawnBlockBreak(Vector3 worldPos, Color blockColor)
        {
            int count = UnityEngine.Random.Range(6, 9);
            for (int i = 0; i < count; i++)
            {
                if (!_chips.TryDequeue(out var chip)) break;
                chip.Sr.color = Color.Lerp(blockColor, Color.white, 0.15f);
                Vector2 vel   = UnityEngine.Random.insideUnitCircle.normalized
                              * UnityEngine.Random.Range(2.5f, 5f);
                vel.y = Mathf.Abs(vel.y) + 1f;
                chip.Launch(worldPos, vel, 0.55f, () => _chips.Enqueue(chip));
            }
        }

        public void SpawnHarvest(Vector3 worldPos)
        {
            int count = UnityEngine.Random.Range(4, 7);
            for (int i = 0; i < count; i++)
            {
                if (!_leaves.TryDequeue(out var leaf)) break;
                leaf.Sr.color = new Color(
                    UnityEngine.Random.Range(0.2f, 0.45f),
                    UnityEngine.Random.Range(0.6f, 1.0f),
                    UnityEngine.Random.Range(0.1f, 0.3f));
                Vector2 vel = new Vector2(
                    UnityEngine.Random.Range(-1.5f, 1.5f),
                    UnityEngine.Random.Range(3f, 6f));
                leaf.Launch(worldPos, vel, 0.85f, () => _leaves.Enqueue(leaf));
            }
        }

        ChipParticle MakeParticle(string label, Texture2D tex)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform);
            go.SetActive(false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 16f);
            sr.sortingLayerName = "VFX";
            sr.sortingOrder     = 10;

            var p = go.AddComponent<ChipParticle>();
            p.Sr = sr;
            return p;
        }

        static Texture2D BuildSolid(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };
            var px = new Color[w * h];
            for (int i = 0; i < px.Length; i++) px[i] = Color.white;
            tex.SetPixels(px);
            tex.Apply();
            return tex;
        }

        static Texture2D BuildLeaf()
        {
            const int W = 6, H = 4;
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };
            var px = new Color32[W * H];
            Color32 c = new Color32(80, 200, 60, 255);
            Color32 z = new Color32(0, 0, 0, 0);
            for (int i = 0; i < px.Length; i++) px[i] = z;
            px[0*W+2] = px[0*W+3] = c;
            px[1*W+1] = px[1*W+2] = px[1*W+3] = px[1*W+4] = c;
            for (int i = 0; i < W; i++) px[2*W+i] = c;
            px[3*W+1] = px[3*W+2] = px[3*W+3] = c;
            tex.SetPixels32(px);
            tex.Apply();
            return tex;
        }
    }

    [AddComponentMenu("")]
    public class ChipParticle : MonoBehaviour
    {
        public SpriteRenderer Sr;

        const float Gravity = -12f;

        Vector2 _vel;
        float   _life;
        float   _t;
        Action  _onDone;

        public void Launch(Vector3 pos, Vector2 vel, float lifetime, Action onDone)
        {
            transform.position = pos;
            _vel    = vel;
            _life   = lifetime;
            _t      = 0f;
            _onDone = onDone;
            gameObject.SetActive(true);
        }

        void Update()
        {
            _t     += Time.deltaTime;
            _vel.y += Gravity * Time.deltaTime;
            transform.position += (Vector3)(_vel * Time.deltaTime);
            transform.Rotate(0f, 0f, _vel.x * 90f * Time.deltaTime);

            var col = Sr.color;
            Sr.color = new Color(col.r, col.g, col.b, 1f - _t / _life);

            if (_t >= _life)
            {
                gameObject.SetActive(false);
                _onDone?.Invoke();
            }
        }
    }
}