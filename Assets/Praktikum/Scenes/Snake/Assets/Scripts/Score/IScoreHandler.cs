using System.Collections.Generic;

namespace Praktikum.Scenes.Snake.Assets.Scripts.Score
{
    public interface IScoreHandler
    {
        List<int> Load();
        void Save();
        void AddScore();
        void Increase();
        void Reset();
        void Delete();
    }
}