using UnityEngine;

namespace EWords.Alert
{
    public class AlertSystem : MonoBehaviour
    {
        [SerializeField] AlertUI _alertUI;

        const float DisplayDuration = 2f;

        public void CreateInfoAlert(string message)
        {
            _alertUI.gameObject.SetActive(true);
            if(ColorUtility.TryParseHtmlString("#6198FF", out Color blueColor))
                _alertUI.Initialize(message, blueColor, DisplayDuration);
        }

        public void CreateErrorAlert(string message)
        {
            _alertUI.gameObject.SetActive(true);
            if(ColorUtility.TryParseHtmlString("#FF4523", out Color redColor))
                _alertUI.Initialize(message, redColor, DisplayDuration);
        }
    }
}