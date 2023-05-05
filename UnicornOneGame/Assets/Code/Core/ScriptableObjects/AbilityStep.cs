using System;

namespace UnicornOne.ScriptableObjects
{
    [Serializable]
    public struct AbilityStep
    {
        public float Time;
        public BaseAbilityAction[] Actions;
    }
}
