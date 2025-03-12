using System;
using Plant;
using UnityEngine;

namespace AddPlant
{
    public class WateringTypeHolder : MonoBehaviour
    {
        [SerializeField] private WateringTypeButton[] _typeButtons;

        public event Action<WateringType> TypeSelected;

        private void OnEnable()
        {
            foreach (WateringTypeButton typeButton in _typeButtons)
            {
                typeButton.TypeSelected += OnTypeSelected;
            }
        }

        private void OnDisable()
        {
            foreach (WateringTypeButton typeButton in _typeButtons)
            {
                typeButton.TypeSelected -= OnTypeSelected;
            }
        }

        private void OnTypeSelected(WateringType type)
        {
            TypeSelected?.Invoke(type);
            gameObject.SetActive(false);
        }
    }
}