import os
import sys
import math
import struct
import wave
import random
import uuid

sys.stdout.reconfigure(encoding='utf-8', errors='ignore')
sys.stderr.reconfigure(encoding='utf-8', errors='ignore')
from PIL import Image, ImageDraw, ImageFilter

BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART_DIR = os.path.join(BASE_DIR, "Assets", "_Project", "Art")
AUDIO_DIR = os.path.join(BASE_DIR, "Assets", "_Project", "Audio")

CHAR_DIR = os.path.join(ART_DIR, "Characters")
ENV_DIR = os.path.join(ART_DIR, "Environment")
VFX_DIR = os.path.join(ART_DIR, "VFX")

def write_dir_meta(dirpath):
    metapath = dirpath + ".meta"
    if not os.path.exists(metapath):
        g = uuid.uuid4().hex
        with open(metapath, "w", encoding="utf-8") as f:
            f.write(f"fileFormatVersion: 2\nguid: {g}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {{}}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n")

def write_sprite_meta(filepath):
    metapath = filepath + ".meta"
    if not os.path.exists(metapath):
        g = uuid.uuid4().hex
        content = f"""fileFormatVersion: 2
guid: {g}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 0
    wrapV: 0
    wrapW: 0
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationMethod: 0
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbook: 0
  ignorePngGamma: 0
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
        with open(metapath, "w", encoding="utf-8") as f:
            f.write(content)

def write_audio_meta(filepath):
    metapath = filepath + ".meta"
    if not os.path.exists(metapath):
        g = uuid.uuid4().hex
        content = f"""fileFormatVersion: 2
guid: {g}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
    preloadAudioData: 1
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
        with open(metapath, "w", encoding="utf-8") as f:
            f.write(content)

os.makedirs(CHAR_DIR, exist_ok=True)
os.makedirs(ENV_DIR, exist_ok=True)
os.makedirs(VFX_DIR, exist_ok=True)
os.makedirs(AUDIO_DIR, exist_ok=True)

write_dir_meta(ART_DIR)
write_dir_meta(CHAR_DIR)
write_dir_meta(ENV_DIR)
write_dir_meta(VFX_DIR)
write_dir_meta(AUDIO_DIR)

print(f"[*] Base directory: {BASE_DIR}")

# ==========================================
# 1. AUDIO GENERATION ENGINE (16-bit PCM WAV)
# ==========================================
SAMPLE_RATE = 44100

def write_wav(filename, samples):
    filepath = os.path.join(AUDIO_DIR, filename)
    with wave.open(filepath, 'w') as wav_file:
        wav_file.setnchannels(1) # Mono
        wav_file.setsampwidth(2) # 16-bit
        wav_file.setframerate(SAMPLE_RATE)
        # Clamp to 16-bit signed integer
        data = bytearray()
        for s in samples:
            s_clamped = max(-1.0, min(1.0, s))
            val = int(s_clamped * 32767.0)
            data.extend(struct.pack('<h', val))
        wav_file.writeframes(data)
    print(f"[Audio] Created: {filename} ({len(samples)/SAMPLE_RATE:.2f}s)")

# SFX: Slash (Fast frequency sweep + white noise)
def gen_sfx_slash():
    duration = 0.22
    n_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        env = math.exp(-t * 18.0) * math.sin(math.pi * min(1.0, t / 0.03))
        freq = 1400.0 * math.exp(-t * 12.0) + 180.0
        tone = math.sin(2 * math.pi * freq * t)
        noise = (random.random() * 2.0 - 1.0) * 0.6
        s = (tone * 0.5 + noise * 0.5) * env
        samples.append(s * 0.85)
    write_wav("sfx_slash.wav", samples)

# SFX: Dodge Whoosh (Low-frequency gust)
def gen_sfx_dodge():
    duration = 0.28
    n_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        env = math.sin(math.pi * (t / duration)) ** 1.8
        freq = 320.0 * (1.0 - t / duration) + 90.0
        tone = math.sin(2 * math.pi * freq * t)
        noise = (random.random() * 2.0 - 1.0) * 0.45
        s = (tone * 0.4 + noise * 0.6) * env
        samples.append(s * 0.8)
    write_wav("sfx_dodge_whoosh.wav", samples)

