using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace EWords
{
    public class LoadAndSave
    {
        const string ThemeKey = "Theme";
        const string _path = "Text/Words";
        const string LearnedPath = "Text/LearnedWords";

        public List<string> LoadWords()
        {
            List<string> words = new();
            TextAsset text = Resources.Load<TextAsset>(_path);
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
        public async void SaveWords(List<string> words) => await File.WriteAllLinesAsync(Constants.Path, words);

        public void SaveLearnedWords(List<string> learnedWords)
        {
            var fs = new FileStream(Constants.LearnedPath, FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, learnedWords);
            fs.Close();
        }

        public List<string> LoadLearnedWords()
        {
            List<string> learnedWords = new();
            if (!File.Exists(Constants.LearnedPath))
                return learnedWords;

            using Stream stream = File.Open(Constants.LearnedPath, FileMode.Open);
            var bformatter = new BinaryFormatter();
            learnedWords = (List<string>)bformatter.Deserialize(stream);
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