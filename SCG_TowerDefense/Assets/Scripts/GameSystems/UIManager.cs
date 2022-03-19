using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text hpText;
    public Text moneyText;

    public Text waveText;


    public void SetHPText(int newHp)
    {
        hpText.text = "HP : " + newHp + " / " + 80;
    }

    public void SetMoneyText(int newMoney)
    {
        moneyText.text = "Money : " + newMoney + "$ / " + 500 + "$";
    }

    public void SetWaveText(int newWave)
    {
        waveText.text = "Wave : " + newWave + " / " + 10;
    }
}
