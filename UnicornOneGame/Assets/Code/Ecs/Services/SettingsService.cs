using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;

namespace UnicornOne.Ecs.Services
{
    internal class SettingsService
    {
        private readonly IGameSettings _gameSettings;

        public CameraSettings Camera => _gameSettings.Camera;

        public SettingsService(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }
    }
}
