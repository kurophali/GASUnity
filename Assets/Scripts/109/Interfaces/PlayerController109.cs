using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController109 : MonoBehaviour
{
    [SerializeField] Vector2 mMovementInput;
    [SerializeField] GameObject mPlayer;
    IGameplayEntity109 mPlayerGE;
    PlayerInput mInput;
    InputAction mMove;
    
    // Start is called before the first frame update
    void Start()
    {
        mInput = GetComponent<PlayerInput>();
        mMove = mInput.actions["Movement"];
        mPlayerGE = mPlayer.GetComponent<IGameplayEntity109>();
        mPlayerGE.VFInitAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        mMovementInput = mMove.ReadValue<Vector2>();
        mPlayerGE.TriggerAbility(0, new Vector4(mMovementInput.x, 0, mMovementInput.y, 0));
    }
}
