using UnityEngine;
using System.Collections;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.MonoBehaviours
{
	public class CollectionViewScript : MonoBehaviour
	{
		[SerializeField] private HeroListViewScript _heroListViewScript;
        [SerializeField] private HeroInfoViewScript _heroInfoViewScript;

		void Start()
		{
			_heroListViewScript.Init(OnHeroListItemClick);

			_heroInfoViewScript.BackButtonClicked += OnHeroInfoViewBackButtonClick;
        }

        private void OnDestroy()
        {
            _heroInfoViewScript.BackButtonClicked -= OnHeroInfoViewBackButtonClick;
        }

        private void OnHeroListItemClick(Hero hero)
		{
            _heroInfoViewScript.SetHero(hero);

            _heroListViewScript.gameObject.SetActive(false);
            _heroInfoViewScript.gameObject.SetActive(true);
        }

		private void OnHeroInfoViewBackButtonClick()
		{
            _heroListViewScript.gameObject.SetActive(true);
            _heroInfoViewScript.gameObject.SetActive(false);
        }
	}
}
