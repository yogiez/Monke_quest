using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Enemies/Create new enemy")] //utworzenie szybszej opcji tworzenia nowych przeciwnikow
public class EnemiesBase : ScriptableObject
{
    [SerializeField] new string name;   //serializefield uzyte aby zmienna pozostala private, ale byla dostepna w inspektorze

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite playerSprite;

    [SerializeField] EnemiesType type1;

    //podstawowe statystyki
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    //lista atakow
    [SerializeField] List<LearnableMove> learnableMoves;

    //ujawnienie prywatnych zmiennych poza klasa
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description;  }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite PlayerSprite
    {
        get { return playerSprite; }
    }

    public EnemiesType Type1
    {
        get { return type1; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves;  }
    }
}
[System.Serializable] //serializacja listy (zapisanie stanu obiektu, aby można było go odtworzyć w razie potrzeby)
//ataki mozliwe do nauki
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
public enum EnemiesType //enum = typ wyliczeniowy
{
    None,
    Human,
    Demon,
    Goblin,
    Ghost,
    Undead
}

//efektywność typów na inne typy
public class TypeChart
{
    static float[][] chart =
    {
        //                   HUM  DEM GOB GHO UND
        /*HUM*/ new float[] { 1f, 1f, 1.5f, 0f, 1f },
        /*DEM*/ new float[] { 1f, 0.5f, 0.5f, 1f, 1.5f },
        /*GOB*/ new float[] { 0.5f, 1.5f, 0.5f, 1f, 1f },
        /*GHO*/ new float[] { 1.5f, 1f, 1f, 0.5f, 0f },
        /*UND*/ new float[] { 1f, 0.5f, 1f, 1.5f, 0.5f }
    };

    public static float GetEffectiveness(EnemiesType attackType, EnemiesType defenseType)
    {
        if (attackType == EnemiesType.None || defenseType == EnemiesType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }


}


