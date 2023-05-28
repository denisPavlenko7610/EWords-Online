using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EWords.Editor
{
    public class DeleteFromLearned : EditorWindow
    {
        [MenuItem("Tools/Remove from Learned")]
        public static async void Remove()
        {
            if (File.Exists(Constants.Path))
            {
                string[] wordsLines = await File.ReadAllLinesAsync(Constants.Path);
                int lastCount = wordsLines.Length;

                string[] learnedLines = await File.ReadAllLinesAsync(Constants.LearnedPath);
                wordsLines.ToList().RemoveAll(line => learnedLines.Contains(line));
                int newCount = wordsLines.Length;
                
                await File.WriteAllLinesAsync(Constants.Path, wordsLines);
                int count = lastCount - newCount;
                Debug.Log(count > 0 
                    ? $"{count} words removed." 
                    : "0 duplicates");
            }
            else
            {
                Debug.LogError("File not found.");
            }
        }
    }
}