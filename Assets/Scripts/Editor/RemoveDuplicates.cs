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
            if (File.Exists(Constants.Path))
            {
                string[] lines = await File.ReadAllLinesAsync(Constants.Path);
                string[] distinctLines = lines.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                await File.WriteAllLinesAsync(Constants.Path, distinctLines);
                int count = lines.Length - distinctLines.Length;
                if (count > 0)
                    Debug.Log($"{count} duplicate words removed.");
                else
                    Debug.Log("0 duplicates");
            }
            else
            {
                Debug.LogError("File not found.");
            }
        }
    }
}