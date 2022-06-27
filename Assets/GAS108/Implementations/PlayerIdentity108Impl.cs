using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class PlayerIdentity108Impl : IPlayerIdentity108
{
    [SerializeField] GameObject mGunnerPrefab;
    GameObject mGunnerInstance;
    protected override void VFOnServerInitializeSpawnables()
    {
        mGunnerInstance = Instantiate(mGunnerPrefab);

        if (mGunnerInstance.GetComponent<IGameplayEntity108>() == null)
        {
            Debug.LogError("Can't spawn an instance that does not have an IGameplayEntity108 attached");
            Destroy(mGunnerInstance);
            return;
        }

        NetworkServer.Spawn(mGunnerInstance, this.connectionToClient);
    }


    IGameplayEntity108 mGEGunner;
    public override void VFOnClientRegisterSpawnedEntity(IGameplayEntity108 ent)
    {
        mGEGunner = ent;
    }

    PlayerInput mInput;
    InputAction mMove;
    Vector2 mLastMovementInput;
    int mAbilityIdxMove;
    protected override void VFOnClientStart()
    {
        mMove = mInput.actions["Movement"];
    }
    protected override void VFOnClientUpdate()
    {
        Vector2 movementInput = mMove.ReadValue<Vector2>();
        if(movementInput != mLastMovementInput)
        {
            mLastMovementInput = movementInput;
            Vector4 triggerVector = new Vector4(movementInput.x, movementInput.y, 0, 0);
        }
    }
}
