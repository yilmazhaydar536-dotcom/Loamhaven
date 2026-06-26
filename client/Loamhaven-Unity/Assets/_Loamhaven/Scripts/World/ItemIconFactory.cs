using System.Collections.Generic;
using UnityEngine;

namespace Loamhaven.World
{
    public static class ItemIconFactory
    {
        static readonly Dictionary<int, Sprite> Cache = new();

        public static Sprite GetIcon(int itemId)
        {
            if (Cache.TryGetValue(itemId, out var hit)) return hit;
            var spr = Build(itemId);
            Cache[itemId] = spr;
            return spr;
        }

        static Sprite Build(int itemId)
        {
            const int S = 16;
            var tex = new Texture2D(S, S, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp,
                name       = $"Icon_{itemId}"
            };

            var px = new Color32[S * S];
            Color32 clear = new Color32(0, 0, 0, 0);
            for (int i = 0; i < px.Length; i++) px[i] = clear;

            Color32 main  = Palette(itemId);
            Color32 light = Lighten(main, 1.35f);
            Color32 dark  = Darken(main,  0.60f);

            for (int y = 2; y < S - 2; y++)
            for (int x = 2; x < S - 2; x++)
            {
                px[y * S + x] = (x == 2 || y == 2)          ? light
                               : (x == S - 3 || y == S - 3) ? dark
                               : main;
            }

            tex.SetPixels32(px);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 16f);
        }

        static Color32 Palette(int id)
        {
            Color32[] p =
            {
                new Color32(180, 100,  40, 255),
                new Color32(160, 160, 160, 255),
                new Color32( 60, 180,  60, 255),
                new Color32(220, 200,  50, 255),
                new Color32(200,  60,  60, 255),
                new Color32(100, 160, 220, 255),
                new Color32(220, 120, 200, 255),
                new Color32(120, 200, 180, 255),
            };
            return p[Mathf.Abs(id) % p.Length];
        }

        static Color32 Darken(Color32 c, float f) =>
            new Color32((byte)(c.r * f), (byte)(c.g * f), (byte)(c.b * f), c.a);

        static Color32 Lighten(Color32 c, float f) =>
            new Color32(
                (byte)Mathf.Min(255, c.r * f),
                (byte)Mathf.Min(255, c.g * f),
                (byte)Mathf.Min(255, c.b * f),
                c.a);
    }
}