using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAIllusion : IGameplayAbility
{
    bool mIllusionTriggered;
    
    public override int VFOnServerUpdateAbility(in IGameplayEntity caster, Vector3 serverTriggerVector)
    {
        return base.VFOnServerUpdateAbility(caster, serverTriggerVector);
    }

    public override int VFOnServerUpdateAbilityForAllies(in IGameplayEntity caster, Vector3 allyTriggerVector)
    {
        return base.VFOnServerUpdateAbilityForAllies(caster, allyTriggerVector);
    }

    public override int VFOnServerUpdateAbilityForEnemies(in IGameplayEntity caster, Vector3 enemyTriggerVector)
    {
        return base.VFOnServerUpdateAbilityForEnemies(caster, enemyTriggerVector);
    }
}
