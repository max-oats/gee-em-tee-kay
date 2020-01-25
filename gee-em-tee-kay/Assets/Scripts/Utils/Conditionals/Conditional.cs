using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConditionalListWrapper
{
    public bool cancelOutAfterSuccess = false;

    public List<ConditionalInspectorObject> list = new List<ConditionalInspectorObject>();
}

/** Room script class. Used for actor scripting */
[System.Serializable]
public class ConditionalInspectorObject
{
    public string condition = "";

    public string script = "";

    public List<ConditionalInspectorObject> nested = new List<ConditionalInspectorObject>();

    public bool showNested = false;
}

[System.Serializable]
public enum ConditionalOperator
{
    TRUE,
    FALSE,
    AND,
    OR,
}

[System.Serializable]
public class Conditional
{
    private bool isTrue = false;
    private string conditionString;

    private List<List<bool>> outcomes = new List<List<bool>>();
    private List<List<string>> conditions = new List<List<string>>();

    public Conditional(string conditionString)
    {
        outcomes.Add(new List<bool>());
        conditions.Add(new List<string>());

        this.conditionString = conditionString;

        AddNewOutcome(0, false);
        AddNewCondition(0, "");

        ParseConditionString();
    }

    public void AddNewOutcome(int depth, bool value)
    {
        while (depth >= outcomes.Count)
        {
            outcomes.Add(new List<bool>());
        }

        outcomes[depth].Add(value);
    }

    public void AddNewCondition(int depth, string value)
    {
        while (depth >= conditions.Count)
        {
            conditions.Add(new List<string>());
        }

        conditions[depth].Add(value);
    }

    public void AppendCondition(int depth, string value)
    {
        while (depth >= conditions.Count)
        {
            conditions.Add(new List<string>());
        }

        if (conditions[depth].Count == 0)
        {
            conditions[depth].Add("");
        }
        conditions[depth][conditions[depth].Count-1] += value;
    }

    public void AddConditionTag(int depth)
    {
        if (conditions[depth].Count == 0)
        {
            conditions[depth].Add("");
        }

        conditions[depth][conditions[depth].Count-1] += ("[" + GetNoOfConditions(depth+1) + "]");
    }

    public int GetNoOfConditions(int depth)
    {
        while (depth >= conditions.Count)
        {
            conditions.Add(new List<string>());
        }

        return conditions[depth].Count;
    }

    public void ParseConditionString()
    {
        int depth = 0;

        for (int i = 0; i < conditionString.Length; ++i)
        {
            if (conditionString[i].Equals('('))
            {
                /** Add a tag so we know where to replace later */
                AddConditionTag(depth);

                /** Increment depth */
                depth++;

                /** Add new condition */
                AddNewCondition(depth, "");
                AddNewOutcome(depth, false);
            }
            else if (conditionString[i].Equals(')'))
            {
                /** Decrement depth and check */
                depth--;
                if (depth < 0)
                {
                    Debug.LogError("ROOM LOADER: Imbalanced bracket on condition string '" + conditionString + "'");
                }
            }
            else
            { 
                AppendCondition(depth, conditionString[i].ToString());
            }
        }

        for (int i = conditions.Count-1; i >= 0; --i)
        {
            for (int j = 0; j < conditions[i].Count; ++j)
            {
                isTrue = ParseIndividualCondition(i, j, conditions[i][j]);
            }
        }
    }

