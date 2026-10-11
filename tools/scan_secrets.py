"""
tools/scan_secrets.py
=====================
Pre-commit / Pre-push Secret Scanner & Anti-Leak Validator for Meta-Factory.
Enforces Karpathy Rule 8 (Secret Hygiene) and Blind Spot 10.

Governed by: /ai-copilot-alignment, /vibe-git-manager, /karpathy-guidelines.
"""

import os
import sys
import re
import argparse

# Reconfigure stdout/stderr for clean cross-platform UTF-8
sys.stdout.reconfigure(encoding='utf-8', errors='ignore')
sys.stderr.reconfigure(encoding='utf-8', errors='ignore')

IGNORED_DIRS = {'.git', '.agents', '__pycache__', 'Library', 'Temp', 'bin', 'obj', 'Build', 'Builds', '.vs', '.idea', 'Logs', 'MemoryCaptures', 'UserSettings', 'extracted_cache'}
IGNORED_FILES = {'.env.local', '.circuit_breaker_state.json', 'google-services.json', 'GoogleService-Info.plist'}
IGNORED_EXTENSIONS = {
    '.png', '.jpg', '.jpeg', '.wav', '.mp3', '.ogg', '.apk', '.xapk', '.zip', '.tar', '.gz', 
    '.ico', '.keystore', '.meta', '.asset', '.prefab', '.unity', '.mat', '.anim', '.controller', 
    '.physicMaterial', '.fontsettings', '.ttf', '.otf', '.fbx', '.obj'
}

SECRET_PATTERNS = [
    ("GitHub Personal Access Token", re.compile(r"\bghp_[A-Za-z0-9_]{36}\b")),
    ("GitHub Fine-Grained PAT", re.compile(r"\bgithub_pat_[A-Za-z0-9_]{82}\b")),
    ("Vercel Access Token", re.compile(r"\bvcp_[A-Za-z0-9_]{40,}\b")),
    ("Private Key Header", re.compile(r"-----BEGIN (?:RSA |EC |OPENSSH |DSA )?PRIVATE KEY-----")),
    ("AWS Access Key ID", re.compile(r"\bAKIA[0-9A-Z]{16}\b")),
    ("Google API Key", re.compile(r"\bAIzaSy[A-Za-z0-9_-]{33}\b")),
]

def mask_secret(secret_str):
    if len(secret_str) <= 8:
        return "****"
    return f"{secret_str[:4]}****{secret_str[-4:]}"

def scan_directory(target_dir):
    """Scan directory recursively for credential leaks."""
    print("=================================================================")
    print(f"[*] SCANNING REPOSITORY FOR SECRETS & PAT LEAKS")
    print(f"[*] Target Directory: {target_dir}")
    print("=================================================================")

    violations = []
    files_checked = 0

    for root, dirs, files in os.walk(target_dir):
        # Exclude ignored directories
        dirs[:] = [d for d in dirs if d not in IGNORED_DIRS]

        for f in files:
            if f in IGNORED_FILES:
                continue

            _, ext = os.path.splitext(f)
            if ext.lower() in IGNORED_EXTENSIONS:
                continue

            file_path = os.path.join(root, f)
            try:
                if os.path.getsize(file_path) > 1024 * 1024:
                    continue
            except OSError:
                continue
            files_checked += 1

            try:
                with open(file_path, "r", encoding="utf-8", errors="ignore") as fp:
                    for line_num, line in enumerate(fp, start=1):
                        for pattern_name, regex in SECRET_PATTERNS:
                            matches = regex.findall(line)
                            for m in matches:
                                violations.append({
                                    "file": file_path,
                                    "line": line_num,
                                    "type": pattern_name,
                                    "masked": mask_secret(m)
                                })
            except Exception as e:
                pass

    print(f"[*] Total Files Checked: {files_checked}")

    if violations:
        print(f"\n[CRITICAL] FOUND {len(violations)} POTENTIAL SECRET LEAK(S):", file=sys.stderr)
        for v in violations:
            rel_path = os.path.relpath(v["file"], target_dir)
            print(f"  [-] {v['type']} at {rel_path}:{v['line']} -> {v['masked']}", file=sys.stderr)
        print("\n[!] Please remove hardcoded credentials or move them to .env.local!", file=sys.stderr)
        return False, violations
    else:
        print("\n[+] SUCCESS: 0 secrets detected. Repository is 100% clean!")
        return True, []

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Scan repository for hardcoded secrets and PATs")
    parser.add_argument("-d", "--dir", default=os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
                        help="Target repository directory to scan")
    args = parser.parse_args()

    clean, _ = scan_directory(os.path.abspath(args.dir))
    sys.exit(0 if clean else 1)
