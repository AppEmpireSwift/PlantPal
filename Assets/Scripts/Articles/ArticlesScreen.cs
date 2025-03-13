using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Articles
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class ArticlesScreen : MonoBehaviour
    {
        [SerializeField] private Color _activeButtonColor;
        [SerializeField] private Color _inactiveButtonColor;

        [SerializeField] private Button _backButton;
        [SerializeField] private Button _allButton;
        [SerializeField] private Button _favouriteButton;
        [SerializeField] private List<ArticlePlane> _articlePlanes;
        [SerializeField] private OpenArticleScreen _openArticleScreen;

        [Header("Animation Settings")] [SerializeField]
        private float _fadeInDuration = 0.3f;

        [SerializeField] private float _fadeOutDuration = 0.2f;
        [SerializeField] private float _buttonScaleDuration = 0.15f;
        [SerializeField] private float _buttonHoverScale = 1.1f;
        [SerializeField] private float _colorChangeDuration = 0.2f;
        [SerializeField] private float _staggerDelay = 0.05f;

        private bool _isFavourite;
        private ScreenVisabilityHandler _screenVisabilityHandler;
        private CanvasGroup _canvasGroup;
        private Sequence _showSequence;

        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

            SetupButtonAnimations();
        }

        private void OnEnable()
        {
            foreach (var articlePlane in _articlePlanes)
            {
                articlePlane.FavouriteStatusChanged += UpdateFavouritePlanes;
                articlePlane.DataOpened += OnArticleOpened;
            }

            _backButton.onClick.AddListener(OnBackClicked);
            _allButton.onClick.AddListener(ShowAllPlanes);
            _favouriteButton.onClick.AddListener(ShowFavouritePlanes);

            if (_openArticleScreen != null)
            {
                _openArticleScreen.BackClicked += OnArticleScreenClosed;
                _openArticleScreen.FavouriteStatusChanged += OnArticleFavouriteChanged;
            }
        }

        private void OnDisable()
        {
            foreach (var articlePlane in _articlePlanes)
            {
                articlePlane.FavouriteStatusChanged -= UpdateFavouritePlanes;
                articlePlane.DataOpened -= OnArticleOpened;
            }

            _backButton.onClick.RemoveListener(OnBackClicked);
            _allButton.onClick.RemoveListener(ShowAllPlanes);
            _favouriteButton.onClick.RemoveListener(ShowFavouritePlanes);

            if (_openArticleScreen != null)
            {
                _openArticleScreen.BackClicked -= OnArticleScreenClosed;
                _openArticleScreen.FavouriteStatusChanged -= OnArticleFavouriteChanged;
            }

            KillAllTweens();
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void SetupButtonAnimations()
        {
            SetupButtonHoverAnimation(_backButton);
            SetupButtonHoverAnimation(_allButton);
            SetupButtonHoverAnimation(_favouriteButton);
        }

        private void SetupButtonHoverAnimation(Button button)
        {
            EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>() ??
                                        button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
            entryPointerEnter.eventID = EventTriggerType.PointerEnter;
            entryPointerEnter.callback.AddListener((data) =>
            {
                button.transform.DOScale(_buttonHoverScale, _buttonScaleDuration).SetEase(Ease.OutBack);
            });

            EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
            entryPointerExit.eventID = EventTriggerType.PointerExit;
            entryPointerExit.callback.AddListener((data) =>
            {
                button.transform.DOScale(1f, _buttonScaleDuration).SetEase(Ease.OutQuad);
            });

            eventTrigger.triggers.Add(entryPointerEnter);
            eventTrigger.triggers.Add(entryPointerExit);

            // Add click animation
            button.onClick.AddListener(() =>
            {
                button.transform.DOScale(0.95f, _buttonScaleDuration / 2)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        button.transform.DOScale(1f, _buttonScaleDuration / 2).SetEase(Ease.OutQuad);
                    });
            });
        }

        public void Enable()
        {
            KillAllTweens();

            _canvasGroup.alpha = 0;

            _showSequence = DOTween.Sequence();

            _showSequence.Append(_canvasGroup.DOFade(1, _fadeInDuration).SetEase(Ease.OutQuad));

            ShowAllPlanes();

            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            KillAllTweens();

            Sequence hideSequence = DOTween.Sequence();

            hideSequence.Append(_canvasGroup.DOFade(0, _fadeOutDuration).SetEase(Ease.InQuad));

            hideSequence.OnComplete(() => { _screenVisabilityHandler.DisableScreen(); });
        }

        private void ShowAllPlanes()
        {
            _isFavourite = false;

            _favouriteButton.image.DOColor(_inactiveButtonColor, _colorChangeDuration);
            _allButton.image.DOColor(_activeButtonColor, _colorChangeDuration);

            Sequence showPlanesSequence = DOTween.Sequence();

            List<ArticlePlane> visiblePlanes = new List<ArticlePlane>();

            foreach (var articlePlane in _articlePlanes)
            {
                if (!articlePlane.gameObject.activeSelf)
                {
                    articlePlane.gameObject.SetActive(true);

                    CanvasGroup planeCanvasGroup = articlePlane.GetComponent<CanvasGroup>() ??
                                                   articlePlane.gameObject.AddComponent<CanvasGroup>();
                    planeCanvasGroup.alpha = 0;

                    showPlanesSequence.Insert((visiblePlanes.Count * _staggerDelay),
                        planeCanvasGroup.DOFade(1, _fadeInDuration).SetEase(Ease.OutQuad));
                }

                visiblePlanes.Add(articlePlane);
            }

            if (_showSequence != null && _showSequence.IsActive())
            {
                _showSequence.Append(showPlanesSequence);
            }
        }

        private void ShowFavouritePlanes()
        {
            _isFavourite = true;

            _favouriteButton.image.DOColor(_activeButtonColor, _colorChangeDuration);
            _allButton.image.DOColor(_inactiveButtonColor, _colorChangeDuration);

            Sequence showFavoritesSequence = DOTween.Sequence();

            List<ArticlePlane> planesToHide = new List<ArticlePlane>();
            List<ArticlePlane> visiblePlanes = new List<ArticlePlane>();

            foreach (var articlePlane in _articlePlanes)
            {
                if (articlePlane.Data.IsFavourite)
                {
                    if (!articlePlane.gameObject.activeSelf)
                    {
                        articlePlane.gameObject.SetActive(true);

                        CanvasGroup planeCanvasGroup = articlePlane.GetComponent<CanvasGroup>() ??
                                                       articlePlane.gameObject.AddComponent<CanvasGroup>();
                        planeCanvasGroup.alpha = 0;

                        showFavoritesSequence.Insert((visiblePlanes.Count * _staggerDelay),
                            planeCanvasGroup.DOFade(1, _fadeInDuration).SetEase(Ease.OutQuad));
                    }

                    visiblePlanes.Add(articlePlane);
                }
                else
                {
                    if (articlePlane.gameObject.activeSelf)
                    {
                        planesToHide.Add(articlePlane);

                        CanvasGroup planeCanvasGroup = articlePlane.GetComponent<CanvasGroup>() ??
                                                       articlePlane.gameObject.AddComponent<CanvasGroup>();

                        showFavoritesSequence.Insert(0, planeCanvasGroup.DOFade(0, _fadeOutDuration)
                            .SetEase(Ease.InQuad)
                            .OnComplete(() => articlePlane.gameObject.SetActive(false)));
                    }
                }
            }
        }

        private void UpdateFavouritePlanes()
        {
            if (_isFavourite)
                ShowFavouritePlanes();
        }

        private void OnBackClicked()
        {
            _backButton.transform.DOScale(0.9f, _buttonScaleDuration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _backButton.transform.DOScale(1f, _buttonScaleDuration / 2).SetEase(Ease.OutQuad);

                    Disable();

                    BackClicked?.Invoke();
                });
        }

        private void OnArticleOpened(ArticleData data)
        {
            if (_openArticleScreen != null)
            {
                Disable();

                DOVirtual.DelayedCall(_fadeOutDuration, () => { _openArticleScreen.Enable(data); });
            }
        }

        private void OnArticleScreenClosed()
        {
            Enable();
        }

        private void OnArticleFavouriteChanged(ArticleData data)
        {
            foreach (var articlePlane in _articlePlanes)
            {
                if (articlePlane.Data == data)
                {
                    articlePlane.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f, 5, 0.5f)
                        .OnComplete(() => { articlePlane.UpdateFavouriteState(); });
                    break;
                }
            }

            UpdateFavouritePlanes();
        }

        private void KillAllTweens()
        {
            if (_showSequence != null && _showSequence.IsActive())
            {
                _showSequence.Kill();
            }

            DOTween.Kill(_canvasGroup);
            DOTween.Kill(_backButton.transform);
            DOTween.Kill(_allButton.transform);
            DOTween.Kill(_favouriteButton.transform);
            DOTween.Kill(_favouriteButton.image);
            DOTween.Kill(_allButton.image);

            foreach (var articlePlane in _articlePlanes)
            {
                DOTween.Kill(articlePlane.transform);

                CanvasGroup planeCanvasGroup = articlePlane.GetComponent<CanvasGroup>();
                if (planeCanvasGroup != null)
                {
                    DOTween.Kill(planeCanvasGroup);
                }
            }
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
    }
}