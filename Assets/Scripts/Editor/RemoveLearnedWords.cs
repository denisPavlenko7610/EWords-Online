using System.IO;
using UnityEditor;
using UnityEngine;

namespace EWords.Editor
{
    public class RemoveLearnedWords : EditorWindow
    {
        [MenuItem("Tools/Remove Learned")]
        public static async void Remove()
        {
            string path = $"{Application.persistentDataPath}/LearnedWords.dat";
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("File removed.");
            }
            else
            {
                Debug.LogError("File not found.");
            }
        }
    }
}