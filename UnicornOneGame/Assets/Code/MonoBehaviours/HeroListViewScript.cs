using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.MonoBehaviours
{
	public class HeroListViewScript : MonoBehaviour
	{
		[SerializeField] private GameObject _heroGrid;

		[SerializeField] private GameObject _heroListItemViewPrefab;

		public void SetHeros(IEnumerable<Hero> heroes)
		{
			foreach (var hero in heroes)
			{
				GameObject heroItemView = Instantiate(_heroListItemViewPrefab);
				heroItemView.transform.SetParent(_heroGrid.transform, false);

				var script = heroItemView.GetComponent<HeroListItemViewScript>();
				script.SetHero(hero);
            }
        }
	}
}
