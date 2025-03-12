using System;
using DG.Tweening;
using Plant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlant.AddCare
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddCareScreen : MonoBehaviour
    {
        [SerializeField] private CareTypeButtonHolder _buttonHolder;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _chosenCareType;
        [SerializeField] private Button _chooseCareButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private CareTypeDataProvider _careTypeDataProvider;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeInDuration = 0.3f;

        [SerializeField] private float _moveInDuration = 0.4f;
        [SerializeField] private float _buttonClickScale = 0.9f;
        [SerializeField] private float _buttonAnimationDuration = 0.2f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private Ease _moveEase = Ease.OutBack;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private CareType _careType;
        private Vector3 _buttonHolderStartPosition;
        private CanvasGroup _canvasGroup;
        private Sequence _openSequence;
        private Sequence _closeSequence;

        public event Action<PlantCareData> DataCreated;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (_buttonHolder != null)
            {
                _buttonHolderStartPosition = _buttonHolder.transform.position;
            }

            SetupButtonAnimations();
        }

        private void OnEnable()
        {
            _buttonHolder.CareTypeChosen += SetCareType;
            _chooseCareButton.onClick.AddListener(OpenChoseCare);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _backButton.onClick.AddListener(OnBackClicked);

            _nameInput.onValueChanged.AddListener(OnTextInputed);
        }

        private void OnDisable()
        {
            _buttonHolder.CareTypeChosen -= SetCareType;
            _chooseCareButton.onClick.RemoveListener(OpenChoseCare);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _backButton.onClick.RemoveListener(OnBackClicked);

            _nameInput.onValueChanged.RemoveListener(OnTextInputed);

            DOTween.Kill(transform);
            DOTween.Kill(_buttonHolder.transform);
            DOTween.Kill(_backButton.transform);
            DOTween.Kill(_chooseCareButton.transform);
            DOTween.Kill(_saveButton.transform);
        }

        private void Start()
        {
            ResetScreen();
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable()
        {
            if (_closeSequence != null && _closeSequence.IsActive())
            {
                _closeSequence.Kill();
            }

            _openSequence = DOTween.Sequence();

            _canvasGroup.alpha = 0f;
            Vector3 startPosition = transform.position;
            transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);

            _openSequence.Append(_canvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeEase));
            _openSequence.Join(transform.DOMove(startPosition, _moveInDuration).SetEase(_moveEase));

            _openSequence.Play();

            _screenVisabilityHandler.EnableScreen();
            ToggleSaveButton();
        }

        public void Disable()
        {
            if (_openSequence != null && _openSequence.IsActive())
            {
                _openSequence.Kill();
            }

            _closeSequence = DOTween.Sequence();

            Vector3 endPosition = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);

            _closeSequence.Append(_canvasGroup.DOFade(0f, _fadeInDuration).SetEase(_fadeEase));
            _closeSequence.Join(transform.DOMove(endPosition, _moveInDuration).SetEase(_moveEase));

            _closeSequence.OnComplete(() => _screenVisabilityHandler.DisableScreen());

            _closeSequence.Play();
        }

        private void ToggleSaveButton()
        {
            bool canSave = !string.IsNullOrEmpty(_nameInput.text) && _careType != CareType.None;

            if (canSave != _saveButton.interactable)
            {
                if (canSave)
                {
                    _saveButton.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack)
                        .OnComplete(() => _saveButton.transform.DOScale(Vector3.one, 0.15f));
                }

                _saveButton.interactable = canSave;
            }
        }

        private void OnTextInputed(string text)
        {
            ToggleSaveButton();
        }

        private void ResetScreen()
        {
            _nameInput.text = string.Empty;
            _chosenCareType.text = "Care type";
            _careType = CareType.None;
            _buttonHolder.gameObject.SetActive(false);

            _backButton.transform.localScale = Vector3.one;
            _chooseCareButton.transform.localScale = Vector3.one;
            _saveButton.transform.localScale = Vector3.one;
        }

        private void OnSaveClicked()
        {
            PlantCareData data = new PlantCareData(_nameInput.text, _careType);

            Sequence saveSequence = DOTween.Sequence();

            saveSequence.Append(_saveButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f));

            saveSequence.AppendInterval(0.2f);

            saveSequence.OnComplete(() =>
            {
                DataCreated?.Invoke(data);
                ResetScreen();
                Disable();
            });

            saveSequence.Play();
        }

        private void OpenChoseCare()
        {
            if (!_buttonHolder.gameObject.activeSelf)
            {
                _buttonHolder.gameObject.SetActive(true);
                _buttonHolder.GetComponent<CanvasGroup>().alpha = 0f;

                Vector3 startPos = _buttonHolder.transform.position;
                _buttonHolder.transform.position = new Vector3(startPos.x, startPos.y - 50f, startPos.z);

                Sequence openButtonsSequence = DOTween.Sequence();
                openButtonsSequence.Append(_buttonHolder.GetComponent<CanvasGroup>().DOFade(1f, 0.3f));
                openButtonsSequence.Join(_buttonHolder.transform.DOMove(startPos, 0.4f).SetEase(Ease.OutBack));

                openButtonsSequence.Play();
            }
            else
            {
                Sequence closeButtonsSequence = DOTween.Sequence();

                Vector3 endPos = new Vector3(
                    _buttonHolder.transform.position.x,
                    _buttonHolder.transform.position.y - 50f,
                    _buttonHolder.transform.position.z
                );

                closeButtonsSequence.Append(_buttonHolder.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                closeButtonsSequence.Join(_buttonHolder.transform.DOMove(endPos, 0.3f).SetEase(Ease.InBack));
                closeButtonsSequence.OnComplete(() => _buttonHolder.gameObject.SetActive(false));

                closeButtonsSequence.Play();
            }
        }

        private void OnBackClicked()
        {
            Sequence backSequence = DOTween.Sequence();

            backSequence.Append(_backButton.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5, 0.5f));

            backSequence.AppendInterval(0.2f);

            backSequence.OnComplete(() =>
            {
                BackClicked?.Invoke();
                ResetScreen();
                Disable();
            });

            backSequence.Play();
        }

        private void SetCareType(CareType type)
        {
            _careType = type;

            Sequence textChangeSequence = DOTween.Sequence();

            textChangeSequence.Append(_chosenCareType.DOFade(0f, 0.2f));

            textChangeSequence.AppendCallback(() =>
            {
                _chosenCareType.text = _careTypeDataProvider.GetDataByType(type).Name;
            });

            textChangeSequence.Append(_chosenCareType.DOFade(1f, 0.2f));

            textChangeSequence.Play();

            if (_buttonHolder.gameObject.activeSelf)
            {
                Sequence closeButtonsSequence = DOTween.Sequence();

                Vector3 endPos = new Vector3(
                    _buttonHolder.transform.position.x,
                    _buttonHolder.transform.position.y - 50f,
                    _buttonHolder.transform.position.z
                );

                closeButtonsSequence.Append(_buttonHolder.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                closeButtonsSequence.Join(_buttonHolder.transform.DOMove(endPos, 0.3f).SetEase(Ease.InBack));
                closeButtonsSequence.OnComplete(() => _buttonHolder.gameObject.SetActive(false));

                closeButtonsSequence.Play();
            }

            ToggleSaveButton();
        }

        private void SetupButtonAnimations()
        {
            AddButtonAnimations(_backButton);

            AddButtonAnimations(_chooseCareButton);

            AddButtonAnimations(_saveButton);
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