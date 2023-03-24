using UnityEngine;
using System.Collections;
using UnicornOne.ScriptableObjects;
using System.Collections.Generic;
using static UnicornOne.MonoBehaviours.HeroListItemViewScript;

namespace UnicornOne.MonoBehaviours
{
	public class HeroSelectViewScript : MonoBehaviour
	{
		[SerializeField] private HeroListViewScript _heroListViewScript;

        public void Init(SelectHeroAction selectHeroCallback)
		{
			_heroListViewScript.Init(selectHeroCallback);
        }
	}
}
