using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

public class DialogueVariableStorage : VariableStorageBehaviour
{
    /// Reset to our default values when the game starts
    void Awake ()
    {
        ResetToDefaults ();
    }

    /// Erase all variables and reset to default values
    public override void ResetToDefaults ()
    {
        /** */
    }

    /// Set a variable's value
    public override void SetValue (string variableName, Yarn.Value value)
    {
        if (value.type == Yarn.Value.Type.Bool)
        {
            Global.flagManager.SetFlag(variableName, value.AsBool);
        }
        else if (value.type == Yarn.Value.Type.Number)
        {
            Global.flagManager.SetFlag(variableName, value.AsNumber);
        }
        else
        {
            Global.flagManager.SetFlag(variableName, value.AsString);
        }
    }

    /// Get a variable's value
    public override Yarn.Value GetValue (string variableName)
    {
        Flag flag = Global.flagManager.GetFlag(variableName);
        
        if (flag.type == FlagType.Bool)
        {
            return new Yarn.Value(flag.asBool);
        }
        else if (flag.type == FlagType.Int)
        {
            return new Yarn.Value(flag.asInt);
        }
        else if (flag.type == FlagType.Float)
        {
            return new Yarn.Value(flag.asFloat);
        }
        else
        {
            return new Yarn.Value(flag.asString);
        }
    }

}
