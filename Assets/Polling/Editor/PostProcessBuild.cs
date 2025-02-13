#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace Polling.Editor
{
    public class PostProcessBuild {

#if UNITY_IOS

        public const string PollingXCFrameworkPath = "Frameworks/Plugins/iOS/Polling.xcframework";

        [PostProcessBuildAttribute(1)]
        public static void AddXCFramework(BuildTarget target, string buildRoot)
        {
            if (target != BuildTarget.iOS)
                return;

            Debug.Log("PostProcessBuild.AddXCFramework: " + PollingXCFrameworkPath);

            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildRoot);
            project.ReadFromFile(projectPath);

            string appTargetId = project.GetUnityMainTargetGuid();
            //string appTargetId = TargetGuidByName(PBXProject.GetUnityTargetName()); // deprecated

            string frameworkPath = PollingXCFrameworkPath;
            string frameworkGuid = project.FindFileGuidByProjectPath(frameworkPath);
            Debug.Log("  XCFramework GUID: " + frameworkGuid);

            project.AddFileToEmbedFrameworks(appTargetId, frameworkGuid);

            project.WriteToFile(projectPath);
        }

#endif

    }
}

#endif
