using UnityEngine;
using System.Collections;
using TMPro;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.MonoBehaviours
{
	public class HeroListItemViewScript : MonoBehaviour
	{
		[SerializeField] private TMP_Text _title;

		public Hero Hero { get; private set; }

		public void SetHero(Hero hero)
		{
			Hero = hero;

            _title.text = hero.name;
        }
	}
}
