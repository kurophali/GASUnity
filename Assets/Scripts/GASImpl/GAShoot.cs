using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Abilities/Shoot")]
public class GAShoot : IGameplayAbility
{
    [SerializeField] GameObject mBulletPrefab;
    GameObject mBulletInstance;
    Transform mBulletSpawnPoint;
    IEnumerator mDisplayTimer;

    public override int VFOnGEAddAbilityInit(IGameplayEntity gameplayEntity)
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
