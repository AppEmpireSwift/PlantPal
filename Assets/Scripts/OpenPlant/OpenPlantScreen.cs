using System;
using System.Collections.Generic;
using System.Linq;
using AddPlant;
using DG.Tweening;
using MainScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPlant
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenPlantScreen : MonoBehaviour
    {
        [SerializeField] private Color _redColor;
        [SerializeField] private Color _greenColor;

        [SerializeField] private Button _backButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private PhotosController _photosController;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Button _wateringButton;
        [SerializeField] private Image _filledImageWateringProgress;
        [SerializeField] private TMP_Text _wateringProgressText;
        [SerializeField] private TMP_Text _wateringTypeText;
        [SerializeField] private TMP_Text _plantTypeText;
        [SerializeField] private Image _plantTypeImage;
        [SerializeField] private PlantTypeDataProvider _plantTypeDataProvider;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _noteText;
        [SerializeField] private GameObject _emptyCarePlane;
        [SerializeField] private List<CarePlane> _carePlanes;
        [SerializeField] private WateringDataProvider _wateringDataProvider;
        [SerializeField] private CareTypeDataProvider _careTypeDataProvider;
        [SerializeField] private Button _deleteButton;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeInDuration = 0.3f;

        [SerializeField] private float _moveInDuration = 0.4f;
        [SerializeField] private float _buttonClickScale = 0.9f;
        [SerializeField] private float _buttonAnimationDuration = 0.2f;
        [SerializeField] private float _careItemAnimDelay = 0.05f;
        [SerializeField] private float _progressBarAnimDuration = 0.7f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private Ease _moveEase = Ease.OutBack;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private PlantPlane _currentPlane;
        private CanvasGroup _canvasGroup;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private float _originalFillAmount;

        public event Action BackClicked;
        public event Action<PlantPlane> EditPlantClicked;
        public event Action<PlantPlane> DeleteClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            SetupButtonAnimations();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _deleteButton.onClick.AddListener(OnDeleteClicked);

            if (_editButton != null)
            {
                _editButton.onClick.AddListener(OnEditClicked);
            }

            if (_wateringButton != null)
            {
                _wateringButton.onClick.AddListener(OnWateringClicked);
            }

            foreach (var carePlane in _carePlanes)
            {
                carePlane.SetDataProvider(_careTypeDataProvider);
            }
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
            _deleteButton.onClick.RemoveListener(OnDeleteClicked);

            if (_editButton != null)
            {
                _editButton.onClick.RemoveListener(OnEditClicked);
            }

            if (_wateringButton != null)
            {
                _wateringButton.onClick.RemoveListener(OnWateringClicked);
            }

            DOTween.Kill(transform);
            DOTween.Kill(_name.transform);
            DOTween.Kill(_backButton.transform);
            DOTween.Kill(_editButton.transform);
            DOTween.Kill(_wateringButton.transform);
            DOTween.Kill(_filledImageWateringProgress.transform);
            DOTween.Kill(_plantTypeImage.transform);

            DOTween.Kill(_filledImageWateringProgress);
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable(PlantPlane plane)
        {
            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            _currentPlane = plane;

            _photosController.SetPhotos(_currentPlane.PlantData.Photo);
            _name.text = _currentPlane.PlantData.Name;

            _originalFillAmount = _currentPlane.PlantData.WateringProgress / _currentPlane.RequireWatering;

            _filledImageWateringProgress.fillAmount = 0;

            _wateringProgressText.text = $"{_currentPlane.PlantData.WateringProgress}/{_currentPlane.RequireWatering}";
            _wateringProgressText.alpha = 0;

            _wateringTypeText.text = _wateringDataProvider.GetName(_currentPlane.PlantData.WateringType);
            _wateringTypeText.alpha = 0;

            _noteText.text = _currentPlane.PlantData.Note;
            _noteText.alpha = 0;

            _plantTypeText.text = _plantTypeDataProvider.GetDataByType(_currentPlane.PlantData.PlantType).TypeName;
            _plantTypeImage.sprite = _plantTypeDataProvider.GetDataByType(_currentPlane.PlantData.PlantType).Sprite;

            UpdateWateringStatus();

            _name.transform.localScale = Vector3.zero;
            if (_plantTypeImage != null)
            {
                _plantTypeImage.transform.localScale = Vector3.zero;
            }

            foreach (var carePlane in _carePlanes)
            {
                carePlane.Disable();
            }

            if (_currentPlane.PlantData.PlantCareDatas.Count > 0)
            {
                foreach (var plantDataPlantCareData in _currentPlane.PlantData.PlantCareDatas)
                {
                    var carePlane = _carePlanes.FirstOrDefault(p => !p.IsActive);
                    if (carePlane != null)
                    {
                        carePlane.Enable(plantDataPlantCareData);
                    }
                }
            }

            _emptyCarePlane.SetActive(_currentPlane.PlantData.PlantCareDatas.Count <= 0);
            if (_emptyCarePlane.activeSelf)
            {
                _emptyCarePlane.transform.localScale = Vector3.zero;
            }

            CreateOpenSequence();

            _screenVisabilityHandler.EnableScreen();
        }

        private void UpdateWateringStatus()
        {
            if (_currentPlane.PlantData.WateringProgress < _currentPlane.RequireWatering)
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

        public void Disable()
        {
            if (_openSequence != null && _openSequence.IsActive())
            {
                _openSequence.Kill();
            }

            _closeSequence = DOTween.Sequence();

            Vector3 endPosition = new Vector3(transform.position.x + 100f, transform.position.y, transform.position.z);

            _closeSequence.Append(_canvasGroup.DOFade(0f, _fadeInDuration).SetEase(_fadeEase));
            _closeSequence.Join(transform.DOMove(endPosition, _moveInDuration).SetEase(Ease.InBack));

            _closeSequence.OnComplete(() => _screenVisabilityHandler.DisableScreen());

            _closeSequence.Play();
        }

        private void ShowWateringStatus()
        {
            _filledImageWateringProgress.DOFillAmount(_originalFillAmount, _progressBarAnimDuration)
                .SetEase(Ease.OutQuad);

            _wateringProgressText.DOFade(1, 0.5f).SetDelay(0.2f);
            _wateringTypeText.DOFade(1, 0.5f).SetDelay(0.3f);
        }

        private void OnBackClicked()
        {
            Sequence backSequence = DOTween.Sequence();

            backSequence.Append(_backButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f));

            backSequence.AppendInterval(0.2f);

            backSequence.OnComplete(() =>
            {
                BackClicked?.Invoke();
                Disable();
            });

            backSequence.Play();
        }

        private void OnEditClicked()
        {
            Sequence editSequence = DOTween.Sequence();

            editSequence.Append(_editButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f));

            editSequence.AppendInterval(0.2f);

            editSequence.OnComplete(() => { EditPlantClicked?.Invoke(_currentPlane); });

            editSequence.Play();
        }

        private void OnWateringClicked()
        {
            Sequence wateringSequence = DOTween.Sequence();

            wateringSequence.Append(_wateringButton.transform.DOScale(0.8f, 0.2f));
            wateringSequence.Append(_wateringButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

            wateringSequence.OnComplete(() =>
            {
                _currentPlane.PlantData.WateringProgress += 1;

                _currentPlane.PlantData.LastWateringDate = DateTime.Now;

                WateringResetManager.UpdateLastWateringDate(_currentPlane.PlantData);

                _wateringProgressText.text =
                    $"{_currentPlane.PlantData.WateringProgress}/{_currentPlane.RequireWatering}";

                float newFillAmount = (float)_currentPlane.PlantData.WateringProgress / _currentPlane.RequireWatering;
                newFillAmount = Mathf.Min(1f, newFillAmount);

                _filledImageWateringProgress.DOFillAmount(newFillAmount, 0.5f)
                    .SetEase(Ease.OutQuad);

                _currentPlane.UpdateWateringCount(_currentPlane.PlantData.WateringProgress);

                UpdateWateringStatus();
            });

            wateringSequence.Play();
        }

        private void CreateOpenSequence()
        {
            if (_closeSequence != null && _closeSequence.IsActive())
            {
                _closeSequence.Kill();
            }

            _openSequence = DOTween.Sequence();

            _canvasGroup.alpha = 0f;
            Vector3 startPosition = transform.position;
            transform.position = new Vector3(transform.position.x + 100f, transform.position.y, transform.position.z);

            _openSequence.Append(_canvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
            _openSequence.Join(transform.DOMove(startPosition, _moveInDuration).SetEase(_moveEase));

            _openSequence.Append(_name.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));

            if (_plantTypeImage != null)
            {
                _openSequence.Join(_plantTypeImage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));
            }

            _openSequence.AppendCallback(() => ShowWateringStatus());

            _openSequence.Join(_noteText.DOFade(1, 0.5f));

            float delay = 0.6f;
            foreach (var carePlane in _carePlanes.Where(p => p.IsActive))
            {
                _openSequence.Insert(delay, carePlane.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));
                delay += _careItemAnimDelay;
            }

            if (_emptyCarePlane.activeSelf)
            {
                _openSequence.Insert(delay, _emptyCarePlane.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));
            }

            _openSequence.Play();
        }

        private void SetupButtonAnimations()
        {
            AddButtonAnimations(_backButton);

            if (_editButton != null)
            {
                AddButtonAnimations(_editButton);
            }

            if (_wateringButton != null)
            {
                AddButtonAnimations(_wateringButton);
            }
        }

        private void OnDeleteClicked()
        {
            DeleteClicked?.Invoke(_currentPlane);
            Disable();
        }

        private void AddButtonAnimations(Button button)
        {
            Button.ButtonClickedEvent originalClickEvent = button.onClick;
            button.onClick = new Button.ButtonClickedEvent();

            button.onClick.AddListener(() =>
            {
                button.transform.DOScale(Vector3.one * _buttonClickScale, _buttonAnimationDuration / 2)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        button.transform.DOScale(Vector3.one, _buttonAnimationDuration / 2)
                            .SetEase(Ease.OutQuad);
                    });

                for (int i = 0; i < originalClickEvent.GetPersistentEventCount(); i++)
                {
                    originalClickEvent.Invoke();
                }
            });
        }
    }
}