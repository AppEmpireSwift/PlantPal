using System;
using System.Collections.Generic;
using System.Linq;
using AddPlant;
using OpenPlant;
using Plant;
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

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _addNewPlantButton.onClick.AddListener(OnAddPlantClicked);
            _addNewPlantEmptyButton.onClick.AddListener(OnAddPlantClicked);

            _addPlantScreen.DataCreated += EnablePlane;
            _addPlantScreen.BackClicked += Enable;

            _openPlantScreen.BackClicked += Enable;
            _openPlantScreen.DeleteClicked += DeletePlane;

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

            foreach (PlantPlane plantPlane in _plantPlanes)
            {
                plantPlane.Opened -= OpenPlantPlane;
            }
        }

        private void Start()
        {
            DisableAllPlanes();
            WateringResetManager.CheckAndResetWateringProgress(_plantPlanes);
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
        }

        private void DeletePlane(PlantPlane plane)
        {
            Enable();
            plane.Disable();
            ToggleEmptyPlane();
        }

        private void DisableAllPlanes()
        {
            foreach (PlantPlane plantPlane in _plantPlanes)
            {
                plantPlane.Disable();
            }

            ToggleEmptyPlane();
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
    }
}