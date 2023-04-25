namespace UnicornOne.Battle
{
    internal static class GlobalDebug
    {
        private static int _nextFreeDebugWindowId = 0;
        public static int NextDebugWindowId => _nextFreeDebugWindowId++;
    }
}
