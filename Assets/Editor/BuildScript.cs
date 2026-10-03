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
        public static void BuildAndroid()
        {
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
