namespace UnicornOne.Battle.Ecs.Services
{
    internal class TimeService : ITimeService
    {
        public float Delta { get; set; }

        private readonly float _startTime;
        public float CurrentTime { get; set; }
        public float TimeSinceStart => CurrentTime - _startTime;

        public TimeService(float startTime)
        {
            _startTime = startTime;
            CurrentTime = startTime;
        }
    }
}
