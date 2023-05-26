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

        List<string> _words = new();
        List<string> _learnedWords = new();
        int _currentNumber = -1;
        int _countBeforeHideFinishText = 2;
        LoadAndSave _loadAndSave;
        Translate _translate;
        public string CurrentWord { get; private set; }
        public string TranslatedWord { get; private set; }

        async void Start()
        {
            Init();
            await ShowWord();
        }

        void Init()
        {
            _translate = new Translate();
            _loadAndSave = new LoadAndSave();
            _learnedWords = _loadAndSave.LoadLearnedWords(Application.persistentDataPath);
            finishText.enabled = false;
            _words = _loadAndSave.LoadWords();
            RemoveLearnedWords();
            Subscribe();
            ShowWordsLeft();
        }

        async void ChangeCulture() => await GetTranslatedText(CurrentWord);

        void ShowWordsLeft() => wordsLeftText.text = $"{_words.Count} words left";

        async UniTask ShowWord()
        {
            var randomNumber = Utils.GetRandomNumber(_words);
            while (_currentNumber == randomNumber && _words.Count > 1)
                randomNumber = Utils.GetRandomNumber(_words);

            _currentNumber = randomNumber;
            CurrentWord = _words[randomNumber];
            mainText.text = CurrentWord;
            mainImage.sprite = Resources.Load<Sprite>(_words[randomNumber]);
            await GetTranslatedText(CurrentWord);
            CheckToHideFinishText();
        }

        async Task GetTranslatedText(string word)
        {
            var text = await _translate.Process("En", word);
            TranslatedWord = text;
            translatedText.text = "...";
            translatedText.text = Utils.ToUpperFirstChar(text);
        }

        private void CheckToHideFinishText()
        {
            if (finishText.isActiveAndEnabled)
                _countBeforeHideFinishText--;

            if (_countBeforeHideFinishText == 0)
                finishText.enabled = false;
        }


        void Learn()
        {
            ShowWord();
        }

        void Know()
        {
            var removed = _words[_currentNumber];
            _words.Remove(removed);
            _learnedWords.Add(removed);
            if (_words.Count == 0)
                ResetWords();

            ShowWordsLeft();
            ShowWord();
            _loadAndSave.SaveLearnedWords(Application.persistentDataPath, _learnedWords);
        }

        void RemoveLearnedWords()
        {
            if (_learnedWords.Count != _words.Count)
                _words = _words.Except(_learnedWords).ToList();
        }

        void ResetWords()
        {
            _learnedWords.Clear();
            _words = _loadAndSave.LoadWords();
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
            _loadAndSave.SaveLearnedWords(Application.persistentDataPath, _learnedWords);
        }

        void OnDestroy()
        {
            Unsubscribe();
            _loadAndSave.SaveLearnedWords(Application.persistentDataPath, _learnedWords);
        }
    }
}