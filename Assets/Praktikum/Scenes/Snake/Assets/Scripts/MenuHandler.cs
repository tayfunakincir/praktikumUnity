using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public Text title;
    public Text highscoresText;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(PrefKeys.Highscores))
        {
            var highscores = PlayerPrefs.GetString(PrefKeys.Highscores);
            var splitHighscores = highscores.Split(',');
        
            var text = "YOUR HIGHSCORES";
        
            for (var i = 0; i < splitHighscores.Length - 1; i++)
            {
                text += "#" + (i + 1) + ": " + splitHighscores[i] + ", ";
            }
        
            highscoresText.text = text;
        }
        else
        {
            highscoresText.text = "";
        }
        
        if (!PlayerPrefs.HasKey(PrefKeys.Score))
        {
            return;
        }
        var score = PlayerPrefs.GetInt(PrefKeys.Score);
        title.text = "Score: " + score;
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
