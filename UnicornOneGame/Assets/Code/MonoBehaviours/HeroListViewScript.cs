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

        [SerializeField] private Hero[] _heroes;

        private SelectHeroAction _selectHeroCallback;

        public void Init(SelectHeroAction selectHeroCallback)
		{
            _selectHeroCallback = selectHeroCallback;

            foreach (var hero in _heroes)
			{
				GameObject heroItemView = Instantiate(_heroListItemViewPrefab);
				heroItemView.transform.SetParent(_heroGrid.transform, false);

				var itemScript = heroItemView.GetComponent<HeroListItemViewScript>();
                itemScript.SetHero(hero);
                itemScript.SelectHeroButtonClicked += _selectHeroCallback;
            }

            _scroll.verticalNormalizedPosition = 1.0f;
        }

        public void OnDestroy()
        {
            foreach (var itemScript in _heroGrid.GetComponentsInChildren<HeroListItemViewScript>())
			{
				itemScript.SelectHeroButtonClicked -= _selectHeroCallback;
            }
        }
    }
}
