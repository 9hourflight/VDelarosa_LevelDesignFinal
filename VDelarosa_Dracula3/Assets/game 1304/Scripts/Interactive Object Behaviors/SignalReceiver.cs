using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum signalTypes { bullet, melee, physics, fire, ice, electricity, genericHazard, scriptedDamage, water, light, dark };

public class SignalReceiver : MonoBehaviour
{
    public List<signalEventEntry> signalEvents;

    public void processSignal(signalTypes signalType, int signalAmount)
    {
        GameObject tempObj;
        bool damagePassesEventCheck = false;
        foreach (signalEventEntry dee in signalEvents)
        {
            damagePassesEventCheck = false;
            if (signalAmount >= dee.damageThreshold)
            {
                if (dee.filterByDamageType == false || ((dee.filterByDamageType) && (dee.damageType == signalType)))
                {
                    switch (dee.comparisonForThreshold)
                    {
                        case comparisonOperator.Equal:
                            damagePassesEventCheck = (dee.damageThreshold == signalAmount);
                            break;
                        case comparisonOperator.greaterThan:
                            damagePassesEventCheck = (signalAmount > dee.damageThreshold);
                            break;
                        case comparisonOperator.greaterThanEqual:
                            damagePassesEventCheck = (signalAmount >= dee.damageThreshold);
                            break;
                        case comparisonOperator.lessThan:
                            damagePassesEventCheck = (signalAmount < dee.damageThreshold);
                            break;
                        case comparisonOperator.lessThanEqual:
                            damagePassesEventCheck = (signalAmount <= dee.damageThreshold);
                            break;
                        case comparisonOperator.notEqual:
                            damagePassesEventCheck = (dee.damageThreshold != signalAmount);
                            break;
                    }
                    if (damagePassesEventCheck)
                    {                                               
                        foreach (EventPackage ep in dee.eventsToSend)
                            EventRegistry.SendEvent(ep, this.gameObject);
                    }
                }
            }
        }
    }

}
