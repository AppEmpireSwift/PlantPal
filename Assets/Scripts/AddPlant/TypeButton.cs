using System;
using Plant;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant
{
    public class TypeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private PlantType _type;

        public event Action<PlantType> TypeSelected;

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