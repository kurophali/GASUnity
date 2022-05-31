using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMovement107 : IGameplayAbility107
{
    private int MovePlayer(in IGameplayEntity107 caster, Vector3 triggerVector)
    {
        triggerVector.y = 0;
        if(triggerVector.magnitude != 0)
        {
            triggerVector = triggerVector.normalized;
        }

        caster.CueTranslate(triggerVector * Time.deltaTime * 3.0f);
        return 0;
    }


    // This behaviour shows using the same cue for all sides.
    public override int VFOnServerUpdateAbilityForAllies(in IGameplayEntity107 caster, Vector3 allyTriggerVector)
    {
        MovePlayer(caster, allyTriggerVector);
        return 0;
    }
    public override int VFOnServerUpdateAbilityForEnemies(in IGameplayEntity107 caster, Vector3 enemyTriggerVector)
    {
        MovePlayer(caster, enemyTriggerVector);

        return 0;
    }

    public override int VFOnServerUpdateAbility(in IGameplayEntity107 caster, Vector3 serverTriggerVector) 
    {
        MovePlayer(caster, serverTriggerVector);
        return 0;
    }

}
