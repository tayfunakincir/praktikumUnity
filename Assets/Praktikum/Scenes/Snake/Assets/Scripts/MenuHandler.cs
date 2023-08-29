using System.Linq;
using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Praktikum.Scenes.Snake.Assets.Scripts
{
    public class MenuHandler : MonoBehaviour
    {

        public Text highscoresTitle;
        public Text highscoresText;
        public Button resetButton;
    
        private static IScoreHandler _scoreHandler;

        private void Awake()
        {
            _scoreHandler ??= gameObject.AddComponent<ScoreHandler>();

            ShowHighscores();
        }
    
        private void ShowHighscores()
        {
            var list = _scoreHandler.Load();
            if (list.Count == 0)
            {
                highscoresTitle.text = "";
                highscoresText.text = "";
                resetButton.enabled = false;
                return;
            }

            highscoresTitle.text = "YOUR HIGHSCORES";
            highscoresText.text = string.Join(" - ", Enumerable.Range(0, list.Count).Select(n => "#" + (n + 1) + ": " + list[n]));
            resetButton.enabled = true;
        }

        public void NextScene()
        {
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void ResetScores()
        {
            _scoreHandler.Delete();
            ShowHighscores();
        }
    }
}
