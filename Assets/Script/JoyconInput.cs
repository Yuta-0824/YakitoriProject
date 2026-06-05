using System.Collections.Generic;
using UnityEngine;

public class JoyconInput : MonoBehaviour
{
    public static JoyconInput Instance;
    public Joycon rightJoycon;

    void Awake() { Instance = this; }

    void Start()
    {
        List<Joycon> joycons = JoyconManager.Instance.j;
        if (joycons != null && joycons.Count > 0)
        {
            foreach (var j in joycons) { if (!j.isLeft) rightJoycon = j; }
        }
    }

    public Quaternion GetRotation() => rightJoycon != null ? rightJoycon.GetVector() : Quaternion.identity;
    public bool IsHoldingZR() => rightJoycon != null && rightJoycon.GetButton(Joycon.Button.SHOULDER_2);
    public bool GetAButton() => rightJoycon != null && rightJoycon.GetButtonDown(Joycon.Button.DPAD_RIGHT);
    public void Recenter() => rightJoycon?.Recenter();
    // JoyconInput.cs ‚Ì’†‚É’Ç‰Á
    public bool GetBButton() => rightJoycon != null && rightJoycon.GetButtonDown(Joycon.Button.DPAD_DOWN);
}