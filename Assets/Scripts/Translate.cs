using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace EWords
{
    using SimpleJSON;

    public class Translate
    {
        string _translatedText = "";

        public async UniTask<string> Process(string targetLang, string sourceText)
        {
            string sourceLang = "auto";
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

            UnityWebRequest www = UnityWebRequest.Get(url);
            await www.SendWebRequest();

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    var jsonNode = JSONNode.Parse(www.downloadHandler.text);
                    _translatedText = jsonNode[0][0][0];
                    return _translatedText;
                }
            }

            return String.Empty;
        }

        public async UniTask<string> Process(string sourceLang, string targetLang, string sourceText)
        {
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(sourceText);

            UnityWebRequest www = UnityWebRequest.Get(url);
            await www.SendWebRequest();

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    var jsonNode = JSONNode.Parse(www.downloadHandler.text);
                    _translatedText = jsonNode[0][0][0];
                    return _translatedText;
                }
            }

            return String.Empty;
        }
    }
}