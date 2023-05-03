namespace UnicornOne.Battle.Ecs.Services
{
    internal interface ITimeService : IService
    {
        public float Delta { get; }
        public float TimeSinceStart { get; }
    }
}
