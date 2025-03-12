using System;
using System.Collections.Generic;
using System.Linq;
using AddPlant.AddCare;
using DG.Tweening;
using Plant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddPlantScreen : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;

        [SerializeField] private TMP_InputField _nameInput;

        [SerializeField] private TMP_Text _plantTypeText;
        [SerializeField] private Button _plantTypeButton;
        [SerializeField] private PlantTypeSelector _plantTypeSelector;
        [SerializeField] private PlantTypeDataProvider _plantTypeDataProvider;

        [SerializeField] private TMP_Text _wateringTypeText;
        [SerializeField] private Button _wateringTypeButton;
        [SerializeField] private WateringTypeHolder _wateringTypeHolder;
        [SerializeField] private FrequencySelector _frequencySelector;

        [SerializeField] private TMP_InputField _noteInput;

        [SerializeField] private PhotosController _photosController;

        [SerializeField] private Button _addPlantCareButton;
        [SerializeField] private AddCareScreen _addCareScreen;
        [SerializeField] private List<CarePlane> _carePlanes;
        [SerializeField] private GameObject _emptyCareObject;

        [SerializeField] private CareTypeDataProvider _careTypeDataProvider;
        [SerializeField] private WateringDataProvider _wateringDataProvider;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.3f;

        [SerializeField] private float _elementsDelay = 0.05f;
        [SerializeField] private float _startScale = 0.8f;
        [SerializeField] private Ease _easeType = Ease.OutBack;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private List<PlantCareData> _careDatas = new();

        private WateringType _wateringType;
        private PlantType _plantType;

        private bool _isEditMode = false;
        private PlantData _plantToEdit;

        public event Action<PlantData> DataCreated;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _addCareScreen.BackClicked += Enable;
            _addCareScreen.DataCreated += EnableCarePlane;
            _nameInput.onValueChanged.AddListener((_ => ToggleSaveButton()));
            _noteInput.onValueChanged.AddListener((_ => ToggleSaveButton()));
            _photosController.SetPhoto += ToggleSaveButton;
            _wateringTypeButton.onClick.AddListener(OnWateringButtonClicked);
            _plantTypeButton.onClick.AddListener(OnPlantTypeClicked);
            _backButton.onClick.AddListener(OnBackClicked);
            _saveButton.onClick.AddListener(OnSaveButtonClicked);

            _plantTypeSelector.TypeSelected += OnPlantTypeSelected;
            _wateringTypeHolder.TypeSelected += OnWateringTypeSelected;

            _addPlantCareButton.onClick.AddListener(OnAddCareClicked);

            foreach (CarePlane carePlane in _carePlanes)
            {
                carePlane.SetDataProvider(_careTypeDataProvider);
                carePlane.Deleted += DeleteCarePlane;
            }
        }

        private void OnDisable()
        {
            _addCareScreen.BackClicked -= Enable;
            _addCareScreen.DataCreated -= EnableCarePlane;
            _nameInput.onValueChanged.RemoveAllListeners();
            _noteInput.onValueChanged.RemoveAllListeners();
            _photosController.SetPhoto -= ToggleSaveButton;
            _wateringTypeButton.onClick.RemoveListener(OnWateringButtonClicked);
            _plantTypeButton.onClick.RemoveListener(OnPlantTypeClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);
            _saveButton.onClick.RemoveListener(OnSaveButtonClicked);

            _plantTypeSelector.TypeSelected -= OnPlantTypeSelected;
            _wateringTypeHolder.TypeSelected -= OnWateringTypeSelected;

            _addPlantCareButton.onClick.RemoveListener(OnAddCareClicked);

            foreach (CarePlane carePlane in _carePlanes)
            {
                carePlane.Deleted -= DeleteCarePlane;
            }

            DOTween.Kill(transform);
            foreach (var carePlane in _carePlanes)
            {
                DOTween.Kill(carePlane.transform);
            }
        }

        private void Start()
        {
            ResetScreen();
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
            AnimateScreenIn();
        }

        public void Disable()
        {
            AnimateScreenOut(() => _screenVisabilityHandler.DisableScreen());
        }

        private void AnimateScreenIn()
        {
            var uiElements = GetUIElementsForAnimation();

            foreach (var element in uiElements)
            {
                element.transform.localScale = Vector3.one * _startScale;
                element.alpha = 0f;
            }

            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                float delay = i * _elementsDelay;

                element.transform.DOScale(1f, _animationDuration)
                    .SetEase(_easeType)
                    .SetDelay(delay);

                element.transform.DOLocalMoveY(element.transform.localPosition.y + 10f, _animationDuration)
                    .From()
                    .SetEase(_easeType)
                    .SetDelay(delay);

                element.alpha = 0f;
                element.DOFade(1f, _animationDuration)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(delay);
            }
        }

        private void AnimateScreenOut(Action onComplete = null)
        {
            var uiElements = GetUIElementsForAnimation();
            int animationCount = uiElements.Count;

            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                float delay = i * _elementsDelay * 0.5f;

                element.transform.DOScale(_startScale, _animationDuration * 0.7f)
                    .SetEase(Ease.InBack)
                    .SetDelay(delay);

                element.DOFade(0f, _animationDuration * 0.7f)
                    .SetEase(Ease.InQuad)
                    .SetDelay(delay)
                    .OnComplete(() =>
                    {
                        animationCount--;
                        if (animationCount <= 0 && onComplete != null)
                        {
                            onComplete.Invoke();
                        }
                    });
            }
        }

        private List<CanvasGroup> GetUIElementsForAnimation()
        {
            var elements = new List<CanvasGroup>();

            elements.Add(GetOrAddCanvasGroup(_nameInput.gameObject));
            elements.Add(GetOrAddCanvasGroup(_plantTypeButton.gameObject));
            elements.Add(GetOrAddCanvasGroup(_wateringTypeButton.gameObject));
            elements.Add(GetOrAddCanvasGroup(_noteInput.gameObject));
            elements.Add(GetOrAddCanvasGroup(_photosController.gameObject));
            elements.Add(GetOrAddCanvasGroup(_addPlantCareButton.gameObject));

            foreach (var carePlane in _carePlanes.Where(p => p.IsActive))
            {
                elements.Add(GetOrAddCanvasGroup(carePlane.gameObject));
            }

            elements.Add(GetOrAddCanvasGroup(_backButton.gameObject));
            elements.Add(GetOrAddCanvasGroup(_saveButton.gameObject));

            return elements;
        }

        private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
        {
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = obj.AddComponent<CanvasGroup>();
            }

            return canvasGroup;
        }

        private void ResetScreen()
        {
            _nameInput.text = string.Empty;
            _noteInput.text = string.Empty;
            _plantTypeText.text = "Plant type";
            _wateringTypeText.text = "Watering type";
            _plantType = PlantType.None;
            _wateringType = WateringType.None;
            _photosController.ResetPhotos();
            _careDatas.Clear();
            DisableAllCarePlanes();
            ToggleSaveButton();
            ToggleEmptyCarePlanes();

            _frequencySelector.gameObject.SetActive(false);
            _wateringTypeHolder.gameObject.SetActive(false);
            _frequencySelector.gameObject.SetActive(false);
            _isEditMode = false;
            _plantToEdit = null;
        }

        private void DisableAllCarePlanes()
        {
            foreach (var carePlane in _carePlanes)
            {
                carePlane.OnReset();
                carePlane.Disable();
            }

            ToggleEmptyCarePlanes();
        }

        private void ToggleEmptyCarePlanes()
        {
            _emptyCareObject.SetActive(_carePlanes.All(p => !p.IsActive));
        }

        private void ToggleSaveButton()
        {
            bool canSave = !string.IsNullOrEmpty(_nameInput.text) && _plantType != PlantType.None &&
                           _wateringType != WateringType.None && !string.IsNullOrEmpty(_noteInput.text) &&
                           _photosController.GetPhoto() != null;

            if (canSave != _saveButton.interactable)
            {
                _saveButton.interactable = canSave;

                if (canSave)
                {
                    _saveButton.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad)
                        .OnComplete(() => { _saveButton.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack); });
                }
            }
        }

        private void OnWateringButtonClicked()
        {
            ShowElementWithAnimation(_wateringTypeHolder.gameObject);
        }

        private void OnPlantTypeClicked()
        {
            ShowElementWithAnimation(_plantTypeSelector.gameObject);
        }

        private void ShowElementWithAnimation(GameObject element)
        {
            element.SetActive(true);
            CanvasGroup canvasGroup = GetOrAddCanvasGroup(element);

            canvasGroup.alpha = 0f;
            element.transform.localScale = Vector3.one * 0.9f;

            DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, _animationDuration * 0.8f).SetEase(Ease.OutQuad))
                .Join(element.transform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack));
        }

        private void OnPlantTypeSelected(PlantType type)
        {
            _plantType = type;
            _plantTypeText.text = _plantTypeDataProvider.GetDataByType(type).TypeName;
            ToggleSaveButton();
        }

        private void OnWateringTypeSelected(WateringType type)
        {
            _wateringType = type;
            _wateringTypeText.text = _wateringDataProvider.GetName(_wateringType);
            _frequencySelector.gameObject.SetActive(_wateringType == WateringType.Custom);
            ToggleSaveButton();
        }

        private void OnAddCareClicked()
        {
            _addCareScreen.Enable();
            Disable();
        }

        private void EnableCarePlane(PlantCareData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Enable();
            var carePlane = _carePlanes.FirstOrDefault(p => !p.IsActive);
            if (carePlane != null)
            {
                carePlane.Enable(data);
                _careDatas.Add(data);

                var canvasGroup = carePlane.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, _animationDuration)
                        .SetEase(Ease.OutQuad);
                }
            }

            ToggleEmptyCarePlanes();
        }

        private void DeleteCarePlane(CarePlane plane)
        {
            if (_careDatas.Contains(plane.CareData))
                _careDatas.Remove(plane.CareData);

            plane.OnReset();
            plane.Disable();
            ToggleEmptyCarePlanes();
        }

        public void LoadPlantData(PlantData plantData)
        {
            if (plantData == null)
                return;

            _isEditMode = true;
            _plantToEdit = plantData;

            _nameInput.text = plantData.Name;
            _noteInput.text = plantData.Note;
            _plantType = plantData.PlantType;
            _wateringType = plantData.WateringType;

            _plantTypeText.text = _plantTypeDataProvider.GetDataByType(_plantType).TypeName;
            _wateringTypeText.text = _wateringType.ToString();

            if (_wateringType == WateringType.Custom)
            {
                _frequencySelector.gameObject.SetActive(true);
                _frequencySelector.SetFrequency(plantData.CustomFrequency);
            }

            _photosController.SetPhotos(plantData.Photo);

            if (plantData.PlantCareDatas != null && plantData.PlantCareDatas.Count > 0)
            {
                _careDatas = new List<PlantCareData>(plantData.PlantCareDatas);

                for (int i = 0; i < _careDatas.Count && i < _carePlanes.Count; i++)
                {
                    _carePlanes[i].Enable(_careDatas[i]);
                }
            }

            ToggleEmptyCarePlanes();
            ToggleSaveButton();
        }

        private bool ValidateInputs()
        {
            return !string.IsNullOrEmpty(_nameInput.text) &&
                   _plantType != PlantType.None &&
                   _wateringType != WateringType.None &&
                   _photosController.GetPhoto() != null;
        }

        private void OnSaveButtonClicked()
        {
            if (!ValidateInputs())
                return;

            PlantData data;

            if (_isEditMode && _plantToEdit != null)
            {
                data = _plantToEdit;
                data.Name = _nameInput.text;
                data.PlantType = _plantType;
                data.WateringType = _wateringType;
                data.Note = _noteInput.text;
                data.Photo = _photosController.GetPhoto();
            }
            else
            {
                data = new PlantData(_nameInput.text, _plantType, _wateringType, _noteInput.text,
                    _photosController.GetPhoto());
            }

            if (_wateringType == WateringType.Custom)
                data.CustomFrequency = _frequencySelector.CurrentFrequency;

            if (_careDatas.Count > 0)
                data.PlantCareDatas = new List<PlantCareData>(_careDatas);

            DataCreated?.Invoke(data);
            Disable();
            ResetScreen();
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            Disable();
            ResetScreen();
        }
    }
}