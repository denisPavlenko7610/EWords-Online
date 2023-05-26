using System;
using Cysharp.Threading.Tasks;

namespace EWords
{
    using SimpleJSON;
    using UnityEngine;

    public class Translate
    {
        string _translatedText = "";

        public async UniTask<string> Process(string targetLang, string sourceText)
        {
            string sourceLang = "auto";
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                         + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

            WWW www = new WWW(url);
            await www;

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    var jsonNode = JSONNode.Parse(www.text);
                    _translatedText = jsonNode[0][0][0];
                    return _translatedText;
                }
            }

            return String.Empty;
        }

        public async UniTask<string> Process(string sourceLang, string targetLang, string sourceText)
        {
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                         + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

            WWW www = new WWW(url);
            await www;

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    var jsonNode = JSONNode.Parse(www.text);
                    _translatedText = jsonNode[0][0][0];
                    return _translatedText;
                }
            }
            
            return String.Empty;
        }
    }
}