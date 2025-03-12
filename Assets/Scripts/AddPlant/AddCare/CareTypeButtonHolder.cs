using System;
using Plant;
using UnityEngine;

namespace AddPlant.AddCare
{
    public class CareTypeButtonHolder : MonoBehaviour
    {
        [SerializeField] private CareTypeButton[] _buttons;

        public event Action<CareType> CareTypeChosen;

        private void OnEnable()
        {
            foreach (CareTypeButton careTypeButton in _buttons)
            {
                careTypeButton.TypeSelected += SelectType;
            }
        }

        private void OnDisable()
        {
            foreach (CareTypeButton careTypeButton in _buttons)
            {
                careTypeButton.TypeSelected -= SelectType;
            }
        }

        private void SelectType(CareType type)
        {
            CareTypeChosen?.Invoke(type);
            gameObject.SetActive(false);
        }
    }
}