# SFX: Parry Clang (High-pitch metal-on-metal clash)
def gen_sfx_parry():
    duration = 0.50
    n_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        env = math.exp(-t * 9.0)
        h1 = math.sin(2 * math.pi * 2350.0 * t) * 0.45
        h2 = math.sin(2 * math.pi * 3820.0 * t) * 0.35
        h3 = math.sin(2 * math.pi * 5140.0 * t) * 0.20
        sparkle = (random.random() * 2.0 - 1.0) * math.exp(-t * 50.0) * 0.4
        s = (h1 + h2 + h3 + sparkle) * env
        samples.append(s * 0.9)
    write_wav("sfx_parry_clang.wav", samples)

# SFX: Hit Impact (Low punchy thud)
def gen_sfx_hit():
    duration = 0.25
    n_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        env = math.exp(-t * 16.0)
        freq = 240.0 * math.exp(-t * 25.0) + 55.0
        tone = math.sin(2 * math.pi * freq * t)
        snap = (random.random() * 2.0 - 1.0) * math.exp(-t * 40.0) * 0.7
        s = (tone * 0.6 + snap * 0.4) * env
        samples.append(s * 0.85)
    write_wav("sfx_hit_impact.wav", samples)

# SFX: Enemy Alert / Telegraph Chime
def gen_sfx_alert():
    duration = 0.24
    n_samples = int(duration * SAMPLE_RATE)
    samples = []
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        env = math.exp(-t * 12.0)
        tone = (math.sin(2 * math.pi * 1480.0 * t) * 0.7 +
                math.sin(2 * math.pi * 2960.0 * t) * 0.3)
        samples.append(tone * env * 0.75)
    write_wav("sfx_enemy_alert.wav", samples)

# BGM: Shadow Battle Rhythm (Loopable drums + drone)
def gen_bgm_battle():
    duration = 4.0 # 4-bar loop
    n_samples = int(duration * SAMPLE_RATE)
    samples = [0.0] * n_samples
    
    # Bass drone
    for i in range(n_samples):
        t = i / SAMPLE_RATE
        drone = (math.sin(2 * math.pi * 55.0 * t) * 0.15 +
                 math.sin(2 * math.pi * 110.0 * t) * 0.10)
        samples[i] += drone

    # Taiko beats at 0.0s, 0.5s, 1.0s, 1.5s, 2.0s, 2.35s, 2.65s, 3.0s, 3.5s
    beat_times = [0.0, 0.5, 1.0, 1.5, 2.0, 2.35, 2.65, 3.0, 3.5]
    for bt in beat_times:
        start_idx = int(bt * SAMPLE_RATE)
        hit_len = int(0.35 * SAMPLE_RATE)
        for j in range(hit_len):
            idx = start_idx + j
            if idx >= n_samples: break
            t_hit = j / SAMPLE_RATE
            env = math.exp(-t_hit * 11.0)
            freq = 95.0 * math.exp(-t_hit * 18.0) + 40.0
            taiko = math.sin(2 * math.pi * freq * t_hit) * env * 0.45
            samples[idx] += taiko

    write_wav("bgm_shadow_battle.wav", samples)

# ==========================================
# 2. SPRITE VISUAL GENERATION (PIL PNGs)
# ==========================================
SIZE = 256
SILHOUETTE_BODY = (18, 18, 26, 255)      # Deep shadow obsidian
SILHOUETTE_ARMOR = (35, 35, 48, 255)     # Shadow plate highlight
HERO_GLOW_CYAN = (0, 230, 255, 255)      # Glowing cyan headband/eyes
ENEMY_SCOUT_ORANGE = (255, 140, 20, 255) # Scout warning glow
RONIN_CRIMSON = (255, 40, 40, 255)       # Ronin strike glow
BLADE_STEEL = (220, 225, 240, 255)       # Razor edge highlight

