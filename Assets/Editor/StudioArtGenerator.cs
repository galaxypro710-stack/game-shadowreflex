using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Công cụ sinh Sprite Art độc quyền studio cho ShadowReflex chuẩn Store Anti-Ban.
    /// Tạo ra các bộ Sprite Ninja, Samurai, Kẻ địch và VFX độc quyền không dính DMCA.
    /// </summary>
    public static class StudioArtGenerator
    {
        private const string ArtDirectory = "Assets/_Project/Art/Sprites";
        private const string ResourcesArtDirectory = "Assets/_Project/Resources/Art/Sprites";

        [MenuItem("Tools/ShadowReflex/Generate Studio Art Sprites", false, 2)]
        public static void GenerateAllSprites()
        {
            if (!Directory.Exists(ArtDirectory))
            {
                Directory.CreateDirectory(ArtDirectory);
            }
            if (!Directory.Exists(ResourcesArtDirectory))
            {
                Directory.CreateDirectory(ResourcesArtDirectory);
            }

            GenerateHeroSprite();
            GenerateHeroDashSprite();
            GenerateEnemyScoutSprite();
            GenerateEnemySwiftSprite();
            GenerateEnemyBruteSprite();
            GenerateEnemyBossSprite();
            GenerateSlashVFXSprite();
            GenerateCounterBurstVFXSprite();
            GenerateTelegraphMarkSprite();

            AssetDatabase.Refresh();
            ConfigureTextureImporters();
            SyncToResources();
            AssetDatabase.Refresh();
            Debug.Log("[StudioArtGenerator] Đã tạo thành công 100% bộ Sprite Art độc quyền cho ShadowReflex!");
        }

        private static void SyncToResources()
        {
            string[] files = Directory.GetFiles(ArtDirectory, "*.png");
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string dest = Path.Combine(ResourcesArtDirectory, fileName);
                File.Copy(file, dest, true);
            }
        }

        private static void GenerateHeroSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color bodyColor = new Color(0.12f, 0.12f, 0.18f, 1f); // Dark shadow ninja
            Color accentColor = new Color(0f, 0.95f, 1f, 1f);      // Cyan neon scarf & katana edge
            Color skinColor = new Color(0.25f, 0.25f, 0.35f, 1f);

            // Thân mình & áo choàng
            DrawCapsule(tex, 64, 50, 24, 45, bodyColor);
            // Đầu ninja với mặt nạ
            DrawCircle(tex, 64, 88, 16, bodyColor);
            // Dải khăn Ninja bay trong gió
            DrawLine(tex, 56, 88, 25, 95, 4, accentColor);
            DrawLine(tex, 25, 95, 10, 85, 3, accentColor);
            // Đôi mắt sắc lẹm (Cyan Neon)
            DrawPixelBox(tex, 68, 90, 6, 2, accentColor);
            // Cánh tay cầm kiếm
            DrawCapsule(tex, 78, 62, 8, 22, bodyColor);
            // Thanh Katana sắc bén
            DrawLine(tex, 82, 60, 115, 100, 3, Color.white);
            DrawLine(tex, 80, 58, 113, 98, 2, accentColor);
            // Chuôi kiếm
            DrawLine(tex, 82, 60, 72, 48, 4, new Color(0.8f, 0.2f, 0.2f, 1f));

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Hero_Ninja.png"));
        }

        private static void GenerateHeroDashSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color ghostColor = new Color(0f, 0.8f, 1f, 0.5f); // Neon cyan ghost trail
            DrawCapsule(tex, 50, 50, 20, 40, ghostColor);
            DrawCircle(tex, 60, 80, 14, ghostColor);
            // Vệt gió lướt
            for (int i = 0; i < 5; i++)
            {
                int y = 30 + i * 15;
                DrawLine(tex, 10, y, 70, y + 10, 2, new Color(0.3f, 0.9f, 1f, 0.4f));
            }

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Hero_Dash.png"));
        }

        private static void GenerateEnemyScoutSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color bodyColor = new Color(0.22f, 0.15f, 0.15f, 1f); // Dark crimson scout
            Color eyeColor = new Color(1f, 0.5f, 0f, 1f);         // Cam rực
            Color weaponColor = new Color(0.85f, 0.85f, 0.9f, 1f);

            // Thân & đầu
            DrawCapsule(tex, 64, 52, 22, 42, bodyColor);
            DrawCircle(tex, 64, 88, 15, bodyColor);
            // Mắt cam báo động
            DrawPixelBox(tex, 54, 90, 6, 3, eyeColor);
            // Song đao ngắn (Kunai / Dagger)
            DrawLine(tex, 50, 50, 25, 30, 3, weaponColor);
            DrawLine(tex, 78, 50, 100, 30, 3, weaponColor);

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Enemy_Scout.png"));
        }

        private static void GenerateEnemySwiftSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color bodyColor = new Color(0.15f, 0.10f, 0.25f, 1f); // Purple shadow assassin
            Color speedColor = new Color(1f, 0.85f, 0f, 1f);      // Vàng neon
            Color bladeColor = new Color(0.9f, 0.3f, 1f, 1f);

            // Dáng nghiêng tiến công cực nhanh
            DrawCapsule(tex, 64, 50, 18, 48, bodyColor);
            DrawCircle(tex, 72, 86, 13, bodyColor);
            DrawPixelBox(tex, 66, 88, 5, 2, speedColor);
            // Song kiếm dài vuốt nhọn
            DrawLine(tex, 45, 60, 15, 85, 3, bladeColor);
            DrawLine(tex, 85, 60, 115, 85, 3, bladeColor);

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Enemy_Swift.png"));
        }

        private static void GenerateEnemyBruteSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color bodyColor = new Color(0.18f, 0.18f, 0.22f, 1f); // Giáp sắt nặng
            Color armorEdge = new Color(0.5f, 0.5f, 0.6f, 1f);
            Color eyeColor = new Color(1f, 0.2f, 0.1f, 1f);

            // Thân giáp đồ sộ
            DrawCapsule(tex, 64, 55, 36, 50, bodyColor);
            DrawCapsule(tex, 64, 55, 32, 46, armorEdge);
            DrawCircle(tex, 64, 94, 18, bodyColor);
            DrawPixelBox(tex, 58, 96, 8, 3, eyeColor);

            // Cây búa tạ khổng lồ
            DrawLine(tex, 90, 20, 110, 95, 6, new Color(0.4f, 0.3f, 0.2f, 1f)); // Cán búa
            DrawPixelBox(tex, 98, 85, 26, 20, new Color(0.35f, 0.35f, 0.45f, 1f)); // Đầu búa vuông nặng

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Enemy_Brute.png"));
        }

        private static void GenerateEnemyBossSprite()
        {
            int w = 144, h = 144;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color shogunBody = new Color(0.10f, 0.08f, 0.12f, 1f);
            Color goldTrim = new Color(1f, 0.8f, 0.2f, 1f);        // Viền hoàng kim
            Color demonRed = new Color(1f, 0.1f, 0.2f, 1f);

            // Giáp vai Samurai (Sode)
            DrawPixelBox(tex, 25, 70, 24, 32, shogunBody);
            DrawPixelBox(tex, 95, 70, 24, 32, shogunBody);
            DrawLine(tex, 25, 102, 49, 102, 3, goldTrim);
            DrawLine(tex, 95, 102, 119, 102, 3, goldTrim);

            // Thân chính
            DrawCapsule(tex, 72, 60, 32, 56, shogunBody);
            // Mũ chiến Kabuto với sừng quỷ
            DrawCircle(tex, 72, 104, 20, shogunBody);
            DrawLine(tex, 72, 118, 55, 138, 4, goldTrim); // Sừng trái
            DrawLine(tex, 72, 118, 89, 138, 4, goldTrim); // Sừng phải
            // Mặt nạ Hannya mắt quỷ đỏ rực
            DrawPixelBox(tex, 64, 105, 6, 3, demonRed);
            DrawPixelBox(tex, 76, 105, 6, 3, demonRed);

            // Đại đao Nodachi
            DrawLine(tex, 100, 30, 135, 130, 4, Color.white);
            DrawLine(tex, 98, 28, 133, 128, 2, demonRed);

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "Enemy_Boss.png"));
        }

        private static void GenerateSlashVFXSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color coreColor = Color.white;
            Color auraColor = new Color(0f, 0.95f, 1f, 0.85f); // Cyan kiếm khí

            // Vẽ vòng cung kiếm chém sắc lạnh
            for (float t = 0f; t <= 1f; t += 0.005f)
            {
                float angle = Mathf.Lerp(-40f, 140f, t) * Mathf.Deg2Rad;
                float radius = Mathf.Lerp(30f, 55f, Mathf.Sin(t * Mathf.PI));
                int cx = 64 + Mathf.RoundToInt(Mathf.Cos(angle) * radius);
                int cy = 64 + Mathf.RoundToInt(Mathf.Sin(angle) * radius);

                float width = Mathf.Sin(t * Mathf.PI) * 5f;
                DrawCircle(tex, cx, cy, Mathf.Max(1, Mathf.RoundToInt(width)), auraColor);
                DrawCircle(tex, cx, cy, Mathf.Max(1, Mathf.RoundToInt(width * 0.5f)), coreColor);
            }

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "VFX_Slash.png"));
        }

        private static void GenerateCounterBurstVFXSprite()
        {
            int w = 128, h = 128;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color burstGold = new Color(1f, 0.9f, 0.2f, 0.9f);
            Color burstWhite = Color.white;

            // Vòng nổ hào quang 8 hướng
            DrawCircle(tex, 64, 64, 25, new Color(1f, 0.8f, 0.1f, 0.4f));
            DrawCircle(tex, 64, 64, 12, burstWhite);

            for (int i = 0; i < 8; i++)
            {
                float ang = i * 45f * Mathf.Deg2Rad;
                int x2 = 64 + Mathf.RoundToInt(Mathf.Cos(ang) * 56f);
                int y2 = 64 + Mathf.RoundToInt(Mathf.Sin(ang) * 56f);
                DrawLine(tex, 64, 64, x2, y2, 3, burstGold);
                DrawLine(tex, 64, 64, x2, y2, 1, burstWhite);
            }

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "VFX_CounterBurst.png"));
        }

        private static void GenerateTelegraphMarkSprite()
        {
            int w = 64, h = 64;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            FillTransparent(tex, w, h);

            Color warnColor = new Color(1f, 0.2f, 0.1f, 1f); // Đỏ rực nguy hiểm
            Color yellowGlow = new Color(1f, 0.8f, 0.1f, 0.9f);

            // Dấu chấm than cảnh báo ra đòn (!)
            DrawCapsule(tex, 32, 42, 6, 20, yellowGlow);
            DrawCapsule(tex, 32, 42, 4, 18, warnColor);
            DrawCircle(tex, 32, 16, 5, yellowGlow);
            DrawCircle(tex, 32, 16, 3, warnColor);

            SaveTextureAsPNG(tex, Path.Combine(ArtDirectory, "VFX_TelegraphMark.png"));
        }

        // ================= HELPER GRAPHICS PRIMITIVES =================

        private static void FillTransparent(Texture2D tex, int w, int h)
        {
            Color[] clear = new Color[w * h];
            for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
            tex.SetPixels(clear);
        }

        private static void DrawCircle(Texture2D tex, int cx, int cy, int r, Color col)
        {
            for (int y = -r; y <= r; y++)
            {
                for (int x = -r; x <= r; x++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            tex.SetPixel(px, py, BlendColor(tex.GetPixel(px, py), col));
                        }
                    }
                }
            }
        }

        private static void DrawCapsule(Texture2D tex, int cx, int cy, int width, int height, Color col)
        {
            int hw = width / 2;
            int hh = height / 2;
            for (int y = -hh; y <= hh; y++)
            {
                for (int x = -hw; x <= hw; x++)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    {
                        tex.SetPixel(px, py, BlendColor(tex.GetPixel(px, py), col));
                    }
                }
            }
        }

        private static void DrawPixelBox(Texture2D tex, int x, int y, int w, int h, Color col)
        {
            for (int dy = 0; dy < h; dy++)
            {
                for (int dx = 0; dx < w; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    {
                        tex.SetPixel(px, py, BlendColor(tex.GetPixel(px, py), col));
                    }
                }
            }
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, int thickness, Color col)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawCircle(tex, x0, y0, thickness / 2, col);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private static Color BlendColor(Color dst, Color src)
        {
            float a = src.a + dst.a * (1f - src.a);
            if (a <= 0f) return Color.clear;
            Color outCol = (src * src.a + dst * dst.a * (1f - src.a)) / a;
            outCol.a = a;
            return outCol;
        }

        private static void SaveTextureAsPNG(Texture2D tex, string path)
        {
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(tex);
        }

        private static void ConfigureTextureImporters()
        {
            string[] files = Directory.GetFiles(ArtDirectory, "*.png");
            foreach (string file in files)
            {
                string assetPath = file.Replace("\\", "/");
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 64;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.alphaIsTransparency = true;
                    importer.SaveAndReimport();
                }
            }
        }
    }
}
