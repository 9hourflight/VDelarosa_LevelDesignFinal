using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[System.Serializable]
public class navPoint
{
	public GameObject navPointObject;
    public bool useTargetPointFacing = false;
    [FormerlySerializedAs("pauseDuration")]
    public float pauseDurationMin = 0.0f;
    public float pauseDurationMax = 0.0f;
    public List<EventPackage> eventsToSend;    
}

public enum navBehavior {patrolLoop,patrolPingPong, wander,oneWay};

public class NavPointContainer : MonoBehaviour 
{

	public List<navPoint> navPoints;
	public navBehavior navigationBehaviorType;
    [Header("Debug")]
    public bool hideDebugDraw = false;
    public bool onlyShowDebugWhenSelected = false;

    void Start () 
	{
		
	}
	
	
	void Update ()
    {
		
	}

    public void drawPatrolGizmos()
    {
#if UNITY_EDITOR
        //TODO: Add better NULL checks for all navpoints
        foreach (navPoint np in navPoints)
        {
            if (np.navPointObject == null)
                return;
        }
        Vector3 lookDir;
        if (navPoints.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int lcv = 0; lcv < navPoints.Count; lcv++)
            {
                //Gizmos.DrawLine(transform.position, np.navPointObject.transform.position);
                Handles.color = Color.blue;
                Handles.DrawDottedLine(transform.position, navPoints[lcv].navPointObject.transform.position, 2.0f);
                Handles.color = Color.white;
                Handles.Label(navPoints[lcv].navPointObject.transform.position, lcv.ToString());
                switch (navigationBehaviorType)
                {
                    case navBehavior.patrolLoop:
                        if (lcv < navPoints.Count - 1)
                        {
                            Handles.color = Color.yellow;
                            Handles.DrawLine(navPoints[lcv].navPointObject.transform.position, navPoints[lcv + 1].navPointObject.transform.position);
                            lookDir = navPoints[lcv + 1].navPointObject.transform.position - navPoints[lcv].navPointObject.transform.position;
                            Handles.ArrowHandleCap(0, navPoints[lcv].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);

                        }
                        else
                        {
                            Handles.color = Color.yellow;
                            Handles.DrawLine(navPoints[lcv].navPointObject.transform.position, navPoints[0].navPointObject.transform.position);
                            lookDir = navPoints[0].navPointObject.transform.position - navPoints[lcv].navPointObject.transform.position;
                            Handles.ArrowHandleCap(0, navPoints[lcv].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);

                        }
                        break;
                    case navBehavior.patrolPingPong:
                        if (lcv < navPoints.Count - 1)
                        {
                            Handles.color = Color.yellow;
                            Handles.DrawLine(navPoints[lcv].navPointObject.transform.position, navPoints[lcv + 1].navPointObject.transform.position);
                            lookDir = navPoints[lcv + 1].navPointObject.transform.position - navPoints[lcv].navPointObject.transform.position;
                            Handles.ArrowHandleCap(0, navPoints[lcv].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);
                            lookDir = navPoints[lcv].navPointObject.transform.position - navPoints[lcv + 1].navPointObject.transform.position;
                            Handles.ArrowHandleCap(0, navPoints[lcv + 1].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);
                        }
                        else
                        {
                            /* Handles.color = Color.yellow;
                             Handles.DrawLine(navPoints[lcv].navPointObject.transform.position, navPoints[0].navPointObject.transform.position);
                             lookDir = navPoints[0].navPointObject.transform.position - navPoints[lcv].navPointObject.transform.position;
                             Handles.ArrowHandleCap(0, navPoints[lcv].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);
                             lookDir = navPoints[lcv].navPointObject.transform.position - navPoints[0].navPointObject.transform.position;
                             Handles.ArrowHandleCap(0, navPoints[0].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);*/
                        }
                        break;
                    case navBehavior.oneWay:
                        { 
                            if (lcv < navPoints.Count - 1)
                            {
                                Handles.color = Color.yellow;
                                Handles.DrawLine(navPoints[lcv].navPointObject.transform.position, navPoints[lcv + 1].navPointObject.transform.position);
                                lookDir = navPoints[lcv + 1].navPointObject.transform.position - navPoints[lcv].navPointObject.transform.position;
                                Handles.ArrowHandleCap(0, navPoints[lcv].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);
                               /* lookDir = navPoints[lcv].navPointObject.transform.position - navPoints[lcv + 1].navPointObject.transform.position;
                                Handles.ArrowHandleCap(0, navPoints[lcv + 1].navPointObject.transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);*/
                            }
                        }
                        break;
                    case navBehavior.wander:

                        Handles.color = Color.yellow;
                        Handles.DrawLine(transform.position, navPoints[lcv].navPointObject.transform.position);
                        lookDir = navPoints[lcv].navPointObject.transform.position - transform.position;
                        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(lookDir), 1.4f, EventType.Repaint);
                        break;
                }
                //Handles.ArrowCap()
            }
        }
#endif
    }
    private void OnDrawGizmos()
    {
        if(!hideDebugDraw && !onlyShowDebugWhenSelected)
            drawPatrolGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if(!hideDebugDraw)
            drawPatrolGizmos();
    }

}
