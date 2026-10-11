using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Script tự động hóa build APK Android & Windows qua Unity CLI Batchmode (Headless Build).
    /// Chuẩn Universal Meta-Factory: Tự động trích xuất Tên sản phẩm (productName) động,
    /// không hardcode bất kỳ tên game con hay package ID nào.
    /// </summary>
    public class BuildScript
    {
        /// <summary>
        /// Kiểm dịch tài nguyên Staging: Đảm bảo không phát hành APK Store khi chưa reskin 100% tài nguyên gốc.
        /// </summary>
        public static bool CheckStagingQuarantine(bool abortIfViolation = false)
        {
            string[] checkPaths = new[] { "Assets/Art/Staging_Reference", "Assets/Staging" };
            int stagingCount = 0;

            foreach (var path in checkPaths)
            {
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (!file.EndsWith(".meta")) stagingCount++;
                    }
                }
            }

            if (stagingCount > 0)
            {
                string warningMsg = $"[BuildScript] QUARANTINE NOTICE: Phát hiện {stagingCount} tài nguyên staging trong Assets/Art/Staging_Reference. " +
                                     "Tài nguyên này hợp lệ cho V1/V2 Internal R&D, nhưng BẮT BUỘC phải được thay thế ở V3 trước khi Release Store.";
                if (abortIfViolation)
                {
                    Debug.LogError($"[BuildScript] QUARANTINE ABORT: {warningMsg} Hủy tiến trình build Release để phòng thủ Store Anti-Ban!");
                    return false;
                }
                else
                {
                    Debug.LogWarning(warningMsg);
                }
            }
            return true;
        }

        public static void BuildAndroidRelease()
        {
            if (!CheckStagingQuarantine(abortIfViolation: true))
            {
                EditorApplication.Exit(1);
                return;
            }
            ConfigureReleaseKeystore();
            BuildAndroid();
        }

        private static void ConfigureReleaseKeystore()
        {
            string cleanGameName = string.IsNullOrEmpty(PlayerSettings.productName) 
                ? "GameRelease" 
                : PlayerSettings.productName.Replace(" ", "");

            string keystoreDir = Path.Combine(Directory.GetCurrentDirectory(), "keystore");
            string keystoreFile = Path.Combine(keystoreDir, $"{cleanGameName}.keystore");
            string infoFile = Path.Combine(keystoreDir, "keystore.info.txt");

            if (File.Exists(keystoreFile) && File.Exists(infoFile))
            {
                string alias = null;
                string pass = null;
                foreach (var line in File.ReadAllLines(infoFile))
                {
                    if (line.StartsWith("Alias:")) alias = line.Substring("Alias:".Length).Trim();
                    else if (line.StartsWith("Password:")) pass = line.Substring("Password:".Length).Trim();
                }

                if (!string.IsNullOrEmpty(alias) && !string.IsNullOrEmpty(pass))
                {
                    PlayerSettings.Android.useCustomKeystore = true;
                    PlayerSettings.Android.keystoreName = keystoreFile;
                    PlayerSettings.Android.keystorePass = pass;
                    PlayerSettings.Android.keyaliasName = alias;
                    PlayerSettings.Android.keyaliasPass = pass;
                    Debug.Log($"[BuildScript] Cấu hình Keystore ký số độc lập thành công cho {cleanGameName} (Alias: {alias})");
                }
            }
        }

        public static void BuildAndroid()
        {
            CheckStagingQuarantine(abortIfViolation: false);

            string cleanGameName = string.IsNullOrEmpty(PlayerSettings.productName) 
                ? "GameRelease" 
                : PlayerSettings.productName.Replace(" ", "");

            Debug.Log($"[BuildScript] Bắt đầu tiến trình Build Android APK headless cho: {cleanGameName}...");
            Directory.CreateDirectory("Builds/Android");

            // Đảm bảo cấu hình Android chuẩn Store
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            EditorUserBuildSettings.buildAppBundle = false;

            string apkLocation = $"Builds/Android/{cleanGameName}.apk";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Project/Scenes/Game.unity"
                },
                locationPathName = apkLocation,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildScript] Build Android APK thành công mỹ mãn: {summary.totalSize} bytes ({summary.totalSize / (1024f * 1024f):F2} MB) tại {apkLocation}");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[BuildScript] Build APK thất bại với {summary.totalErrors} lỗi.");
                EditorApplication.Exit(1);
            }
        }

        public static void BuildWindows()
        {
            string cleanGameName = string.IsNullOrEmpty(PlayerSettings.productName) 
                ? "GameRelease" 
                : PlayerSettings.productName.Replace(" ", "");

            Debug.Log($"[BuildScript] Bắt đầu tiến trình Build Windows Standalone headless cho: {cleanGameName}...");
            Directory.CreateDirectory("Builds/Windows");

            // Thiết lập độ phân giải cửa sổ thu nhỏ 540x960 (Portrait 9:16) chuẩn mobile cho bản Windows
            PlayerSettings.defaultScreenWidth = 540;
            PlayerSettings.defaultScreenHeight = 960;
            PlayerSettings.defaultWebScreenWidth = 540;
            PlayerSettings.defaultWebScreenHeight = 960;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;

            string exeLocation = $"Builds/Windows/{cleanGameName}.exe";

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/_Project/Scenes/Game.unity"
                },
                locationPathName = exeLocation,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildScript] Build Windows thành công mỹ mãn: {summary.totalSize} bytes tại {exeLocation}");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[BuildScript] Build Windows thất bại với {summary.totalErrors} lỗi.");
                EditorApplication.Exit(1);
            }
        }
    }
}
