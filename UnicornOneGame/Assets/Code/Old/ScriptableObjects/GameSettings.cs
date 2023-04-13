using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicornOne.ScriptableObjects.Data;
using UnicornOne.ScriptableObjects.Interfaces;
using UnityEngine;

namespace UnicornOne.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Custom/Old/GameSettings")]
    public class GameSettings : ScriptableObject, IGameSettings
    {
        public CameraSettings Camera;
        public DamageNumbersSettings DamageNumbers;

        CameraSettings IGameSettings.Camera => Camera;
        DamageNumbersSettings IGameSettings.DamageNumbers => DamageNumbers;
    }
}
