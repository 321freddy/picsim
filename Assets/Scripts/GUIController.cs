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
    public GameObject goButton;
    public GameObject pauseButton;

    private Simulation simulation;
    private Command currentCommand = null;
    private bool simulationRunning = false;


    // Start is called before the first frame update
    void Start()
    {
        // Populate file dropdown
        foreach (var file in FileManager.listAll())
        {
            fileDropdown.options.Add(new Dropdown.OptionData(file.Name));
        }

        // Hide pause button
        GameObject.Find("PauseButton").SetActive(false);
    }

    private void setSimulationRunning(bool running)
    {
        simulationRunning = running;

        // Update go/pause button
        if (simulationRunning)
        {
            pauseButton.SetActive(true);
            goButton.SetActive(false);
        }
        else
        {
            pauseButton.SetActive(false);
            goButton.SetActive(true);
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
    
    public void onFileSelected()
    {
        // clear file view
        foreach (Transform child in codeContainer.transform) Destroy(child.gameObject);

        var filename = fileDropdown.options[fileDropdown.value].text; // Get file name
        Debug.Log("selected file " + filename);

        var lines = FileManager.getLines(filename); // Read file
        simulation = Simulation.CreateFromProgram(lines); // Init simulation

        setSimulationRunning(false);
        currentCommand = simulation.getCurrentCommand();

        var lineNum = 0;
        foreach (string line in lines) // Populate code box
        {
            var lineObject = Instantiate(codeLineTemplate, codeContainer.transform).GetComponent<Text>();
            lineObject.text = line;

            if (lineNum == currentCommand.Line) lineObject.color = COLOR_RUNNING; // Set color to green if first command
            lineNum++;
        }

    }

    public void stepIn()
    {
        var success = simulation.step(); // Run command
        if (success)
        {
            // Update GUI
            setCommandColor(currentCommand, COLOR_WAS_RUNNING); // Update next command color to green
            currentCommand = simulation.getCurrentCommand();
            setCommandColor(currentCommand, COLOR_RUNNING);

            GameObject.Find("wRegister").GetComponent<Text>().text = "W Register: " + simulation.Memory.w_Register.ToString("X");
            GameObject.Find("ZeroFlag").GetComponent<Text>().text = "Zero Flag: " + simulation.Memory.ZeroFlag.ToString();
            GameObject.Find("ProgCounter").GetComponent<Text>().text = "Program Counter: " + simulation.Memory.ProgramCounter.ToString();
        }
        else
        {
            // Program reached end
            setSimulationRunning(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (simulationRunning)
        {
            stepIn();
        }
    }

    public void onGoBtnClick()
    {
        Debug.Log("Running program");
        setSimulationRunning(!simulationRunning); // Toggle
    }

    public void onStepInBtnClick()
    {
        if (!simulationRunning)
        {
            Debug.Log("Step in");
            stepIn();
        }
    }

    public void onStepOutBtnClick()
    {
        if (!simulationRunning)
        {
            Debug.Log("Step out");
            // ...
        }
    }

    public void onResetBtnClick()
    {
        Debug.Log("Reset program");
        simulation.Reset();

        // Reset code color
        foreach (var codeLine in codeContainer.transform.GetComponentsInChildren<Text>())
        {
            codeLine.color = COLOR_DEFAULT;
        }

        // Reset text
        GameObject.Find("wRegister").GetComponent<Text>().text = "W Register: 0";
        GameObject.Find("ZeroFlag").GetComponent<Text>().text = "Zero Flag: 0";
        GameObject.Find("ProgCounter").GetComponent<Text>().text = "Program Counter: 0";

        setSimulationRunning(false);
        currentCommand = simulation.getCurrentCommand();
        setCommandColor(currentCommand, COLOR_RUNNING); // Mark first command
    }
}
