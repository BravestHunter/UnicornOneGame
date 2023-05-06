namespace UnicornOne.Battle.Ecs.Components
{
    internal struct AbilitySetComponent
    {
        public struct AbilityState
        {
            public int AbilityId;
            public float TimeLastUsed;
        }

        public AbilityState[] AbilitySet;
    }
}
