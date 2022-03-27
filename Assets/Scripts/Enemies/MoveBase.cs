using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Enemies/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] EnemiesType type;
    [SerializeField] int power;
    [SerializeField] int accurency;
    [SerializeField] int pp;

    //ujawnienie prywatnych zmiennych poza klasa
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public EnemiesType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accurency
    {
        get { return accurency; }
    }

    public int PP
    {
        get { return pp; }
    }

}
