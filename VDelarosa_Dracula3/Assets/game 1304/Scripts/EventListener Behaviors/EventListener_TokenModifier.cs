using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TokenOperation
{
    public string tokenName;
    public operationType operationtype;
    public int amount;
}

public class EventListener_TokenModifier : MonoBehaviour
{
    public List<string> eventsToListenFor;
    public List<TokenOperation> tokenOperations;

    void Start()
    {
        if (eventsToListenFor.Count > 0)
        {
            foreach (string s in eventsToListenFor)
            {
                if (s != "")
                    EventRegistry.AddEvent(s, modifyToken, gameObject);
            }
        }
    }

    void modifyToken(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        int tempTokenVal;
        foreach(TokenOperation to in tokenOperations)
        {
            tempTokenVal = TokenRegistry.getToken(to.tokenName);
            switch (to.operationtype)
            {
                case operationType.add:
                    TokenRegistry.setToken(to.tokenName, tempTokenVal + to.amount);
                    break;
                case operationType.divide:
                    TokenRegistry.setToken(to.tokenName, tempTokenVal / to.amount);
                    break;
                case operationType.multiply:
                    TokenRegistry.setToken(to.tokenName, tempTokenVal * to.amount);
                    break;
                case operationType.set:
                    TokenRegistry.setToken(to.tokenName, to.amount);
                    break;
                case operationType.subtract:
                    TokenRegistry.setToken(to.tokenName, tempTokenVal - to.amount);
                    break;
            }
        }
    }
}
