using System.Collections.Generic;
using System.Linq;
using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;

public class ScoreHandler : IScoreHandler
{
    private List<int> _scores;

    public ScoreHandler()
    {
        Clear();
        _scores = Load();
    }
    
    public List<int> Load()
    {
        // if (!PlayerPrefs.HasKey(PrefKeys.Highscores))
        // {
        //     return new List<int>();
        // }
        //
        // var highscores = PlayerPrefs.GetString(PrefKeys.Highscores);
        // var highscoresSplit = highscores.Split(',');
        //
        // return highscoresSplit.Select(int.Parse).ToList();
        //
        return new List<int>();
    }

    public void Save()
    {
        // sort first
        _scores.Sort();
        
        // save three entries
        var value = "";
        for (var i = 0; i < 3; i++)
        {
            value += _scores[i] + ",";
        }

        PlayerPrefs.SetString(PrefKeys.Highscores, value);
        PlayerPrefs.Save();
    }

    public void Add(int score)
    {
        _scores.Add(score);
        PlayerPrefs.SetInt(PrefKeys.Score, score);
    }

    private static void Clear()
    {
        PlayerPrefs.DeleteKey(PrefKeys.Score);
    }
}
