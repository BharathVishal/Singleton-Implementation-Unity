using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Mediation.Dashboard.Editor
{
    static class JwtToken
    {
        static readonly Uri k_TokenExchangeUri = new Uri("https://services.unity.com/api/auth/v1/genesis-token-exchange/unity");

        const string k_errorRetrievingToken = "An error occured while attempting to retrieve a token to communicate with the Dashboard; ";
        const string k_errorInvalidToken    = "An error occured while attempting to retrieve a token to communicate with the Dashboard, the response format was unexpected; ";

        internal static string Token { get; private set; }
        internal static DateTime ExpireTime { get; private set; }

        internal static void GetTokenAsync(Action<string> onTokenObtained)
        {
            //Todo: figure out why the token must be refreshed in some cases. For now always refresh.
            Token = null;

            if (!string.IsNullOrEmpty(Token))
            {
                if (DateTime.Now > ExpireTime)
                {
                    //Token is expired, Refresh it.
                    CloudProjectSettings.RefreshAccessToken(_ => RetrieveTokenAsync(onTokenObtained));
                }
                else
                {
                    //We have a valid token, use it.
                    onTokenObtained?.Invoke(Token);
                }
            }
            else
            {
                //We need to Retrieve a token from the backend
                RetrieveTokenAsync(onTokenObtained);
            }
        }

        static void RetrieveTokenAsync(Action<string> onTokenRetrieved)
        {
            var request = new UnityWebRequest(k_TokenExchangeUri, UnityWebRequest.kHttpVerbPOST);
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"token\": \"" + CloudProjectSettings.accessToken + "\"}");

            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var requestAsyncOp = request.SendWebRequest();
            requestAsyncOp.completed += _ =>
            {
#if UNITY_2020_1_OR_NEWER
                bool isError = request.result != UnityWebRequest.Result.Success;
#else
                bool isError = request.isNetworkError || request.isHttpError;
#endif
                if (isError)
                {
                    Debug.LogWarning(k_errorRetrievingToken + request.error);
                    onTokenRetrieved(null);
                    return;
                }

                var tokenWrapper = JsonUtility.FromJson<JwtTokenWrapper>(request.downloadHandler.text);
                if (string.IsNullOrEmpty(tokenWrapper.JwtToken))
                {
                    Debug.LogWarning(k_errorInvalidToken + request.downloadHandler.text);
                    onTokenRetrieved(null);
                    return;
                }

                Token = tokenWrapper.JwtToken;
                ParseTTL(Token);
                onTokenRetrieved(Token);

                request.Dispose();
            };
        }

        static void ParseTTL(string jwt)
        {
            string[] jwtArray = jwt.Split('.');
            var jwtString = jwtArray[1];

            string jsonString = jwtString.FromBase64Safely();
            var header = JsonUtility.FromJson<JwtHeader>(jsonString);
            ExpireTime = DateTime.Now.AddSeconds(header.ExpiresAtTime - header.IssuedAtTime);
        }

        static string FromBase64Safely(this String str)
        {
            while (str.Length % 4 != 0)
                str += "=";

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        [System.Serializable]
        class JwtTokenWrapper
        {
            #pragma warning disable CS0649
            public string token;
            public string JwtToken => token;
            #pragma warning restore CS0649
        }

        [System.Serializable]
        class JwtHeader
        {
            #pragma warning disable CS0649
            public int iat;
            public int IssuedAtTime => iat;

            public int exp;
            public int ExpiresAtTime => exp;
            #pragma warning restore CS0649
        }
    }
}
