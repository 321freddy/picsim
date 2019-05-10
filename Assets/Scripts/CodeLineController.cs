using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeLineController : MonoBehaviour
{
    public Color COLOR_DEFAULT;
    public Color COLOR_RUNNING;

    public Color COLOR_BP_ACTIVE;
    public Color COLOR_BP_INACTIVE;

    public Command Command { get; set; }

    public string Text
    {
        get => GetComponentInChildren<Text>().text;
        set
        {
            GetComponentInChildren<Text>().text = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        setBreakpointActive(Command?.breakpoint ?? false);
    }

    public void onClick()
    {
        toggleBreakpoint();
    }

    public void setRunning(bool running)
    {
        var color = running ? COLOR_RUNNING : COLOR_DEFAULT;
        var adjustedColor = new Color(color.r, color.g, color.b, color.a);
        if (!running && (transform.GetSiblingIndex() % 2) == 0)
            color.a /= 2;

        GetComponent<RawImage>().color = color;
    }

    public void toggleBreakpoint()
    {
        if (Command == null) return;

        var active = (Command.breakpoint = !Command.breakpoint);
        setBreakpointActive(active);
    }

    public void setBreakpointActive(bool active)
    {
        GetComponentInChildren<Image>().color = active ? COLOR_BP_ACTIVE : COLOR_BP_INACTIVE;
    }

}
