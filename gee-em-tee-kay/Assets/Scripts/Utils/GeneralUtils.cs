using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeneralUtils : MonoBehaviour 
{
    /**
     * GetAllDescendents
     * - Pulls a list of all children and grandchildren recursively, given a "parent" transform
     */
    public static List<Transform> GetAllDescendents(Transform trunk)
    {
        List<Transform> newList = new List<Transform>();

        foreach (Transform stem in trunk)
        {
            newList.Add(stem);
            newList.AddRange(GetAllDescendents(stem));
        }

        return newList;
    }

    /**
     * Util function to grab lines from txt files
     */
    public static List<string> LoadLinesFromFile(string filename)
    {
        List<string> lines = new List<string>();
        StreamReader sr = new StreamReader(Application.dataPath + filename);

        // Grab lines
        while (sr.Peek() >= 0)
        {
            lines.Add(sr.ReadLine());
        }

        return lines;
    }

    public static List<Color> LoadColoursFromFile(string filename)
    {
        List<Color> colours = new List<Color>();

        List<string> lines = LoadLinesFromFile(filename);

        foreach (string colour in lines)
        {
            Color parsedColour = new Color();

            if (ColorUtility.TryParseHtmlString(colour, out parsedColour))
            {
                colours.Add(parsedColour);
            }
            else
            {
                Debug.LogError("Error while loading '" + filename + "': Colour value '" + colour + "' is not a proper colour code. Will not be added.");
            }
        }

        return colours;
    }

    public static List<float> LoadFloatsFromFile(string filename)
    {
        List<float> floats = new List<float>();

        List<string> lines = LoadLinesFromFile(filename);

        foreach (string line in lines)
        {
            float attemptfloat = 0f;

            if (float.TryParse(line, out attemptfloat))
            {
                floats.Add(attemptfloat);
            }
            else
            {
                Debug.LogError("Error while loading '" + filename + "': Float value '" + line + "' could not be parsed. Will not be added.");
            }
        }

        return floats;
    }
}