def create_hero_idle():
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 110, 140
    # Head & Mask
    d.ellipse([cx-18, cy-75, cx+18, cy-40], fill=SILHOUETTE_BODY)
    # Headband Scarf trailing left
    d.polygon([(cx-10, cy-58), (cx-55, cy-70), (cx-75, cy-62), (cx-45, cy-52)], fill=HERO_GLOW_CYAN)
    # Slit eye glow
    d.line([(cx+3, cy-56), (cx+12, cy-57)], fill=(255, 255, 255, 255), width=3)
    # Torso & Gi
    d.polygon([(cx-22, cy-42), (cx+22, cy-42), (cx+18, cy+15), (cx-18, cy+15)], fill=SILHOUETTE_BODY)
    d.line([(cx-10, cy-40), (cx+10, cy+10)], fill=SILHOUETTE_ARMOR, width=3)
    # Legs (Combat Stance)
    d.polygon([(cx-18, cy+12), (cx-35, cy+65), (cx-42, cy+95), (cx-26, cy+95), (cx-12, cy+55)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+14, cy+12), (cx+32, cy+55), (cx+45, cy+95), (cx+30, cy+95), (cx+16, cy+55)], fill=SILHOUETTE_BODY)
    # Katana on back / ready
    d.line([(cx-35, cy+10), (cx+55, cy-60)], fill=BLADE_STEEL, width=4)
    d.line([(cx-45, cy+22), (cx-35, cy+10)], fill=(120, 90, 60, 255), width=6) # Hilt
    d.ellipse([cx-36, cy+10, cx-32, cy+14], fill=(220, 180, 50, 255)) # Tsuba guard
    # Left Arm
    d.line([(cx-18, cy-35), (cx-32, cy-10), (cx-20, cy+5)], fill=SILHOUETTE_BODY, width=9)
    # Right Arm forward
    d.line([(cx+18, cy-35), (cx+35, cy-15), (cx+48, cy-18)], fill=SILHOUETTE_BODY, width=9)

    out = os.path.join(CHAR_DIR, "hero_idle.png")
    im.save(out, "PNG")
    print(f"[Sprite] Created: hero_idle.png")

def create_hero_attack():
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 90, 140
    # Head lunging forward
    d.ellipse([cx+5, cy-70, cx+40, cy-35], fill=SILHOUETTE_BODY)
    # Long flowing headband trail
    d.polygon([(cx+8, cy-52), (cx-50, cy-65), (cx-80, cy-50), (cx-35, cy-45)], fill=HERO_GLOW_CYAN)
    # Blazing eye slit
    d.line([(cx+26, cy-52), (cx+38, cy-53)], fill=(255, 255, 255, 255), width=3)
    # Torso lunging at angle
    d.polygon([(cx-5, cy-38), (cx+42, cy-32), (cx+28, cy+20), (cx-15, cy+15)], fill=SILHOUETTE_BODY)
    # Legs in full deep lunge
    d.polygon([(cx-15, cy+12), (cx-55, cy+55), (cx-78, cy+95), (cx-60, cy+95), (cx-38, cy+50)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+26, cy+15), (cx+58, cy+55), (cx+75, cy+95), (cx+60, cy+95), (cx+40, cy+50)], fill=SILHOUETTE_BODY)
    # Both arms thrusting forward with sword
    d.line([(cx+25, cy-30), (cx+70, cy-32)], fill=SILHOUETTE_BODY, width=11)
    # Blazing extended Katana
    d.line([(cx+70, cy-32), (cx+155, cy-35)], fill=BLADE_STEEL, width=5)
    # Katana Slash Motion Arc (semi-transparent glowing white arc)
    d.arc([cx+60, cy-110, cx+170, cy+40], start=-60, end=40, fill=(240, 250, 255, 220), width=8)
    d.arc([cx+65, cy-105, cx+165, cy+35], start=-55, end=35, fill=HERO_GLOW_CYAN, width=3)

    out = os.path.join(CHAR_DIR, "hero_attack.png")
    im.save(out, "PNG")
    print(f"[Sprite] Created: hero_attack.png")

