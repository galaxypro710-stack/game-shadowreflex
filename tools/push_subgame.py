#!/usr/bin/env python3
"""
push_subgame.py: Autonomous Isolated Local Git Push & Identity Guard
===================================================================
Tuân thủ nghiêm ngặt Karpathy Rule 8 (Multi-Account Git Governance)
và AGENTS.md Mục 3 (Autonomous Ship & PR Gate).

Chức năng:
1. Đọc GITHUB_TOKEN trực tiếp từ .env.local của CHÍNH SUBGAME NÀY (Local Scope).
2. Kiểm tra bắt buộc git config --local user.name và user.email (CẤM fallback global).
3. Tiêm token tạm thời (Ephemeral Injection) vào lệnh push HTTPS, TUYỆT ĐỐI KHÔNG
   ghi đè token vào file .git/config.
4. Masking 100% token khi in ra stdout/log (ghp_****).
"""

import os
import sys
import subprocess
import re

def mask_token(token: str) -> str:
    if not token or len(token) < 8:
        return "****"
    return f"{token[:4]}****{token[-4:]}"

def get_local_env_token(project_dir: str) -> str:
    env_local = os.path.join(project_dir, ".env.local")
    if not os.path.exists(env_local):
        print(f"[!] ERROR: Khong tim thay file .env.local tai {project_dir}")
        print("    Vui long copy .env.example sang .env.local va dien GITHUB_TOKEN!")
        sys.exit(1)
        
    with open(env_local, "r", encoding="utf-8", errors="ignore") as f:
        for line in f:
            line = line.strip()
            if line.startswith("GITHUB_TOKEN=") and not line.startswith("#"):
                token = line.split("=", 1)[1].strip().strip('"').strip("'")
                return token
    return ""

def check_local_git_identity(project_dir: str):
    name_res = subprocess.run(["git", "config", "--local", "user.name"], cwd=project_dir, capture_output=True, text=True)
    email_res = subprocess.run(["git", "config", "--local", "user.email"], cwd=project_dir, capture_output=True, text=True)
    
    local_name = name_res.stdout.strip()
    local_email = email_res.stdout.strip()
    
    if not local_name or not local_email:
        print("[!] ERROR: Vi pham Karpathy Rule 8 (Multi-Account Git Governance)!")
        print("    Subgame nay CHUA DUOC CAU HINH LOCAL GIT IDENTITY (--local).")
        print("    CẤM dùng fallback global. Vui long chay:")
        print("      git config --local user.name 'TenTaiKhoan'")
        print("      git config --local user.email 'EmailTaiKhoan'")
        sys.exit(1)
        
    print(f"[*] Local Git Identity Verified: {local_name} <{local_email}>")

def get_current_branch(project_dir: str) -> str:
    res = subprocess.run(["git", "rev-parse", "--abbrev-ref", "HEAD"], cwd=project_dir, capture_output=True, text=True)
    branch = res.stdout.strip()
    return branch if branch else "main"

def get_remote_url(project_dir: str) -> str:
    res = subprocess.run(["git", "remote", "get-url", "origin"], cwd=project_dir, capture_output=True, text=True)
    url = res.stdout.strip()
    if not url:
        print("[!] ERROR: Khong tim thay git remote 'origin'!")
        sys.exit(1)
    return url

def main():
    # Target directory is the project containing this script or current working dir
    script_dir = os.path.dirname(os.path.abspath(__file__))
    project_dir = os.path.abspath(os.path.join(script_dir, "..")) if os.path.basename(script_dir) == "tools" else os.getcwd()
    
    print("=" * 65)
    print(f"[*] AUTONOMOUS SUBGAME GIT PUSH GUARD: {os.path.basename(project_dir)}")
    print("=" * 65)
    
    # 1. Check local identity
    check_local_git_identity(project_dir)
    
    # 2. Get local token
    token = get_local_env_token(project_dir)
    if not token or token == "your_github_token_here" or token == "your_local_github_token_here":
        print("[!] ERROR: GITHUB_TOKEN trong .env.local chua duoc dien hoac la dummy placeholder!")
        sys.exit(1)
    print(f"[*] Local Subgame Token Loaded: {mask_token(token)} (From .env.local)")
    
    # 3. Check remote origin
    origin_url = get_remote_url(project_dir)
    clean_origin = re.sub(r"https://[^@]+@", "https://", origin_url)
    print(f"[*] Target Remote: {clean_origin}")
    
    # Format ephemeral URL with token
    if not clean_origin.startswith("https://github.com/"):
        print(f"[!] ERROR: Remote URL {clean_origin} khong phai HTTPS GitHub standard!")
        sys.exit(1)
        
    repo_path = clean_origin.replace("https://github.com/", "")
    ephemeral_push_url = f"https://{token}@github.com/{repo_path}"
    
    # 4. Check branch
    branch = get_current_branch(project_dir)
    print(f"[*] Pushing Branch: {branch}...")
    
    # 5. Execute ephemeral push
    push_cmd = ["git", "push", ephemeral_push_url, branch]
    res = subprocess.run(push_cmd, cwd=project_dir, capture_output=True, text=True)
    
    # Mask token in stderr / stdout
    safe_stdout = res.stdout.replace(token, mask_token(token))
    safe_stderr = res.stderr.replace(token, mask_token(token))
    
    if res.returncode == 0:
        print(f"[+] SUCCESS: Da push thanh cong len origin/{branch} (Exit Code 0)!")
        if safe_stdout.strip():
            print(safe_stdout.strip())
        if safe_stderr.strip():
            print(safe_stderr.strip())
    else:
        print(f"[!] PUSH FAILED with exit code {res.returncode}:")
        print(safe_stderr.strip())
        sys.exit(res.returncode)

if __name__ == "__main__":
    main()
