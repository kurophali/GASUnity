using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameplayAbility109 : ScriptableObject
{
    public float mCurrentCooldown { get; private set; }
    public Vector4 mTriggerVector { get; private set; }
    public bool mPushTriggerOnNextRun { get; private set; }

    public IGameplayState109 mOwnerState;

    public int OnInit(IGameplayEntity109 ownerEntity)
    {
        mCurrentCooldown = 0f;
        mPushTriggerOnNextRun = false;
        mTriggerVector = new Vector4(0, 0, 0);
        mOwnerState = ownerEntity.mAllyState;

        return 0;
    }

    public int OnFixedUpdate(float deltaTime)
    {
        int output = VFOnFixedUpdate(deltaTime); // do custom updates first
        if (output != 0) Debug.Log(output);

        output = OnClientFixedUpdatePushTrigger(); // push ability trigger to server
        if (output != 0) Debug.Log(output);

        return output;
    }

    protected virtual int VFOnFixedUpdate(float deltaTime)
    {
        /* Update states here */
        return 0;
    }

    public int OnClientTriggerAbility(Vector4 triggerVector)
    {
        /* Queue up the trigger for next FixedUpdate() if validated */

        /* Set local ability trigger */
        if (mCurrentCooldown > 0) return 1;

        /* Validate the trigger locally */
        if(OnClientDecidePush(triggerVector) != 0) return 2;

        mTriggerVector = triggerVector;
        mPushTriggerOnNextRun = true;

        return 0;
    }

    protected virtual int OnClientDecidePush(Vector4 triggerVector)
    {
        return 0;
    }

    int OnClientFixedUpdatePushTrigger()
    {
        /* Push the last valid trigger*/
        if (mPushTriggerOnNextRun)
        {
            CmdTriggerAbility(mTriggerVector);
            mPushTriggerOnNextRun = false;
        }
        return 0;
    }

    void CmdTriggerAbility(Vector4 triggerVector)
    {
        // This only sets the mTriggerVector on the server side
        if (mCurrentCooldown > 0) return;

        mTriggerVector = triggerVector;

        // Call custom functions
        VFOnServerCaughtTrigger();
    }

    protected virtual int VFOnServerCaughtTrigger()
    {
        // Queue state changes here
        Debug.Log("Called cmd");

        return 0;
    }
}
