using UnityEngine;
using System.Collections;
using TMPro;
using UnicornOne.ScriptableObjects;
using System;

namespace UnicornOne.MonoBehaviours
{
	public class HeroListItemViewScript : MonoBehaviour
	{
		[SerializeField] private TMP_Text _title;

		public Hero Hero { get; private set; }

		public delegate void SelectHeroAction(Hero hero);
		public event SelectHeroAction SelectHeroButtonClicked;

		public void SetHero(Hero hero)
		{
			Hero = hero;

            _title.text = hero.name;
        }

		public void OnShowInfoButtonClick()
		{
            SelectHeroButtonClicked?.Invoke(Hero);
        }
    }
}
