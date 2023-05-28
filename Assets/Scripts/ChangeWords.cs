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
        [SerializeField] Image mainImage;
        [SerializeField] Button learnButton;
        [SerializeField] Button knowButton;
        [SerializeField] TextMeshProUGUI leftWords;
        [SerializeField] TextMeshProUGUI learnedWords;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Button addButton;
        [SerializeField] AlertSystem _alertSystem;

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
            if (!_alertSystem)
                _alertSystem = FindObjectOfType<AlertSystem>();
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
            _alertSystem.Init();
            inputField.characterLimit = 22;
            RemoveLearnedWords();
            Subscribe();
        }

        public void AddWord()
        {
            var word = inputField.text;
            inputField.text = String.Empty;
            if(String.IsNullOrEmpty(word))
                return;

            if (_words.Contains(word) || _learnedWords.Contains(word))
            {
                _alertSystem.CreateInfoAlert("This word is already exists");
                return;
            }
                
            _words.Add(word);
            _loadAndSave.SaveWords(_words);
            ShowCount();
        }

        bool IsHasWords()
        {
            var hasWords = _words.Count > 0;
            if(!hasWords)
                _alertSystem.CreateErrorAlert(Constants.WordsListIsEmpty);

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
            mainText.text = CurrentWord;
            mainImage.sprite = await _imageSearcher.LoadImage(CurrentWord);
            mainImage.preserveAspect = true;
            
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
            _loadAndSave.SaveLearnedWords(removed);
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
            addButton.onClick.AddListener(AddWord);
        }
        async void LearnSubscribe() => await Learn();

        async void KnowSubscribe() => await Next();

        void Unsubscribe()
        {
            learnButton.onClick.RemoveListener(LearnSubscribe);
            knowButton.onClick.RemoveListener(KnowSubscribe);
            addButton.onClick.RemoveListener(AddWord);
        }

        void OnDisable() => Unsubscribe();

        void OnDestroy() => Unsubscribe();
    }
}