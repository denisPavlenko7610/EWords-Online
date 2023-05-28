using TMPro;
using UnityEngine;

namespace EWords.Alert
{
    public class AlertUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI alertText;
        
        float _currentDisplayTime;

        public void Initialize(string message, Color color, float duration)
        {
            alertText.text = message;
            alertText.color = color;
            _currentDisplayTime = duration;
        }

        void Update()
        {
            if (!(_currentDisplayTime > 0))
                return;
            
            _currentDisplayTime -= Time.deltaTime;
            if (_currentDisplayTime <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}