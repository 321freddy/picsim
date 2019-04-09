using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GUIController : MonoBehaviour
{
    public Dropdown fileDropdown;
    public GameObject codeContainer;

    public GameObject codeLineTemplate;

    // Start is called before the first frame update
    void Start()
    {
        // Populate file dropdown
        foreach (var file in FileManager.listAll())
        {
            fileDropdown.options.Add(new Dropdown.OptionData(file.Name));
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Find("wRegister").GetComponent<Text>().text ="W-Register: " + Memory.w_Register.ToString("X");
        GameObject.Find("ZeroFlag").GetComponent<Text>().text ="Zero-Flag: " + Memory.zero_Flag.ToString();
        if(Memory.p_Counter == 0)
        {
            GameObject.Find("ProgCounter").GetComponent<Text>().text ="PCL: " + Memory.p_Counter.ToString();
        }
        else
        {
            GameObject.Find("ProgCounter").GetComponent<Text>().text ="PCL: " + (Memory.p_Counter - 1).ToString();
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

        Program.Parse(lines);
    }

    public void onBtnClick()
    {
        Debug.Log("clicked button!!");
    }
}
