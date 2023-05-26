using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace EWords
{
    [RequireComponent(typeof(AudioSource), typeof(Button))]
    public class Speaker : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] Button speakerButtonNative;
        [SerializeField] Button speakerButtonTranslate;
        [SerializeField] ChangeWords changeWords;

        void OnEnable()
        {
            speakerButtonNative.onClick.AddListener(PlayNative);
            speakerButtonTranslate.onClick.AddListener(PlayTranslate);
        }

        void OnDisable()
        {
            speakerButtonNative.onClick.RemoveListener(PlayNative);
            speakerButtonTranslate.onClick.RemoveListener(PlayTranslate);
        }

        void OnValidate()
        {
            if (!audioSource)
                audioSource = GetComponent<AudioSource>();

            if (!changeWords)
                changeWords = GetComponent<ChangeWords>();
        }

        void PlayNative()
        {
            Process("En", changeWords.CurrentWord);
        }

        void PlayTranslate()
        {
            Process("En", changeWords.TranslatedWord);
        }

        async UniTask Process(string targetLang, string sourceText)
        {
            var url = "https://translate.google.com.vn/translate_tts?/ie=UTF-8&q=" + WWW.EscapeURL(sourceText)
                + "&tl=" + targetLang + "&client=tw-ob";

            var www = new WWW(url);
            await www;

            if (www.isDone)
            {
                audioSource.clip = www.GetAudioClip(false, false, AudioType.MPEG);
                audioSource.Play();
            }
        }
    }
}