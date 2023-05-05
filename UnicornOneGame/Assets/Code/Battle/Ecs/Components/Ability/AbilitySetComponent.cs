namespace UnicornOne.Battle.Ecs.Components
{
    internal struct AbilitySetComponent
    {
        public struct AbilityState
        {
            public int AbilityIndex;
            public float TimeLastUsed;
        }

        public AbilityState[] AbilitySet;
    }
}
