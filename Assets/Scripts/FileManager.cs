using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

public static class FileManager
{
    public const string PATH = ".\\TestProgramme\\";
    

    // List all files in Program Directory
    public static FileInfo[] listAll()
    {
        var info = new DirectoryInfo(PATH);
        return info.GetFiles().OrderBy(x => x.Name, new NaturalStringComparer()).ToArray();
    }

    // Returns list of all lines in a file
    public static List<string> getLines(string filename)
    {
        var lines = new List<string>();
        var reader = new StreamReader(PATH + filename);
        string line;

        while((line = reader.ReadLine()) != null)
        {
            lines.Add(line);
        }
        return lines;
    }
}
