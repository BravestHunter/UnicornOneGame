using System.Collections.Generic;
using TMPro;
using UnicornOne.Battle.Utils;
using UnityEngine;

namespace UnicornOne.Battle.MonoBehaviours
{
    internal class DebugStatusUIScript : MonoBehaviour
    {
        private static readonly Dictionary<Role, Color> RoleColorMap = new Dictionary<Role, Color>()
        {
            [Role.Ally] = Color.green,
            [Role.Enemy] = Color.red
        };
        private static readonly Color DefaultColor = Color.grey;

        [SerializeField] private TMP_Text _text;

        private bool _isDirty = true;

        public Role Role
        {
            set
            {
                Color color;
                if (!RoleColorMap.TryGetValue(value, out color))
                {
                    color = DefaultColor;
                }
                _text.color = color;
            }
        }

        private int _maxHealth = 0;
        public int MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                if (_maxHealth == value)
                {
                    return;
                }

                _maxHealth = value;
                _isDirty = true;
            }
        }

        private int _currentHealth = 0;
        public int CurrentHealth
        {
            get { return _currentHealth; }
            set 
            {
                if (_currentHealth == value)
                {
                    return;
                }

                _currentHealth = value;
                _isDirty = true;
            }
        }

        private void Update()
        {
            if (_isDirty)
            {
                _text.text = $"HP: {CurrentHealth}/{MaxHealth}";

                _isDirty = false;
            }
        }
    }
}
