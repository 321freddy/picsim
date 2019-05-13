using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

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

    public GameObject ramContainer;
    public GameObject registerTemplate;
    public GameObject registerRowTemplate;
    public GameObject registerTextTemplate;

    public GameObject EEPROMContainer;

    public Slider speedSlider;

    private Simulation simulation;
    private Command currentCommand = null;
    private bool simulationRunning = false;
    private bool fileSelected = false;

    private int frequencyIndex;

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

        loadRamView();
        loadEEPROMView();
    }

    private void setSimulationRunning(bool running)
    {
        simulationRunning = running;

        // Update go/pause button
        pauseButton.SetActive(running);
        goButton.SetActive(!running);
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
        GameObject.Find("wRegister").GetComponent<Text>().text = simulation.Memory.w_Register.ToString("X2");
        GameObject.Find("ProgCounter").GetComponent<Text>().text = simulation.Memory.ProgramCounter.ToString("X2");
        GameObject.Find("Value_PCL").GetComponent<Text>().text = (simulation.Memory.getRaw((byte) 0x02)).ToString("X2");
        GameObject.Find("Value_FSR").GetComponent<Text>().text = (simulation.Memory.getRaw((byte)0x04)).ToString("X2");
        GameObject.Find("Value_PCLATH").GetComponent<Text>().text = (simulation.Memory.getRaw((byte)0x0A)).ToString("X2");

        //Stack
        GameObject.Find("Stack00").GetComponent<Text>().text = (simulation.Memory.getStack(0)).ToString("X2");
        GameObject.Find("Stack01").GetComponent<Text>().text = (simulation.Memory.getStack(1)).ToString("X2");
        GameObject.Find("Stack02").GetComponent<Text>().text = (simulation.Memory.getStack(2)).ToString("X2");
        GameObject.Find("Stack03").GetComponent<Text>().text = (simulation.Memory.getStack(3)).ToString("X2");
        GameObject.Find("Stack04").GetComponent<Text>().text = (simulation.Memory.getStack(4)).ToString("X2");
        GameObject.Find("Stack05").GetComponent<Text>().text = (simulation.Memory.getStack(5)).ToString("X2");
        GameObject.Find("Stack06").GetComponent<Text>().text = (simulation.Memory.getStack(6)).ToString("X2");
        GameObject.Find("Stack07").GetComponent<Text>().text = (simulation.Memory.getStack(7)).ToString("X2");

        //Write Bits to GUI StatusReg
        GameObject.Find("C (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.C), 2);
        GameObject.Find("DC (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.DC), 2);
        GameObject.Find("Z (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.Z), 2);
        GameObject.Find("PD (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.PD), 2);
        GameObject.Find("TO (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.TO), 2);
        GameObject.Find("RP0 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.RP0), 2);
        GameObject.Find("RP1 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.RP1), 2);
        GameObject.Find("IRP (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.Status, Bit.IRP), 2);

        //Write Bits to GUI OPTION reg
        GameObject.Find("RBPU (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.RBPU), 2);
        GameObject.Find("INTEDG (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.INTEDG), 2);
        GameObject.Find("T0CS (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.T0CS), 2);
        GameObject.Find("T0SE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.T0SE), 2);
        GameObject.Find("PSA (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.PSA), 2);
        GameObject.Find("PS2 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.PS2), 2);
        GameObject.Find("PS1 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.PS1), 2);
        GameObject.Find("PS0 (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.OPTION, Bit.PS0), 2);
        
        //Write Bits to GUI INTCON reg
        GameObject.Find("GIE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.GIE), 2);
        GameObject.Find("EEIE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.EEIE), 2);
        GameObject.Find("T0IE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.T0IE), 2);
        GameObject.Find("INTE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.INTE), 2);
        GameObject.Find("RBIE (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.RBIE), 2);
        GameObject.Find("T0IF (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.T0IF), 2);
        GameObject.Find("INTF (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.INTF), 2);
        GameObject.Find("RBIF (1)").GetComponent<Text>().text = Convert.ToString(Bit.get(simulation.Memory.INTCON, Bit.RBIF), 2);
    }

    private void updateScroll()
    {
        if (Input.GetMouseButton(0)) return;

        var line = getCodeLine(currentCommand)?.GetComponent<RectTransform>();
        if (line == null) return;

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
            if (cmd != null)
            {
                codeLines[cmd.Line].Command = cmd;
            }
        }

        goButton.GetComponent<Button>().interactable = true;
        pauseButton.GetComponent<Button>().interactable = true;
        stepInButton.GetComponent<Button>().interactable = true;
        stepOutButton.GetComponent<Button>().interactable = true;
        resetButton.GetComponent<Button>().interactable = true;
        fileSelected = true;
        scrollRect.verticalNormalizedPosition = 1f;

        //Refresh View
        refreshRamView();
        refreshEEPROMView();
    }

    public void onFrequencySelected()
    {
        frequencyIndex = freqDropdown.value; // Get frequency index from dropdown menue not actual frequency
       //Debug.Log(frequencyIndex);
    }

    public void stepIn()
    {
        int cycles = simulation.step(); // Run command
        if (cycles > 0)
        {
            // Update GUI
            getCodeLine(currentCommand).setRunning(false); // Update next command color to green
            currentCommand = simulation.getCurrentCommand();
            getCodeLine(currentCommand)?.setRunning(true);
            // if (currentCommand == null)                   Debug.Log("NO CMD in line: "+simulation.Memory.ProgramCounter);
            // else if (getCodeLine(currentCommand) == null) Debug.Log("NO CODE LINE in line: "+currentCommand.Line);

            updateRegisterDisplay();
            updateScroll();

            /* Update Timer and sleep time:
             * Every time a step is done by the GUI, the function Timer.setFrequency is used to get the right value for the timer, depending on the used frequency 
             * Sleep is used to slow down the GUI a bit, only for optics, not functional
             */
            Timer.setFrequency(frequencyIndex);
            Timer.TotalTime += Timer.microseconds_per_step * cycles;
            GameObject.Find("Laufzeit").GetComponent<Text>().text = Timer.TotalTime.ToString("0.00") + " µs"; //Write Data to GUI after formating it 

            //Refresh View
            refreshRamView();
            refreshEEPROMView();
        }
        else
        {
            // Program reached end
            setSimulationRunning(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
        onSpeedChanged();
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
        foreach (var codeLine in codeContainer.GetComponentsInChildren<CodeLineController>())
        {
            codeLine.setRunning(false);
        }

        setSimulationRunning(false);
        currentCommand = simulation.getCurrentCommand();
        getCodeLine(currentCommand).setRunning(true); // Mark first command
        updateScroll();
        TimerReset();
        // OutputChangedRA();
        // OutputChangedRB();
        refreshRamView();
        refreshEEPROMView();
        updateRegisterDisplay();
    }
    public void onHelpClicked()
    {
        System.Diagnostics.Process.Start("Help.jpg");
        //Application.OpenURL("http://lmgtfy.com/?q=Pic+Simulator");
    }
    public void TimerReset()    //Reset timer and total time
    {
        GameObject.Find("Laufzeit").GetComponent<Text>().text = Convert.ToString("0.00") + " µs";
        Timer.TotalTime = 0;
    }

    /* OutputChangedRA():
     * If the bits of PORTRA are changed, this function is called.
     * It calculates the new value and saves it to the register.
     * For debugging reasons it generates an output.
     * @registerValue: registerValue is calculated by adding every bit with its decimal value. Saved in RA-Register.
     * @registerRA: Address of RA-Register.
     * @fileselected: if TRUE, a file is selected and the register is initialized. Catch NULL Exception.
     * @pinNumber: pinNumber is extracted from the name of the selected button.
     * @pinsRA[8]: Holds buttons in GUI. Organized as an array. 8 Bits long.
     * @simulation.Memory.setRaw: save value at given address.
     * @simulation.Memory.getRaw: load value from given address.
     * @RefreshRam(): Refresh Ram view after values in register have been changed.
     */
    public void OutputChangedRA()
    {
        // File selected
        if (fileSelected == true)
        { 
            // Extract pin number from selected button name [0,1,2,3,4,5,6,7]
            var selected = EventSystem.current.currentSelectedGameObject.transform;
            if (selected.parent.name != "Pins") return;
            var selectedObject = selected.gameObject;

            // Check current pin value and change it. (0/1)?
            int pin = int.Parse(selectedObject.GetComponent<Text>().text);
            selectedObject.GetComponent<Text>().text = (1 - pin).ToString();

            // Read values of all pins and calculate the value of the whole byte (HEX)
            int registerValue = 0;
            for (int i = 0; i < 8; i++)
            {
                var isset = int.Parse(selected.parent.GetChild(i).GetComponent<Text>().text);
                registerValue = Bit.setTo(registerValue, 7 - i, isset);
            }
            // Save claculated value
            simulation.Memory.PORTA = (byte)registerValue;
            
            // Print value of RA register (HEX)
            Debug.Log("Inhalt RA-Register: " + simulation.Memory.PORTA.ToString("X"));

            //Refresh View
            refreshRamView();
            refreshEEPROMView();
        }
    }

    /* OutputChangedRB():
     * If the bits of PORTRA are changed, this function is called.
     * It calculates the new value and saves it to the register.
     * For debugging reasons it generates an output.
     * @registerValue: registerValue is calculated by adding every bit with its decimal value. Saved in RA-Register.
     * @registerRB: Address of RA-Register.
     * @fileselected: if TRUE, a file is selected and the register is initialized. Catch NULL Exception.
     * @pinNumber: pinNumber is extracted from the name of the selected button.
     * @pinsRB[8]: Holds buttons in GUI. Organized as an array. 8 Bits long.
     * @simulation.Memory.setRaw: save value at given address.
     * @simulation.Memory.getRaw: load value from given address.
     * @RefreshRam(): Refresh Ram view after values in register have been changed.
     */
    public void OutputChangedRB()
    {
        // File selected
        if (fileSelected == true)
        { 
            // Extract pin number from selected button name [0,1,2,3,4,5,6,7]
            var selected = EventSystem.current.currentSelectedGameObject.transform;
            if (selected.parent.name != "Pins") return;
            var selectedObject = selected.gameObject;

            // Check current pin value and change it. (0/1)?
            int pin = int.Parse(selectedObject.GetComponent<Text>().text);
            selectedObject.GetComponent<Text>().text = (1 - pin).ToString();

            // Read values of all pins and calculate the value of the whole byte (HEX)
            int registerValue = 0;
            for (int i = 0; i < 8; i++)
            {
                var isset = int.Parse(selected.parent.GetChild(i).GetComponent<Text>().text);
                registerValue = Bit.setTo(registerValue, 7 - i, isset);
            }
            // Save claculated value
            simulation.Memory.PORTB = (byte)registerValue;
            
            // Print value of RB register (HEX)
            Debug.Log("Inhalt RB-Register: " + simulation.Memory.PORTB.ToString("X"));
            
            //Refresh View
            refreshRamView();
            refreshEEPROMView();
        }
    }
   
    /* loadRamView():
     * loads the RAM view if this function is called. Reads values form memory with the function getRaw(BYTE ADDRESS).
     * Output for debugging reasons.
     * @registerNumStr: Number of each register as a STRING value. Used for Naming each cloned Object right.
     * @registerNumInt: Number of each register as an INT value. Used for addressing the right memory.
     * @registerRow: Template which is cloned for each row in the register view.
     * @registerBlock: Template which is cloned for each block in the register view. Eight blocks per registerRow.
     * @text: Template which is cloned for each block in the register view. Shows register value after refreshing.
     * 
     */
    public void loadRamView()
    {
        string registerNumStr;
        int registerNumInt;
        // for-loop iterates 32 times, one time for each row in the RAM-View
        for (int i = 0; i < 32; i++)
        {
            // Clone registerRow-Template
            var registerRow = Instantiate(registerRowTemplate, ramContainer.transform);
            // Renaing each clone 
            registerRow.name = "Row " + (i * 8).ToString("X");
            // for-loop iterates 8 time per row, cloning and renaming each generated registerBlock
            for (int j = 0; j < 8; j++)
            {
                // Clone registerBlock-Template
                var registerBlock = Instantiate(registerTemplate, registerRow.transform);
                // Formating the generated HEX-Values
                // @if: numbers from 0x00-0x0F
                // @else: numbers from 0x10-0xFF
                if(i*8+j < 16)
                {
                    // Name
                    registerNumStr = "0" + (i * 8 + j).ToString("X");
                    // Value
                    registerNumInt = i * 8 + j;
                }
                else
                {
                    // Name
                    registerNumStr = (i * 8 + j).ToString("X");
                    // Value
                    registerNumInt = i * 8 + j;
                }
                // Rename each block with the above calculated name
                registerBlock.name = "Block: " + registerNumStr;
                // Clone text-template
                var text = Instantiate(registerTextTemplate, registerBlock.transform);
                // Rename eacht text-element with the same name as the block above
                text.name = "Register: " + registerNumStr;
                // Read memory and write the right value to the text-element
                // GameObject.Find(("Register: " + registerNumStr)).GetComponent<Text>().text = (simulation.Memory.getRaw((byte) registerNumInt)).ToString("X2");
            }
        }
    }

    /* refreshRamView:
     * refresh every text box which was genereatet/cloned before
     * does not create any new object
     * can only be used, after RamView was initialized 
     */
    public void refreshRamView()
    {
        // int registerNumInt;
        // string registerNumStr;

        for (int y = 0; y < 32; y++)
        {
            var row = ramContainer.transform.GetChild(y);
            for (int x = 0; x < 8; x++)
            {
                // // Value
                // registerNumInt = i;
                // if (i < 16)
                // {
                //     // Name
                //     registerNumStr = "0" + registerNumInt.ToString("X");
                // }
                // else
                // {
                //     // Name
                //     registerNumStr = registerNumInt.ToString("X");
                // }

                // // Read memory and write the right value to the text-element
                // GameObject.Find(("Register: " + registerNumStr)).GetComponent<Text>().text = (simulation.Memory.getRaw((byte)registerNumInt)).ToString("X2");


                var block = row.GetChild(x);
                block.GetComponentInChildren<Text>().text = simulation.Memory.getRaw((byte) (y * 8 + x)).ToString("X2");
            }
        }
    }
    /* loadEEPROMView():
     * loads the RAM view if this function is called. Reads values form memory with the function getRaw(BYTE ADDRESS).
     * Output for debugging reasons.
     * @registerNumStr: Number of each register as a STRING value. Used for Naming each cloned Object right.
     * @registerNumInt: Number of each register as an INT value. Used for addressing the right memory.
     * @registerRow: Template which is cloned for each row in the register view.
     * @registerBlock: Template which is cloned for each block in the register view. Eight blocks per registerRow.
     * @text: Template which is cloned for each block in the register view. Shows register value after refreshing.
     * 
     */
    public void loadEEPROMView()
    {
        string registerNumStr;
        int registerNumInt;
        // for-loop iterates 32 times, one time for each row in the RAM-View
        for (int i = 0; i < 8; i++)
        {
            // Clone registerRow-Template
            var registerRow = Instantiate(registerRowTemplate, EEPROMContainer.transform);
            // Renaing each clone 
            registerRow.name = "Row " + (i * 8).ToString("X");
            // for-loop iterates 8 time per row, cloning and renaming each generated registerBlock
            for (int j = 0; j < 8; j++)
            {
                // Clone registerBlock-Template
                var registerBlock = Instantiate(registerTemplate, registerRow.transform);
                // Formating the generated HEX-Values
                // @if: numbers from 0x00-0x0F
                // @else: numbers from 0x10-0xFF
                if (i * 8 + j < 16)
                {
                    // Name
                    registerNumStr = "0" + (i * 8 + j).ToString("X");
                    // Value
                    registerNumInt = i * 8 + j;
                }
                else
                {
                    // Name
                    registerNumStr = (i * 8 + j).ToString("X");
                    // Value
                    registerNumInt = i * 8 + j;
                }
                // Rename each block with the above calculated name
                registerBlock.name = "Block: " + registerNumStr;
                // Clone text-template
                var text = Instantiate(registerTextTemplate, registerBlock.transform);
                // Rename eacht text-element with the same name as the block above
                text.name = "Register: " + registerNumStr;
                // Read memory and write the right value to the text-element
                // GameObject.Find(("Register: " + registerNumStr)).GetComponent<Text>().text = (simulation.Memory.getRaw((byte) registerNumInt)).ToString("X2");
            }
        }
    }

    /* refreshEEPROMView:
     * refresh every text box which was genereatet/cloned before
     * does not create any new object
     * can only be used, after RamView was initialized 
     */
    public void refreshEEPROMView()
    {
        for (int y = 0; y < 8; y++)
        {
            var row = EEPROMContainer.transform.GetChild(y);
            for (int x = 0; x < 8; x++)
            {
                var block = row.GetChild(x);
                // block.GetComponentInChildren<Text>().text = (simulation.Memory.readEEPROM()).ToString("X2");
                block.GetComponentInChildren<Text>().text = simulation.Memory.getEEPROM((byte)(y * 8 + x)).ToString("X2"); ;
            }
        }
    }
    public void onSpeedChanged()
    {
        Time.timeScale = speedSlider.value;
    }

    public void onWatchdogToggled()
    {
        if (simulation != null)
        {
            simulation.Memory.WatchdogActive = GameObject.Find("Watchdog")?.GetComponentInChildren<Toggle>()?.isOn ?? true;
        }
    }
}

