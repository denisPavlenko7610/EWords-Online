﻿using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

namespace EWords.Images
{
    public class ImageSearcher
    {
        string _keyword;

        public async Task<Sprite> LoadImage(string name)
        {
            _keyword = name;
            string imageUrl = await GetImageLink(_keyword);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                byte[] imageData = await DownloadImage(imageUrl);

                Texture2D texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
                texture.LoadImage(imageData);
                texture.alphaIsTransparency = true;
                
                Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(.5f,.5f), 100);
                return sprite;
            }
            
            Debug.Log("No image found on the page.");
            return null;
        }

        async Task<string> GetImageLink(string keyword)
        {
            string url = $"https://www.google.com/search?q={Uri.EscapeDataString(keyword)}&tbm=isch&tbs=ic:trans";
            using UnityWebRequest www = UnityWebRequest.Get(url);
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string htmlContent = www.downloadHandler.text;
                string imageUrl = ExtractImageLink(htmlContent);
                return imageUrl;
            }

            Debug.LogError($"Error loading page: {www.error}");
            return null;
        }

        string ExtractImageLink(string htmlContent)
        {
            int imgTagStartIndex = htmlContent.IndexOf("<img class=\"yWs4tf\"", StringComparison.Ordinal);
            if (imgTagStartIndex != -1)
            {
                int srcAttrStartIndex = htmlContent.IndexOf("src=\"", imgTagStartIndex, StringComparison.Ordinal) + 5;
                int srcAttrEndIndex = htmlContent.IndexOf("\"", srcAttrStartIndex, StringComparison.Ordinal);
                if (srcAttrEndIndex != -1)
                {
                    string imageUrl = htmlContent.Substring(srcAttrStartIndex, srcAttrEndIndex - srcAttrStartIndex);
                    return imageUrl;
                }
            }

            return null;
        }

        async Task<byte[]> DownloadImage(string imageUrl)
        {
            using UnityWebRequest www = UnityWebRequest.Get(imageUrl);
            
            DownloadHandlerTexture downloadHandler = new DownloadHandlerTexture(true);
            www.downloadHandler = downloadHandler;

            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = downloadHandler.texture;
                return texture.EncodeToPNG();
            }

            Debug.LogError($"Error downloading image: {www.error}");

            return null;
        }
    }
}