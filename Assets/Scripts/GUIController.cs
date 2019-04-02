using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public const string PATH = ".\\TestProgramme\\";

    public Dropdown fileDropdown;


    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        var info = new DirectoryInfo(PATH);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            fileDropdown.options.Add(new Dropdown.OptionData(file.Name));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("test log 123");
    }

    public void onFileSelected()
    {
        var name = fileDropdown.options[fileDropdown.value].text;
        Debug.Log("selected file " + name);
    }

    public void onBtnClick()
    {
        Debug.Log("clicked button!!");
    }
}
