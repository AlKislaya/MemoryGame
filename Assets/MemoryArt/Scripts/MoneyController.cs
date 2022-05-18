using System;
using MemoryArt.Global.Patterns;
using UnityEngine;

namespace MemoryArt
{
    public class MoneyController : Singleton<MoneyController>
    {
        private const string MoneyPrefsKey = "money_balance";
        private const int _firstBalance = 150;

        private int _money;

        public int MoneyBalance => _money;

        public event Action<int> BalanceChanged;

        protected override void Awake()
        {
            base.Awake();

            if (PlayerPrefs.HasKey(MoneyPrefsKey))
            {
                _money = PlayerPrefs.GetInt(MoneyPrefsKey);
            }
            else
            {
                _money = _firstBalance;
                SaveToPrefs();
            }
        }

        public void AddMoney(int value)
        {
            _money += value;
            SaveToPrefs();
            BalanceChanged?.Invoke(_money);
        }

        public bool GetMoney(int value)
        {
            if (value > _money)
            {
                return false;
            }

            _money -= value;
            SaveToPrefs();
            return true;
        }

        private void SaveToPrefs()
        {
            PlayerPrefs.SetInt(MoneyPrefsKey, _money);
        }
    }
}