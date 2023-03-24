using UnityEngine;
using System.Collections;
using UnicornOne.ScriptableObjects;
using TMPro;
using System;

namespace UnicornOne.MonoBehaviours
{
	public class SquadHeroButtonScript : MonoBehaviour
	{
		[SerializeField] private TMP_Text _text;

		public Hero Hero { get; private set; } = null;

		public delegate void SquadHeroSelect(SquadHeroButtonScript squadHeroScript);
		public event SquadHeroSelect SelectButtonClicked;

		public void SetHero(Hero hero)
		{
			Hero = hero;

			_text.text = hero.name;
        }

		public void OnSelectButtonClick()
		{
			SelectButtonClicked?.Invoke(this);
        }
	}
}
