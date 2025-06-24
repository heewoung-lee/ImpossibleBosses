using UnityEngine;

namespace GameManagers.Interface.GoogleAuthLogin
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
    public interface IGoogleAuthLoginLoader
    {
        public TextAsset[] 	LoadGoogleAuthJsonFiles();
        public GoogleLoginWrapper ParseJsontoGoogleAuth(TextAsset[] jsonFile);
    }
}
