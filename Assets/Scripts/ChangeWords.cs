using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EWords
{
    [Serializable]
    public class ChangeWords : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] TextMeshProUGUI wordsLeftText;
        [SerializeField] TextMeshProUGUI finishText;
        [SerializeField] TextMeshProUGUI translatedText;
        [SerializeField] Image mainImage;
        [SerializeField] Button learnButton;
        [SerializeField] Button knowButton;

        List<string> words = new();
        List<string> learnedWords = new();
        int currentNumber = -1;
        int countBeforeHideFinishText = 2;
        LoadAndSave loadAndSave;
        Translate translate;
        public string CurrentWord { get; private set; }
        public string TranslatedWord { get; private set; }

        async void Start()
        {
            Init();
            await ShowWord();
        }

        void Init()
        {
            translate = new Translate();
            loadAndSave = new LoadAndSave();
            learnedWords = loadAndSave.LoadLearnedWords(Application.persistentDataPath);
            finishText.enabled = false;
            words = loadAndSave.LoadWords();
            RemoveLearnedWords();
            Subscribe();
            ShowWordsLeft();
        }

        async void ChangeCulture() => await GetTranslatedText(CurrentWord);

        void ShowWordsLeft() => wordsLeftText.text = $"{words.Count} words left";

        async UniTask ShowWord()
        {
            var randomNumber = Utils.GetRandomNumber(words);
            while (currentNumber == randomNumber && words.Count > 1)
                randomNumber = Utils.GetRandomNumber(words);

            currentNumber = randomNumber;
            CurrentWord = words[randomNumber];
            mainText.text = CurrentWord;
            mainImage.sprite = Resources.Load<Sprite>(words[randomNumber]);
            await GetTranslatedText(CurrentWord);
            CheckToHideFinishText();
        }

        async Task GetTranslatedText(string word)
        {
            var text = await translate.Process("En", word);
            TranslatedWord = text;
            translatedText.text = "...";
            translatedText.text = Utils.ToUpperFirstChar(text);
        }

        private void CheckToHideFinishText()
        {
            if (finishText.isActiveAndEnabled)
                countBeforeHideFinishText--;

            if (countBeforeHideFinishText == 0)
                finishText.enabled = false;
        }


        void Learn()
        {
            ShowWord();
        }

        void Know()
        {
            var removed = words[currentNumber];
            words.Remove(removed);
            learnedWords.Add(removed);
            if (words.Count == 0)
                ResetWords();

            ShowWordsLeft();
            ShowWord();
            loadAndSave.SaveLearnedWords(Application.persistentDataPath, learnedWords);
        }

        void RemoveLearnedWords()
        {
            if (learnedWords.Count != words.Count)
                words = words.Except(learnedWords).ToList();
        }

        void ResetWords()
        {
            learnedWords.Clear();
            words = loadAndSave.LoadWords();
            finishText.enabled = true;
        }

        void Subscribe()
        {
            learnButton.onClick.AddListener(Learn);
            knowButton.onClick.AddListener(Know);
        }

        void Unsubscribe()
        {
            learnButton.onClick.RemoveListener(Learn);
            knowButton.onClick.RemoveListener(Know);
        }

        void OnDisable()
        {
            Unsubscribe();
            loadAndSave.SaveLearnedWords(Application.persistentDataPath, learnedWords);
        }

        void OnDestroy()
        {
            Unsubscribe();
            loadAndSave.SaveLearnedWords(Application.persistentDataPath, learnedWords);
        }
    }
}