    public bool ParseIndividualCondition(int depth, int index, string condition)
    {
        string[] words = condition.Split(' ');

        List<ConditionalOperator> operators = new List<ConditionalOperator>();

        foreach (string word in words)
        {
            if (word.Equals("&&"))
            {
                operators.Add(ConditionalOperator.AND);
            }
            else if (word.Equals("||"))
            {
                operators.Add(ConditionalOperator.OR);
            }
            else if (word[0].Equals('['))
            {
                string num = word.Substring(1, word.Length-2);

                int value = -1;
                if (int.TryParse(num, out value))
                {
                    if (outcomes[depth+1][value])
                    {
                        operators.Add(ConditionalOperator.TRUE);
                    }
                    else
                    {
                        operators.Add(ConditionalOperator.FALSE);
                    }
                }
            }
            else
            {
                bool invert = false;
                bool value = false;
                string finalWord = word;

                if (word[0].Equals('!'))
                {
                    invert = true;

                    finalWord = word.Substring(1);
                }

                if (finalWord.Contains("=="))
                {
                    string[] split = finalWord.Split(new string[]{"=="}, System.StringSplitOptions.RemoveEmptyEntries);

                    string flagName = split[0];
                    float flagValue = -1f;
                    int intValue = -1;
                    if (split[1][split[1].Length-1].Equals('f'))
                    {
                        // float
                        if (float.TryParse(split[1], out flagValue))
                        {
                            if (Global.flagManager.GetFlag(flagName) == flagValue)
                            {
                                value = true;
                            }
                        }
                        else
                        {
                            Debug.LogError("Attempted to parse float but failed");
                        }
                    }
                    else if (int.TryParse(split[1], out intValue))
                    {
                        if (Global.flagManager.GetFlag(flagName) == intValue)
                        {
                            value = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("Attempted to parse int but failed.");
                    }
                }
                else if (finalWord.Contains("!="))
                {
                    string[] split = finalWord.Split(new string[]{"!="}, System.StringSplitOptions.RemoveEmptyEntries);

                    string flagName = split[0];
                    float flagValue = -1f;
                    int intValue = -1;
                    if (split[1][split[1].Length-1].Equals('f'))
                    {
                        // float
                        if (float.TryParse(split[1], out flagValue))
                        {
                            if (Global.flagManager.GetFlag(flagName) != flagValue)
                            {
                                value = true;
                            }
                        }
                        else
                        {
                            Debug.LogError("Attempted to parse float but failed");
                        }
                    }
                    else if (int.TryParse(split[1], out intValue))
                    {
                        if (Global.flagManager.GetFlag(flagName) != intValue)
                        {
                            value = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("Attempted to parse int but failed.");
                    }
                }
                else if (finalWord.Contains(">"))
                {
                   string[] split = finalWord.Split(new string[]{">"}, System.StringSplitOptions.RemoveEmptyEntries);

                    string flagName = split[0];
                    float flagValue = -1f;
                    int intValue = -1;
                    if (split[1][split[1].Length-1].Equals('f'))
                    {
                        // float
                        if (float.TryParse(split[1], out flagValue))
                        {
                            if (Global.flagManager.GetFlag(flagName) > flagValue)
                            {
                                value = true;
                            }
                        }
                        else
                        {
                            Debug.LogError("Attempted to parse float but failed");
                        }
                    }
                    else if (int.TryParse(split[1], out intValue))
                    {
                        if (Global.flagManager.GetFlag(flagName) > intValue)
                        {
                            value = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("Attempted to parse int but failed.");
                    }
                }
                else if (finalWord.Contains("<"))
                {
                    string[] split = finalWord.Split(new string[]{"<"}, System.StringSplitOptions.RemoveEmptyEntries);

                    string flagName = split[0];
                    float flagValue = -1f;
                    int intValue = -1;
                    if (split[1][split[1].Length-1].Equals('f'))
                    {
                        // float
                        if (float.TryParse(split[1], out flagValue))
                        {
                            if (Global.flagManager.GetFlag(flagName) < flagValue)
                            {
                                value = true;
                            }
                        }
                        else
                        {
                            Debug.LogError("Attempted to parse float but failed");
                        }
                    }
                    else if (int.TryParse(split[1], out intValue))
                    {
                        if (Global.flagManager.GetFlag(flagName) < intValue)
                        {
                            value = true;
                        }
                    }
                    else
                    {
                        Debug.LogError("Attempted to parse int but failed.");
                    }
                }
                else
                {
                    if (Global.flagManager.GetFlag(finalWord))
                    {
                        value = true;
                    }
                }

                if (invert)
                {
                    value = !value;
                }

                if (value)
                {
                    operators.Add(ConditionalOperator.TRUE);
                }
                else
                {
                    operators.Add(ConditionalOperator.FALSE);
                }
            }
        }

        bool isTrue = false;
        for (int i = 0; i < operators.Count; ++i)
        {
            if (i == 0)
            {
                if (operators[i] == ConditionalOperator.TRUE)
                {
                    isTrue = true;
                }
                else if (operators[i] == ConditionalOperator.FALSE)
                {
                    isTrue = false;
                }
            }
            else
            {
                if (operators[i] == ConditionalOperator.AND)
                {
                    bool rightSide = false;
                    if (operators[i+1] == ConditionalOperator.TRUE)
                    {
                        rightSide = true;
                    }

                    isTrue = isTrue && rightSide;
                }
                else if (operators[i] == ConditionalOperator.OR)
                {
                    bool rightSide = false;
                    if (operators[i+1] == ConditionalOperator.TRUE)
                    {
                        rightSide = true;
                    }

                    isTrue = isTrue || rightSide;
                }
            }
        }

        outcomes[depth][index] = isTrue;

        return isTrue;
    }

    public bool IsTrue()
    {
        return isTrue;
    }
}
