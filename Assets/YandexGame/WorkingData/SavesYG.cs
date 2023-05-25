using System.Collections.Generic;
using MemoryArt.Game.Levels;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;
        
        // Ваши сохранения
        public int Coins = 50;
        public string LanguageKey = "RU";
        public Dictionary<string, LevelsProgress> LevelsProgress = new Dictionary<string, LevelsProgress>();
    }
}
