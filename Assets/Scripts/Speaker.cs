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
        [SerializeField] ChangeWords changeWords;

        void OnEnable()
        {
            speakerButtonNative.onClick.AddListener(PlayNative);
        }

        void OnDisable()
        {
            speakerButtonNative.onClick.RemoveListener(PlayNative);
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