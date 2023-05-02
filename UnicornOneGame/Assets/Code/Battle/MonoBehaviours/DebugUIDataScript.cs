using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class DebugUIDataScript : MonoBehaviour
    {
        private const int VerticalDistanceBetweenWindows = 20;

        private static DebugUIDataScript sInstance;
        public static DebugUIDataScript Instance => sInstance;

        private int _nextAvailableWindowId = 0;
        public int NextAvailableWindowId => _nextAvailableWindowId++;
        private int _nextVerticalOffset = VerticalDistanceBetweenWindows;
        
        private void Awake()
        {
            if (!sInstance) sInstance = this;
        }

        public int GetVerticalOffset(int height)
        {
            int offset = _nextVerticalOffset;

            _nextVerticalOffset += height + VerticalDistanceBetweenWindows;

            return offset;
        }
    }
}
