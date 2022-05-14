using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMovement : IGameplayAbility
{
    private int MovePlayer(in IGameplayEntity caster, Vector3 triggerVector)
    {
        caster.IssueTranslate(triggerVector);
        return 0;
    }


    // This behaviour shows using the same cue for all sides.
    public override int VFOnServerUpdateAllyRpcs(in IGameplayEntity caster, Vector3 allyTriggerVector)
    {
        MovePlayer(caster, allyTriggerVector);
        return 0;
    }
    public override int VFOnServerUpdateEnemyRpcs(in IGameplayEntity caster, Vector3 enemyTriggerVector)
    {
        MovePlayer(caster, enemyTriggerVector);
        return 0;
    }

    public override int VFOnServerUpdateItself(in IGameplayEntity caster, Vector3 serverTriggerVector) 
    {
        MovePlayer(caster, serverTriggerVector);
        return 0;
    }

}
