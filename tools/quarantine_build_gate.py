"""
tools/quarantine_build_gate.py
==============================
Store Anti-Ban Quarantine & Asset/Font/Secret Hygiene Enforcement Gate.
Governed by: /ai-copilot-alignment, /harness-engineering, /karpathy-guidelines

Pre-Build & Pre-Release Gatekeeper:
1. Asset Quarantine Gate: Blocks release builds if un-reskinned Staging assets remain.
2. Font License Hygiene: Verifies font files (.ttf/.otf) have proper open-source licensing (OFL/Apache).
3. Poison String & Tracking Leak Detection: Scans for competitor tracker endpoints and leaked domains.
"""

import os
import re
import sys
import argparse

sys.stdout.reconfigure(encoding='utf-8', errors='ignore')
sys.stderr.reconfigure(encoding='utf-8', errors='ignore')

# Prohibited third-party tracker domains from reversed APKs
SUSPICIOUS_DOMAINS = [
    r'https?://[a-zA-Z0-9.-]*appsflyer\.com',
    r'https?://[a-zA-Z0-9.-]*adjust\.com',
    r'https?://[a-zA-Z0-9.-]*branch\.io',
    r'https?://[a-zA-Z0-9.-]*singular\.net',
    r'https?://[a-zA-Z0-9.-]*kochava\.com',
]

FONT_EXTENSIONS = ('.ttf', '.otf', '.woff', '.woff2')
LICENSE_PATTERNS = ('ofl', 'license', 'sil open font license', 'apache')


def resolve_project_path(game_name=None, project_path=None):
    if project_path and os.path.exists(project_path):
        return os.path.abspath(project_path)

    if game_name:
        candidate_paths = [
            os.path.join(r"D:\bkk\games", game_name),
            os.path.join(r"D:\bkk", game_name),
            os.path.join(os.getcwd(), game_name)
        ]
        for cp in candidate_paths:
            if os.path.exists(cp):
                return os.path.abspath(cp)

    return os.path.abspath(os.getcwd())


def check_staging_assets(project_path, is_release=False):
    staging_dirs = [
        os.path.join(project_path, "Assets", "Art", "Staging_Reference"),
        os.path.join(project_path, "Assets", "Staging")
    ]
    
    found_staging_files = []
    for sdir in staging_dirs:
        if os.path.exists(sdir):
            for root, _, files in os.walk(sdir):
                for f in files:
                    if not f.endswith(".meta") and not f.startswith("."):
                        found_staging_files.append(os.path.relpath(os.path.join(root, f), project_path))

    manifest_path = os.path.join(project_path, "ASSET_REPLACEMENT_MANIFEST.md")
    manifest_unreplaced = []
    if os.path.exists(manifest_path):
        try:
            with open(manifest_path, 'r', encoding='utf-8', errors='ignore') as mf:
                lines = mf.readlines()
            for line in lines:
                if '[STAGING]' in line or 'Pending' in line:
                    manifest_unreplaced.append(line.strip())
        except Exception:
            pass

    return found_staging_files, manifest_unreplaced


def check_font_hygiene(project_path):
    assets_dir = os.path.join(project_path, "Assets")
    if not os.path.exists(assets_dir):
        return [], []

    unlicensed_fonts = []
    licensed_fonts = []

    for root, _, files in os.walk(assets_dir):
        font_files = [f for f in files if f.lower().endswith(FONT_EXTENSIONS) and not f.endswith(".meta")]
        if not font_files:
            continue

        # Check for licensing indicators in current folder or parent folders up to Assets
        has_license = False
        curr = root
        while curr.startswith(assets_dir):
            parent_files = [pf.lower() for pf in os.listdir(curr)]
            if any(any(pat in pf for pat in LICENSE_PATTERNS) for pf in parent_files):
                has_license = True
                break
            curr = os.path.dirname(curr)

        for ff in font_files:
            rel_path = os.path.relpath(os.path.join(root, ff), project_path)
            if has_license:
                licensed_fonts.append(rel_path)
            else:
                unlicensed_fonts.append(rel_path)

    return unlicensed_fonts, licensed_fonts


