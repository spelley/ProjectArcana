using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurnTaker
{
    Vector3Int curPosition {get; set;}
    bool incapacitated { get; }
    int ct { get; set; }
    int speed { get; }
    bool moved { get; set; }
    bool acted { get; set; }
    Sprite sprite { get; set; }

    bool canMove { get; }
    bool canAct { get; }

    void StartTurn();

    void EndTurn();
}
