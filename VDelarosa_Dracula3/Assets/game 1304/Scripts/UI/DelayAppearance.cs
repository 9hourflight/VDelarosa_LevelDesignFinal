using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAppearance : MonoBehaviour
{
    public float delayTime = 1.5f;
    
    CanvasRenderer r;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        /*if(TryGetComponent<CanvasRenderer>(out r))
        {
            r. .enabled = false;*/
            Invoke("EnableMe", delayTime);
        /*}*/
    }

    void EnableMe()
    {
        gameObject.SetActive(true);
        /*if (r != null)
            r.enabled = true;*/
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
