namespace UnicornOne.Battle.Ecs.Services
{
    internal interface ITimeService
    {
        public float Delta { get; }
        public float TimeSinceStart { get; }
    }
}
