#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameconClicker.Editor
{

    public class CommandLineBuild {
        public static void BuildProject()
        {
            string[] argv = System.Environment.GetCommandLineArgs();
            int argc = argv.Length;

            Debug.Log("CommandLineBuild.BuildProject\n" +
                      "  Args: " + string.Join("\n    ", argv));

            int idx = 0;
            string targetPlatform = "", buildRoot = "";

            while (idx < argc) {
                string arg = argv[idx++];
                if (arg.Equals("-targetPlatform") && idx < argc) {
                    targetPlatform = argv[idx];
                } else if (arg.Equals("-buildRoot") && idx < argc) {
                    buildRoot = argv[idx];
                } else if (arg.Equals("-teamId") && idx < argc) {
                    System.Environment.SetEnvironmentVariable("DEVELOPMENT_TEAM", argv[idx]);
                } else if (arg.Equals("-internalBuild")) {
                    System.Environment.SetEnvironmentVariable("INTERNAL_BUILD", "YES");
                }
            }

            if (targetPlatform.Equals("")) {
                Debug.LogError("-targetPlatform <platform> is required");
                return;
            }
            if (buildRoot.Equals("")) {
                Debug.LogError("-buildRoot <path> is required");
                return;
            }

            if (targetPlatform.Equals("Android"))
                BuildProject(BuildTarget.Android, buildRoot);
            else if (targetPlatform.Equals("iOS"))
                BuildProject(BuildTarget.iOS, buildRoot);
        }

        public static void BuildProject(BuildTarget target, string buildRoot)
        {
            Debug.Log("CommandLineBuild.BuildProject\n" +
                      "  Target: " + target + "\n" +
                      "  BuildRoot: " + buildRoot);

            if (target == BuildTarget.Android)
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            BuildOptions options = BuildOptions.None;
            if (target == BuildTarget.Android)
                options = options | BuildOptions.AcceptExternalModificationsToPlayer;

            var playerOptions = new BuildPlayerOptions{
                // TODO: ideally get the scenes using some Unity API
                scenes = new[] { "Assets/Scenes/MainGame.unity" },
                target = target,
                locationPathName = buildRoot,
                options = options,
            };

            Debug.Log("CommandLineBuild.BuildProject\n" +
                      "  BuildPlayerOptions:\n" +
                      "    Scenes: " + string.Join(",", playerOptions.scenes) + "\n" +
                      "    Options: " + playerOptions.options);

            BuildPipeline.BuildPlayer(playerOptions);
        }
    }

}

#endif
