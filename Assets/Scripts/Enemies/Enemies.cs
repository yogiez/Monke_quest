using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemies
{
    [SerializeField] EnemiesBase _base;
    [SerializeField] int level;

    public EnemiesBase Base 
    { 
        get { return _base; }
    }
    public int Level 
    { 
        get { return level; }
    }

    public int HP { get; set; }

    public List<Move> Moves { get; set; } //rzeczywista lista atakow ktore ma postac

    public void Init()
    {
        HP = MaxHp;

        //generowanie wszystkich atakow ktore ma miec postac na podstawie jej poziomu
        Moves = new List<Move>();   //wszystkie ataki do nowej listy
        foreach (var move in Base.LearnableMoves)  //petla przechodzaca przez wszystkie ataki mozliwe do nauki i dodanie do listy odpowiednich zaleznie od poziomu postaci
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)   //postac moze miec tylko 4 ataki naraz, jezeli ma juz 4 = break
                break;
        }    
    }
    //skalowanie statystyk do poziomu postaci
    //https://bulbapedia.bulbagarden.net/wiki/Stat
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5;  }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    //atak postaci
    //https://bulbapedia.bulbagarden.net/wiki/Damage
    public DamageDetails TakeDamage(Move move, Enemies attacker)
    {
        float critical = 1f; //ataki krytyczne
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1); //dodatkowe obrazenia zalezne od typow postaci

        var damageDetails = new DamageDetails()
        {
            TypeEffectives = type,
            Critical = critical,
            Dead = false
        };

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Dead = true;
        }

        return damageDetails;
    }

    //wybor ataku dla przeciwnika
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Dead { get; set; }
    public float Critical { get; set; }
    public float TypeEffectives { get; set; }
}
