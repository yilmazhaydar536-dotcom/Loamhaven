using System.Collections.Generic;
using UnityEngine;

namespace Loamhaven.World
{
    public static class PlantSpriteFactory
    {
        public enum PlantType   { Wheat, Carrot, Potato, Tomato }
        public enum GrowthStage { Sprout = 0, Growing = 1, Mature = 2 }

        static readonly Dictionary<(PlantType, GrowthStage), Sprite> Cache = new();

        const int W = 16;
        const int H = 32;

        public static Sprite GetSprite(PlantType type, GrowthStage stage)
        {
            var key = (type, stage);
            if (Cache.TryGetValue(key, out var hit)) return hit;
            var spr = Build(type, stage);
            Cache[key] = spr;
            return spr;
        }

        static Sprite Build(PlantType type, GrowthStage stage)
        {
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp,
                name       = $"Plant_{type}_{stage}"
            };

            var px = new Color32[W * H];
            Color32 clear = new Color32(0, 0, 0, 0);
            for (int i = 0; i < px.Length; i++) px[i] = clear;

            Color32 stem  = StemColor(type);
            Color32 leaf  = LeafColor(type);
            Color32 fruit = FruitColor(type);

            int stemH = stage switch
            {
                GrowthStage.Sprout  =>  5,
                GrowthStage.Growing => 13,
                GrowthStage.Mature  => 22,
                _                   =>  8
            };

            int cx = W / 2;
            for (int y = 0; y < stemH; y++) Set(px, cx, y, stem);

            if (stage == GrowthStage.Sprout)
            {
                Set(px, cx - 1, 3, leaf);
                Set(px, cx + 1, 3, leaf);
                Set(px, cx,     4, leaf);
            }
            else
            {
                int ly1 = stemH / 3;
                int ly2 = stemH * 2 / 3;
                FillRect(px, cx - 3, ly1, 3, 2, leaf);
                FillRect(px, cx + 1, ly2, 3, 2, leaf);
            }

            if (stage == GrowthStage.Mature)
            {
                int fy = stemH - 5;
                FillRect(px, cx - 2, fy, 5, 5, fruit);
                Set(px, cx - 2, fy,     Darken(fruit, 0.65f));
                Set(px, cx + 2, fy,     Darken(fruit, 0.65f));
                Set(px, cx - 2, fy + 4, Darken(fruit, 0.65f));
                Set(px, cx + 2, fy + 4, Darken(fruit, 0.65f));
                Set(px, cx,     fy + 4, Lighten(fruit, 1.25f));
            }

            tex.SetPixels32(px);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, W, H), new Vector2(0.5f, 0f), 16f);
        }

        static Color32 StemColor(PlantType t) => t switch
        {
            PlantType.Wheat  => new Color32( 80, 160, 60, 255),
            PlantType.Carrot => new Color32( 60, 140, 40, 255),
            PlantType.Potato => new Color32( 70, 130, 50, 255),
            PlantType.Tomato => new Color32( 50, 150, 50, 255),
            _                => new Color32( 70, 140, 55, 255)
        };

        static Color32 LeafColor(PlantType t) => t switch
        {
            PlantType.Wheat  => new Color32(110, 200,  80, 255),
            PlantType.Carrot => new Color32( 60, 210,  50, 255),
            PlantType.Potato => new Color32( 80, 185,  70, 255),
            PlantType.Tomato => new Color32( 55, 205,  60, 255),
            _                => new Color32( 80, 200,  70, 255)
        };

        static Color32 FruitColor(PlantType t) => t switch
        {
            PlantType.Wheat  => new Color32(220, 190,  70, 255),
            PlantType.Carrot => new Color32(240, 130,  30, 255),
            PlantType.Potato => new Color32(165, 135,  75, 255),
            PlantType.Tomato => new Color32(220,  50,  50, 255),
            _                => new Color32(200, 100,  50, 255)
        };

        static void Set(Color32[] px, int x, int y, Color32 c)
        {
            if (x < 0 || x >= W || y < 0 || y >= H) return;
            px[y * W + x] = c;
        }

        static void FillRect(Color32[] px, int x, int y, int w, int h, Color32 c)
        {
            for (int dy = 0; dy < h; dy++)
            for (int dx = 0; dx < w; dx++)
                Set(px, x + dx, y + dy, c);
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