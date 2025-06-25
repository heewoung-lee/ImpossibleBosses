using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateInterfaceTemplate
{
    [MenuItem("Assets/Create/C# Interface %#i", false, 80)]
    private static void CreateInterface()
    {
        string templatePath = "Assets/Editor/ScriptTemplates/InterfaceTemplate.txt";
        string defaultName = "NewInterface.cs";
        
        if (!File.Exists(templatePath))
            Debug.LogError("Template NOT found!");
        
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, defaultName);
    }
}