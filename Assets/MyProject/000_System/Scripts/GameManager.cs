using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] PlayerInputManager inputManager;
    public PlayerInputManager InputManager=>inputManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance=this;
    }
}
