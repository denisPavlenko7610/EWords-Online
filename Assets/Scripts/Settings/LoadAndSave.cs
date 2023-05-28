using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace EWords
{
    public class LoadAndSave
    {
        const string ThemeKey = "Theme";
        const string Path = "Text/Text";

        public List<string> LoadWords()
        {
            List<string> words = new();
            TextAsset text = Resources.Load<TextAsset>(Path);
            if (text == null)
                throw new FileNotFoundException("Text txt not found");

            using StreamReader sr = new StreamReader(new MemoryStream(text.bytes));

            while (sr.ReadLine() is { } line)
                words.Add(line);

            return words.Shuffle();
        }
        public async void SaveWords(List<string> words) => await File.WriteAllLinesAsync($"{Application.dataPath}/Resources/{Path}.txt", words);

        public void SaveLearnedWords(string learnedWord)
        {
            var fs = new FileStream($"{Application.persistentDataPath}/LearnedWords.dat", FileMode.Create);
            var bf = new BinaryFormatter();
            List<string> list = new();
            list.Add(learnedWord);
            bf.Serialize(fs, list);
            fs.Close();
        }

        public List<string> LoadLearnedWords(string path)
        {
            List<string> learnedWords = new();
            var pathToLearnedWords = path + "/LearnedWords.dat";
            if (!File.Exists(pathToLearnedWords))
                return learnedWords;

            using Stream stream = File.Open(pathToLearnedWords, FileMode.Open);
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