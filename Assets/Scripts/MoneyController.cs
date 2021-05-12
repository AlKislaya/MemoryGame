using UnityEngine;

public class MoneyController : Singleton<MoneyController>
{
    private const string MoneyPrefsKey = "money_balance";
    private const int _firstBalance = 50;

    public int MoneyBalance => _money;

    private int _money;

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
    }

    public bool GetMoney(int value)
    {
        if (value > _money)
        {
            return false;
        }

        _money -= value;
        return true;
    }

    private void SaveToPrefs()
    {
        PlayerPrefs.SetInt(MoneyPrefsKey, _money);
    }
}
