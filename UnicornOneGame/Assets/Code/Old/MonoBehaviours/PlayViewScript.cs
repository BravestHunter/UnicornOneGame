using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using UnicornOne.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using static UnicornOne.MonoBehaviours.HeroListItemViewScript;

namespace UnicornOne.MonoBehaviours
{
	public class PlayViewScript : MonoBehaviour
	{
        [SerializeField] private GameObject _mainContent;
        [SerializeField] private HeroSelectViewScript _heroSelectViewScript;
        [SerializeField] private SquadHeroButtonScript[] _squadHeroButtonScripts;
        [SerializeField] private TMP_Dropdown _levelDropdown;

        public TMP_Dropdown.OptionData SelectedLevelData => _levelDropdown.options[_levelDropdown.value];

        public delegate void GameStartRequest(IEnumerable<Hero> heroes);
        public event GameStartRequest GameRequested;

        private SquadHeroButtonScript _squadHeroButtonInSelect = null;

        private void Start()
        {
            foreach (var script in _squadHeroButtonScripts)
            {
                script.SelectButtonClicked += OnSquadHeroButtonClick;
            }

            _heroSelectViewScript.Init(OnHeroListItemClick);
        }

        private void OnDestroy()
        {
            foreach (var script in _squadHeroButtonScripts)
            {
                script.SelectButtonClicked -= OnSquadHeroButtonClick;
            }
        }

        public void OnPlayButtonClick()
		{
            GameRequested?.Invoke(_squadHeroButtonScripts.Select(s => s.Hero).Where(h => h != null));
        }

        private void OnSquadHeroButtonClick(SquadHeroButtonScript squadHeroScript)
        {
            _squadHeroButtonInSelect = squadHeroScript;

            _mainContent.SetActive(false);
            _heroSelectViewScript.gameObject.SetActive(true);
        }

        private void OnHeroListItemClick(Hero hero)
        {
            _squadHeroButtonInSelect.SetHero(hero);
            _squadHeroButtonInSelect = null;

            _mainContent.SetActive(true);
            _heroSelectViewScript.gameObject.SetActive(false);
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
