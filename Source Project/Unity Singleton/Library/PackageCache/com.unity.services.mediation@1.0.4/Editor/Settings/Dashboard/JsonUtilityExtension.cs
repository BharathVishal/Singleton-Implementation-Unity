using UnityEngine;

namespace Unity.Services.Mediation.Dashboard.Editor
{
    static class JsonUtilityExtension
    {
        public static T[] FromJsonArray<T>(string json)
        {
            string wrappedJson = "{\"T_Array\" : " + json + " }";
            ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(wrappedJson);
            return wrapper.T_Array;
        }

        [System.Serializable]
        class ArrayWrapper<T>
        {
            #pragma warning disable CS0649
            public T[] T_Array;
            #pragma warning restore CS0649
        }
    }
}
