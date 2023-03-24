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
        [SerializeField] private Hero[] EditorHeroes;
#endif

        private void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            _ecsWorld.GameFinished += OnGameFinished;

#if UNITY_EDITOR
            StartGame(
                SharedData.SelectedLevel ?? EditorLevel,
                SharedData.SelectedHeroes ?? EditorHeroes
            );
#endif
        }

        private void Update()
        {
            
        }

        private void OnDestroy()
        {
            _ecsWorld.GameFinished -= OnGameFinished;
        }


        private void StartGame(Level level, Hero[] heroes)
        {
            _ecsWorld.Init(level, heroes);
        }

        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (current.name != "GameScene")
            {
                return;
            }

            StartGame(SharedData.SelectedLevel, SharedData.SelectedHeroes);
        }

        private void OnGameFinished()
        {
            SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        }
    }
}
