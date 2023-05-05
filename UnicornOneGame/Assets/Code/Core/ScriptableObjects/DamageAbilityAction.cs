using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "DamageAbilityAction", menuName = "Custom/DamageAbilityAction")]
    public class DamageAbilityAction : BaseAbilityAction
    {
        public int Amount;
    }
}
