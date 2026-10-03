using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Công cụ tổng hợp âm thanh thủ tục (Procedural Sound Synthesizer) độc quyền cho ShadowReflex.
    /// Tạo ra các file .WAV nguyên bản cho Slash, Whoosh (Dodge), Parry Clang và Hit Impact.
    /// 100% bản quyền studio, zero DMCA risk.
    /// </summary>
    public static class AudioSynthesizerGenerator
    {
        private const string AudioDirectory = "Assets/_Project/Audio/Clips";
        private const string ResourcesAudioDirectory = "Assets/_Project/Resources/Audio/Clips";
        private const int SampleRate = 44100;

        [MenuItem("Tools/ShadowReflex/Generate Studio Audio SFX", false, 3)]
        public static void GenerateAllSFX()
        {
            if (!Directory.Exists(AudioDirectory))
            {
                Directory.CreateDirectory(AudioDirectory);
            }
            if (!Directory.Exists(ResourcesAudioDirectory))
            {
                Directory.CreateDirectory(ResourcesAudioDirectory);
            }

            GenerateSlashWav();
            GenerateWhooshWav();
            GenerateParryClangWav();
            GenerateHitImpactWav();
            GenerateTelegraphWarningWav();
            GenerateClickWav();
            GenerateRewardWav();
            GenerateZenCombatBgmWav();

            // Đồng bộ sang thư mục Resources để đảm bảo runtime Android độc lập 100%
            SyncToResources();

            AssetDatabase.Refresh();
            Debug.Log("[AudioSynthesizerGenerator] Đã tổng hợp thành công 100% SFX & BGM độc quyền cho ShadowReflex!");
        }

        private static void SyncToResources()
        {
            string[] files = Directory.GetFiles(AudioDirectory, "*.wav");
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string dest = Path.Combine(ResourcesAudioDirectory, fileName);
                File.Copy(file, dest, true);
            }
        }

        private static void GenerateSlashWav()
        {
            // Âm thanh kiếm chém sắc nhọn: White noise quét tần số cao giảm nhanh + sine tone vuốt xuống
            float duration = 0.22f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rnd = new System.Random(42);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Pow(1f - progress, 2.5f);

                // High frequency sweep
                float freq = Mathf.Lerp(2200f, 400f, progress);
                float tone = Mathf.Sin(2f * Mathf.PI * freq * t);
                float noise = ((float)rnd.NextDouble() * 2f - 1f) * 0.7f;

                samples[i] = (tone * 0.5f + noise * 0.5f) * env * 0.8f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_slash.wav"), samples, SampleRate);
        }

        private static void GenerateWhooshWav()
        {
            // Âm thanh lướt gió né đòn: Bandpass filtered noise vuốt hình chuông gaussian
            float duration = 0.28f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rnd = new System.Random(1337);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                // Chuông envelope mượt mà
                float env = Mathf.Sin(progress * Mathf.PI);

                float freq = Mathf.Lerp(300f, 900f, Mathf.Sin(progress * Mathf.PI));
                float tone = Mathf.Sin(2f * Mathf.PI * freq * t);
                float noise = ((float)rnd.NextDouble() * 2f - 1f);

                samples[i] = (tone * 0.3f + noise * 0.7f) * env * 0.65f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_whoosh.wav"), samples, SampleRate);
        }

        private static void GenerateParryClangWav()
        {
            // Âm thanh đao kiếm chạm nhau đanh thép vang dội (Parry Clang / Perfect Counter)
            // Kết hợp nhiều họa âm kim loại (Inharmonic metal frequencies: 1200Hz, 2450Hz, 3800Hz, 5100Hz)
            float duration = 0.45f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 9f); // Tiếng ngân kim loại tắt dần

                float h1 = Mathf.Sin(2f * Mathf.PI * 1400f * t) * 0.45f;
                float h2 = Mathf.Sin(2f * Mathf.PI * 2760f * t) * 0.35f;
                float h3 = Mathf.Sin(2f * Mathf.PI * 4120f * t) * 0.20f;
                float h4 = Mathf.Sin(2f * Mathf.PI * 5680f * t) * 0.15f;

                samples[i] = (h1 + h2 + h3 + h4) * env * 0.9f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_parry_clang.wav"), samples, SampleRate);
        }

        private static void GenerateHitImpactWav()
        {
            // Âm thanh trúng đòn uy lực: Xung trầm (80Hz sub-bass punch) + crack tiếng va chạm
            float duration = 0.25f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rnd = new System.Random(999);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 12f);

                float bass = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(160f, 50f, progress) * t);
                float crack = ((float)rnd.NextDouble() * 2f - 1f) * Mathf.Exp(-progress * 30f);

                samples[i] = (bass * 0.7f + crack * 0.5f) * env * 0.85f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_hit_impact.wav"), samples, SampleRate);
        }

        private static void GenerateTelegraphWarningWav()
        {
            // Âm thanh cảnh báo quái giương đòn (Telegraph Danger Sting): Họa âm 1200Hz + 1760Hz
            float duration = 0.20f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 14f); // Tắt nhanh, đanh sắc

                float t1 = Mathf.Sin(2f * Mathf.PI * 1320f * t) * 0.6f;
                float t2 = Mathf.Sin(2f * Mathf.PI * 1760f * t) * 0.4f;

                samples[i] = (t1 + t2) * env * 0.85f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_telegraph_warning.wav"), samples, SampleRate);
        }

        private static void GenerateClickWav()
        {
            // Âm thanh gõ gỗ UI Button Click
            float duration = 0.05f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rnd = new System.Random(77);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                float progress = t / duration;
                float env = Mathf.Exp(-progress * 40f);

                float tone = Mathf.Sin(2f * Mathf.PI * 880f * t) * 0.5f;
                float noise = ((float)rnd.NextDouble() * 2f - 1f) * 0.5f;

                samples[i] = (tone + noise) * env * 0.8f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_click.wav"), samples, SampleRate);
        }

        private static void GenerateRewardWav()
        {
            // Âm thanh nhận thưởng vàng/nâng cấp (Ascending triad: C5 - E5 - G5)
            float noteDur = 0.12f;
            float duration = noteDur * 3f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            float[] freqs = new float[] { 523.25f, 659.25f, 783.99f };

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;
                int noteIndex = Mathf.Min((int)(t / noteDur), 2);
                float noteT = t - (noteIndex * noteDur);
                float env = Mathf.Exp(-(noteT / noteDur) * 4f);

                float freq = freqs[noteIndex];
                float tone = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.7f;
                float overtone = Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.3f;

                samples[i] = (tone + overtone) * env * 0.75f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "sfx_reward.wav"), samples, SampleRate);
        }

        private static void GenerateZenCombatBgmWav()
        {
            // Nhạc nền chiến đấu Zen Martial (Loop 4.0 giây tại nhịp 120 BPM)
            // Nhịp trống Taiko (Beat 0s, 2s) + Gõ phách Rim (Beat 1s, 3s) + Âm vang Drone huyền ảo
            float duration = 4.0f;
            int totalSamples = (int)(SampleRate * duration);
            float[] samples = new float[totalSamples];
            System.Random rnd = new System.Random(555);

            for (int i = 0; i < totalSamples; i++)
            {
                float t = (float)i / SampleRate;

                // 1. Taiko Drum Kick trên beat 0.0s và 2.0s
                float taiko1 = GetDrumSample(t - 0.0f, rnd);
                float taiko2 = GetDrumSample(t - 2.0f, rnd);

                // 2. Wooden Rim Click trên beat 1.0s và 3.0s
                float rim1 = GetRimSample(t - 1.0f, rnd);
                float rim2 = GetRimSample(t - 3.0f, rnd);

                // 3. Drone nền Zen (A2 = 110Hz + LFO nhẹ nhàng)
                float lfo = 0.8f + 0.2f * Mathf.Sin(2f * Mathf.PI * 0.5f * t);
                float drone = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.12f * lfo;
                float subDrone = Mathf.Sin(2f * Mathf.PI * 55f * t) * 0.10f;

                samples[i] = (taiko1 + taiko2 + rim1 + rim2 + drone + subDrone) * 0.85f;
            }

            SaveWavFile(Path.Combine(AudioDirectory, "bgm_combat_zen.wav"), samples, SampleRate);
        }

        private static float GetDrumSample(float dt, System.Random rnd)
        {
            if (dt < 0f || dt > 0.45f) return 0f;
            float progress = dt / 0.45f;
            float env = Mathf.Exp(-progress * 8f);
            float freq = Mathf.Lerp(120f, 45f, progress);
            float tone = Mathf.Sin(2f * Mathf.PI * freq * dt) * 0.6f;
            float punch = ((float)rnd.NextDouble() * 2f - 1f) * Mathf.Exp(-progress * 25f) * 0.4f;
            return (tone + punch) * env;
        }

        private static float GetRimSample(float dt, System.Random rnd)
        {
            if (dt < 0f || dt > 0.15f) return 0f;
            float progress = dt / 0.15f;
            float env = Mathf.Exp(-progress * 20f);
            float tone = Mathf.Sin(2f * Mathf.PI * 950f * dt) * 0.3f;
            float click = ((float)rnd.NextDouble() * 2f - 1f) * 0.7f;
            return (tone + click) * env;
        }

        // ================= WAV FILE WRITER =================

        private static void SaveWavFile(string filePath, float[] samples, int sampleRate)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                int channels = 1;
                int bitsPerSample = 16;
                int byteRate = sampleRate * channels * (bitsPerSample / 8);
                short blockAlign = (short)(channels * (bitsPerSample / 8));
                int subChunk2Size = samples.Length * (bitsPerSample / 8);
                int chunkSize = 36 + subChunk2Size;

                // RIFF chunk descriptor
                writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
                writer.Write(chunkSize);
                writer.Write(new char[4] { 'W', 'A', 'V', 'E' });

                // "fmt " sub-chunk
                writer.Write(new char[4] { 'f', 'm', 't', ' ' });
                writer.Write(16); // SubChunk1Size (16 for PCM)
                writer.Write((short)1); // AudioFormat (1 for PCM)
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(byteRate);
                writer.Write(blockAlign);
                writer.Write((short)bitsPerSample);

                // "data" sub-chunk
                writer.Write(new char[4] { 'd', 'a', 't', 'a' });
                writer.Write(subChunk2Size);

                // Write 16-bit PCM samples
                for (int i = 0; i < samples.Length; i++)
                {
                    short val = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
                    writer.Write(val);
                }
            }
        }
    }
}
