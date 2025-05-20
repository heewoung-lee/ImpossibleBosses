using UnityEngine;

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

    public TextAsset[] LoadJson()
    {
        TextAsset[] jsonFiles = Managers.ResourceManager.LoadAll<TextAsset>("GoogleLoginData");
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
