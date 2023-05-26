using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWords.Images;
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

        List<string> _words = new();
        List<string> _learnedWords = new();
        int _currentNumber = -1;
        LoadAndSave _loadAndSave;
        Translate _translate;
        ImageSearcher _imageSearcher;
        public string CurrentWord { get; private set; }
        public string TranslatedWord { get; private set; }

        async void Start()
        {
            Init();
            
            _imageSearcher = new ImageSearcher();
            await ShowWord();
        }

        void Init()
        {
            _translate = new Translate();
            _loadAndSave = new LoadAndSave();
            _learnedWords = _loadAndSave.LoadLearnedWords(Application.persistentDataPath);
            _words = _loadAndSave.LoadWords();
            RemoveLearnedWords();
            Subscribe();
        }

        async void ChangeCulture() => await GetTranslatedText(CurrentWord);

        async Task ShowWord()
        {
            var randomNumber = Utils.GetRandomNumber(_words);
            while (_currentNumber == randomNumber && _words.Count > 1)
                randomNumber = Utils.GetRandomNumber(_words);

            _currentNumber = randomNumber;
            CurrentWord = _words[randomNumber];
            mainText.text = CurrentWord;
            Material material = new Material(Shader.Find("UI/Unlit/Transparent"));
            Texture2D texture = await _imageSearcher.LoadImage();
            material.SetTexture("_MainTex", texture);
            mainImage.material = material;
            Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f), 100);
            mainImage.sprite = sprite;

            mainImage.preserveAspect = true;
            await GetTranslatedText(CurrentWord);
        }

        async Task GetTranslatedText(string word)
        {
            var text = await _translate.Process("Ru", word);
            TranslatedWord = text;
            translatedText.text = "...";
            translatedText.text = Utils.ToUpperFirstChar(text);
        }


        async Task Learn()
        {
            await ShowWord();
        }

        async Task Know()
        {
            var removed = _words[_currentNumber];
            _words.Remove(removed);
            _learnedWords.Add(removed);
            if (_words.Count == 0)
                ResetWords();

            await ShowWord();
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
        }

        void Subscribe()
        {
            learnButton.onClick.AddListener(LearnSubscribe);
            knowButton.onClick.AddListener(KnowSubscribe);
        }
        async void LearnSubscribe() => await Learn();

        async void KnowSubscribe() => await Know();

        void Unsubscribe()
        {
            learnButton.onClick.RemoveListener(LearnSubscribe);
            knowButton.onClick.RemoveListener(KnowSubscribe);
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