def check_poison_strings(project_path):
    assets_dir = os.path.join(project_path, "Assets")
    if not os.path.exists(assets_dir):
        return []

    hits = []
    scan_exts = ('.json', '.asset', '.cs', '.txt')
    regexes = [re.compile(p, re.IGNORECASE) for p in SUSPICIOUS_DOMAINS]

    for root, _, files in os.walk(assets_dir):
        for f in files:
            if f.endswith(scan_exts) and not f.endswith(".meta"):
                file_path = os.path.join(root, f)
                try:
                    with open(file_path, 'r', encoding='utf-8', errors='ignore') as sf:
                        content = sf.read()
                    for r in regexes:
                        match = r.search(content)
                        if match:
                            rel_p = os.path.relpath(file_path, project_path)
                            hits.append((rel_p, match.group(0)))
                except Exception:
                    pass

    return hits


def run_gate(project_path, is_release=False, audit_only=False):
    print("=" * 65)
    print(f"STORE ANTI-BAN QUARANTINE GATE: {os.path.basename(project_path)}")
    print(f"Project Path: {project_path}")
    print(f"Target Mode:  {'RELEASE CANDIDATE (STORE)' if is_release else 'INTERNAL R&D / STAGING'}")
    print("=" * 65)

    has_fatal_error = False

    # 1. Staging Check
    staging_files, manifest_unreplaced = check_staging_assets(project_path, is_release)
    print(f"[*] 1. Staging Asset Quarantine: {len(staging_files)} raw staging files detected.")
    if is_release and staging_files:
        print("    [!] FATAL: Staging assets present in Release Candidate mode!")
        for sf in staging_files[:5]:
            print(f"        - {sf}")
        if len(staging_files) > 5:
            print(f"        ... and {len(staging_files) - 5} more files.")
        has_fatal_error = True
    elif staging_files:
        print(f"    [INFO] Staging files permitted for Internal R&D (V1/V2). Reskin required before V3 Store Release.")
    else:
        print("    [+] PASS: 0 staging assets. Clean for store release.")

    # 2. Font License Hygiene
    unlicensed_fonts, licensed_fonts = check_font_hygiene(project_path)
    print(f"[*] 2. Font License Hygiene: {len(licensed_fonts)} licensed, {len(unlicensed_fonts)} unverified.")
    if unlicensed_fonts:
        print("    [WARN] Unverified font files found without accompanying OFL/License documentation:")
        for uf in unlicensed_fonts[:5]:
            print(f"        - {uf}")
        if is_release:
            print("    [!] FATAL: Store release requires verified Open Font License (OFL) or commercial license.")
            has_fatal_error = True
    else:
        print("    [+] PASS: All detected fonts possess recognized license context or standard Unity fonts.")

    # 3. Poison String / Third-party Tracker Scan
    poison_hits = check_poison_strings(project_path)
    print(f"[*] 3. Poison Tracker & String Scan: {len(poison_hits)} detected.")
    if poison_hits:
        print("    [WARN] Suspicious tracker domains or reversed competitor endpoints detected:")
        for ph_path, ph_domain in poison_hits[:5]:
            print(f"        - {ph_path} -> {ph_domain}")
        if is_release:
            print("    [!] FATAL: Clean all reversed competitor tracker domains before release.")
            has_fatal_error = True
    else:
        print("    [+] PASS: Zero competitor tracker URLs detected.")

    print("=" * 65)
    if has_fatal_error and not audit_only:
        print("[!] QUARANTINE GATE REJECTED (Exit Code 1) - Store release aborted!")
        return 1
    else:
        print("[+] QUARANTINE GATE APPROVED (Exit Code 0) - Proceeding.")
        return 0


def main():
    parser = argparse.ArgumentParser(description="Store Anti-Ban Quarantine & Hygiene Gate")
    parser.add_argument("-p", "--project", help="Path to Unity project")
    parser.add_argument("-g", "--game", help="Game workspace name (under D:\\bkk\\games\\)")
    parser.add_argument("--release", action="store_true", help="Enforce strict Release Candidate rules")
    parser.add_argument("--audit-only", action="store_true", help="Report findings without failing exit code")

    args = parser.parse_args()
    proj = resolve_project_path(game_name=args.game, project_path=args.project)

    exit_code = run_gate(proj, is_release=args.release, audit_only=args.audit_only)
    sys.exit(exit_code)


if __name__ == "__main__":
    main()
