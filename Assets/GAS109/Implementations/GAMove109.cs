using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Abilities 109/GAMove")]
public class GAMove109 : IGameplayAbility109
{
    [SerializeField] float mTurnRate = 5.0f;
    protected override int OnClientDecidePush(Vector4 triggerVector)
    {
        // Significanlly reduce bandwidth usage with this 

        if(triggerVector == mTriggerVector)
        {
            return 1; 
        }

        return 0;
    }
    protected override int VFOnFixedUpdate(float deltaTime)
    {
        Vector3 movementVector = new Vector3(mTriggerVector.x, mTriggerVector.y, mTriggerVector.z);
        mOwnerState.SetVelocity(movementVector);

        if (movementVector.magnitude != 0)
        {
            Quaternion mRotation = Quaternion.LookRotation(movementVector, Vector3.up);
            Quaternion newRotation = Quaternion.RotateTowards(mOwnerState.GetRotation(), mRotation, mTurnRate);
            mOwnerState.SetRotaion(newRotation);
        }

        return 0;
    }
}
