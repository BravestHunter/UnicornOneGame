using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnicornOne.ScriptableObjects;
using UnicornOne.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnicornOne.MonoBehaviours
{
    public class MainMenuSceneLogic : MonoBehaviour
    {
        [SerializeField] private Canvas _backgroundCanvas;
        [SerializeField] private Canvas _topCanvas;
        [SerializeField] private Canvas _navbarCanvas;
        [SerializeField] private Canvas _contentCanvas;
        [SerializeField] private Canvas _pageCanvas;

        [SerializeField] private ShopViewScript _shopViewScript;
        [SerializeField] private PlayViewScript _playViewScript;
        [SerializeField] private CollectionViewScript _collectionViewScript;
        [SerializeField] private SettingsViewScript _settingsViewScript;

        [SerializeField] private Level[] _availableLevels;

        private AsyncOperation _gameSceneLoadingOperation = null;

        private void Start()
        {
            _playViewScript.SetAvailableLevels(_availableLevels);
            _playViewScript.GameRequested += OnGameRequested;

            _settingsViewScript.BackButtonClicked += SettingsViewBackButtonClick;
        }

        private void OnDestroy()
        {
            _settingsViewScript.BackButtonClicked -= SettingsViewBackButtonClick;

            _playViewScript.GameRequested -= OnGameRequested;
        }

        private void Update()
        {
        }

        public void OnSettingButtonClick()
        {
            _topCanvas.gameObject.SetActive(false);
            _navbarCanvas.gameObject.SetActive(false);
            _contentCanvas.gameObject.SetActive(false);
            _pageCanvas.gameObject.SetActive(true);

            _settingsViewScript.gameObject.SetActive(true);
        }

        public void OnShopButtonClick()
        {
            _shopViewScript.gameObject.SetActive(true);
            _playViewScript.gameObject.SetActive(false);
            _collectionViewScript.gameObject.SetActive(false);
        }

        public void OnPlayButtonClick()
        {
            _shopViewScript.gameObject.SetActive(false);
            _playViewScript.gameObject.SetActive(true);
            _collectionViewScript.gameObject.SetActive(false);
        }
        public void OnCollectionButtonClick()
        {
            _shopViewScript.gameObject.SetActive(false);
            _playViewScript.gameObject.SetActive(false);
            _collectionViewScript.gameObject.SetActive(true);
        }

        private void OnGameRequested(IEnumerable<Hero> heroes)
        {
            if (_gameSceneLoadingOperation != null)
            {
                return;
            }

            if (heroes.Count() < 1)
            {
                return;
            }

            // Clean previous value
            SharedData.SelectedLevel = null;
            SharedData.SelectedHeroes = null;

            var selectedOption = _playViewScript.SelectedLevelData;
            SharedData.SelectedLevel = _availableLevels.FirstOrDefault(l => l.name == selectedOption.text);
            if (SharedData.SelectedLevel == null)
            {
                SharedData.SelectedLevel = _availableLevels.First();
            }

            if (SharedData.SelectedHeroes == null)
            {
                SharedData.SelectedHeroes = heroes.ToArray();
            }

            _gameSceneLoadingOperation = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        }

        private void SettingsViewBackButtonClick()
        {
            _topCanvas.gameObject.SetActive(true);
            _navbarCanvas.gameObject.SetActive(true);
            _contentCanvas.gameObject.SetActive(true);
            _pageCanvas.gameObject.SetActive(false);

            _settingsViewScript.gameObject.SetActive(false);
        }
    }
}
