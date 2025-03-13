using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Articles
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenArticleScreen : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _favouriteButton;
        [SerializeField] private Image _articleImage;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _favouriteButtonText;

        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.3f;

        [SerializeField] private float _punchScale = 1.2f;
        [SerializeField] private int _vibrato = 5;
        [SerializeField] private float _elasticity = 0.5f;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private ArticleData _currentArticle;
        private ScrollRect _parentScrollRect;
        private Sequence _textAnimationSequence;

        public event Action BackClicked;
        public event Action<ArticleData> FavouriteStatusChanged;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _parentScrollRect = _favouriteButtonText.GetComponentInParent<ScrollRect>();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _favouriteButton.onClick.AddListener(OnFavouriteClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
            _favouriteButton.onClick.RemoveListener(OnFavouriteClicked);

            if (_textAnimationSequence != null)
            {
                _textAnimationSequence.Kill();
                _textAnimationSequence = null;
            }
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void Enable(ArticleData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _currentArticle = data;
            _screenVisabilityHandler.EnableScreen();
            _articleImage.sprite = data.Sprite;
            _titleText.text = data.Title;
            _descriptionText.text = data.Description;

            UpdateFavouriteButtonState();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            Disable();
        }

        private void OnFavouriteClicked()
        {
            if (_currentArticle != null)
            {
                _currentArticle.IsFavourite = !_currentArticle.IsFavourite;
                UpdateFavouriteButtonState();
                FavouriteStatusChanged?.Invoke(_currentArticle);

                if (_parentScrollRect != null)
                {
                    _parentScrollRect.enabled = false;
                }

                AnimateFavouriteButtonText();
            }
        }

        private void UpdateFavouriteButtonState()
        {
            if (_currentArticle == null)
                return;

            _favouriteButtonText.text = _currentArticle.IsFavourite ? "Remove" : "Add to favorite";
        }

        private void AnimateFavouriteButtonText()
        {
            if (_textAnimationSequence != null)
            {
                _textAnimationSequence.Kill();
                _textAnimationSequence = null;
            }

            Vector3 originalScale = _favouriteButtonText.transform.localScale;

            _textAnimationSequence = DOTween.Sequence();

            _textAnimationSequence.Append(
                _favouriteButtonText.transform.DOPunchScale(
                    new Vector3(_punchScale, _punchScale, _punchScale),
                    _animationDuration,
                    _vibrato,
                    _elasticity
                )
            );


            _textAnimationSequence.OnComplete(() =>
            {
                _favouriteButtonText.transform.localScale = originalScale;

                if (_parentScrollRect != null)
                {
                    _parentScrollRect.enabled = true;
                }
            });

            _textAnimationSequence.Play();
        }
    }
}