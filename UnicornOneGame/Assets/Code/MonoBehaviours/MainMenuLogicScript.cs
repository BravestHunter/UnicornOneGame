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
    public class MainMenuLogicScript : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Dropdown _levelDropdown;

        [SerializeField] private Level[] _availableLevels;

        private AsyncOperation _gameSceneLoadingOperation = null;

        private void Start()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);

            List<TMP_Dropdown.OptionData> levelOptions = _availableLevels
                .Select(l => new TMP_Dropdown.OptionData(l.name))
                .ToList();
            _levelDropdown.AddOptions(levelOptions);
        }

        private void Update()
        {

        }

        private void OnPlayButtonClicked()
        {
            if (_gameSceneLoadingOperation != null)
            {
                return;
            }

            // Clean previous value
            SharedData.SelectedLevel = null;

            var selectedOption = _levelDropdown.options[_levelDropdown.value];
            SharedData.SelectedLevel = _availableLevels.FirstOrDefault(l => l.name == selectedOption.text);
            if (SharedData.SelectedLevel == null)
            {
                SharedData.SelectedLevel = _availableLevels.First();
            }

            _gameSceneLoadingOperation = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        }
    }
}
