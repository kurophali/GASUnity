using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerLocals107 : NetworkBehaviour
{
    PlayerInput mInput;
    InputAction mMove, mFire;
    IGameplayEntity107 mGEGunner;
    int mGAMovementIdx, mGAAimIdx, mGAIllusionIdx;
    CinemachineVirtualCamera mCMAim, mCMThirdPerson;
    GameObject mCameraReference;
    Transform mMainCamTransform;
    Vector3 mCameraLookAt;
    public void AssignGameplayEntity(IGameplayEntity107 ge)
    {
        Debug.Log("AssignGameplayEntity called");
        mGEGunner = ge;

        mCameraReference = new GameObject();
        mCameraReference.transform.position = mGEGunner.gameObject.transform.position;
        mCameraReference.transform.rotation = mGEGunner.gameObject.transform.rotation;

        Debug.Log("AssignGameplayEntity called, now before addability");

        //mGEGunner.AddAbility(typeof(GAMovement), ref mGAMovementIdx);
        mGEGunner.AddAbility(typeof(GAIllusion107), ref mGAIllusionIdx);
        mGEGunner.AddAbility(typeof(GAAim107), ref mGAAimIdx);

        Debug.Log("AssignGameplayEntity called, now after addability");

        mCMAim = GameObject.Find("CMAim").GetComponent<CinemachineVirtualCamera>();
        mCMAim.LookAt = mCameraReference.transform;
        mCMAim.Follow = mCameraReference.transform;
        mCMThirdPerson = GameObject.Find("CMThirdPerson").GetComponent<CinemachineVirtualCamera>();
        mCMThirdPerson.LookAt = mCameraReference.transform;
        mCMThirdPerson.Follow = mCameraReference.transform;
    }

    private void Start()
    {
        if (isServer)
        {
            Debug.Log("isServer");
            return;
        }

        if (!hasAuthority) return;

        Cursor.lockState = CursorLockMode.Locked;

        mInput = GetComponent<PlayerInput>();

        mMainCamTransform = Camera.main.transform;
        mMove = mInput.actions["Movement"];
        mFire = mInput.actions["Fire"];

        mFire.performed += _ => Fire();
    }

    void Fire()
    {

    }

    void Update()
    {
        if (!hasAuthority || isServer) return;

        if(mGEGunner == null)// async call to initialize mCameraReference
        {
            Debug.Log("Waiting for mGameplayEntity to be initialized");
            return;
        }

        if(mCameraReference == null) // async call to initialize mCameraReference
        {
            Debug.Log("Waiting for mCameraReference to be initialized");
            return;
        }

        mCameraReference.transform.position = mGEGunner.gameObject.transform.position;
        Vector2 movementInput = mMove.ReadValue<Vector2>();
        Vector3 moveValue = new Vector3(movementInput.x, 0f, movementInput.y);
        Vector3 moveDirection = moveValue.x * mMainCamTransform.right.normalized
            + moveValue.z * mMainCamTransform.forward.normalized;

        //mGEGunner.CmdTriggerAbility(mGAMovementIdx, moveDirection);
        mGEGunner.CmdTriggerAbility(mGAIllusionIdx, moveDirection);

        Ray centerRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        bool rayCastResult = Physics.Raycast(centerRay, out hit);
        if (rayCastResult)
        {
            mCameraLookAt = hit.point;
        }
        else
        {
            mCameraLookAt = mMainCamTransform.position + mMainCamTransform.forward * 1000f;
        }

        mGEGunner.CmdTriggerAbility(mGAAimIdx, mCameraLookAt);
    }

    private void OnDestroy()
    {
        if (hasAuthority && Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
    }
}
