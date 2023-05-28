using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EWords
{
    public class LoadAndSave
    {
        const string ThemeKey = "Theme";
        const string Path = "Text/Words";
        const string LearnedPath = "Text/LearnedWords";

        public List<string> LoadWords()
        {
            List<string> words = new();
            TextAsset text = Resources.Load<TextAsset>(Path);
            if (text == null)
            {                
                Debug.LogError("File not found");
                return words;
            }

            using StreamReader sr = new StreamReader(new MemoryStream(text.bytes));

            while (sr.ReadLine() is { } line)
                words.Add(line);

            return words.Shuffle();
        }
        public async void SaveWords(List<string> words) => await File.WriteAllLinesAsync($"{Application.dataPath}/Resources/{Path}.txt", words);

        public async void SaveLearnedWords(string learnedWord)
        {
            if(string.IsNullOrWhiteSpace(learnedWord))
                return;
            
            List<string> list = new();
            list.Add(learnedWord);
            await File.AppendAllLinesAsync($"{Application.dataPath}/Resources/{LearnedPath}.txt", list);
        }

        public List<string> LoadLearnedWords()
        {
            List<string> learnedWords = new();
            TextAsset text = Resources.Load<TextAsset>(LearnedPath);
            if (text == null)
            {                
                Debug.LogError("File not found");
                return learnedWords;
            }

            using StreamReader sr = new StreamReader(new MemoryStream(text.bytes));

            while (sr.ReadLine() is { } line)
                learnedWords.Add(line);

            return learnedWords;
        }

        public Theme LoadThemeSettings()
        {
            var theme = Theme.Black;
            if (!PlayerPrefs.HasKey(ThemeKey))
                return theme;

            var themeInSettings = PlayerPrefs.GetString(ThemeKey);
            theme = themeInSettings == Theme.Black.ToString()
                ? Theme.Black
                : Theme.White;

            return theme;
        }

        public void SaveThemeSettings(Theme theme) => PlayerPrefs.SetString(ThemeKey, theme.ToString());
    }
}