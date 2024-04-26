using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputProvider
{
    Vector3 MoveVector { get; }
    bool IsAttack { get; }
}
