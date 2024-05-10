using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] UIMouseCursor uiMouseCursor;
    [SerializeField] PortraitController portraitController;
    public PortraitController PortraitController=> portraitController;
}
