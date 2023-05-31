using EWords.Alert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWords.Images;
using RDExtensions.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EWords
{
    [Serializable]
    public class ChangeWords : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] TextMeshProUGUI translatedText;
        [SerializeField] TextMeshProUGUI learnedWordExample;
        [SerializeField] Image mainImage;
        [SerializeField] Button learnButton;
        [SerializeField] Button knowButton;
        [SerializeField] TextMeshProUGUI leftWords;
        [SerializeField] TextMeshProUGUI learnedWords;
        [SerializeField] AlertSystem alertSystem;

        List<string> _words = new();
        List<string> _learnedWords = new();
        int _currentNumber = -1;
        
        LoadAndSave _loadAndSave;
        Translate _translate;
        ImageSearcher _imageSearcher;
        

        public string CurrentWord { get; private set; }
        public string TranslatedWord { get; private set; }

        void OnValidate()
        {
            if (!alertSystem)
                alertSystem = FindObjectOfType<AlertSystem>();
        }

        async void Start()
        {
            Init();
            
            _imageSearcher = new ImageSearcher();
            ShowCount();

            if(!IsHasWords())
                return;
            
            await ShowWord();
        }

        void Init()
        {
            _translate = new Translate();
            _loadAndSave = new LoadAndSave();
            _learnedWords = _loadAndSave.LoadLearnedWords();
            _words = _loadAndSave.LoadWords();
            RemoveLearnedWords();
            Subscribe();
        }

        bool IsHasWords()
        {
            var hasWords = _words.Count > 0;
            if(!hasWords)
                alertSystem.CreateErrorAlert(Constants.WordsListIsEmpty);

            return hasWords;
        }
        
        void ShowCount()
        {
            ShowWordsCount(_words, leftWords, "Left: ");
            ShowWordsCount(_learnedWords, learnedWords, "Learned: ");
        }
        void ShowWordsCount(List<string> words, TextMeshProUGUI textField, string text) => textField.text = text + words.Count;

        async void ChangeCulture() => await GetTranslatedText(CurrentWord);

        async Task ShowWord()
        {
            var randomNumber = Utils.GetRandomNumber(_words);
            while (_currentNumber == randomNumber && _words.Count > 1)
                randomNumber = Utils.GetRandomNumber(_words);

            _currentNumber = randomNumber;
            if(!IsHasWords())
                return;
            
            CurrentWord = _words[randomNumber];
            mainText.text = CurrentWord.ToUpperFirstChar();
            Sprite image = await _imageSearcher.LoadImageFromGoogle(CurrentWord);
            
            if(!image)
                image = await _imageSearcher.LoadImage(CurrentWord);
            
            if(!image)
                return;

            try
            {
                mainImage.sprite = image;
                mainImage.preserveAspect = true;
            }
            catch (Exception e)
            {
               //
            }

            learnedWordExample.text = $"Learned word: {_learnedWords[Utils.GetRandomNumber(_learnedWords)].ToUpperFirstChar()}";
            await GetTranslatedText(CurrentWord);
        }

        async Task GetTranslatedText(string word)
        {
            var text = await _translate.Process("Ru", word);
            TranslatedWord = text;
            translatedText.text = "...";
            translatedText.text = text.ToUpperFirstChar();
        }

        async Task Learn()
        {
            if(!IsHasWords())
                return;
                
            var removed = _words[_currentNumber];
            _words.Remove(removed);
            _loadAndSave.SaveWords(_words);
            _learnedWords.Add(removed);
            ShowCount();
            await ShowWord();
        }

        async Task Next()
        {
            if(!IsHasWords())
                return;
                
            await ShowWord();
        }

        void RemoveLearnedWords()
        {
            if (_learnedWords.Count != _words.Count)
                _words = _words.Except(_learnedWords).ToList();
        }

        void Subscribe()
        {
            learnButton.onClick.AddListener(LearnSubscribe);
            knowButton.onClick.AddListener(KnowSubscribe);
        }
        async void LearnSubscribe() => await Learn();

        async void KnowSubscribe() => await Next();

        void Unsubscribe()
        {
            learnButton.onClick.RemoveListener(LearnSubscribe);
            knowButton.onClick.RemoveListener(KnowSubscribe);
        }

        void OnDisable()
        {
            Unsubscribe();
            _loadAndSave.SaveLearnedWords(_learnedWords);
        }

        void OnDestroy()
        {
            Unsubscribe();
            _loadAndSave.SaveLearnedWords(_learnedWords);
        }

        void OnApplicationQuit()
        {
            _loadAndSave.SaveLearnedWords(_learnedWords);
        }
    }
}