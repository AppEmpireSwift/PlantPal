using System;
using Plant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant
{
    public class CarePlane : MonoBehaviour
    {
        [SerializeField] private Image _typeImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _deleteButton;

        private CareTypeDataProvider _careTypeDataProvider;

        public event Action<CarePlane> Deleted;

        public PlantCareData CareData { get; private set; }
        public bool IsActive { get; private set; }

        private void OnEnable()
        {
            _deleteButton.onClick.AddListener(OnDeleteClicked);
        }

        private void OnDisable()
        {
            _deleteButton.onClick.RemoveListener(OnDeleteClicked);
        }

        public void SetDataProvider(CareTypeDataProvider dataProvider)
        {
            _careTypeDataProvider = dataProvider;
        }

        public void Enable(PlantCareData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            IsActive = true;
            gameObject.SetActive(IsActive);

            CareData = data;

            _typeImage.sprite = _careTypeDataProvider.GetDataByType(CareData.CareType).Sprite;
            _nameText.text = CareData.Name;
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);
        }

        public void OnReset()
        {
            _nameText.text = string.Empty;
            CareData = null;
        }

        private void OnDeleteClicked()
        {
            Deleted?.Invoke(this);
        }
    }
}