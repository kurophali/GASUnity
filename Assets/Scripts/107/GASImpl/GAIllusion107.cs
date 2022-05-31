using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAIllusion107 : IGameplayAbility107
{
    bool mIllusionTriggered;
    
    public override int VFOnServerUpdateAbility(in IGameplayEntity107 caster, Vector3 serverTriggerVector)
    {
        return base.VFOnServerUpdateAbility(caster, serverTriggerVector);
    }

    public override int VFOnServerUpdateAbilityForAllies(in IGameplayEntity107 caster, Vector3 allyTriggerVector)
    {
        return base.VFOnServerUpdateAbilityForAllies(caster, allyTriggerVector);
    }

    public override int VFOnServerUpdateAbilityForEnemies(in IGameplayEntity107 caster, Vector3 enemyTriggerVector)
    {
        return base.VFOnServerUpdateAbilityForEnemies(caster, enemyTriggerVector);
    }
}
