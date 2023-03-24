using UnityEngine;
using System.Collections;
using TMPro;

namespace UnicornOne.MonoBehaviours
{
	public class HeroListItemViewScript : MonoBehaviour
	{
		[SerializeField] private TMP_Text _title;

		public string Title
		{
			set { _title.text = value; }
		}
	}
}
