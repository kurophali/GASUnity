using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAMovement : IGameplayAbility
{

    public override int VFOnServerUpdateAllyRpcs(IGameplayEntity caster, Vector3 allyTriggerVector)
    {
        caster.RpcTranslate(allyTriggerVector);
        return 0;
    }
    public override int VFOnServerUpdateEnemyRpcs(IGameplayEntity caster, Vector3 enemyTriggerVector)
    {
        caster.RpcTranslate(enemyTriggerVector);
        return 0;
    }

    public override int VFOnServerUpdateItself(IGameplayEntity caster, Vector3 serverTriggerVector) 
    {
        caster.Translate(serverTriggerVector);
        return 0;
    }

}
