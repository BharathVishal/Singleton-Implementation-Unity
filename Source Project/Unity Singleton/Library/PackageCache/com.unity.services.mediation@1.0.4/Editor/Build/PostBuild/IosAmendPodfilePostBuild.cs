#if UNITY_IOS

using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Unity.Services.Mediation.Build.Editor
{
    static class IosAmendPodfilePostBuild
    {
        const string k_UseFrameworksAddition = "\n\nuse_frameworks!";
        const string k_UnityIphoneTargetAddition = "\n\ntarget 'Unity-iPhone' do\nend";
        const string k_PostInstallAddition = @"

post_install do |installer|
  applicationTargets = [
    'Pods-Unity-iPhone',
  ]
  libraryTargets = [
    'Pods-UnityFramework',
  ]

  embedded_targets = installer.aggregate_targets.select { |aggregate_target|
    libraryTargets.include? aggregate_target.name
  }
  embedded_pod_targets = embedded_targets.flat_map { |embedded_target| embedded_target.pod_targets }
  host_targets = installer.aggregate_targets.select { |aggregate_target|
    applicationTargets.include? aggregate_target.name
  }

  host_targets.each do |host_target|
    host_target.xcconfigs.each do |config_name, config_file|
      host_target.pod_targets.each do |pod_target|
        if embedded_pod_targets.include? pod_target
          pod_target.specs.each do |spec|
            if spec.attributes_hash['ios'] != nil
              frameworkPaths = spec.attributes_hash['ios']['vendored_frameworks']
            else
              frameworkPaths = spec.attributes_hash['vendored_frameworks']
            end
            if frameworkPaths != nil
              frameworkNames = Array(frameworkPaths).map(&:to_s).map do |filename|
                extension = File.extname filename
                File.basename filename, extension
              end
              frameworkNames.each do |name|
                puts ""Removing #{name} from OTHER_LDFLAGS of target #{host_target.name}""
                config_file.frameworks.delete(name)
              end
            end
          end
        end
      end
      xcconfig_path = host_target.xcconfig_path(config_name)
      config_file.save_as(xcconfig_path)
    end
  end
end
";

        //IOSResolver.BUILD_ORDER_GEN_PODFILE = 40 (private), this step is 48, the install step is 50
        [PostProcessBuild(48)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            //TODO not ideal, the key should be reused between locations
            var forceDynamicLinkingSetting = new ReloadableUserSetting<bool>(MediationSettingsProvider.instance,
                MediationUserSettingsKeys.forceDynamicLinkingKey, true);
            if (!forceDynamicLinkingSetting) { return; }
            var podfilePath = pathToBuiltProject + "/Podfile";
            var podfileContent = File.ReadAllText(podfilePath);
            var amendedPodfileContents = AmendPodfileContent(podfileContent);
            File.WriteAllText(podfilePath, amendedPodfileContents);
        }

        internal static String AmendPodfileContent(string podfileContent)
        {
            UnityEngine.Debug.Log($"[Mediation] Amending Podfile content to force dynamic linking");
            //Removals

            var useFrameworksStaticLinkingRegex = new Regex(@"use_frameworks!\s*?:linkage\s*?=>\s*?:static",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var hasUseFrameworksStaticLinking = useFrameworksStaticLinkingRegex.IsMatch(podfileContent);
            if (hasUseFrameworksStaticLinking)
            {
                podfileContent = useFrameworksStaticLinkingRegex.Replace(podfileContent, "");
            }

            //Additions
            var unityIphoneTargetRegex = new Regex(@"target\s*?'Unity-iPhone'\s*?do",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var hasUnityIphoneTarget = unityIphoneTargetRegex.IsMatch(podfileContent);

            var useFrameworksRegex = new Regex(@"use_frameworks!",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var hasUseFrameworks = useFrameworksRegex.IsMatch(podfileContent);

            var postInstallRegex = new Regex(@"post_install\s*?do",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var hasPostInstall = postInstallRegex.IsMatch(podfileContent);

            if (!hasUseFrameworks)
            {
                podfileContent += k_UseFrameworksAddition;
            }

            if (!hasUnityIphoneTarget)
            {
                podfileContent += k_UnityIphoneTargetAddition;
            }

            if (!hasPostInstall)
            {
                podfileContent += k_PostInstallAddition;
            }

            return podfileContent;
        }
    }
}

#endif
