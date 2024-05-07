using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputProvider
{
    Vector3 MoveVector { get; }

    /// <summary>
    /// ナビメッシュエージェントの高さ
    /// </summary>
    float AgentHeight{get;}

    bool IsAttack { get; }
    bool IsDead { get; }

    bool IsMove{ get; }

    bool IsDamage{get;set;}
    /// <summary>
    /// 操作可能か
    /// </summary>
    bool CanControl{get;}

    public void StartTurn();
    public void EndTurn();
}
