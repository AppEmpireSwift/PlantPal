using System;
using Plant;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant.AddCare
{
    public class CareTypeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private CareType _type;

        public event Action<CareType> TypeSelected;

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