def create_hero_dodge():
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 130, 160
    # Shadow blur trail behind dodge
    for off_x in [35, 20]:
        d.ellipse([cx-off_x-15, cy-55, cx-off_x+15, cy-25], fill=(0, 180, 220, 60))
        d.polygon([(cx-off_x-15, cy-25), (cx-off_x+15, cy-25), (cx-off_x+25, cy+40), (cx-off_x-20, cy+40)], fill=(0, 180, 220, 50))
    # Low sliding crouch
    d.ellipse([cx-15, cy-55, cx+15, cy-25], fill=SILHOUETTE_BODY)
    d.polygon([(cx-8, cy-40), (cx-45, cy-55), (cx-65, cy-45), (cx-30, cy-35)], fill=HERO_GLOW_CYAN)
    # Torso low
    d.polygon([(cx-20, cy-25), (cx+18, cy-25), (cx+12, cy+30), (cx-18, cy+30)], fill=SILHOUETTE_BODY)
    # Sliding legs
    d.polygon([(cx-18, cy+25), (cx-60, cy+45), (cx-90, cy+55), (cx-75, cy+62), (cx-30, cy+45)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+10, cy+25), (cx+35, cy+45), (cx+50, cy+60), (cx+38, cy+62), (cx+15, cy+40)], fill=SILHOUETTE_BODY)
    # Defensive guard sword
    d.line([(cx-25, cy-15), (cx+45, cy-50)], fill=BLADE_STEEL, width=4)

    out = os.path.join(CHAR_DIR, "hero_dodge.png")
    im.save(out, "PNG")
    print(f"[Sprite] Created: hero_dodge.png")

def create_enemy_scout_sprites():
    # 1. Idle
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 130, 140
    d.ellipse([cx-15, cy-70, cx+15, cy-40], fill=SILHOUETTE_BODY)
    d.line([(cx-10, cy-55), (cx-3, cy-55)], fill=ENEMY_SCOUT_ORANGE, width=3) # Orange eye
    d.polygon([(cx-18, cy-40), (cx+18, cy-40), (cx+14, cy+15), (cx-14, cy+15)], fill=SILHOUETTE_BODY)
    d.polygon([(cx-14, cy+12), (cx-30, cy+60), (cx-40, cy+95), (cx-25, cy+95), (cx-10, cy+55)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+12, cy+12), (cx+28, cy+60), (cx+38, cy+95), (cx+24, cy+95), (cx+10, cy+55)], fill=SILHOUETTE_BODY)
    # Twin daggers held low
    d.line([(cx-18, cy-30), (cx-40, cy-5)], fill=SILHOUETTE_BODY, width=7)
    d.line([(cx-40, cy-5), (cx-58, cy+10)], fill=BLADE_STEEL, width=4)
    d.line([(cx+18, cy-30), (cx+38, cy-5)], fill=SILHOUETTE_BODY, width=7)
    d.line([(cx+38, cy-5), (cx+56, cy+10)], fill=BLADE_STEEL, width=4)
    im.save(os.path.join(CHAR_DIR, "enemy_scout_idle.png"), "PNG")

    # 2. Windup (Telegraph alert with glowing raised daggers)
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 130, 140
    # Glowing aura
    d.ellipse([cx-40, cy-95, cx+40, cy-15], fill=(255, 140, 20, 45))
    d.ellipse([cx-16, cy-70, cx+16, cy-38], fill=SILHOUETTE_BODY)
    d.line([(cx-12, cy-54), (cx-2, cy-54)], fill=(255, 220, 50, 255), width=4) # Fierce alert eye
    d.polygon([(cx-18, cy-38), (cx+18, cy-38), (cx+15, cy+15), (cx-15, cy+15)], fill=SILHOUETTE_BODY)
    d.polygon([(cx-15, cy+12), (cx-35, cy+60), (cx-42, cy+95), (cx-26, cy+95), (cx-10, cy+55)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+12, cy+12), (cx+28, cy+60), (cx+38, cy+95), (cx+24, cy+95), (cx+10, cy+55)], fill=SILHOUETTE_BODY)
    # Daggers raised HIGH ready to plunge
    d.line([(cx-16, cy-32), (cx-35, cy-75)], fill=SILHOUETTE_BODY, width=8)
    d.line([(cx-35, cy-75), (cx-48, cy-105)], fill=ENEMY_SCOUT_ORANGE, width=6)
    d.line([(cx+16, cy-32), (cx+35, cy-75)], fill=SILHOUETTE_BODY, width=8)
    d.line([(cx+35, cy-75), (cx+48, cy-105)], fill=ENEMY_SCOUT_ORANGE, width=6)
    im.save(os.path.join(CHAR_DIR, "enemy_scout_windup.png"), "PNG")

    # 3. Strike
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 150, 140
    d.ellipse([cx-25, cy-65, cx+5, cy-35], fill=SILHOUETTE_BODY)
    d.polygon([(cx-28, cy-35), (cx+8, cy-35), (cx+2, cy+18), (cx-25, cy+18)], fill=SILHOUETTE_BODY)
    d.polygon([(cx-22, cy+15), (cx-45, cy+55), (cx-58, cy+95), (cx-42, cy+95), (cx-18, cy+50)], fill=SILHOUETTE_BODY)
    d.polygon([(cx, cy+15), (cx+30, cy+55), (cx+50, cy+95), (cx+35, cy+95), (cx+15, cy+50)], fill=SILHOUETTE_BODY)
    # Forward scissor strike
    d.line([(cx-10, cy-25), (cx-65, cy-15)], fill=SILHOUETTE_BODY, width=9)
    d.line([(cx-65, cy-15), (cx-120, cy-5)], fill=(255, 60, 60, 255), width=5)
    # Red strike slash slash arc
    d.arc([cx-130, cy-60, cx-40, cy+40], start=120, end=240, fill=(255, 80, 50, 230), width=6)
    im.save(os.path.join(CHAR_DIR, "enemy_scout_strike.png"), "PNG")
    print("[Sprite] Created 3 Scout sprites.")

