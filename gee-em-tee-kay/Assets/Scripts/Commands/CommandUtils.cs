using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Apply this attribute to methods in your scripts to expose
/// them to Yarn.

/** For example:
    *  [YarnCommand("dosomething")]
    *      void Foo() {
    *         do something!
    *      }
    */
public class CommandAttribute : System.Attribute
{
    public string[] commandStrings { get; private set; }

    public CommandAttribute(params string[] commandString) 
    {
        this.commandStrings = commandString;
    }
}

public static class CommandUtils
{
    public static string FillInTags(string input)
    {
        bool isListening = false;
        string builtString = "";
        string finalString = "";
        foreach (char c in input)
        {
            if (!isListening)
            {
                if (c == '{')
                {
                    isListening = true;
                }
                else
                {
                    finalString += c;
                }
            }
            else if (isListening)
            {
                if (c == '}')
                {
                    if (builtString.Length > 0 && builtString[0] == '$')
                    {
                        // we've found a tag!
                        finalString += Global.flagManager.GetFlag(builtString);
                    }
                    else
                    {
                        finalString += '{' + builtString + '}';
                    }

                    builtString = "";
                    isListening = false;
                }
                else
                {
                    builtString += c;
                }
            }
        }

        return finalString;
    }

    /// commands that can be automatically dispatched look like this:
    /// COMMANDNAME OBJECTNAME <param> <param> <param> ...
    /** We can dispatch this command if:
        * 1. it has at least 2 words
        * 2. the second word is the name of an object
        * 3. that object has components that have methods with the YarnCommand attribute that have the correct commandString set
        */
    public static IEnumerator Dispatch(string command, MonoBehaviour mono) 
    {
        // Trim command
        command = command.Trim();
        
        var words = command.Split(' ');

        bool requiredDispatch = false;
        string commandName;

        if (words.Length > 0)
        {
            commandName = words[0].ToLower();
            if (commandName == "wait") // WAIT COMMAND
            {
                if (words.Length > 1)
                {
                    float waitTime;

                    if (float.TryParse(words[1], out waitTime))
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                }
                else
                {
                    yield return null;
                }
            }
            else if (commandName == "wait_for_flag")
            {
                if (words.Length == 2)
                {
                    while (!Global.flagManager.GetFlag(words[1].ToLower()))
                    {
                        yield return null;
                    }
                }
            }
            else if (commandName == "set_flag")
            {
                words = command.Split(new char[]{' '}, 3);
                if (words.Length == 2)
                {
                    Global.flagManager.SetFlag(words[1].ToLower(), true);
                }

                if (words.Length == 3)
                {
                    bool bvalue = false;
                    float fvalue = -1f;
                    if (bool.TryParse(words[2], out bvalue))
                    {
                        Global.flagManager.SetFlag(words[1].ToLower(), bvalue);
                    }
                    else if (float.TryParse(words[2], out fvalue))
                    {
                        Global.flagManager.SetFlag(words[1].ToLower(), fvalue);
                    }
                    else
                    {
                        Global.flagManager.SetFlag(words[1].ToLower(), words[2]);   
                    }
                }
            }
            else if (commandName == "start_dialogue")
            {
                if (words.Length == 2)
                {
                    Global.dialogueHandler.StartDialogue(words[1]);
                }
            }
            else
            {
                requiredDispatch = true;
            }
        }

        if (!requiredDispatch)
        {
            Debug.Log("COMMAND: performed command: '" + command + "'.");
            yield break;
        }

        // need 2 parameters in order to have both a command name
        // and the name of an object to find
        if (words.Length < 2)
        {
            Debug.LogError("COMMAND: not enough parameters to perform command: " + command);
            yield break;
        }

        commandName = words[1];

        var objectName = words[0];

        if (objectName.ToLower() == "me")
        {
            objectName = mono.gameObject.name;
        }

        var sceneObject = GameObject.Find(objectName);
        if (sceneObject == null)
        {
            Debug.LogError("COMMAND: unable to find object: " + objectName);
            yield break;
        }

        if (commandName[0] == '#')
        {
            CommandShortcutComponent csc = sceneObject.GetComponent<CommandShortcutComponent>();
            if (csc == null)
            {
                yield break;
            }

            string[] lines = csc.GetShortcut(commandName);

            if (lines != null)
            {
                foreach (string line in lines)
                {
                    yield return mono.StartCoroutine(Dispatch(line, csc));
                }
            }

            yield break;
        }

        List<string[]> errorValues = new List<string[]>();

        List<object> parameters = new List<object>();;

        for (int i = 2; i < words.Length; ++i)
        {
            float output = -1f;
            if (float.TryParse(words[i], out output))
            {
                parameters.Add(output);
            }
            else
            {
                parameters.Add(words[i]);
            }
        }

        bool wasFound = false;

        // Find every MonoBehaviour (or subclass) on the object
        foreach (var component in sceneObject.GetComponents<MonoBehaviour>()) 
        {
            if (component == null)
            {
                Debug.Log("COMMAND: component null: " + sceneObject.name);
                continue;
            }

            var type = component.GetType();

            // Find every method in this component
            foreach (var method in type.GetMethods())
            {
                // Find the YarnCommand attributes on this method
                var attributes = (CommandAttribute[]) method.GetCustomAttributes(typeof(CommandAttribute), true);

                // Find the YarnCommand whose commandString is equal to the command name
                foreach (var attribute in attributes) 
                {
                    foreach (var commandNameString in attribute.commandStrings)
                    {
                        if (commandNameString == commandName) 
                        {
                            var methodParameters = method.GetParameters();
                            bool paramsMatch = false;
                            // Check if this is a params array
                            if (methodParameters.Length == 1 && methodParameters[0].ParameterType.IsAssignableFrom(typeof(string[])))
                            {
                                    // // Cool, we can send the command!
                                    // string[][] paramWrapper = new string[1][];
                                    // paramWrapper[0] = parameters.ToArray();
                                    // method.Invoke(component, paramWrapper);
                                    // paramsMatch = true;
                                    // wasFound = true;
                            }
                            // Otherwise, verify that this method has the right number of parameters
                            else if (methodParameters.Length == parameters.Count)
                            {
                                paramsMatch = true;

                                for (int i = 0; i < methodParameters.Length; ++i)
                                {
                                    if (!methodParameters[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                                    {
                                        paramsMatch = false;
                                        break;
                                    }
                                }

                                if (paramsMatch)
                                {
                                    // Cool, we can send the command!
                                    method.Invoke(component, parameters.ToArray());
                                    wasFound = true;
                                }
                            }

                            //parameters are invalid, but name matches.
                            if (!paramsMatch)
                            {
                                //save this error in case a matching command is never found.
                                errorValues.Add(new string[] { method.Name, commandName, methodParameters.Length.ToString(), parameters.Count.ToString() });
                            }
                        }
                    }

                    if (wasFound)
                    {   
                        break;
                    }
                }
                if (wasFound)
                {
                    break;
                }
            }
            if (wasFound)
            {   
                break;
            }
        }
        
        if (wasFound)
        {
            Debug.Log("COMMAND: " + objectName + " performed command: '" + command.Split(new char[]{' '}, 2)[1] + "'.");
        }
        else
        {
            Debug.LogError("COMMAND FAILED: " + objectName + " failed to perform command: " + command.Split(new char[]{' '}, 2)[1] + "'.");
        }

        yield break;
    }
}