using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IGameplayState109
{
    private IGameplayState109() { }
    public IGameplayState109(IGameplayEntity109 gameplayEntity)
    {
        mRigidBody = gameplayEntity.GetComponent<Rigidbody>();
    }

    private Rigidbody mRigidBody;

    public Quaternion GetRotation()
    {
        return mRigidBody.rotation;
    }
    public void SetRotaion(Quaternion rotation)
    {
        mRigidBody.rotation = rotation;
    }

    public Vector3 GetVelocity()
    {
        return mRigidBody.velocity;
    }

    public event Action<Vector3> onSetVelocity;
    public void SetVelocity(Vector3 velocity)
    {
        mRigidBody.velocity = velocity;
        onSetVelocity(velocity);
    }

    public Vector3 GetPosition()
    {
        return mRigidBody.position;
    }

    public void SetPosition(Vector3 position)
    {
        mRigidBody.position = position;
    }
}
