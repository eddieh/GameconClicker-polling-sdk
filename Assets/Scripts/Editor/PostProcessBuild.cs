#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace GameconClicker.Editor
{
    public class PostProcessBuild {

#if UNITY_IOS

        public const string PollingBundleIdInternal = "com.polling.internal.ios.PollingDemo";
        public const string PollingBundleId = "com.polling.examples.ios.PollingDemo";

        [PostProcessBuildAttribute(2)]
        public static void ModifyInfoPlist(BuildTarget target, string buildRoot)
        {
            if (target != BuildTarget.iOS)
                return;

            string infoPlistPath = buildRoot + "/Info.plist";
            Debug.Log("PostProcessBuild.ModifyInfoPlist: " + infoPlistPath);

            PlistDocument infoPlist = new PlistDocument();
            infoPlist.ReadFromFile(infoPlistPath);

            PlistElementDict root = infoPlist.root;

            // App is not using encryption so add the App Uses Non-Exempt Encryption to NO
            root.SetString("ITSAppUsesNonExemptEncryption", "NO");

            infoPlist.WriteToFile(infoPlistPath);
        }

        [PostProcessBuildAttribute(3)]
        public static void ConfigureBuild(BuildTarget target, string buildRoot)
        {
            if (target != BuildTarget.iOS)
                return;

            Debug.Log("PostProcessBuild.ConfigureBuild");

            var internalBuild = System.Environment.GetEnvironmentVariable("INTERNAL_BUILD");
            var teamId = System.Environment.GetEnvironmentVariable("DEVELOPMENT_TEAM");

            //CODE_SIGN_IDENTITY = iPhone Developer
            //CODE_SIGN_IDENTITY[sdk=iphoneos*] = iPhone Developer
            //PROVISIONING_PROFILE_SPECIFIER = $(PROVISIONING_PROFILE_SPECIFIER_APP)

            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildRoot);
            project.ReadFromFile(projectPath);

            string appTargetId = project.GetUnityMainTargetGuid();

            string bundleId = "";
            if (internalBuild != null && teamId != null) {
                project.SetTeamId(appTargetId, teamId); // sets DEVELOPMENT_TEAM
                project.SetBuildProperty(appTargetId, "UNIQUE_BUNDLE_DISAMBIGUATOR", "");
                bundleId = PollingBundleIdInternal + "${UNIQUE_BUNDLE_DISAMBIGUATOR}";
            } else {
                /* TODO: explain why we do this */
                project.SetTeamId(appTargetId, "(set dev team to build)");
                project.SetBuildProperty(appTargetId, "UNIQUE_BUNDLE_DISAMBIGUATOR", ".${DEVELOPMENT_TEAM}");
                bundleId = PollingBundleId + "${UNIQUE_BUNDLE_DISAMBIGUATOR}";
            }

            project.SetBuildProperty(appTargetId, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
            project.SetBuildProperty(appTargetId, "CODE_SIGN_STYLE", "automatic");

            project.WriteToFile(projectPath);
        }

#endif

    }
}

#endif
