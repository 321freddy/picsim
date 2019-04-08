using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PicSimulator.Model
{
    public static class FileHandler
    {
        /// <summary>
        /// Liest alle Zeilen der LST Datei aus
        /// </summary>
        /// <param name="pathToFile">Den Pfad zur LST Datei</param>
        /// <returns>Gibt eine String Liste zurück, jede Zeile der Datei in einem String</returns>
        public static List<string> GetStringListFromPathToLstFile(string pathToFile)
        {
            var allFileLines = File.ReadAllLines(pathToFile);

            var onlyImportantFileInfos = GetOnlyImportantInfosFromFile(allFileLines);

            return allFileLines.ToList();

        }

        /// <summary>
        /// Schneidet alle irrelevanten Infos der File weg
        /// </summary>
        /// <param name="allFileLines">alle Lines der File </param>
        /// <returns>Ein Dictionary mit Programmzähler, Befehl</returns>
        private static Dictionary<int, string> GetOnlyImportantInfosFromFile(string[] allFileLines)
        {
            var program = new Dictionary<int, string>();

            foreach (var stringLine in allFileLines)
            {
                if (!string.IsNullOrWhiteSpace(stringLine.Substring(0, 8)))
                {
                    var programCounter = stringLine.Substring(0, 4);
                    try
                    {
                        var counter = int.Parse(programCounter);
                        var command = stringLine.Substring(5, 4);
                        program.Add(counter, command);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            return program;
        }
    }
}
