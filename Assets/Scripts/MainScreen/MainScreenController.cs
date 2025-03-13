using System;
using System.Collections.Generic;
using System.Linq;
using AddPlant;
using Articles;
using OpenPlant;
using Plant;
using SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MainScreenController : MonoBehaviour
    {
        [SerializeField] private List<PlantPlane> _plantPlanes;
        [SerializeField] private Button _articlesButton;
        [SerializeField] private Button _addNewPlantEmptyButton;
        [SerializeField] private Button _addNewPlantButton;
        [SerializeField] private AddPlantScreen _addPlantScreen;
        [SerializeField] private GameObject _emptyPlane;
        [SerializeField] private PlantTypeDataProvider _plantTypeDataProvider;
        [SerializeField] private OpenPlantScreen _openPlantScreen;
        [SerializeField] private AddPlantScreen _editScreen;
        [SerializeField] private ArticlesScreen _articlesScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private DataSaver _dataSaver;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _dataSaver = new DataSaver();
        }

        private void OnEnable()
        {
            _addNewPlantButton.onClick.AddListener(OnAddPlantClicked);
            _addNewPlantEmptyButton.onClick.AddListener(OnAddPlantClicked);

            _addPlantScreen.DataCreated += EnablePlane;
            _addPlantScreen.BackClicked += Enable;

            _openPlantScreen.BackClicked += Enable;
            _openPlantScreen.DeleteClicked += DeletePlane;

            _articlesButton.onClick.AddListener(OnArticlesClicked);
            _articlesScreen.BackClicked += Enable;

            _editScreen.PlantUpdated += Enable;

            foreach (PlantPlane plantPlane in _plantPlanes)
            {
                plantPlane.SetDataProvider(_plantTypeDataProvider);
                plantPlane.Opened += OpenPlantPlane;
            }
        }

        private void OnDisable()
        {
            _addNewPlantButton.onClick.RemoveListener(OnAddPlantClicked);
            _addNewPlantEmptyButton.onClick.RemoveListener(OnAddPlantClicked);

            _addPlantScreen.DataCreated -= EnablePlane;
            _addPlantScreen.BackClicked -= Enable;

            _openPlantScreen.BackClicked -= Enable;
            _openPlantScreen.DeleteClicked -= DeletePlane;

            _editScreen.PlantUpdated -= Enable;

            _articlesButton.onClick.RemoveListener(OnArticlesClicked);
            _articlesScreen.BackClicked -= Enable;

            foreach (PlantPlane plantPlane in _plantPlanes)
            {
                plantPlane.Opened -= OpenPlantPlane;
            }
        }

        private void Start()
        {
            DisableAllPlanes();
            LoadPlantData();
            WateringResetManager.CheckAndResetWateringProgress(_plantPlanes);
        }

        private void OnApplicationQuit()
        {
            SavePlantData();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SavePlantData();
            }
        }

        public void Enable()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void Disable()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OpenPlantPlane(PlantPlane plane)
        {
            _openPlantScreen.Enable(plane);
            Disable();
        }

        private void EnablePlane(PlantData plantData)
        {
            if (plantData == null)
                throw new ArgumentNullException(nameof(plantData));

            Enable();

            _plantPlanes.FirstOrDefault(p => !p.IsActive && p.PlantData == null)?.Enable(plantData);
            ToggleEmptyPlane();
            SavePlantData();
        }

        private void DeletePlane(PlantPlane plane)
        {
            Enable();
            plane.Disable();
            ToggleEmptyPlane();
            SavePlantData();
        }

        private void DisableAllPlanes()
        {
            foreach (PlantPlane plantPlane in _plantPlanes)
            {
                plantPlane.Disable();
            }

            ToggleEmptyPlane();
        }

        private void OnArticlesClicked()
        {
            _articlesScreen.Enable();
            Disable();
        }

        private void ToggleEmptyPlane()
        {
            _emptyPlane.SetActive(_plantPlanes.All(p => !p.IsActive));
        }

        private void OnAddPlantClicked()
        {
            _addPlantScreen.Enable();
            Disable();
        }

        private void SavePlantData()
        {
            List<PlantData> activePlantData = _plantPlanes
                .Where(p => p.IsActive && p.PlantData != null)
                .Select(p => p.PlantData)
                .ToList();

            _dataSaver.SaveData(activePlantData);
        }

        private void LoadPlantData()
        {
            List<PlantData> savedPlantData = _dataSaver.LoadData();
            
            if (savedPlantData != null && savedPlantData.Count > 0)
            {
                for (int i = 0; i < savedPlantData.Count && i < _plantPlanes.Count; i++)
                {
                    _plantPlanes[i].Enable(savedPlantData[i]);
                }
                
                ToggleEmptyPlane();
            }
        }
    }
}