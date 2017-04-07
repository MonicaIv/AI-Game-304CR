using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	const float minPathUpdateTime = .2f; //min time it takes to find/update a path 
	const float pathUpdateMoveThreshold = .5f;

	public Transform target; //depending on state can be player or an empty waypoint
	public float speed = 20; //speed of seeker
	public float turnSpeed = 3; //turning speed
	public float turnDst = 5; //turning distance 
	public float stoppingDst = 10; // stopping distance

    public Transform player; //to attach to player and to target him

    public Transform[] EmptyObjects;
    int WP; //waypoint index

    float nextAttackTime;
    float myCollisionRadius;
    float playerCollisionRadius;
    bool hasTarget = true;
    public enum State { Patrol, Chasing }; //seekers states 
    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;
    State currentState;
    LivingState targetEntity; //function used for dying when get caught 
    Path path;
    Vector3 moveDir;

    // starts the program
	void Start() {
		StartCoroutine (UpdatePath ());
        currentState = State.Patrol;
	}

    //start after start function;
    void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null) //searching for an object with a player tag 
        {
            hasTarget = true;       
            player = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = player.GetComponent<LivingState>();
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            playerCollisionRadius = player.GetComponent<CapsuleCollider>().radius;
        }
    }
    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {   //Starts when it finds a path, calls Follow path coroutine 
		if (pathSuccessful) {                                             //takes in waypoints and bool with info if the path is successful
			path = new Path(waypoints, transform.position, turnDst, stoppingDst);   //if successful stops and starts the coroutine
               StopCoroutine("FollowPath");
			   StartCoroutine("FollowPath");
		}
	}
    private void Update()    //updates player position every frame and checks if it's in the seekers sight radius
    {                           //changes states depending on environment changes
        if (hasTarget)
        {
            if (Time.time > nextAttackTime) //pause before attacking 
            {
                float sqrDstToplayer = (player.position - transform.position).sqrMagnitude;

                 if (sqrDstToplayer > Mathf.Pow(attackDistanceThreshold + myCollisionRadius + playerCollisionRadius, 2) * 250)
                {
                    currentState = State.Patrol;
                }
                else if (sqrDstToplayer < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + playerCollisionRadius, 2) * 250)
                {
                    currentState = State.Chasing;
                    Chasing();
                }

            }
        }
    }
    void Chasing() //changes seeker state to chasing by changing its target; searching for an object with a tag "player"
    {
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        if (currentState == State.Chasing)
        {
            target = player;
            PathRequestManager.RequestPath(new PathRequest(transform.position, player.transform.position, OnPathFound));
        }
    }

    IEnumerator UpdatePath() // Updates the path, called when the seekers are patrolling or chasing
    {                           //creating paths depending on target's positions (old and new)
        Vector3 PlayerLastPos = new Vector3(target.position.x, target.position.y, target.position.z); 
        if (Time.timeSinceLevelLoad < .1f)
        {
            yield return new WaitForSeconds(.1f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, PlayerLastPos, OnPathFound)); 

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime); ; //.2f
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, PlayerLastPos, OnPathFound)); //updating player position
                targetPosOld = target.position; 
            }
        }
    }

    IEnumerator FollowPath() //chenges paths and states depending on envirnment changes again 
    {                       //stops/starts coroutines so they don't confuse with each other
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1;
        moveDir = target.transform.position - transform.position;
        transform.LookAt(path.lookPoints[0]);

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;

                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0) //to slow down a while before seeker reaches it's target
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        if (currentState == State.Patrol)  //Patrolling state calculations 
                        {
                            print("hi");
                            gameObject.GetComponent<Renderer>().material.color = Color.green;

                            if (WP < EmptyObjects.Length)  //looping through the array of emty game objects 
                            {
                                target = EmptyObjects[WP];  //changing the target to the epmty way point on the map
                                followingPath = false;
                                StopCoroutine(UpdatePath()); //update the coroutine
                                StartCoroutine(UpdatePath());
                                WP++; //reset
                            }
                            else
                            {
                                WP = 0; 
                            }
                        }
                    }
                }
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position); //Controlling AI movemnet and facing
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }
    public void OnDrawGizmos() //visal representation of the path 
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
    

