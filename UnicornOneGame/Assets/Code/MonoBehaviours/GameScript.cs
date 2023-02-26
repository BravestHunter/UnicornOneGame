using System.Collections;
using System.Collections.Generic;
using UnicornOne.ScriptableObjects;
using UnicornOne.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnicornOne.MonoBehaviours
{
    public class GameScript : MonoBehaviour
    {
        [SerializeField] private EcsWorldScript _ecsWorld;

#if UNITY_EDITOR
        [SerializeField] private Level EditorLevel;
#endif

        private void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

#if UNITY_EDITOR
            StartGame(SharedData.SelectedLevel ?? EditorLevel);
#endif

            _ecsWorld.GameFinished += OnGameFinished;
        }

        private void Update()
        {
            
        }

        private void OnDestroy()
        {
            _ecsWorld.GameFinished -= OnGameFinished;
        }


        private void StartGame(Level level)
        {
            _ecsWorld.Init(level);
        }

        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (current.name != "GameScene")
            {
                return;
            }

            StartGame(SharedData.SelectedLevel);
        }

        private void OnGameFinished()
        {
            SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        }
    }
}
