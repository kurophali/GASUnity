using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Cinemachine;

public class GunnerLocalController : NetworkBehaviour
{
    PlayerInput mInput;
    InputAction mMove, mFire;
    IGameplayEntity mGameplayEntity;
    int mGAMovementIdx, mGAAimIdx;
    CinemachineVirtualCamera mCMAim, mCMThirdPerson;
    GameObject mCameraReference;
    Transform mMainCamTransform;
    Vector3 mCameraLookAt;

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
        mGameplayEntity = GetComponent<IGameplayEntity>();
        mMainCamTransform = Camera.main.transform;
        mMove = mInput.actions["Movement"];
        mFire = mInput.actions["Fire"];

        mCameraReference = new GameObject();
        mCameraReference.transform.position = gameObject.transform.position;
        mCameraReference.transform.rotation = gameObject.transform.rotation;
        mCMAim = GameObject.Find("CMAim").GetComponent<CinemachineVirtualCamera>();
        mCMAim.LookAt = mCameraReference.transform;
        mCMAim.Follow = mCameraReference.transform;
        mCMThirdPerson = GameObject.Find("CMThirdPerson").GetComponent<CinemachineVirtualCamera>();
        mCMThirdPerson.LookAt = mCameraReference.transform;
        mCMThirdPerson.Follow = mCameraReference.transform;

        mGameplayEntity.AddAbility(typeof(GAMovement), ref mGAMovementIdx);
        mGameplayEntity.AddAbility(typeof(GAAim), ref mGAAimIdx);

        mFire.performed += _ => Fire();
    }

    void Fire()
    {

    }

    void Update()
    {
        if (!hasAuthority || isServer) return;

        mCameraReference.transform.position = gameObject.transform.position;

        Vector2 movementInput = mMove.ReadValue<Vector2>();
        Vector3 moveValue = new Vector3(movementInput.x, 0f, movementInput.y);
        Vector3 moveDirection = moveValue.x * mMainCamTransform.right.normalized
            + moveValue.z * mMainCamTransform.forward.normalized;

        mGameplayEntity.CmdTriggerAbility(mGAMovementIdx, moveDirection);

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

        mGameplayEntity.CmdTriggerAbility(mGAAimIdx, mCameraLookAt);
    }

    private void OnDestroy()
    {
        if(hasAuthority && Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
    }
}
