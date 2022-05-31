using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAShoot107 : IGameplayAbility107
{
    [SerializeField] GameObject mBulletPrefab;
    GameObject mBulletInstance;
    Transform mBulletSpawnPoint;
    IEnumerator mDisplayTimer;

    public override int VFOnGEAddAbilityInit(IGameplayEntity107 gameplayEntity)
    {
        mBulletInstance = GameObject.Instantiate(mBulletPrefab);
        mBulletInstance.SetActive(false);
        mBulletSpawnPoint = mAbilityOwner.GetNode(mAbilityOwner.FindNodeIndex("ShootingPoint"));
        if(mBulletSpawnPoint == null)
        {
            Debug.Log("Model does not have a node named 'ShootingPoint'");
            return 1;
        }

        return base.VFOnGEAddAbilityInit(gameplayEntity);
    }


    protected override int VFOnServerTriggerDetected()
    {
        // Change the bullet's position to the spawn point's then set it to active then disable it after some time
        
        return base.VFOnServerTriggerDetected();
    }

    public override int VFOnGERemoveAbilityRelease()
    {
        
        return base.VFOnGERemoveAbilityRelease();
    }
}
