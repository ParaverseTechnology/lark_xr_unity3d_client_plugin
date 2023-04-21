using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateButton : Button
{
    public bool Pressed {
        get {
            return IsPressed();
        }
    }
}
