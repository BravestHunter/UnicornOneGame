using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Custom/Ability")]
    public class Ability : ScriptableObject
    {
        public float Cooldown;
        public float Duration;
        public AbilityStep[] Steps;
    }
}
