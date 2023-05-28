using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EWords.Editor
{
    public class RemoveDuplicates : EditorWindow
    {
        [MenuItem("Tools/Remove Duplicates")]
        public static async void Remove()
        {
            string path = Application.dataPath + "/Resources/Text/Text.txt";
            if (File.Exists(path))
            {
                string[] lines = await File.ReadAllLinesAsync(path);
                string[] distinctLines = lines.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                await File.WriteAllLinesAsync(path, distinctLines);
                Debug.Log("Duplicates removed.");
            }
            else
            {
                Debug.LogError("File not found.");
            }
        }
    }
}