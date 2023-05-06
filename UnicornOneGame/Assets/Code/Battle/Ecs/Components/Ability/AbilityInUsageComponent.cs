namespace UnicornOne.Battle.Ecs.Components
{
    internal struct AbilityInUsageComponent
    {
        public int AbilityIndex;
        public float StartTime;
        public int NextStepIndex;

        public override string ToString()
        {
            return AbilityIndex.ToString();
        }
    }
}
