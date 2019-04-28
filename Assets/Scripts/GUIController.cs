using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GUIController : MonoBehaviour
{
    public Dropdown fileDropdown;
    public Dropdown freqDropdown;
    public ScrollRect scrollRect;
    public GameObject codeContainer;
    public GameObject codeLineTemplate;
    public GameObject goButton;
    public GameObject pauseButton;
    public GameObject stepInButton;
    public GameObject stepOutButton;
    public GameObject resetButton;

    private Simulation simulation;
    private Command currentCommand = null;
    private bool simulationRunning = false;

    public static int frequency_string;
    public double total_time = 0;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = scrollRect ?? codeContainer.GetComponentInParent<ScrollRect>();

        Screen.SetResolution(1280, 720, false); 

        // Populate file dropdown
        foreach (var file in FileManager.listAll())
        {
            fileDropdown.options.Add(new Dropdown.OptionData(file.Name));
        }
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

    private CodeLineController getCodeLine(Command cmd)
    {
        if (cmd != null)
        {
            var obj = codeContainer.transform.GetChild(cmd.Line);
            if (obj != null)
            {
                return obj.GetComponent<CodeLineController>();
            }
        }

        return null;
    }

    private void updateRegisterDisplay()
    {
        GameObject.Find("wRegister").GetComponent<Text>().text = simulation.Memory.w_Register.ToString("X2") + "h";
        //GameObject.Find("StatusRegister").GetComponent<Text>().text = "STATUS: " + Convert.ToString(simulation.Memory.get(Address.STATUS), 2).PadLeft(8, '0');
        GameObject.Find("ProgCounter").GetComponent<Text>().text = simulation.Memory.ProgramCounter.ToString();

        //Write Bits to GUI StatusReg
        GameObject.Find("C (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.C), 2);
        GameObject.Find("DC (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.DC), 2);
        GameObject.Find("Z (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.Z), 2);
        GameObject.Find("PD (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.PD), 2);
        GameObject.Find("TO (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.TO), 2);
        GameObject.Find("RP0 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.RP0), 2);
        GameObject.Find("RP1 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.RP1), 2);
        GameObject.Find("IRP (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.IRP), 2);
    }

    private void updateScroll()
    {
        if (Input.GetMouseButton(0)) return;

        var line = getCodeLine(currentCommand).GetComponent<RectTransform>();
        var container = codeContainer.GetComponent<RectTransform>();
        var viewport = scrollRect.GetComponent<RectTransform>();
        var lineY = -line.localPosition.y;
        var containerY = container.localPosition.y;

        if (lineY + 10 > containerY + viewport.rect.height)
            scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(lineY / container.rect.height);
        else if (lineY - 10 < containerY)
            scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(lineY / container.rect.height);
    }

    public void onFileSelected()
    {
        // clear file view
        foreach (Transform child in codeContainer.transform) Destroy(child.gameObject);

        var filename = fileDropdown.options[fileDropdown.value].text; // Get file name
        Debug.Log("selected file " + filename);

        TimerReset();   //Reset Timer everytime the selectedFile is changed
        onFrequencySelected();  //Get act. frequency

        var lines = FileManager.getLines(filename); // Read file
        simulation = Simulation.CreateFromProgram(lines); // Init simulation

        updateRegisterDisplay();
        setSimulationRunning(false);
        currentCommand = simulation.getCurrentCommand();

        var lineNum = 0;
        var codeLines = new CodeLineController[lines.Count];
        foreach (string line in lines) // Populate code box
        {
            var codeLine = Instantiate(codeLineTemplate, codeContainer.transform).GetComponent<CodeLineController>();
            codeLine.Text = line;
            codeLine.setRunning(lineNum == currentCommand.Line); // Set color to green if first command
            codeLines[lineNum] = codeLine;
            lineNum++;

            // Update width
            codeLine.GetComponent<LayoutElement>().minWidth = scrollRect.GetComponent<RectTransform>().rect.width - 15 - 20;
        }

        // Set command references on code line objects
        foreach (var cmd in simulation.Commands)
        {
            codeLines[cmd.Line].Command = cmd;
        }

        goButton.GetComponent<Button>().interactable = true;
        pauseButton.GetComponent<Button>().interactable = true;
        stepInButton.GetComponent<Button>().interactable = true;
        stepOutButton.GetComponent<Button>().interactable = true;
        resetButton.GetComponent<Button>().interactable = true;
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void onFrequencySelected()
    {
        frequency_string = freqDropdown.value; // Get frequency index from dropdown menue not actual frequency
       //Debug.Log(frequency_string);
    }

    public void stepIn()
    {
        var success = simulation.step(); // Run command
        if (success)
        {
            // Update GUI
            getCodeLine(currentCommand).setRunning(false); // Update next command color to green
            currentCommand = simulation.getCurrentCommand();
            getCodeLine(currentCommand).setRunning(true);

            updateRegisterDisplay();
            updateScroll();

            /* Update Timer and sleep time:
             * Every time a step is done by the GUI, the function Timer.setFrequency is used to get the right value for the timer, depending on the used frequency 
             * Sleep is used to slow down the GUI a bit, only for optics, not functional
             */
            Timer.setFrequency();
            total_time = total_time + Timer.microseconds_per_step;
            GameObject.Find("Laufzeit").GetComponent<Text>().text = total_time.ToString("0.00") + " µs"; //Write Data to GUI after formating it 
            System.Threading.Thread.Sleep(100); //Sleep for 100ms
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
            if (simulation.reachedBreakpoint()) setSimulationRunning(false);

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

        TimerReset();

        // Reset code color
        foreach (var codeLine in codeContainer.GetComponentsInChildren<CodeLineController>())
        {
            codeLine.setRunning(false);
        }

        updateRegisterDisplay();
        setSimulationRunning(false);
        currentCommand = simulation.getCurrentCommand();
        getCodeLine(currentCommand).setRunning(true); // Mark first command
        updateScroll();
    }
    public void onHelpClicked()
    {
        System.Diagnostics.Process.Start("Help.jpg");
    }
    public void TimerReset()    //Reset timer and total time
    {
        GameObject.Find("Laufzeit").GetComponent<Text>().text = Convert.ToString("0.00") + " µs";
        total_time = 0;
    }
}
