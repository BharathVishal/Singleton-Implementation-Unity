#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    static class AndroidJavaObjectExtensions
    {
        public static T ToEnum<T>(this AndroidJavaObject androidEnum) where T : Enum
        {
            return (T)(ValueType)androidEnum.Call<int>("getValue");
        }

        public static AndroidJavaObject ToAndroidEnum(string enumClassPath, int enumValue)
        {
            return new AndroidJavaClass(enumClassPath).CallStatic<AndroidJavaObject[]>("values")[enumValue];
        }
    }
}
#endif
