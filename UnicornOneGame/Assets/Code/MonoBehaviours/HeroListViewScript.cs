using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.ScriptableObjects;
using static UnicornOne.MonoBehaviours.HeroListItemViewScript;
using UnityEngine.UI;

namespace UnicornOne.MonoBehaviours
{
	public class HeroListViewScript : MonoBehaviour
	{
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private GameObject _heroGrid;

		[SerializeField] private GameObject _heroListItemViewPrefab;

		private ShowHeroInfoAction _showHeroInfoCallback;

        public void Init(IEnumerable<Hero> heroes, ShowHeroInfoAction showHeroInfoCallback)
		{
			_showHeroInfoCallback = showHeroInfoCallback;

            foreach (var hero in heroes)
			{
				GameObject heroItemView = Instantiate(_heroListItemViewPrefab);
				heroItemView.transform.SetParent(_heroGrid.transform, false);

				var itemScript = heroItemView.GetComponent<HeroListItemViewScript>();
                itemScript.SetHero(hero);
                itemScript.ShowInfoButtonClicked += _showHeroInfoCallback;
            }

            _scroll.verticalNormalizedPosition = 1.0f;
        }

        public void OnDestroy()
        {
            foreach (var itemScript in _heroGrid.GetComponentsInChildren<HeroListItemViewScript>())
			{
				itemScript.ShowInfoButtonClicked -= _showHeroInfoCallback;
            }
        }
    }
}
