using System.Collections.Generic;

namespace Praktikum.Scenes.Snake.Assets.Scripts.Score
{
    public interface IScoreHandler
    {
        List<int> Load();
        void Save();
        void Add(int score);
    }
}