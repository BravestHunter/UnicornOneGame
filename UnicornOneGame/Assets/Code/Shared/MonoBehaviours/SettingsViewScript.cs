using UnityEngine;
using System.Collections;
using System;

namespace UnicornOne.MonoBehaviours
{
	public class SettingsViewScript : MonoBehaviour
	{
		public event Action BackButtonClicked; 

		public void OnBackButtonClick()
		{
			BackButtonClicked?.Invoke();
        }
	}
}
