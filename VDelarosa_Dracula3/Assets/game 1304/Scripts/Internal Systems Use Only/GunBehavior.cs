using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunBehavior : WeaponBehavior
{
    
	public GameObject barrelObject;
	public GameObject bulletPrefab;
	//public float cooldown = 0.5f;
	public bool isAutomatic = false;
    public bool requiresAmmunition = true;
    public string ammoType = "bullet";
    public int startingAmmoCount = 10;
    // Use this for initialization

    private Vector3 targetPoint;
    
    private Vector3 sourcePoint;
    private Vector3 a = new Vector3();
    private Vector3 b = new Vector3();
    private void Start()
    {
        targetPoint = new Vector3();
        sourcePoint = new Vector3();
        
    }
    public override void Fire()
	{
        
        Vector3 headingAngle;
        
        if (!isCooling)
        {
            base.Fire();
            if (GameManager.player.GetComponent<GAME1304PlayerController>().ConsumeAmmo(ammoType)||(!requiresAmmunition))
            {
                targetPoint = GameManager.playerController.GetCamTraceTarget();
                sourcePoint = barrelObject.transform.position;



                Vector3 axis = Vector3.Cross(sourcePoint, targetPoint);
                float angle = Vector3.Angle(sourcePoint, targetPoint);

                //headingAngle = (Quaternion.AngleAxis(angle,axis)).eulerAngles;
                headingAngle = (targetPoint - sourcePoint).normalized;

                GameObject bulletObj = GameObject.Instantiate(bulletPrefab, barrelObject.transform);
                bulletObj.transform.parent = null;

                if (bulletObj.GetComponent<BulletBehavior>() != null)
                    bulletObj.GetComponent<BulletBehavior>().init(headingAngle);
                a = barrelObject.transform.position;
                b = barrelObject.transform.position + headingAngle * 10;
                isCooling = true;
                cooldownTimer = cooldown; //TODO: Make this take place in the parent so it's not at risk for individual interpretation
            }
        }
		//bulletPrefab
	}
    public void Update()
    {
        base.Update();        
    }
}