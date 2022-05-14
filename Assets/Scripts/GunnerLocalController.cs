using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class GunnerLocalController : NetworkBehaviour
{
    PlayerInput mInput;
    InputAction mMove, mFire;
    IGameplayEntity mGameplayEntity;
    int mAbilityMovementIdx;

    private void Start()
    {
        if (isServer)
        {
            Debug.Log("isServer");
            return;
        }

        if (!hasAuthority) return;

        mInput = GetComponent<PlayerInput>();
        mGameplayEntity = GetComponent<IGameplayEntity>();

        mMove = mInput.actions["Movement"];
        mFire = mInput.actions["Fire"];


        mGameplayEntity.AddAbility(typeof(GAMovement), ref mAbilityMovementIdx);

        mFire.performed += _ => Fire();
    }

    void Fire()
    {

    }

    void Update()
    {
        if (!hasAuthority || isServer) return;

        Vector2 movementInput = mMove.ReadValue<Vector2>();
        Vector3 movement = movementInput;
        mGameplayEntity.CmdTriggerAbility(mAbilityMovementIdx, movement);
        //transform.position += movement * Time.deltaTime;
    }
}
