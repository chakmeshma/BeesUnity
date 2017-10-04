using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : MonoBehaviour {
    public enum ButtonType
    {
        CollectHoneyButton,
        GoHomeButton,
        DiscoverButton
    }

    public ButtonType buttonType;
}
