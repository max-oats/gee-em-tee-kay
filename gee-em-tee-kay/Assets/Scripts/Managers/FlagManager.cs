using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    [SerializeField] private string globalListName = "global";
    [SerializeField] private string tempListName = "temp";
    [HideInInspector, SerializeField] private List<FlagList> flagLists = new List<FlagList>();

    public bool Evaluate(string conditionString)
    {
        /** Always true if no condition */
        if (conditionString.Length == 0)
        {
            return true;
        }

        Conditional condition = new Conditional(conditionString);

        return condition.IsTrue();
    }

    public bool DoesFlagExist(string flagName)
    {
        FlagList flagList = GetFlagList(ref flagName);

        return (flagList.flags.Find(x => x.name == flagName) != null);
    }

    public void ClearTempFlags()
    {
        int index = GetFlagListIndex(tempListName);
        flagLists[index].Clear();
    }

    /** Global setters */
    public void SetFlag(string flagName, string value)
    {
        GetFlag(flagName).Set(value);
    }

    public void SetFlag(string flagName, bool value)
    {
        GetFlag(flagName).Set(value);
    }

    public void SetFlag(string flagName, int value)
    {
        GetFlag(flagName).Set(value);
    }

    public void SetFlag(string flagName, float value)
    {
        GetFlag(flagName).Set(value);
    }

    public Flag GetFlag(string flagName)
    {
        FlagList flagList = GetFlagList(ref flagName);

        return flagList.GetFlag(flagName);
    }

    private FlagList GetFlagList(ref string flagName)
    {
        // If required, remove the dollar sign
        if (flagName[0] == '$')
        {
            flagName = flagName.Substring(1);
        }

        // Replace all double underscores with dots! :)
        flagName = flagName.Replace("__", ".");

        string fullFlagName = flagName;
        string flagListName = "";
        if (flagName[0] == '_')
        {
            /** Temporary flag */
            flagName = flagName.Substring(1);
            flagListName = tempListName;
        }
        else if (fullFlagName.IndexOf('.') == -1)
        {
            flagListName = globalListName;
        }
        else
        {
            int i = fullFlagName.IndexOf('.');

            flagName = fullFlagName.Substring(i+1);
            flagListName = fullFlagName.Substring(0, i);
        }

        flagListName = flagListName.ToLower();

        FlagList flagList = flagLists.Find(x => x.name == flagListName);
        if (flagList == null)
        {
            flagList = new FlagList(flagListName);
            flagLists.Add(flagList);
        }

        return flagList;
    }

    /** Get the index of the flag list */
    private int GetFlagListIndex(string flagList)
    {
        int index = flagLists.FindIndex(x => x.name == flagList);
        if (index == -1)
        {
            index = flagLists.Count;

            flagLists.Add(new FlagList(flagList));
        }

        return index;
    }

    public List<FlagList> GetFlagLists()
    {
        return flagLists;
    }
}

[System.Serializable]
public class FlagList
{
    public FlagList(string name)
    {
        this.name = name;
    }

    // The name of the object to which the flags are associated
    public string name;

    // The associated list of flags
    public List<Flag> flags = new List<Flag>();

    public void AddFlag(string flagName)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flags.Add(new Flag(flagName, false));
        }
    }

    public void SetFlag(string flagName, bool value)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flags.Add(new Flag(flagName, value));
        }
        else
        {
            flags[flagIndex].asBool = value;
        }
    }

    public void SetFlag(string flagName, int value)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flags.Add(new Flag(flagName, value));
        }
        else
        {
            flags[flagIndex].asInt = value;
        }
    }

    public void SetFlag(string flagName, float value)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flags.Add(new Flag(flagName, value));
        }
        else
        {
            flags[flagIndex].asFloat = value;
        }
    }

    public void SetFlag(string flagName, string value)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flags.Add(new Flag(flagName, value));
        }
        else
        {
            flags[flagIndex].asString = value;
        }
    }

    public Flag GetFlag(string flagName)
    {
        int flagIndex = flags.FindIndex(x => x.name == flagName);
        if (flagIndex == -1)
        {
            flagIndex = flags.Count;
            AddFlag(flagName);
        }
        return GetFlag(flagIndex);
    }

    public Flag GetFlag(int flagIndex)
    {
        if (flagIndex >= 0 && flagIndex < flags.Count)
        {
            return flags[flagIndex];
        }

        return null;
    }

    public void Clear()
    {
        flags?.Clear();
    }
}

public enum FlagType
{
    Bool = 0,
    Int = 1,
    Float = 2,
    String = 3,
}

[System.Serializable]
public class Flag
{
    public Flag(string name, bool value)
    {
        this.name = name;

        Set(value);
    }

    public Flag(string name, int value)
    {
        this.name = name;

        Set(value);
    }

    public Flag(string name, float value)
    {
        this.name = name;

        Set(value);
    }

    public Flag(string name, string value)
    {
        this.name = name;

        Set(value);
    }

    public void Set(bool value)
    {
        asBool = value;
        type = FlagType.Bool;
    }
    
    public void Set(int value)
    {
        asInt = value;
        type = FlagType.Int;
    }
    
    public void Set(float value)
    {
        asFloat = value;
        type = FlagType.Float;
    }

    public void Set(string value)
    {
        asString = value;
        type = FlagType.String;
    }

    public string name = "";
    public bool asBool = false;
    public int asInt = -1;
    public float asFloat = -1f;
    public string asString = "";
    public FlagType type = 0;

    public static implicit operator bool(Flag f) => f.asBool;
    public static implicit operator int(Flag f) => f.asInt;
    public static implicit operator float(Flag f) => f.asFloat;
    public static implicit operator string(Flag f) => f.asString;
}