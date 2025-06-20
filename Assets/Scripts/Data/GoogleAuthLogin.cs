using GameManagers;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Zenject;

namespace Data
{
    [System.Serializable]
    public struct InstalledData
    {
        public string client_id;
        public string client_secret;
    }
    [System.Serializable]
    public struct GoogleLoginWrapper
    {
        public InstalledData installed;
    }
    public class GoogleAuthLogin
    {
        [Inject] private IResourcesLoader _loader;
        public TextAsset[] LoadJson()
        {
            TextAsset[] jsonFiles = _loader.LoadAll<TextAsset>("GoogleLoginData");
            return jsonFiles;
        }

        public GoogleLoginWrapper ParseJsontoGoogleAuth(TextAsset[] jsonFile)
        {
            foreach (TextAsset file in jsonFile)
            {
                if (file.text.Contains("\"installed\"") == false)
                    continue;

                return JsonUtility.FromJson<GoogleLoginWrapper>(file.text);
            }

            return default(GoogleLoginWrapper);
        }
    }
}