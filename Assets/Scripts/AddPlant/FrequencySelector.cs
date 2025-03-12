using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant
{
    public class FrequencySelector : MonoBehaviour
    {
        [SerializeField] private Button _plusButton;
        [SerializeField] private Button _minusButton;
        [SerializeField] private TMP_Text _numberText;

        public int CurrentFrequency { get; private set; }

        private void OnEnable()
        {
            CurrentFrequency = 1;
            _numberText.text = CurrentFrequency.ToString();

            _plusButton.onClick.AddListener(IncreaseNumber);
            _minusButton.onClick.AddListener(DecreaseNumber);
        }

        private void OnDisable()
        {
            _plusButton.onClick.RemoveListener(IncreaseNumber);
            _minusButton.onClick.RemoveListener(DecreaseNumber);
        }

        private void IncreaseNumber()
        {
            CurrentFrequency = Mathf.Clamp(CurrentFrequency + 1, 1, 99);
            _numberText.text = CurrentFrequency.ToString();
        }

        private void DecreaseNumber()
        {
            CurrentFrequency = Mathf.Clamp(CurrentFrequency - 1, 1, 99);
            _numberText.text = CurrentFrequency.ToString();
        }

        public void SetFrequency(int value)
        {
            CurrentFrequency = value;
            _numberText.text = CurrentFrequency.ToString();
        }
    }
}