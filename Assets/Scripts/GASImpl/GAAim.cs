using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAAim : IGameplayAbility
{
    int LookAtTarget(in IGameplayEntity caster, Vector3 triggerVector)
    {
        caster.CueLookAt(triggerVector);
        return 0;
    }

    public override int VFOnServerUpdateAbilityForAllies(in IGameplayEntity caster, Vector3 allyTriggerVector)
    {
        return LookAtTarget(caster, allyTriggerVector);
    }

    public override int VFOnServerUpdateAbilityForEnemies(in IGameplayEntity caster, Vector3 enemyTriggerVector)
    {
        return LookAtTarget(caster, enemyTriggerVector);
    }

    public override int VFOnServerUpdateAbility(in IGameplayEntity caster, Vector3 serverTriggerVector)
    {
        return LookAtTarget(caster, serverTriggerVector);
    }
}
