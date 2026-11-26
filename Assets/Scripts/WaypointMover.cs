using System.Collections;
using Mono.Cecil;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    public Transform WaypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex;
    private bool isWaiting; 
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        waypoints = new Transform[WaypointParent.childCount];
        for(int i = 0; i < WaypointParent.childCount; i++)
        {
            waypoints[i] = WaypointParent.GetChild(i);
        }
    }
    // Update is called once per frame
    void Update()
    {
     if(PauseController.IsGamePaused || isWaiting)
        {
            animator.SetBool("isWalking",false);
            return;
        }   
        MoveToWaypoint();
    }
    void MoveToWaypoint()
    {
    Transform target = waypoints[currentWaypointIndex];
    Vector2 direction = (target.position - transform.position).normalized;
    
    transform.position = Vector2.MoveTowards(transform.position,target.position,moveSpeed*Time.deltaTime);
    animator.SetFloat("InputX",direction.x);
    animator.SetFloat("InputY",direction.y);
    animator.SetBool("isWalking",direction.magnitude > 0f);
    

    if(Vector2.Distance(transform.position,target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }   
    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking",false);
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length-1);
        isWaiting = false;
    }
}

