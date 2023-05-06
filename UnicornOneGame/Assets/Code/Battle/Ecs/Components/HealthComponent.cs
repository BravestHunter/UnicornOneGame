namespace UnicornOne.Battle.Ecs.Components
{
    internal struct HealthComponent
    {
        public int Max;
        public int Current;

        public override string ToString()
        {
            return $"{Current}/{Max}";
        }
    }
}
