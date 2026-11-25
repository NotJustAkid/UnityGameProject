using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    //Rigidbody2D is a component in Unity that allows a GameObject to be affected by 2D physics. It enables the GameObject to move, rotate, and interact with other physics objects in the scene. In this script, the Rigidbody2D component is used to control the player's movement based on input.
    private Rigidbody2D rb;
    //moveInput is a Vector2 variable that stores the player's movement input. It represents the direction and magnitude of the player's movement in the 2D space.
    //e.g., if the player is moving up, moveInput will have a positive y value, and if the player is moving left, moveInput will have a negative x value.
    private Vector2 moveInput;

    //Animation is a component in Unity that allows you to control and play animations on a GameObject. In this script, the Animator component is used to manage the player's walking animation based on the movement input.
    //e.g., when the player is moving, the walking animation will play, and when the player stops moving, the walking animation will stop.
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepsSpeed = 0.3f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            if(rb.linearVelocity != Vector2.zero)
            {
                 rb.linearVelocity = Vector2.zero; //stop movement
                 animator.SetBool("isWalking", false);
                 StopFootsteps();
            }
            return;
        }
 
        rb.linearVelocity = moveInput*moveSpeed;
        bool isMoving = moveInput != Vector2.zero;

        animator.SetBool("isWalking", isMoving);
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
        
        if(isMoving)
        {
            animator.SetFloat("LastInputX",moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
            if (!playingFootsteps) StartFootsteps();
        }
        else 
        {
            StopFootsteps();
        }
    }
    //context is a parameter of type InputAction.CallbackContext that represents the context of the input action that triggered the Move method. It contains information about the input action, such as its phase (started, performed, or canceled) and the value of the input.
    //callback is a method that is called when a specific event occurs. In this case, the Move method is a callback that is triggered when the player provides input for movement.
    //InputAction.CallbackContext is a struct that contains information about the input action that triggered the callback. It provides access to the input value, the phase of the input action, and other relevant data.
    //Example of input 
    void StopMovementAnimations()
    {
        animator.SetBool("isWalking", false);
            
    }
    public void Move(InputAction.CallbackContext context)
    {
         //moveInput is a Vector2 that represents the player's movement input. The context.ReadValue<Vector2>() method reads the input value from the InputAction.CallbackContext and assigns it to moveInput.
        moveInput = context.ReadValue<Vector2>();
    }
    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootsteps),0f,footstepsSpeed);
    }
    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootsteps));
    }
    void PlayFootsteps()
    {
        SoundEffectManager.Play("Footsteps");
    }
}
