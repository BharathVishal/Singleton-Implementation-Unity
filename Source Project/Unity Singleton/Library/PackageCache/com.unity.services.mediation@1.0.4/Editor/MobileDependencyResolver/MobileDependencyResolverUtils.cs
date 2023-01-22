using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MobileDependencyResolver.Utils.Editor
{
    static class MobileDependencyResolverUtils
    {
        static Type s_MobileDependencyResolverType;
        static Type s_IosResolverType;

        public static bool IsPresent => MobileDependencyResolverType != null;

        public static void ResolveIfNeeded()
        {
            if (!AutomaticResolutionEnabled)
            {
                Resolve();
            }
        }

        public static bool AutomaticResolutionEnabled
        {
            get
            {
                var psrType = MobileDependencyResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("AutomaticResolutionEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool GradleTemplateEnabled
        {
            get
            {
                var psrType = MobileDependencyResolverType;
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("GradleTemplateEnabled");
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
        }

        public static bool MainTemplateEnabled
        {
            get
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return false;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return false;
                return (bool)autoResolutionProperty.GetValue(null);
            }
            set
            {
                var psrType = Type.GetType("GooglePlayServices.SettingsDialog, Google.JarResolver");
                if (psrType == null) return;
                var autoResolutionProperty = psrType.GetProperty("PatchMainTemplateGradle", BindingFlags.Static | BindingFlags.NonPublic);
                if (autoResolutionProperty == null) return;
                autoResolutionProperty.SetValue(null, value);
            }
        }

        public static void Resolve()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("Resolve");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {Type.Missing, Type.Missing, Type.Missing});
        }

        public static void ResolveSync(bool forceResolution)
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("ResolveSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {forceResolution});
        }

        public static void DeleteResolvedLibraries()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return;
            var resolveMethod = psrType.GetMethod("DeleteResolvedLibrariesSync");
            if (resolveMethod == null) return;
            resolveMethod.Invoke(null, new object[] {});
        }

        public static IList<KeyValuePair<string, string>> GetPackageSpecs()
        {
            var psrType = MobileDependencyResolverType;
            if (psrType == null) return new List<KeyValuePair<string, string>>();
            var getPackageSpecsMethod = psrType.GetMethod("GetPackageSpecs");
            if (getPackageSpecsMethod == null) return new List<KeyValuePair<string, string>>();
            return (IList<KeyValuePair<string, string>>)getPackageSpecsMethod.Invoke(null, new object[] { null });
        }

        public static void PodUpdate(string pathToBuiltProject)
        {
            var iosResolverType = IosResolverType;
            if (iosResolverType == null) return;
            var method = IosResolverType.GetMethod("RunPodCommand", BindingFlags.Static | BindingFlags.NonPublic);
            var returnValue = method?.Invoke(obj: null, parameters: new object[] { "update", pathToBuiltProject, false });
            if (returnValue != null)
            {
                var memberInfo = returnValue.GetType().GetMember("message").First();
                if (memberInfo != null)
                {
                    Debug.Log($"result.message: {memberInfo.GetValue(returnValue)}");
                }
            }
        }

        static Type MobileDependencyResolverType
        {
            get
            {
                if (s_MobileDependencyResolverType != null)
                {
                    return s_MobileDependencyResolverType;
                }

                try
                {
                    s_MobileDependencyResolverType = Type.GetType("GooglePlayServices.PlayServicesResolver, Google.JarResolver");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_MobileDependencyResolverType;
            }
        }

        static Type IosResolverType
        {
            get
            {
                if (s_IosResolverType != null)
                {
                    return s_IosResolverType;
                }

                try
                {
                    s_IosResolverType = Type.GetType("Google.IOSResolver, Google.IOSResolver");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return s_IosResolverType;
            }
        }
        static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
