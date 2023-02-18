using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Astronaut2DUserControl : MonoBehaviour
{
    private Animator anim;

    //variables for input
    private bool jump = false;
    private bool thrust = false;
    private bool thrustOnce = false;
    [SerializeField]
    private float maxThrustDuration = 1.0f; //cut thrust after how long
    private float thrustDuration = 0.0f; //duration of the current thrust session
    private float h = 0f; //horizontal value

    private Locomotion astronaut;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        astronaut = GetComponent<Locomotion>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>(); //receiving input vector
        h = move.x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jump = context.performed;
        if (context.started && !astronaut.IsGrounded && !thrustOnce) //astronaut cannot start thrust while grounded
        {
            //start new thrust session
            thrustDuration = 0.0f;
            thrust = true;
            thrustOnce = true;
        }
        if (context.canceled)
        {
            thrustDuration = 0.0f;
            thrust = false;
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.instance.PauseOrPlay();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //passing values to animator
        anim.SetFloat("h", Mathf.Abs(h));
        anim.SetBool("thrust", thrust);
    }

    private void FixedUpdate()
    {
        astronaut.Move(h, jump, thrust);
        jump = false;
        if (thrust) 
        {
            thrustDuration += Time.deltaTime;
            if (thrustDuration >= maxThrustDuration)
            {
                //reset thrust session
                thrust = false;
                thrustDuration = 0f;
            }
        }
        if (astronaut.IsGrounded)
        {
            thrustOnce = false;
        }
    }
}
