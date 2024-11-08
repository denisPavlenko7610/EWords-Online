﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EWords
{
    public class ThemeSettings : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] TextMeshProUGUI translatedText;
        [SerializeField] TextMeshProUGUI leftCount;
        [SerializeField] TextMeshProUGUI learnedCount;
        [SerializeField] Color blackColor;
        [SerializeField] Color whiteColor;
        [SerializeField] Image backgroundImage;
        [SerializeField] Button switchThemeButton;

        Theme _theme = Theme.Black;
        LoadAndSave _loadAndSave;

        void OnEnable()
        {
            switchThemeButton.onClick.AddListener(SwitchTheme);
        }

        void OnDisable()
        {
            switchThemeButton.onClick.RemoveListener(SwitchTheme);
        }

        void Start()
        {
            _loadAndSave = new LoadAndSave();
            _theme = _loadAndSave.LoadThemeSettings();
            SetTheme();
        }

        public void SetTheme()
        {
            Color buttonTextColor;
            Color textColor;
            if (_theme == Theme.Black)
            {
                buttonTextColor = whiteColor;
                textColor = blackColor;
                switchThemeButton.image.sprite = Resources.Load<Sprite>("Tools/SunBlack");
            }
            else
            {
                switchThemeButton.image.sprite = Resources.Load<Sprite>("Tools/SunWhite");;
                textColor = whiteColor;
                buttonTextColor = blackColor;
            }

            translatedText.color = textColor;
            leftCount.color = textColor;
            learnedCount.color = textColor;
            backgroundImage.color = buttonTextColor;
            mainText.color = textColor;
        }
        
        void SwitchTheme()
        {
            _theme = _theme == Theme.Black 
                ? Theme.White 
                : Theme.Black;

            SetTheme();
            _loadAndSave.SaveThemeSettings(_theme);
        }
    }
}