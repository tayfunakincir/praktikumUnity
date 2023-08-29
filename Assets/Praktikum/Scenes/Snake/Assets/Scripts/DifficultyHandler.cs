using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Praktikum.Scenes.Snake.Assets.Scripts
{
    public class DifficultyHandler : MonoBehaviour
    {
        // not an optimal solution

        public void EasyGame()
        {
            PlayerPrefs.SetInt(PrefKeys.Difficulty, PrefKeys.Easy);
            StartGame();
        }

        public void MediumGame()
        {
            PlayerPrefs.SetInt(PrefKeys.Difficulty, PrefKeys.Medium);
            StartGame();
        }

        public void HardGame()
        {
            PlayerPrefs.SetInt(PrefKeys.Difficulty, PrefKeys.Hard);
            StartGame();
        }

        private static void StartGame()
        {
            SceneManager.LoadScene(2);
        }
    }
}
