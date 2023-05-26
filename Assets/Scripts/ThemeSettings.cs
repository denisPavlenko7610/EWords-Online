using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EWords
{
    public class ThemeSettings : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] TextMeshProUGUI translatedText;
        [SerializeField] Color blackColor;
        [SerializeField] Color whiteColor;
        [SerializeField] Image backgroundImage;
        [SerializeField] Image arrowImage;
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
                arrowImage.sprite = Resources.Load<Sprite>("Tools/ArrowWhite");
            }
            else
            {
                switchThemeButton.image.sprite = Resources.Load<Sprite>("Tools/SunWhite");;
                arrowImage.sprite = Resources.Load<Sprite>("Tools/ArrowBlack");;
                textColor = whiteColor;
                buttonTextColor = blackColor;
            }

            translatedText.color = textColor;
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