def create_ronin_sprites():
    # Heavy Ronin (Large horned silhouette)
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 135, 135
    # Horned Kabuto helmet
    d.ellipse([cx-22, cy-75, cx+22, cy-40], fill=SILHOUETTE_BODY)
    d.polygon([(cx-15, cy-72), (cx-35, cy-100), (cx-25, cy-85)], fill=SILHOUETTE_ARMOR) # Left Horn
    d.polygon([(cx+15, cy-72), (cx+35, cy-100), (cx+25, cy-85)], fill=SILHOUETTE_ARMOR) # Right Horn
    d.line([(cx-12, cy-54), (cx-4, cy-54)], fill=RONIN_CRIMSON, width=4) # Red glowing eyes
    # Broad shoulder pads
    d.polygon([(cx-45, cy-45), (cx-20, cy-45), (cx-25, cy-25), (cx-48, cy-30)], fill=SILHOUETTE_ARMOR)
    d.polygon([(cx+20, cy-45), (cx+45, cy-45), (cx+48, cy-30), (cx+25, cy-25)], fill=SILHOUETTE_ARMOR)
    # Bulk Torso
    d.polygon([(cx-28, cy-40), (cx+28, cy-40), (cx+22, cy+25), (cx-22, cy+25)], fill=SILHOUETTE_BODY)
    # Sturdy legs
    d.polygon([(cx-22, cy+20), (cx-38, cy+65), (cx-48, cy+100), (cx-30, cy+100), (cx-14, cy+55)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+18, cy+20), (cx+34, cy+65), (cx+44, cy+100), (cx+28, cy+100), (cx+12, cy+55)], fill=SILHOUETTE_BODY)
    # Giant Odachi blade resting on shoulder
    d.line([(cx+15, cy-15), (cx-65, cy-95)], fill=BLADE_STEEL, width=7)
    im.save(os.path.join(CHAR_DIR, "enemy_ronin_idle.png"), "PNG")

    # Ronin Windup
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 135, 135
    # Intense red warning aura
    d.ellipse([cx-50, cy-115, cx+50, cy-20], fill=(255, 30, 30, 50))
    d.ellipse([cx-22, cy-75, cx+22, cy-40], fill=SILHOUETTE_BODY)
    d.line([(cx-12, cy-54), (cx-4, cy-54)], fill=(255, 255, 200, 255), width=5)
    d.polygon([(cx-45, cy-45), (cx-20, cy-45), (cx-25, cy-25), (cx-48, cy-30)], fill=SILHOUETTE_ARMOR)
    d.polygon([(cx+20, cy-45), (cx+45, cy-45), (cx+48, cy-30), (cx+25, cy-25)], fill=SILHOUETTE_ARMOR)
    d.polygon([(cx-28, cy-40), (cx+28, cy-40), (cx+22, cy+25), (cx-22, cy+25)], fill=SILHOUETTE_BODY)
    d.polygon([(cx-22, cy+20), (cx-42, cy+65), (cx-52, cy+100), (cx-32, cy+100), (cx-14, cy+55)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+18, cy+20), (cx+34, cy+65), (cx+44, cy+100), (cx+28, cy+100), (cx+12, cy+55)], fill=SILHOUETTE_BODY)
    # Odachi raised overhead
    d.line([(cx, cy-30), (cx-20, cy-80)], fill=SILHOUETTE_BODY, width=12)
    d.line([(cx-20, cy-80), (cx-45, cy-135)], fill=RONIN_CRIMSON, width=8)
    im.save(os.path.join(CHAR_DIR, "enemy_ronin_windup.png"), "PNG")

    # Ronin Strike
    im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 150, 135
    d.ellipse([cx-30, cy-65, cx+10, cy-30], fill=SILHOUETTE_BODY)
    d.polygon([(cx-35, cy-32), (cx+15, cy-32), (cx+8, cy+25), (cx-30, cy+25)], fill=SILHOUETTE_BODY)
    d.polygon([(cx-25, cy+20), (cx-55, cy+60), (cx-70, cy+100), (cx-50, cy+100), (cx-20, cy+50)], fill=SILHOUETTE_BODY)
    d.polygon([(cx+5, cy+20), (cx+30, cy+60), (cx+45, cy+100), (cx+30, cy+100), (cx+15, cy+50)], fill=SILHOUETTE_BODY)
    # Downward cleave
    d.line([(cx-10, cy-20), (cx-80, cy+10)], fill=SILHOUETTE_BODY, width=12)
    d.line([(cx-80, cy+10), (cx-135, cy+40)], fill=BLADE_STEEL, width=7)
    d.arc([cx-145, cy-40, cx-30, cy+75], start=100, end=260, fill=(255, 40, 40, 240), width=9)
    im.save(os.path.join(CHAR_DIR, "enemy_ronin_strike.png"), "PNG")
    print("[Sprite] Created 3 Ronin sprites.")

