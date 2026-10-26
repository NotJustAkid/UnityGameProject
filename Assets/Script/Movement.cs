 using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed;

    public Animator animator;

    private Vector3 direction;

   //get input
   //apply movement
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal,vertical);

        

        AnimateMovement(direction);
    }
 private void FixedUpdate()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    void AnimateMovement(Vector3 direction)
    {
        if (direction != null)
        {
            if(direction.magnitude > 0)
            {
                animator.SetBool("isMoving", true);
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
       
    }

}
