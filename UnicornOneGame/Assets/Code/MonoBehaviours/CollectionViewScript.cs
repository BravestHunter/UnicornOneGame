using UnityEngine;
using System.Collections;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.MonoBehaviours
{
	public class CollectionViewScript : MonoBehaviour
	{
		[SerializeField] private HeroListViewScript _heroListViewScript;

		[SerializeField] private Hero[] _heroes;

		void Start()
		{
			_heroListViewScript.SetHeros(_heroes);
        }
	}
}
