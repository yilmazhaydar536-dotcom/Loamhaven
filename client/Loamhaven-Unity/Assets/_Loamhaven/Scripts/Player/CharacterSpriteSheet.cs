using UnityEngine;

namespace Loamhaven.Player
{
    // Priority 2: Generates a 16x32 side-view character with 6 shirt-colour variants.
    // Colour is assigned by Abs(accountId.GetHashCode()) % 6 so all clients agree without
    // any server message.
    public static class CharacterSpriteSheet
    {
        const int W = 16;
        const int H = 32;

        static readonly Color32[] ShirtColors =
        {
            new Color32(220,  50,  50, 255), // red
            new Color32( 50,  80, 220, 255), // blue
            new Color32( 50, 180,  60, 255), // green
            new Color32(220, 180,  40, 255), // yellow
            new Color32(160,  50, 200, 255), // purple
            new Color32(220, 120,  30, 255), // orange
        };

        static readonly Color32 Skin    = new Color32(255, 210, 160, 255);
        static readonly Color32 Hair    = new Color32(100,  65,  20, 255);
        static readonly Color32 Pants   = new Color32( 60,  60, 120, 255);
        static readonly Color32 Shoe    = new Color32( 60,  40,  20, 255);
        static readonly Color32 Eye     = new Color32( 30,  30,  30, 255);
        static readonly Color32 Outline = new Color32(  0,   0,   0, 200);
        static readonly Color32 Clear   = new Color32(  0,   0,   0,   0);

        public static Sprite CreateSprite(int accountId)
        {
            int idx = Mathf.Abs(accountId.GetHashCode()) % ShirtColors.Length;
            return Build(ShirtColors[idx]);
        }

        static Sprite Build(Color32 shirt)
        {
            var tex = new Texture2D(W, H, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp,
                name       = "CharacterTex"
            };

            var px = new Color32[W * H];
            for (int i = 0; i < px.Length; i++) px[i] = Clear;

            // Head
            FillRect(px,  5, 22, 6, 8, Skin);
            FillRect(px,  5, 28, 6, 4, Hair);
            FillRect(px,  4, 26, 1, 4, Hair);
            FillRect(px, 11, 26, 1, 4, Hair);
            Set(px,  7, 25, Eye);
            Set(px,  9, 25, Eye);

            // Neck
            FillRect(px, 6, 21, 4, 1, Skin);

            // Torso + arms
            FillRect(px,  3, 14, 10, 7, shirt);
            FillRect(px,  1, 14,  2, 7, shirt); // left arm
            FillRect(px, 13, 14,  2, 7, shirt); // right arm
            FillRect(px,  1, 12,  2, 2, Skin);  // left hand
            FillRect(px, 13, 12,  2, 2, Skin);  // right hand

            // Pants
            FillRect(px, 3,  7, 5, 7, Pants);
            FillRect(px, 8,  7, 5, 7, Pants);

            // Shoes
            FillRect(px, 3, 5, 5, 2, Shoe);
            FillRect(px, 8, 5, 5, 2, Shoe);

            AddOutline(px);

            tex.SetPixels32(px);
            tex.Apply();

            return Sprite.Create(tex,
                new Rect(0, 0, W, H),
                new Vector2(0.5f, 0f), // pivot at feet
                16f);
        }

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

        // 4-neighbour outline: draw Outline on transparent pixels adjacent to opaque ones.
        static void AddOutline(Color32[] px)
        {
            var src = (Color32[])px.Clone();
            int[] ox = { -1, 1,  0, 0 };
            int[] oy = {  0, 0, -1, 1 };
            for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (src[y * W + x].a > 0) continue;
                for (int d = 0; d < 4; d++)
                {
                    int nx = x + ox[d], ny = y + oy[d];
                    if (nx < 0 || nx >= W || ny < 0 || ny >= H) continue;
                    if (src[ny * W + nx].a > 0) { px[y * W + x] = Outline; break; }
                }
            }
        }
    }
}
