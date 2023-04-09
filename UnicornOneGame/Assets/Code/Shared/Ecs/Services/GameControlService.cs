using System;

namespace UnicornOne.Ecs.Services
{
    public class GameControlService
    {
        public bool GameFinished { get; private set; } = false;

        public void ReportGameFinish()
        {
            GameFinished = true;
        }
    }
}
