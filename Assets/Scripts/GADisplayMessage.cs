using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GADisplayMessage : IGameplayAbility
{
    public override int VFValidateEntityForAbility(IGameplayEntity gameplayEntity)
    {
        return base.VFValidateEntityForAbility(gameplayEntity);
    }

    protected override int VFOnUpdate()
    {
        return 0;
    }

    protected override int VFOnTriggerDetected()
    {
        return 0;
    }

    public override int VFRelease()
    {
        return base.VFRelease();
    }

    protected override int VFTriggerValidator(IGameplayEntity caster)
    {
        return base.VFTriggerValidator(caster);
    }
}
