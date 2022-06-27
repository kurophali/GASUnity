using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XbotCue : MonoBehaviour
{
    [SerializeField] GameObject mStateGO;
    Animator mAnimator;
    IGameplayState109 mEntityState; // All cues will automatically play to this

    private void Start()
    {
        mAnimator = GetComponent<Animator>();
        mEntityState = mStateGO.GetComponent<IGameplayEntity109>().mAllyState;
        mEntityState.onSetVelocity += SetAnimatorSpeed;
    }

    // Get state params here becaus it state param only updates in FixedUdpate
    private void FixedUpdate()
    {
        mAnimator.SetFloat("speed", mEntityState.GetVelocity().magnitude);
    }
       
    void SetAnimatorSpeed(Vector3 velocity)
    {
        mAnimator.SetFloat("speed", velocity.magnitude);
    }

    private void OnDestroy()
    {
        mEntityState.onSetVelocity -= SetAnimatorSpeed;
    }
}
