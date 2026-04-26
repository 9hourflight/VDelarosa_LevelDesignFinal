using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletBehavior : MonoBehaviour 
{

	public int damageAmount = 100;
	public int speed = 10;
	public int maxRange = 50;
	private Rigidbody bulletRB;
	private Vector3 startPosition;
    public bool hurtsEnemies = true;
    public bool hurtsPlayer = true;
    public float lifetime = 0f;
    public bool destroyOnCollision = true;
    public bool destroyOnDamage = true;
    private float age = 0f;
    public signalTypes damageType = signalTypes.bullet;

	void Start () 
	{
        age = 0f;
	}

	public void init(Vector3 heading)
	{
		bulletRB = GetComponent<Rigidbody>();
		startPosition = transform.position;
		transform.forward = heading;
		if(bulletRB != null)
			bulletRB.velocity = transform.forward * speed;
	}

    private void Update()
    {
        age += Time.deltaTime;
        if ((lifetime>0)&&(age>lifetime))
            GameObject.Destroy(gameObject);
    }
    void FixedUpdate () 
	{
		//if(bulletRB != null)
		//	bulletRB.velocity = transform.forward * speed;
			//bulletRB.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
		if(Vector3.Distance(transform.position, startPosition) > maxRange)
			GameObject.Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		NPCBehavior eb;
        GAME1304PlayerController pb;
        SignalReceiver sr;
		if(!collision.collider.isTrigger)
		{
            //HelperFunctions.sendSignal(collision.gameObject, damageAmount, damageType);
            if(hurtsEnemies)
            { 
			    eb = collision.gameObject.GetComponent<NPCBehavior>();
                if (eb != null)
                {
                    if (eb.isAlive)
                    {
                        eb.Damage(damageAmount, damageType);
                        if(destroyOnDamage)
                            GameObject.Destroy(gameObject);
                    }
                }
            }
            if (hurtsPlayer)
            {
                pb = collision.gameObject.GetComponent<GAME1304PlayerController>();
                if (pb != null)
                {
                    pb.takeDamage(damageAmount, damageType);
                    if (destroyOnDamage)
                        GameObject.Destroy(gameObject);
                }
            }

            sr = collision.gameObject.GetComponent<SignalReceiver>();
            if (sr != null)
            {
                sr.processSignal(damageType, damageAmount);
                if (destroyOnDamage)
                    GameObject.Destroy(gameObject);
            }

            

            BreakableObject bo = collision.collider.gameObject.GetComponent<BreakableObject>();
            if (bo != null)
                bo.Damage(damageAmount, damageType, -collision.relativeVelocity);

            if (destroyOnCollision)
                GameObject.Destroy(gameObject);
		}
	}
    private void OnTriggerEnter(Collider other)    
    {
        NPCBehavior eb;
        GAME1304PlayerController pb;
        SignalReceiver sr;
        if (!other.isTrigger)
        {
            //HelperFunctions.sendSignal(collision.gameObject, damageAmount, damageType);
            if (hurtsEnemies)
            {
                eb = other.gameObject.GetComponent<NPCBehavior>();
                if (eb != null)
                {
                    if (eb.isAlive)
                    {
                        eb.Damage(damageAmount, damageType);
                        if (destroyOnDamage)
                            GameObject.Destroy(gameObject);
                    }
                }
            }
            if (hurtsPlayer)
            {
                pb = other.gameObject.GetComponent<GAME1304PlayerController>();
                if (pb != null)
                {
                    pb.takeDamage(damageAmount, damageType);
                    if (destroyOnDamage)
                        GameObject.Destroy(gameObject);
                }
            }

            sr = other.gameObject.GetComponent<SignalReceiver>();
            if (sr != null)
            {
                sr.processSignal(damageType, damageAmount);
                if (destroyOnDamage)
                    GameObject.Destroy(gameObject);
            }



            BreakableObject bo = other.gameObject.GetComponent<BreakableObject>();
            if (bo != null)
                bo.Damage(damageAmount, damageType, transform.forward);

            if (destroyOnCollision)
                GameObject.Destroy(gameObject);
        }
    }
}
