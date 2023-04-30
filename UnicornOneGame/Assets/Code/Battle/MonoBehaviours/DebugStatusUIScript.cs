using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnicornOne.Battle.ScriptableObjects;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class DebugStatusUIScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private bool _isDirty = true;
         
        private string _hpInfo = string.Empty;
        public string HpInfo
        {
            get { return _hpInfo; }
            set
            {
                if (_hpInfo == value)
                {
                    return;
                }

                _hpInfo = value;
                _isDirty = true;
            }
        }

        private bool _showHpInfo = false;
        public bool ShowHpInfo
        {
            get { return _showHpInfo; }
            set
            {
                if (_showHpInfo == value)
                {
                    return;
                }

                _showHpInfo = value;
                _isDirty = true;
            }
        }

        private string _aiInfo = string.Empty;
        public string AiInfo
        {
            get { return _aiInfo; }
            set
            {
                if (_aiInfo == value)
                {
                    return;
                }

                _aiInfo = value;
                _isDirty = true;
            }
        }

        private bool _showAiInfo = false;
        public bool ShowAiInfo
        {
            get { return _showAiInfo; }
            set
            {
                if (_showAiInfo == value)
                {
                    return;
                }

                _showAiInfo = value;
                _isDirty = true;
            }
        }

        private void Update()
        {
            if (_isDirty)
            {
                _text.text = AssembleInfoString();

                _isDirty = false;
            }
        }

        public void UpdateSettings(DebugStatusUISettings settings, bool isAlly)
        {
            _text.fontSize = settings.FontSize;
            _text.color = isAlly ? settings.AllyColor : settings.EnemyColor;
            ShowHpInfo = settings.ShowHpInfo;
            ShowAiInfo = settings.ShowAiInfo;
        }

        private string AssembleInfoString()
        {
            List<string> infos = new();

            if (ShowAiInfo)
            {
                infos.Add(AiInfo);
            }
            if (ShowHpInfo)
            {
                infos.Add(HpInfo);
            }

            return string.Join('\n', infos.Where(str => !string.IsNullOrEmpty(str)));
        }
    }
}
