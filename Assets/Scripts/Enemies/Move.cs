using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; } //referencje do zmiennych z MoveBase, szybszy sposob do tworzenia prywatnych zmiennych kiedy nie musisz ich ujawniac poza klase np. do inspektora
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
}
