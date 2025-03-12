using System;
using Plant;
using UnityEngine;

namespace AddPlant
{
    public class PlantTypeSelector : MonoBehaviour
    {
        [SerializeField] private TypeButton[] _typeButtons;

        public event Action<PlantType> TypeSelected;

        private void OnEnable()
        {
            foreach (TypeButton typeButton in _typeButtons)
            {
                typeButton.TypeSelected += OnTypeSelected;
            }
        }

        private void OnDisable()
        {
            foreach (TypeButton typeButton in _typeButtons)
            {
                typeButton.TypeSelected -= OnTypeSelected;
            }
        }

        private void OnTypeSelected(PlantType type)
        {
            TypeSelected?.Invoke(type);
            gameObject.SetActive(false);
        }
    }
}