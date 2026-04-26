using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TokenUpdatePackage
{
    public string tokenName;
    public comparisonOperator comparison;
    public int value;
    public List<EventPackage> eventsToSend;
    /*private string address;
    public void init()
    {
        address = System.Guid.NewGuid().ToString();
    }*/
}

public class TokenUpdateListener : MonoBehaviour
{
    
    public List<TokenUpdatePackage> tokenUpdates;

    void Start()
    {        
        foreach (TokenUpdatePackage tup in tokenUpdates)
        {            
            TokenRegistry.AddListener(tup);
        }
    }


    
}
