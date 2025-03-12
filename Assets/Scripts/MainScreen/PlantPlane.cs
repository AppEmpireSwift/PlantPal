using System;
using PhotoPicker;
using Plant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class PlantPlane : MonoBehaviour
    {
        [SerializeField] private Color _redColor;
        [SerializeField] private Color _greenColor;

        [SerializeField] private ImagePlacer _imagePlacer;
        [SerializeField] private TMP_Text _plantName;
        [SerializeField] private TMP_Text _typeName;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _openButton;
        [SerializeField] private Image _typeImage;

        private PlantTypeDataProvider _plantTypeDataProvider;
        private int _wateringCount;

        public event Action<PlantPlane> Opened;

        public PlantData PlantData { get; private set; }
        public bool IsActive { get; private set; }
        public int RequireWatering { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnPlaneOpened);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnPlaneOpened);
        }

        public void SetDataProvider(PlantTypeDataProvider dataProvider)
        {
            _plantTypeDataProvider = dataProvider;
        }

        public void Enable(PlantData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            IsActive = true;
            gameObject.SetActive(IsActive);
            PlantData = data;

            _imagePlacer.SetImage(PlantData.Photo);
            _plantName.text = PlantData.Name;

            var typeData = _plantTypeDataProvider.GetDataByType(PlantData.PlantType);

            _typeImage.sprite = typeData.Sprite;
            _typeName.text = typeData.TypeName;

            _wateringCount = PlantData.WateringProgress;
            UpdateWateringProgress();
        }

        public void Disable()
        {
            IsActive = false;
            gameObject.SetActive(IsActive);

            PlantData = null;
        }

        public void UpdateWateringCount(int newCount)
        {
            _wateringCount = newCount;
            UpdateWateringProgress();
        }

        private void UpdateWateringProgress()
        {
            int requiredWaterings = GetRequiredWateringCount(PlantData.WateringType, PlantData.CustomFrequency);

            RequireWatering = requiredWaterings;

            if (_wateringCount < requiredWaterings)
            {
                _statusText.text = "Need watering";
                _statusText.color = _redColor;
            }
            else
            {
                _statusText.text = "Good";
                _statusText.color = _greenColor;
            }
        }

        public int GetRequiredWateringCount(WateringType wateringType, int customFrequency)
        {
            return wateringType switch
            {
                WateringType.Daily => 1,
                WateringType.EveryTwoDays => 1,
                WateringType.TwiceAWeek => 2,
                WateringType.OnceAWeek => 1,
                WateringType.EveryTwoWeeks => 1,
                WateringType.OnceAMonth => 1,
                WateringType.Custom => customFrequency,
                _ => 1
            };
        }

        private void OnPlaneOpened()
        {
            Opened?.Invoke(this);
        }
    }
}