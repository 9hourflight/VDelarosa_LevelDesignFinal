using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDamageSource : MonoBehaviour
{
    public float minimumVelocityForDamage = 0f;
    public bool onlyDamageIfThisObjectIsMoving = true;
    public bool damageStartsEnabled = true;
    private bool _damageEnabled;
    public float damageMultiplier = 1f;

    [Header("Event Listening")]
    public string enableDamage;
    public string disableDamage;
    public string toggleDamage;    

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _damageEnabled = damageStartsEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_damageEnabled)
            return;

        GAME1304PlayerController pc = null;
        NPCBehavior eb = null;
        BreakableObject bo = null;
        Rigidbody rb = GetComponent<Rigidbody>();
        float impactForce = 0f;
        if(collision.gameObject != null)
        {
            pc = collision.gameObject.GetComponent<GAME1304PlayerController>();
            eb = collision.gameObject.GetComponent<NPCBehavior>();
            bo = collision.gameObject.GetComponent<BreakableObject>();
            impactForce = collision.relativeVelocity.magnitude;
        }
        if (rb != null)
        {
            if (onlyDamageIfThisObjectIsMoving && rb.velocity.magnitude == 0)
            {
                //kind of a kludge to avoid preemptively returning out of this function
                impactForce = 0;
            }
        }

        if (impactForce >= minimumVelocityForDamage)
        {
            if (eb != null)
                eb.Damage((int)(impactForce * _rb.mass * damageMultiplier), signalTypes.physics);
            if (pc != null)
                pc.takeDamage((int)(impactForce * _rb.mass * damageMultiplier), signalTypes.physics);
            if (bo != null)
                bo.Damage((int)(impactForce * _rb.mass * damageMultiplier), signalTypes.physics,rb.velocity);
        }

    }
}