def create_environment_background():
    # 540x960 9:16 vertical bamboo night
    W, H = 540, 960
    im = Image.new("RGBA", (W, H), (12, 14, 22, 255))
    d = ImageDraw.Draw(im)

    # 1. Full Moon in top right
    moon_x, moon_y = 380, 180
    # Outer glow
    for r in range(95, 60, -5):
        alpha = int((1.0 - (r-60)/35.0) * 45)
        d.ellipse([moon_x-r, moon_y-r, moon_x+r, moon_y+r], fill=(210, 225, 255, alpha))
    d.ellipse([moon_x-60, moon_y-60, moon_x+60, moon_y+60], fill=(235, 242, 255, 255))

    # 2. Distant mountains
    d.polygon([(0, 650), (140, 480), (320, 680), (450, 510), (540, 620), (540, 960), (0, 960)], fill=(18, 22, 34, 255))

    # 3. Ground plane & stone path
    d.rectangle([0, 720, W, H], fill=(15, 17, 24, 255))
    d.line([(0, 720), (W, 720)], fill=(35, 40, 55, 255), width=3)

    # 4. Layered bamboo stalks
    bamboo_x_list = [35, 65, 110, 420, 460, 505]
    for bx in bamboo_x_list:
        width = 16 if bx < 200 else 18
        # Stalk
        d.rectangle([bx, 0, bx+width, 730], fill=(8, 10, 15, 255))
        # Joints
        for jy in range(60, 720, 110):
            d.line([(bx-4, jy), (bx+width+4, jy)], fill=(25, 30, 42, 255), width=4)
        # Leaves
        for ly in range(90, 650, 140):
            dir_sign = 1 if bx < 200 else -1
            d.polygon([(bx+width//2, ly), (bx+width//2 + 55*dir_sign, ly-25), (bx+width//2 + 75*dir_sign, ly-15), (bx+width//2 + 35*dir_sign, ly)], fill=(10, 14, 18, 255))

    # 5. Fog gradient at combat level (y=620..740)
    for fy in range(620, 730, 5):
        alpha = int((1.0 - abs(fy - 680)/70.0) * 35)
        d.line([(0, fy), (W, fy)], fill=(90, 120, 160, alpha), width=5)

    im.save(os.path.join(ENV_DIR, "bg_bamboo_night.png"), "PNG")
    print("[Sprite] Created: bg_bamboo_night.png (540x960)")

def create_vfx_sprites():
    # 1. Slash FX (Curved razor glow)
    im = Image.new("RGBA", (256, 128), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    d.arc([10, -50, 245, 170], start=25, end=155, fill=(0, 230, 255, 160), width=16)
    d.arc([15, -45, 240, 165], start=30, end=150, fill=(255, 255, 255, 255), width=7)
    im.save(os.path.join(VFX_DIR, "slash_fx.png"), "PNG")

    # 2. Counter Flash FX (Perfect counter burst)
    im = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    cx, cy = 64, 64
    d.ellipse([cx-40, cy-40, cx+40, cy+40], fill=(255, 240, 120, 90))
    d.ellipse([cx-22, cy-22, cx+22, cy+22], fill=(255, 255, 255, 240))
    d.line([(cx-60, cy), (cx+60, cy)], fill=(255, 255, 255, 255), width=4)
    d.line([(cx, cy-60), (cx, cy+60)], fill=(255, 255, 255, 255), width=4)
    d.line([(cx-40, cy-40), (cx+40, cy+40)], fill=(0, 230, 255, 220), width=3)
    d.line([(cx-40, cy+40), (cx+40, cy-40)], fill=(0, 230, 255, 220), width=3)
    im.save(os.path.join(VFX_DIR, "counter_flash_fx.png"), "PNG")

    # 3. Telegraph Warning Crest
    im = Image.new("RGBA", (96, 96), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    d.ellipse([6, 6, 90, 90], fill=(255, 30, 30, 220))
    d.ellipse([14, 14, 82, 82], fill=(255, 120, 20, 240))
    # Exclamation mark
    d.rectangle([43, 22, 53, 56], fill=(255, 255, 255, 255))
    d.ellipse([43, 64, 53, 74], fill=(255, 255, 255, 255))
    im.save(os.path.join(VFX_DIR, "telegraph_warning_icon.png"), "PNG")
    print("[Sprite] Created 3 VFX sprites.")

if __name__ == "__main__":
    print("=== BẮT ĐẦU SINH TÀI NGUYÊN V1.5 KẾ THỪA GAME FEEL ===")
    gen_sfx_slash()
    gen_sfx_dodge()
    gen_sfx_parry()
    gen_sfx_hit()
    gen_sfx_alert()
    gen_bgm_battle()
    create_hero_idle()
    create_hero_attack()
    create_hero_dodge()
    create_enemy_scout_sprites()
    create_ronin_sprites()
    create_environment_background()
    create_vfx_sprites()

    # Generate meta files for all art and audio
    print("[*] Generating .meta files for Unity...")
    for root, _, files in os.walk(ART_DIR):
        for f in files:
            if f.endswith(".png"):
                write_sprite_meta(os.path.join(root, f))
    for root, _, files in os.walk(AUDIO_DIR):
        for f in files:
            if f.endswith(".wav"):
                write_audio_meta(os.path.join(root, f))

    print("=== HOÀN TẤT SINH 100% ASSETS V1.5 THÀNH CÔNG ===")
