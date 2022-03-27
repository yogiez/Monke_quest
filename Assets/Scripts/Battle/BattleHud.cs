using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text levelText;
    [SerializeField] Text nameText;
    [SerializeField] HPBar hpBar;

    Enemies _enemy;

    //funkcja ustawia odpowiednia nazwe, lvl oraz HP postaci
    public void SetData(Enemies enemy)
    {
        _enemy = enemy;

        nameText.text = enemy.Base.Name;
        levelText.text = "Lvl " + enemy.Level;
        hpBar.SetHP((float)enemy.HP / enemy.MaxHp);
    }

    //update paska hp w UI po ataku
    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float) _enemy.HP / _enemy.MaxHp);
    }
}
