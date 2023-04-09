using UnityEngine;
using System.Collections;
using UnicornOne.ScriptableObjects;

namespace UnicornOne.MonoBehaviours
{
	public class HeroPreviewScript : MonoBehaviour
	{
		[SerializeField] private GameObject _container;

		public void SetHero(Hero hero)
		{
			foreach (Transform child in _container.transform)
			{
				Destroy(child.gameObject);
			}

			var heroGameObject = Instantiate(hero.PrefabInfo.Prefab);
			heroGameObject.transform.SetParent(_container.transform, false);
        }
	}
}
