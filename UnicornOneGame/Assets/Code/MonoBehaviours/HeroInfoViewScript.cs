using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnicornOne.ScriptableObjects;
using TMPro;

namespace UnicornOne.MonoBehaviours
{
	public class HeroInfoViewScript : MonoBehaviour
	{
        [SerializeField] private TMP_Text _title;
        [SerializeField] private HeroPreviewScript _previewScript;

        public event Action BackButtonClicked;

        public void SetHero(Hero hero)
        {
            _title.text = hero.name;

            _previewScript.SetHero(hero);
        }

        public void OnBackButtonClick()
        {
            BackButtonClicked?.Invoke();
        }
    }
}
