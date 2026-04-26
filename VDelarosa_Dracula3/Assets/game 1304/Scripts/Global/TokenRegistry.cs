using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class tokenCondition
{
    public string tokenName;
    public comparisonOperator comparisonOp;
    public int value;
    public tokenCondition(string name, comparisonOperator op, int val)
    {
        tokenName = name;
        comparisonOp = op;
        value = val;
    }
}

public static class TokenRegistry
{

    public static Dictionary<string, int> tokens;
    private static List<TokenUpdatePackage> tokenUpdates;

    private static void init()
    {
        if (tokens == null)
            tokens = new Dictionary<string, int>();
        if (tokenUpdates == null)
            tokenUpdates = new List<TokenUpdatePackage>();
    }
    public static int getToken(string tokenName)
    {
        init();
        if (tokens.ContainsKey(tokenName))
            return (tokens[tokenName]);
        else
        {
            setToken(tokenName, 0); //set it to a default value, in this case 0
            return 0;
        }
            
    }

    public static void modifyToken(TokenOperation to)
    {
        modifyToken(to.tokenName, to.amount, to.operationtype);
    }

    public static void modifyToken(string tokenName, int value, operationType opType)
    {
        if (!tokens.ContainsKey(tokenName))
        {
            Debug.LogError("Token not found in registry");
            return;
        }
        if (opType == operationType.set)
        {
            setToken(tokenName, value);
            return;
        }
        int oldValue;
        init();
        if (tokens.ContainsKey(tokenName))
        {
            oldValue = tokens[tokenName];
            switch (opType)
            {
                case operationType.add:                    
                    tokens[tokenName] += value;
                    break;
                case operationType.divide:
                    tokens[tokenName] /= value;
                    break;
                case operationType.multiply:
                    tokens[tokenName] *= value;
                    break;                
                case operationType.subtract:
                    tokens[tokenName] -= value;
                    break;
            }
            Debug.Log("Token: " + tokenName + " changed from " + oldValue + " to" + tokens[tokenName]);
        }
        CheckListeners(tokenName, tokens[tokenName]);
    }

    public static void setToken(string tokenName, int tokenValue)
    {
        init();
        if (tokens.ContainsKey(tokenName))
        {
            tokens[tokenName] = tokenValue;
        }
        else
        {
            tokens.Add(tokenName, tokenValue);
        }
        CheckListeners(tokenName, tokenValue);
    }

    public static bool testToken(string tokenName, comparisonOperator op, int value)
    {
        return testToken(new tokenCondition(tokenName, op, value));
    }

    public static bool testToken(tokenCondition condition)
    {
        init();
        switch (condition.comparisonOp)
        {
            case comparisonOperator.Equal:
                if (TokenRegistry.getToken(condition.tokenName) == condition.value)
                    return true;
                break;
            case comparisonOperator.greaterThan:
                if (TokenRegistry.getToken(condition.tokenName) > condition.value)
                    return true;
                break;
            case comparisonOperator.greaterThanEqual:
                if (TokenRegistry.getToken(condition.tokenName) >= condition.value)
                    return true;
                break;
            case comparisonOperator.lessThan:
                if (TokenRegistry.getToken(condition.tokenName) < condition.value)
                    return true;
                break;
            case comparisonOperator.lessThanEqual:
                if (TokenRegistry.getToken(condition.tokenName) <= condition.value)
                    return true;
                break;
            case comparisonOperator.notEqual:
                if (TokenRegistry.getToken(condition.tokenName) != condition.value)
                    return true;
                break;
        }
        return false;
    }

    public static void AddListener(TokenUpdatePackage tup)
    {
        init();
        tokenUpdates.Add(tup);
    }

    public static void CheckListeners(string tokenName, int tokenValue)
    {
        bool checksOut;
        foreach(TokenUpdatePackage tup in tokenUpdates)
        {
            if(tup.tokenName == tokenName)
            {
                checksOut = false;
                switch(tup.comparison)
                {
                    case comparisonOperator.Equal:
                        if (tokenValue == tup.value)
                            checksOut = true;
                        break;
                    case comparisonOperator.greaterThan:
                        if (tokenValue > tup.value)
                            checksOut = true;
                        break;
                    case comparisonOperator.greaterThanEqual:
                        if (tokenValue >= tup.value)
                            checksOut = true;
                        break;
                    case comparisonOperator.lessThan:
                        if (tokenValue < tup.value)
                            checksOut = true;
                        break;
                    case comparisonOperator.lessThanEqual:
                        if (tokenValue <= tup.value)
                            checksOut = true;
                        break;
                    case comparisonOperator.notEqual:
                        if (tokenValue != tup.value)
                            checksOut = true;
                        break;
                }
                if (checksOut)
                {
                    foreach (EventPackage ep in tup.eventsToSend)
                    {
                        EventRegistry.SendEvent(ep, null);
                    }
                }
            }
        }
    }
}
