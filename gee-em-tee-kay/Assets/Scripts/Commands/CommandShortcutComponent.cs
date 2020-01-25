using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CommandShortcut
{
    public string name;

    public string command;
}

public class CommandShortcutComponent : MonoBehaviour
{
    [HideInInspector, SerializeField] public List<CommandShortcut> shortcuts = new List<CommandShortcut>();

    public string[] GetShortcut(string shortcutName)
    {
        CommandShortcut cs = shortcuts.Find(x => x.name == shortcutName);
        if (cs != null)
        {
            return cs.command.Split('\n');
        }

        return null;
    }
}