using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMovement : IGameplayAbility
{
    protected override int VFOnTriggerDetected()
    {
        return 0;
    }

    protected override int VFOnUpdate()
    {
        Debug.Log("Running VFOnUpdate with Vector " + mTriggerVector);
        mAbilityOwner.gameObject.transform.position += mTriggerVector * Time.deltaTime;

        return 0;
    }

    public override int VFOnClientTrigger(Vector3 triggerVector)
    {
        mTriggerVector = triggerVector;
        //Debug.Log(triggerVector);
        return 0;
    }

    public override Vector3 VFProcessTriggerVectorForAllies(Vector3 triggerVector)
    {
        return mTriggerVector;
    }

    public override Vector3 VFProcessTriggerVectorForEnemies(Vector3 triggerVector)
    {
        return triggerVector;
    }

    public override int VFRelease()
    {
        return 0;
    }

    protected override int VFTriggerValidator(IGameplayEntity caster)
    {
        return 0;
    }

    public override int VFValidateEntityForAbility(IGameplayEntity gameplayEntity)
    {
        return 0;
    }
}
