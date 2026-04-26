using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OOICategory { threat, desire, ally, curiosity, unknown};

public class ObjectOfInterest : MonoBehaviour
{
    [HideInInspector]
    public float visibility;
    [HideInInspector]
    public bool isAlive = true;
    // Start is called before the first frame update
    void Awake()
    {
        NPCManager.registerOOI(this.gameObject);
    }


}
