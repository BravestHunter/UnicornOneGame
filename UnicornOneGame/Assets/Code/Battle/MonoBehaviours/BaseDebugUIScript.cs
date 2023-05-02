using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    abstract internal class BaseDebugUIScript : MonoBehaviour
    {
        protected abstract Vector2Int Size { get; }
        protected abstract string Title { get; }
        protected abstract void BuildWindowLayout();

        private int _windowId;
        private Rect _windowRect;

        private void Awake()
        {
            _windowId = DebugUIDataScript.Instance.NextAvailableWindowId;

            int verticalOffset = DebugUIDataScript.Instance.GetVerticalOffset(Size.y);
            _windowRect = new Rect(20, verticalOffset, Size.x, Size.y);
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(_windowId, _windowRect, BuildWindow, Title);
        }

        private void BuildWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            BuildWindowLayout();
        }
    }
}
