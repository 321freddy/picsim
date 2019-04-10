using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GUIController : MonoBehaviour
{
    public Color COLOR_DEFAULT;
    public Color COLOR_WAS_RUNNING;
    public Color COLOR_RUNNING;
    public Color COLOR_BREAKPOINT;

    public Dropdown fileDropdown;
    public GameObject codeContainer;
    public GameObject codeLineTemplate;

    private Simulation simulation;
    private Command lastCommand = null;

    // Start is called before the first frame update
    void Start()
    {
        // Populate file dropdown
        foreach (var file in FileManager.listAll())
        {
            fileDropdown.options.Add(new Dropdown.OptionData(file.Name));
        }
    }

    private void setCommandColor(Command cmd, Color color)
    {
        if (cmd != null)
        {
            var obj = codeContainer.transform.GetChild(cmd.Line);
            if (obj != null)
            {
                obj.GetComponent<Text>().color = color;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (simulation != null)
        {
            setCommandColor(lastCommand, COLOR_WAS_RUNNING); // Update current command color to green
            lastCommand = simulation.getCurrentCommand();
            setCommandColor(lastCommand, COLOR_RUNNING);

            var success = simulation.step();
            if (success)
            {
                // Update GUI
                GameObject.Find("wRegister").GetComponent<Text>().text ="W-Register: " + simulation.Memory.w_Register.ToString("X");
                GameObject.Find("ZeroFlag").GetComponent<Text>().text ="Zero-Flag: " + simulation.Memory.ZeroFlag.ToString();
                GameObject.Find("ProgCounter").GetComponent<Text>().text ="PCL: " + simulation.Memory.ProgramCounter.ToString();
            }
            else
            {
                simulation = null;
                lastCommand = null;
            }

        }
    }

    public void onFileSelected()
    {
        // clear file view
        foreach (Transform child in codeContainer.transform) Destroy(child.gameObject);

        var filename = fileDropdown.options[fileDropdown.value].text;
        Debug.Log("selected file " + filename);
        
        var lines = FileManager.getLines(filename);
        

        foreach (string line in lines)
        {
            var lineObject = Instantiate(codeLineTemplate, codeContainer.transform);
            lineObject.GetComponent<Text>().text = line;
        }

        simulation = Simulation.CreateFromProgram(lines);
    }

    public void onBtnClick()
    {
        Debug.Log("clicked button!!");
    }
}
