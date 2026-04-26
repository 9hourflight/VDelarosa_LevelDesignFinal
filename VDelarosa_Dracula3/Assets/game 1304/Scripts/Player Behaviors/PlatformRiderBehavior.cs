using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRiderBehavior : MonoBehaviour 
{
	private Transform platformTransform = null;
	private Vector3 prePos = Vector3.zero;
	private Vector3 playerPrePos = Vector3.zero;
	private Quaternion preRot;
	GAME1304PlayerController fpsc;
	private bool isRotating = false;
	private bool isTranslating = false;
    private bool isMovering = false;
    private float distanceThreshold = 4.0f;
    Rigidbody playerRB;
    
    MoverBehavior mb;
    RotatingMoverBehavior rmb;
    ConstantRotationBehavior crb;
    AdvancedMoverBehavior amb;
	Rigidbody stoodOnRB;
	ConveyorBehavior tmb;
    //platCheckInfo platInfo;
    RaycastHit hitInfo;
    // Use this for initialization
    void Start () 
	{
		fpsc = gameObject.GetComponent<GAME1304PlayerController>();
        playerRB = fpsc.gameObject.GetComponent<Rigidbody>();

        mb = null;
        rmb = null;
        crb=null;
        amb=null;
		tmb = null;
    }
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		//TODO: move all of this to a MovingPlatform physics layer??
		//RaycastHit hitInfo;
        //platCheckInfo platInfo;

		isRotating = false;
		isTranslating = false;

        if (fpsc != null)
		{
            hitInfo = fpsc.platformCheck(1.05f);
			if ((hitInfo.transform != null )) // &&(!isStatic))
			{
				//if(!hitInfo.transform.gameObject.isStatic)

				//on a basic mover
                if(hitInfo.transform.gameObject.TryGetComponent<MoverBehavior>(out mb))
                { 
					Debug.Log("moving platform hit");
					isTranslating = true;
                    
					if (platformTransform == null)
					{
						platformTransform = hitInfo.transform;
						prePos = platformTransform.transform.position;
						playerPrePos = transform.gameObject.GetComponent<Rigidbody>().transform.position;
					}
				}
				
				//on an advanced mover
                if (hitInfo.transform.gameObject.TryGetComponent<AdvancedMoverBehavior>(out amb))
                {
                    Debug.Log("moving platform hit");
					isTranslating = true;
					if (platformTransform == null)
					{
						platformTransform = hitInfo.transform;
						prePos = platformTransform.transform.position;
						playerPrePos = transform.gameObject.GetComponent<Rigidbody>().transform.position;
					}
				}

				//on a treadmill
				if (hitInfo.transform.gameObject.TryGetComponent<ConveyorBehavior>(out tmb))
				{
					isTranslating = true;
					//if (platformTransform == null)
					{
						platformTransform = hitInfo.transform;
						prePos = platformTransform.transform.position - (platformTransform.forward * tmb.getActualSpeed()*Time.fixedDeltaTime);
						playerPrePos = transform.gameObject.GetComponent<Rigidbody>().transform.position;
					}
				}

				//on a physics object
				if (hitInfo.transform.gameObject.TryGetComponent<Rigidbody>(out stoodOnRB))
				{
					if (!stoodOnRB.isKinematic)
					{
						isTranslating = true;
						if (platformTransform == null)
						{
							platformTransform = hitInfo.transform;
							prePos = platformTransform.transform.position;
							playerPrePos = transform.gameObject.GetComponent<Rigidbody>().transform.position;
						}
					}
				}

				if (hitInfo.transform.gameObject.TryGetComponent<RotatingMoverBehavior>(out rmb))				
				{
					isRotating = true;
					if (platformTransform == null)
					{
						platformTransform = hitInfo.transform;
						preRot = platformTransform.rotation;
					}
				}
				
				if(hitInfo.transform.gameObject.TryGetComponent<ConstantRotationBehavior>(out crb))                
                {
                    isRotating = true;
                    if (platformTransform == null)
                    {
                        platformTransform = hitInfo.transform;
                        preRot = platformTransform.rotation;
                    }
                }

				
			}
			else
			{			
				//Debug.Log("moving platform NOT hit");	
				mb = null;
				rmb = null;
                crb = null;
				amb = null;
				tmb = null;
				stoodOnRB = null;
			}
			
		}
        else
        {
            //Debug.Log("moving platform NOT hit");	
            mb = null;
            rmb = null;
            crb = null;
            amb = null;
			tmb = null;
			stoodOnRB = null;
		}

        if (rmb == null && mb == null && crb == null && amb == null && tmb == null && stoodOnRB == null)
            platformTransform = null;
        /*prePos = platformTransform.position;
        playerPrePos = playerRB.transform.position;
        preRot = platformTransform.rotation;*/
    }


	void LateUpdate()
	{
		float rotDiff;
		Vector3 updateVector;
		

		if (platformTransform != null)
		{
			
			if(isTranslating)				
			{
				//updateVector = playerRB.transform.position - playerPrePos; //new Vector3(0, playerRB.position.y- playerPrePos.y, 0);
				//this.gameObject.transform.position += updateVector;*/
				//updateVector = platformTransform.position - prePos;
				updateVector =new Vector3(0, playerRB.transform.position.y-playerPrePos.y,0);
				if (Vector3.Magnitude(platformTransform.position - prePos) < distanceThreshold)
				{
					this.gameObject.transform.position += (platformTransform.position - prePos);
					//playerRB.MovePosition(playerRB.transform.position + platformTransform.position - prePos);
				}

                /*if ((platformTransform.position - prePos).y != 0)
                    playerRB.MovePosition(playerRB.transform.position + new Vector3(0,-0.01f,0)); // + updateVector);
                */
                /*if(playerRB!=null)
					playerRB.MovePosition(playerRB.transform.position+platformTransform.position - prePos);*/
            }
			if(isRotating)
			{
				rotDiff = (platformTransform.rotation.eulerAngles.y - preRot.eulerAngles.y); //*0.95f;
				this.gameObject.transform.RotateAround(platformTransform.position, Vector3.up, rotDiff);
				fpsc.RotateCamHorizontally(rotDiff);
			}
			prePos = platformTransform.position;
			playerPrePos = playerRB.transform.position;
			preRot = platformTransform.rotation;

		}
	}


}
