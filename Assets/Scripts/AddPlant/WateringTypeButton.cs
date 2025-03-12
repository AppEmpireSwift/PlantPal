using System;
using Plant;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant
{
    public class WateringTypeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private WateringType _type;

        public event Action<WateringType> TypeSelected;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            TypeSelected?.Invoke(_type);
        }
    }
}