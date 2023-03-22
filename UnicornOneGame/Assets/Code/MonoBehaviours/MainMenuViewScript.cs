using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using UnicornOne.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;

namespace UnicornOne.MonoBehaviours
{
	public class MainMenuViewScript : MonoBehaviour
	{
        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Dropdown _levelDropdown;

        public TMP_Dropdown.OptionData SelectedLevelData => _levelDropdown.options[_levelDropdown.value];

        public event Action PlayButtonClicked;

        public void OnPlayButtonClick()
		{
            PlayButtonClicked?.Invoke();
        }

		public void SetAvailableLevels(IEnumerable<Level> levels)
		{
            List<TMP_Dropdown.OptionData> levelOptions = levels
                .Select(l => new TMP_Dropdown.OptionData(l.name))
                .ToList();
            _levelDropdown.AddOptions(levelOptions);
        }
	}
}
