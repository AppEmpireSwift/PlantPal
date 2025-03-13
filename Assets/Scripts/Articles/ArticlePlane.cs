using System;
using UnityEngine;
using UnityEngine.UI;

namespace Articles
{
    public class ArticlePlane : MonoBehaviour
    {
        [SerializeField] private Sprite _isFavouriteSprite;
        [SerializeField] private Sprite _notFavouriteSprite;

        [SerializeField] private Image _articleImage;
        [SerializeField] private ArticleData _articleData;
        [SerializeField] private Button _favouriteButton;
        [SerializeField] private Button _openButton;

        public event Action<ArticleData> DataOpened;
        public event Action FavouriteStatusChanged;

        public ArticleData Data => _articleData;

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnOpenClicked);
            _favouriteButton.onClick.AddListener(OnFavouriteClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnOpenClicked);
            _favouriteButton.onClick.RemoveListener(OnFavouriteClicked);
        }

        private void Start()
        {
            if (_articleData != null)
            {
                _articleImage.sprite = _articleData.Sprite;
                UpdateFavouriteSprite();
            }
        }

        private void OnFavouriteClicked()
        {
            if (_articleData != null)
            {
                _articleData.IsFavourite = !_articleData.IsFavourite;
                UpdateFavouriteSprite();
                FavouriteStatusChanged?.Invoke();
            }
        }

        private void OnOpenClicked()
        {
            DataOpened?.Invoke(_articleData);
        }

        public void UpdateFavouriteState()
        {
            UpdateFavouriteSprite();
        }

        private void UpdateFavouriteSprite()
        {
            if (_articleData == null || _favouriteButton == null)
                return;

            _favouriteButton.image.sprite = _articleData.IsFavourite ? _isFavouriteSprite : _notFavouriteSprite;
        }
    }
}