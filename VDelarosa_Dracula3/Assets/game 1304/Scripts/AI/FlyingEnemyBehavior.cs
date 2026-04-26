using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class FlyingEnemyBehavior : NPCBehavior
{
	[Header("Mortality")]
	/*public GameObject objectToDrop;
	public int maxHealth = 100;
	
	public bool ragdollOnDeath = true;
	public List<string> eventsToFireOnDeath;
    public bool isAlive
    {
        get
        {
            return (_currentHealth > 0);
        }
    }*/
    private int _currentHealth;
    /*[Space(5)]
	[Header("Movement")]
	public float patrolSpeed = 5.0f;
	public float investigateSpeed = 7.0f;
	public float attackingSpeed = 10.0f;
	public float fleeingSpeed = 10.0f;*/

	/*[Space(5)]
	[Header("Behavior")]	
	public BehaviorType startingBehavior;
	public BehaviorType currentBehavior { get { return _currentBehavior; } }
	[Tooltip("Events that will change this NPC's behavior")]
	public List<behaviorChangeEventEntry> behaviorChangeEvents;*/
	//new public Attitude attitudeTowardsPlayer = Attitude.neutral;
	/*[Tooltip("Events that will change how this NPC reacts to the player")]
	public List<attitudeChangeEventEntry> attitudeChangeEvents;*/
	

	/*public GameObject followFleeTarget;
	public float followDistance = 5.0f;
	public List<followTargetChangeEventEntry> followDifferentTargetEvents;
    [Tooltip("Nav container object that contains this character's patrol information")]
	public NavPointContainer navContainerObject;*/
	private List<navPoint> navPoints;
    private BehaviorType _currentBehavior, _previousBehavior, _behaviorLastFrame;
    private Vector3 fleeDestination;
	private int currentPointIndex;
	private int patrolEventsToFireIndex;
	private int pointCount;
	private navBehavior patrolType;
	private int pingPongDir = 1;
	private float patrolWaitTimer = 0f;
    private float patrolWaitDuration = 0f;
    //public List<healthThresholdData> healthLevelChanges;

    /*[Space(5)]
    [Header("Attacking")]
    public NPCAttackType attackType = NPCAttackType.touch;
    public GameObject bulletPrefab;
    [Tooltip("The time between acquiring a target and attacking. Min-max range.")]
    public int attackWindupMax;
    [Tooltip("The time between acquiring a target and attacking. Min-max range.")]
    public int attackWindupMin;
    [Tooltip("The time between attacks. Min-max range.")]
    public int attackcoolDownMin;
    [Tooltip("The time between attacks. Min-max range.")]
    public int attackcoolDownMax;
    [Tooltip("Only used if attack mode is ranged. Set to 0 for unlimited range.")]
    public float rangedAttackMaxDistance = 10f;
    [Tooltip("Only used if attack mode is ranged.")]
    public float rangedAttackMinDistance = 0f;    
    public AudioClip rangedAttackSound;
    public AudioClip meleedAttackSound;*/
    private float currentAttackCooldown = 0f;

    /*[Space(5)]
	[Header("Suspicion")]
	public float awarenessMeterUnawareToInvestigate = 5.0f;
	public float awarenessMeterInvestigateToBusted = 7.5f;
	public float idleSenseRate = 5.0f;
	public float suspiciousSenseRate = 10.0f;
    public float veryNearSenseMultiplier = 5f;
    public float nearSenseMultiplier = 2f;
    public float farSenseMultiplier = 0.75f;
    public float veryFarSenseMultiplier = 0.5f;
	public float senseBleedOffRate = 1.0f;	
	public float maximumSuspicion = 15f;
	[Tooltip("This means that the AI will always know where the player is and won't lose track.")]
	public bool hasPsychicAwarenessOfPlayer = false;
	public AudioClip soundToPlayOnBusted;	
	public GameObject suspicionIndicator;
	public GameObject alertedIndicator;*/
    private float currentSuspicion = 0;
    private AudioSource enemyAudioSource;

/*  [Space(5)]
    [Header("Senses")]
    public float patrolFOV = 100.0f;
    public float investigateFOV = 110.0f;
    public float pursuitFOV = 130.0f;
    //TODO: add peripheral cone    
    public bool playerInSight { get { return _playerInSight; } }
    public Vector3 personalLastSighting;
    public List<visionRangeChangeEventEntry> visionRadiusChangeEvents;*/

    private bool _playerInSight;
    private NavMeshAgent nav;
    private SphereCollider col;
    private Vector3 previousSighting;
    private GameObject player;
    private SphereCollider visionSphere;

    /*[Space(5)]
	[Header("Fleeing")]
	public float fleeRadius = 5.0f;
	public List<GameObject> fleePoints;*/
	private GameObject nextFleeTarget, currentFleeTarget;
	

	void Awake()
	{
        //Initialize everything
		NPCManager.registerAI(this);
		_currentHealth = maxHealth;
		_currentBehavior = startingBehavior;
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag("Player");
        enemyAudioSource = GetComponent<AudioSource>();   
        visionSphere = GetComponent<SphereCollider>();

        //Hide the indicators for the NPCs suspicion
        HelperFunctions.hideObjectAndChildren(suspicionIndicator);
        HelperFunctions.hideObjectAndChildren(alertedIndicator);

        //Patrol stuff
        NavPointContainer npc = null;
		if (PatrolContainer !=null)
			npc = PatrolContainer.GetComponent<NavPointContainer>();
		if (npc!=null)
		{
			navPoints = npc.navPoints;
			patrolType = npc.navigationBehaviorType;
		}
        if (startingBehavior == BehaviorType.patrolling)
        {
            if ((PatrolContainer == null) || (npc == null))
                startingBehavior = BehaviorType.idle;
            if(npc!=null)
            {
                if(npc.navPoints.Count == 0)
                    startingBehavior = BehaviorType.idle;
            }
        }
        
        if ( patrolType == navBehavior.wander)
			currentPointIndex = UnityEngine.Random.Range(0,navPoints.Count);
		else 
			currentPointIndex = 0;

		//TODO: Just use SetBehavior for this and let it do the heavy lifting
		switch(startingBehavior)
		{
		case BehaviorType.attacking:
			nav.speed = attackingSpeed;
			break;
		case BehaviorType.patrolling:
			nav.speed = patrolSpeed;
			GoToNextPoint();
			break;
		case BehaviorType.following:
			nav.speed = patrolSpeed;
			//nav.SetDestination(followFleeTarget.transform.position);
			InvokeRepeating("followTarget", 0f, 0.25f);
			break;
        case BehaviorType.fleeing:
            nav.speed = fleeingSpeed;
            break;
		}
		_previousBehavior = BehaviorType.none;
		_behaviorLastFrame = BehaviorType.none;
        _playerInSight = false; // hasPsychicAwarenessOfPlayer;
		if(hasPsychicAwarenessOfPlayer)
			currentSuspicion = awarenessMeterInvestigateToBusted;

	}
	

	void Start () 
	{
		foreach(attitudeChangeEventEntry acee in attitudeChangeEvents)
		{
			if(acee.eventName!="")
				EventRegistry.AddEvent(acee.eventName, changeAttitudeOnEvent, gameObject);
		}
		foreach(behaviorChangeEventEntry bcee in behaviorChangeEvents)
		{
			if (bcee.eventName!="")
				EventRegistry.AddEvent(bcee.eventName, changeBehaviorOnEvent, gameObject);
		}
		foreach(followTargetChangeEventEntry bcee in followDifferentTargetEvents)
		{
			if (bcee.eventName!="")
				EventRegistry.AddEvent(bcee.eventName, changeFollowTargetEvent, gameObject);
		}
		foreach(visionRangeChangeEventEntry vcee in visionRadiusChangeEvents)
		{
			if (vcee.eventName!="")
				EventRegistry.AddEvent(vcee.eventName, changeVisionRangeEvent, gameObject);
		}        
    }

	public void changeBehaviorOnEvent(string eventName)
	{
		foreach(behaviorChangeEventEntry bcee in behaviorChangeEvents)
		{
			if(bcee.eventName == eventName)
			{
				setBehavior(bcee.behaviorToChangeTo);
			}
		}
	}

	public void changeAttitudeOnEvent(string eventName)
	{
		foreach(attitudeChangeEventEntry ace in attitudeChangeEvents)
		{
			if(ace.eventName == eventName)
			{
                setAttitude(ace.attitudeToChangeTo);
				//attitudeTowardsPlayer = ace.attitudeToChangeTo;
				//setAttitude(ace.attitudeToChangeTo);
			}
		}
	}

	public void changeFollowTargetEvent(string eventName)
	{
		foreach(followTargetChangeEventEntry ftce in followDifferentTargetEvents)
		{
			if(ftce.eventName == eventName)
			{
				followFleeTarget = ftce.newFollowTarget;
			}
		}
	}

	public void changeVisionRangeEvent(string eventName)
	{
		foreach(visionRangeChangeEventEntry vce in visionRadiusChangeEvents)
		{
			if((vce.eventName == eventName)&&(visionSphere!=null))
			{
				visionSphere.radius = vce.newVisionRange;
			}
		}
	}

    public void setAttitude(AttitudeType newAttitude)
    {
        attitudeTowardsPlayer = newAttitude;
        if ((currentBehavior == BehaviorType.attacking) && ((attitudeTowardsPlayer== AttitudeType.friendly)||(attitudeTowardsPlayer == AttitudeType.neutral)))
            setBehavior(_previousBehavior);
        if (attitudeTowardsPlayer == AttitudeType.fearful)
            setBehavior(BehaviorType.fleeing);

        if ((attitudeTowardsPlayer == AttitudeType.friendly) || (attitudeTowardsPlayer == AttitudeType.neutral))
            wipeSuspicion();
    }

	public void setBehavior (BehaviorType newBehavior)
	{
		//TODO: add some state machine transition stuff here if needed 
		_previousBehavior = currentBehavior;
		_currentBehavior = newBehavior;
		if(_previousBehavior == BehaviorType.following)
			CancelInvoke();
		if(currentBehavior == BehaviorType.attacking)
		{
			currentSuspicion = awarenessMeterInvestigateToBusted;
		}
		if(currentBehavior != BehaviorType.following)
		{
			if(_previousBehavior == BehaviorType.following)
				CancelInvoke();
		}
		if(currentBehavior == BehaviorType.following)
		{
			if(_previousBehavior != BehaviorType.following)
			{
				nav.speed = patrolSpeed;
				//nav.SetDestination(followFleeTarget.transform.position);
				InvokeRepeating("followTarget", 0f, 0.25f);
			}
		}        
        if (currentBehavior != BehaviorType.fleeing)
        {
            if (_previousBehavior == BehaviorType.fleeing)
                CancelInvoke();
        }
    }

	public void Damage(int damageAmount)
	{
        //TODO: add health change threshold events here
		GameObject dropObject;
		if(_currentHealth <= 0)
			return;		
		_currentHealth -= damageAmount;
		if(_currentHealth <= 0)
		{
			HelperFunctions.hideObjectAndChildren(suspicionIndicator);
			HelperFunctions.hideObjectAndChildren(alertedIndicator);
			if (LootTable.Count > 0)
			{
				int randomLootIndex = UnityEngine.Random.Range(0, LootTable.Count);
				if (LootTable[randomLootIndex] != null)
				{
					foreach (LootEntry le in LootTable[randomLootIndex].lootEntries)
					{
						if (UnityEngine.Random.Range(0, 100) < le.percentChance)
						{
							dropObject = Instantiate(le.prefabToDrop);
							dropObject.transform.position = gameObject.transform.position;
						}
					}
				}
			}
			
			if(ragdollOnDeath)
			{				
				nav.enabled = false;
				Rigidbody rb = gameObject.GetComponent<Rigidbody>();
				if(rb != null)
				{
					Vector3 currentVel = rb.velocity;
					rb.isKinematic = false;
					rb.AddRelativeTorque(new Vector3(30, 0, 0));
					rb.AddForceAtPosition(transform.forward * 20 + new Vector3(0, 1, 0),transform.position+new Vector3(0,0.5f,0));
				}
				PhysicsCarryObject pco = gameObject.GetComponent<PhysicsCarryObject>();
				if(pco != null)
				{
					pco.enabled = true;
				}
				setBehavior(BehaviorType.none);
			}
			else
				GameObject.Destroy(gameObject);
			foreach(EventPackage ep in eventsToSendOnDeath)
			{
				EventRegistry.SendEvent(ep,gameObject);
			}
		}
	}
		
	public void wipeSuspicion()
	{
		currentSuspicion = 0;
	}

	void Update () 
	{
        //paused or dead. Nothing to do here
		if((GameManager.isPaused)||(_currentHealth<=0))
			return;

        if(currentAttackCooldown>0)
        {
            currentAttackCooldown -= Time.deltaTime;
            if (currentAttackCooldown < 0)
                currentAttackCooldown = 0;
        }

		if((playerInSight)||(hasPsychicAwarenessOfPlayer))
		{
            //The player is in sight, but we aren't currently engaged with attacking them or running from them
			if((currentBehavior != BehaviorType.attacking)&&(currentBehavior != BehaviorType.fleeing))
			{
                float playerDistance = getDistanceToPlayer();
                float distanceMultiplier = 1f;
                if(playerDistance >= visionSphere.radius * 0.75f)
                {
                    distanceMultiplier = veryFarSenseMultiplier;
                }
                
                if ((playerDistance >= visionSphere.radius * 0.5f)&&(playerDistance < visionSphere.radius * 0.75f))
                {
                    distanceMultiplier = farSenseMultiplier;
                }

                if ((playerDistance >= visionSphere.radius * 0.25f) && (playerDistance < visionSphere.radius * 0.5f))
                {
                    distanceMultiplier = nearSenseMultiplier;
                }

                if (playerDistance < visionSphere.radius * 0.25f)
                {
                    distanceMultiplier = veryNearSenseMultiplier;
                }



                currentSuspicion += Time.deltaTime * idleSenseRate * player.GetComponent<GAME1304PlayerController>().currentVisibility  * distanceMultiplier;
				if(currentSuspicion >= awarenessMeterUnawareToInvestigate)
				{
					HelperFunctions.showObjectAndChildren(suspicionIndicator);						
				}
				if (currentSuspicion >= maximumSuspicion)
					currentSuspicion = maximumSuspicion;

				if(currentSuspicion > awarenessMeterInvestigateToBusted)
				{										
					HelperFunctions.showObjectAndChildren(alertedIndicator);
					HelperFunctions.hideObjectAndChildren(suspicionIndicator);
					
                    if(attitudeTowardsPlayer == AttitudeType.aggressive)
                        setBehavior(BehaviorType.attacking);
                    if (attitudeTowardsPlayer == AttitudeType.fearful)
                        setBehavior(BehaviorType.fleeing);
                }
			}
		}
		else
		{		
            //Player is out of sight, so cool down the suspicion meter
			currentSuspicion -= Time.deltaTime * senseBleedOffRate;
			if(currentSuspicion < awarenessMeterInvestigateToBusted)
			{
				HelperFunctions.showObjectAndChildren(suspicionIndicator);
				HelperFunctions.hideObjectAndChildren(alertedIndicator);
			}
			if(currentSuspicion < awarenessMeterUnawareToInvestigate)
			{
				HelperFunctions.hideObjectAndChildren(suspicionIndicator);
			}
			if(currentSuspicion < 0)
				currentSuspicion = 0;
			if((currentBehavior == BehaviorType.attacking)|| (currentBehavior == BehaviorType.fleeing))

            {
				if(currentSuspicion <= awarenessMeterUnawareToInvestigate)
				{				
					HelperFunctions.hideObjectAndChildren(suspicionIndicator);						
					HelperFunctions.hideObjectAndChildren(alertedIndicator);						
					currentSuspicion = 0; //TODO: rethink this
					setBehavior(_previousBehavior);
					CancelInvoke();
				}
			}
		}

		switch(currentBehavior)
		{
		case BehaviorType.idle: //do nothing, you're idle
			break;
		case BehaviorType.attacking:
			nav.speed = attackingSpeed;
            //TODO: incorporate the actual attack types
			if(_behaviorLastFrame != BehaviorType.attacking)
			{						
				InvokeRepeating("chasePlayer", 0f, 0.25f);
			}
			break;
		case BehaviorType.fleeing:
			nav.speed = fleeingSpeed;
            if (_behaviorLastFrame != BehaviorType.fleeing)
            {
                setFleePoint();
                InvokeRepeating("fleePlayer", 0f, 0.25f);
            }
            break;
		case BehaviorType.following:
			break;
		case BehaviorType.patrolling:						
			if(_behaviorLastFrame != BehaviorType.patrolling)
			{
				nav.speed = patrolSpeed;
				HelperFunctions.hideObjectAndChildren(suspicionIndicator);						
				HelperFunctions.hideObjectAndChildren(alertedIndicator);
				GoToNextPoint();
			}
			if(!nav.pathPending && nav.remainingDistance < 0.5f)
			{
				patrolWaitTimer += Time.deltaTime;
                    if (patrolWaitTimer >= patrolWaitDuration)
                    {													
					//TODO: make this more efficient. take the assumptions out of the chain somewhere else
					if(navPoints != null)
					{
						if(navPoints.Count > 0)
						{
							if(navPoints[patrolEventsToFireIndex] != null)
							{
								if(navPoints[patrolEventsToFireIndex].eventsToSend.Count != 0)
								{									
                                    foreach(EventPackage ep in navPoints[patrolEventsToFireIndex].eventsToSend)
                                            EventRegistry.SendEvent(ep,gameObject);
                                    }
							}
						}
					}
					GoToNextPoint();
				}
			}
			break;
		}
		/**/

		_behaviorLastFrame = currentBehavior;
	}

	void followTarget()
	{		
		if(currentBehavior == BehaviorType.following)
		{
			setAgentDestination(followFleeTarget.transform.position);
			//nav.SetDestination(followFleeTarget.transform.position);
			//if(!agent.pathPending && agent.remainingDistance >= followDistance)
			if(nav.remainingDistance >= followDistance)
			{
				nav.speed = patrolSpeed;
			}
			else
			{
				nav.speed = 0;
			}
		}
	}

    float getDistanceToPlayer()
    {
        return ((transform.position - player.transform.position).magnitude);
    }

	void chasePlayer()
	{		
        switch (attackType)
        {
            case NPCAttackType.touch:
                nav.SetDestination(player.transform.position);
            break;
            case NPCAttackType.rangedProjectile:
                if ((getDistanceToPlayer() <= rangedAttackMaxDistance)&&playerInSight)
                {
                    //agent.isStopped = true;
                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = lookRotation;
                    if (currentAttackCooldown <= 0)
                    {
                        fireProjectileAtPlayer();
                    }//TODO: what does it do if it's in range but outside of the cooldown?
                    

                }
                else
                {
                    //if(agent.remainingDistance > 0.5f)
                        reacquire();
                }
            break;
            case NPCAttackType.melee:
            break;
        }

		
	}    

    void reacquire()
    {
        Vector3 destinationPosition;
        Vector3 destinationOffset;
        Vector2 horizontalOffset;
        
        horizontalOffset = UnityEngine.Random.insideUnitCircle * 2f;
        destinationOffset = new Vector3(horizontalOffset.x, 0, horizontalOffset.y);
    
        destinationPosition = player.transform.position + destinationOffset;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(destinationPosition, out hit, 5.0f, 1);
        destinationPosition = hit.position;        
        
        nav.SetDestination(destinationPosition);

        //nav.SetDestination(player.transform.position + ((transform.position - player.transform.position).normalized * rangedAttackMaxDistance));
    }

    void fireProjectileAtPlayer()
    {
        //TODO: turn to face player
        //TODO: wait for windup
        GameObject bulletObj = GameObject.Instantiate(bulletPrefab,transform.position+transform.forward*1.5f,Quaternion.identity);
        bulletObj.transform.parent = null;
        if (bulletObj.GetComponent<BulletBehavior>() != null)
        {
            bulletObj.GetComponent<BulletBehavior>().init(transform.forward);
            currentAttackCooldown = attackcoolDownMax;
            if ((rangedAttackSound != null) && (enemyAudioSource != null))
            {
                enemyAudioSource.clip = rangedAttackSound;
                enemyAudioSource.Play();
            }
        }
        //TODO: make this a random value between the min and max
    }

    void setFleePoint()
    {
        //TODO: make this not terrible
        Vector3 fleePos;
        Vector2 fleeOffset;
        if (fleePoints.Count == 0)
        {
            float dist = (transform.position - player.transform.position).magnitude;
            fleeOffset = (UnityEngine.Random.insideUnitCircle).normalized * dist * 2;
            fleePos = transform.position + new Vector3(fleeOffset.x, 0, fleeOffset.y);

            NavMeshHit hit;
            NavMesh.SamplePosition(fleePos, out hit, 10.0f, 1);
            nav.SetDestination(hit.position);
        }
        else
        {
            nav.SetDestination(getFurthestFleePointFromPlayer());
        }
    }

    Vector3 getFurthestFleePointFromPlayer()
    {
        Vector3 playerLoc = player.transform.position;
        Vector3 farthestPoint = transform.position;
        float dist = 0;
        foreach(GameObject go in fleePoints)
        {
            if ((go.transform.position - playerLoc).magnitude > dist)
            {
                farthestPoint = go.transform.position;
                dist = (go.transform.position - playerLoc).magnitude;
            }
        }
        return farthestPoint;
    }

	void fleePlayer()
	{        
        if (nav.remainingDistance <= 1.0f)
            setFleePoint();
	}

	void OnTriggerStay(Collider other)
	{
		if(GameManager.isPaused || _currentHealth <= 0)
			return;
		float testFOV;
		if(other.gameObject == player)
		{
			if(currentBehavior == BehaviorType.attacking)
				testFOV = pursuitFOV;
			else
				testFOV = patrolFOV;
            //default "player in sight" before re-checking to see if the player is actually in sight
            _playerInSight = false; 
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			if(angle < (testFOV * 0.5f))
			{
				RaycastHit hit;
				//TODO: offset the raycast by the character's half height instead of just 1
				//LayerMask mask = LayerMask.GetMask("Player");
				if(Physics.Raycast(transform.position+transform.up, direction.normalized, out hit, col.radius)) //,mask.value))
				{
					if(hit.collider.gameObject == player)
					{
						_playerInSight = true;

					}
				}
				if(Physics.Raycast(transform.position, direction.normalized, out hit, col.radius)) //,mask.value))
				{
					if(hit.collider.gameObject == player)
					{
						_playerInSight = true;

					}
				}
			}
		}
	}

	void GoToNextPoint()
	{
		//if we've been given bad data, exit
		//should probably pop an assert here as well
		patrolWaitTimer = 0;
		if((navPoints == null)||(navPoints.Count == 0))
      //if (navPoints == null)
            {
			setBehavior(BehaviorType.idle);
			return;			
		}
		if (navPoints.Count <=0 )
			return;
		nav.SetDestination(navPoints[currentPointIndex].navPointObject.transform.position);
		patrolEventsToFireIndex = currentPointIndex;
        patrolWaitDuration = UnityEngine.Random.Range(navPoints[currentPointIndex].pauseDurationMin, navPoints[currentPointIndex].pauseDurationMax);
        switch (patrolType)
		{
		case navBehavior.patrolLoop: 
			currentPointIndex += 1;
			if (currentPointIndex >= navPoints.Count)
				currentPointIndex = 0;
			break;
		case navBehavior.patrolPingPong:
			currentPointIndex += pingPongDir;
			if (currentPointIndex >= navPoints.Count)
			{
				pingPongDir = -1;
				currentPointIndex=navPoints.Count-2;
			}
			else if (currentPointIndex < 0)
			{
				pingPongDir = 1;
				currentPointIndex=1;
			}
			break;
		case navBehavior.wander:
			currentPointIndex = UnityEngine.Random.Range(0,navPoints.Count);
			break;
		}
	}
}
