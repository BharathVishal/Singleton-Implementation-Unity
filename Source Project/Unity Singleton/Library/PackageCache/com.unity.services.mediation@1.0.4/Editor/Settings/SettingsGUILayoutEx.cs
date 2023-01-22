using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Unity.Services.Mediation.Settings.Editor
{
    static class SettingsGUILayoutEx
    {
        static MethodInfo s_MatchSearchGroups;
        public static bool SettingsToggleLeft(string label, UserSetting<bool> value, string searchContext)
        {
            if (!MatchSearchGroups(searchContext, label))
                return value;
            var res = EditorGUILayout.ToggleLeft(label, value);
            SettingsGUILayout.DoResetContextMenuForLastRect(value);
            return res;
        }

        public static string SettingsPopupString(string label, UserSetting<string> value, string[] displayedOptions, string searchContext)
        {
            var index = Array.IndexOf(displayedOptions, value);
            if (index == -1)
            {
                index = 0;
                GUI.changed = true;
            }

            if (!MatchSearchGroups(searchContext, label))
                return displayedOptions[index];

            index = EditorGUILayout.Popup(label, index, displayedOptions);
            SettingsGUILayout.DoResetContextMenuForLastRect(value);
            return displayedOptions[index];
        }

        internal static bool MatchSearchGroups(string searchContext, string label)
        {
            if (s_MatchSearchGroups == null)
            {
                s_MatchSearchGroups = typeof(SettingsGUILayout).GetMethod("MatchSearchGroups", BindingFlags.Static | BindingFlags.NonPublic);
            }

            try
            {
                return (bool)s_MatchSearchGroups.Invoke(null, new object[] {searchContext, label});
            }
            catch (Exception)
            {
                //TODO: analytics event
                //If we can't search, show the UI
                return true;
            }
        }
    }
}
