using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public enum ActionTypes
    {
        Game,
    }
    public InputActionMap Game{get;private set;}

    [SerializeField]PlayerInput playerInput;
    public ActionTypes currentType{get;private set;}=ActionTypes.Game;



    void Awake()
    {
        Game=playerInput.actions.FindActionMap(ActionTypes.Game.ToSafeString());
    }


    public void ChangeActionType(ActionTypes type)
    {
        playerInput.SwitchCurrentActionMap(type.ToString());
    }      


}
