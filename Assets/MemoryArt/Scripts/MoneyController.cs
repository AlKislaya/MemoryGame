using MemoryArt.Global.Patterns;
using UnityEngine;
using YG;

namespace MemoryArt
{
    public class MoneyController : Singleton<MoneyController>
    {
#if UNITY_WEBGL
        public int MoneyBalance => YandexGame.SDKEnabled ? YandexGame.savesData.Coins : 0;
#else
        public int MoneyBalance => _money;
        private const string MoneyPrefsKey = "money_balance";
        private const int _firstBalance = 50;

        private int _money;
#endif
            
        protected override void Awake()
        {
            base.Awake();
#if !UNITY_WEBGL
            if (PlayerPrefs.HasKey(MoneyPrefsKey))
            {
                _money = PlayerPrefs.GetInt(MoneyPrefsKey);
            }
            else
            {
                _money = _firstBalance;
                SaveToPrefs();
            }
#endif
        }

        public void AddMoney(int value)
        {
#if UNITY_WEBGL
            YandexGame.savesData.Coins += value;
            YandexGame.SaveProgress();
#else
            _money += value;
            SaveToPrefs();
#endif
        }

        public bool GetMoney(int value)
        {
#if UNITY_WEBGL
            if (value > MoneyBalance)
            {
                return false;
            }
            
            YandexGame.savesData.Coins -= value;
            YandexGame.SaveProgress();
            return true;
#else
            if (value > _money)
            {
                return false;
            }

            _money -= value;
            SaveToPrefs();
            return true;
#endif
        }

#if !UNITY_WEBGL
        private void SaveToPrefs()
        {
            PlayerPrefs.SetInt(MoneyPrefsKey, _money);
        }
#endif
    }
}