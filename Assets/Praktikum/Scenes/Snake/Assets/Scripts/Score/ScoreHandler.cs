using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Praktikum.Scenes.Snake.Assets.Scripts.Score
{
    public class ScoreHandler : MonoBehaviour, IScoreHandler
    {
        private readonly List<int> _scores;
        private int _currentScore;

        public ScoreHandler()
        {
            _scores = Load();
        
            Reset();
        }
    
        public List<int> Load()
        {
            var filePath = GetFolderPath() + PrefKeys.Highscores + ".json";
            if (!File.Exists(filePath))
            {
                return new List<int>();
            }
        
            var jsonString = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<Highscore>(jsonString);

            if (data.scores == null || data.scores.Length == 0)
            {
                return new List<int>();
            }
        
            return data.scores.ToList();
        }

        public void Save()
        {
            // sort first
            _scores.Sort();
            _scores.Reverse();
                
            // save json
            var data = new Highscore()
            {
                scores = Enumerable.Range(0, Mathf.Min(_scores.Count, 3))
                    .Select(n => _scores[n])
                    .ToArray()
            };
        
            // Debug.Log(string.Join(",", data.scores.Select(n => "Score: " + n + " ")));
        
            CreateFolder();

            var jsonString = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetFolderPath() + PrefKeys.Highscores + ".json", jsonString);
        }

        public void AddScore()
        {
            _scores.Add(_currentScore);
        }

        public void Increase()
        {
            _currentScore += 1;
        }

        public void Reset()
        {
            _currentScore = 0;
        }

        public void Delete()
        {
            File.Delete(GetFolderPath() + PrefKeys.Highscores + ".json");
        }

        private static void CreateFolder()
        {
            if (Directory.Exists(GetFolderPath()))
            {
                return;
            }
        
            Directory.CreateDirectory(GetFolderPath());
        }
    
        private static string GetFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Praktikum\\Unity\\Snake\\";
        }
    }
}
