using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameplayEntity109 : MonoBehaviour
{
    public IGameplayState109 mAllyState { get; private set; }

    [SerializeField] GAMove109 mGAMovement;

    void Start()
    {
        mAllyState = new IGameplayState109(this);
    }

    private void FixedUpdate()
    {
        mGAMovement.OnFixedUpdate(Time.deltaTime);
    }

    public virtual void VFInitAbilities()
    {
        mGAMovement.OnInit(this);
    }
    public void TriggerAbility(int abilityID ,Vector4 triggerVector)
    {
        /* Call this function in the controller to catch an input to be sent to server on the client side */
        mGAMovement.OnClientTriggerAbility(triggerVector);
    }
}
