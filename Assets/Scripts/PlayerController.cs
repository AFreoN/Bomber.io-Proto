using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("For Movement Speed and Rotation")]
    public float MoveSpeed = 250f;
    public float RotSpeed = 200f;

    [Header("For Bomb Spawn")]
    public Transform Bomb;
    [Range(0,1)]
    public float SpeedReduction = .5f;
    public float SpawnRate = 4f;
    public float ExplosionTime = 2f;
    float t = 0;

    [Header("For Ragdoll")] // For Ragdoll Settings
    public Transform Hip;
    public float MaxRagdollTime = 3f;
    float rt;

    Rigidbody rb;
    Animator anim;
    [HideInInspector]
    public playerState PlayerState = playerState.Idle;

    //For Checking Player Have bomb or not
    [Header("For Throwing bomb")]
    public float ThrowDistance = 6f;
    public float ThrowTime = .75f;
    Transform b = null;
    bool BombThrowed = false;

    float tempRotSpeed;
    bool haveInput = false;

    Vector2 StartPos, EndPos;
    float angle;

    public GameObject DirectionIndicator;
    public GameObject bombHitIndicator;
    public float HitPointRotSpeed = 100f;
    int RightRot = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        t = SpawnRate;
        rt = MaxRagdollTime;
        angle = 0;
        PlayerState = playerState.Run;

        DirectionIndicator.SetActive(false);
        bombHitIndicator.SetActive(false);
    }

    void Update()
    {
        if(PlayerState  == playerState.Run)
        {
            CheckForBomb();
        }
        else
        {
            t = SpawnRate;
        }

        if(PlayerState == playerState.Run || PlayerState == playerState.Bomb)
        {
            if(DirectionIndicator.activeInHierarchy == false)
            {
                DirectionIndicator.SetActive(true);
            }
        }
        else if(DirectionIndicator.activeInHierarchy == true)
        {
            DirectionIndicator.SetActive(false);
        }

        if(PlayerState != playerState.Bomb)
        {
            if (BombThrowed == true)
            {
                BombThrowed = false;
            }
            if(bombHitIndicator.activeInHierarchy == true)
            {
                bombHitIndicator.SetActive(false);
            }
        }


        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.touchSupported)
            {
                TouchInput();
            }
            else
            {
                EditorMouseInput2();
            }
        }
        else if(haveInput)
        {
            tempRotSpeed = 0;
            haveInput = false;
            anim.SetBool("run", false);
        }
        if (haveInput == false && PlayerState != playerState.Bomb && PlayerState != playerState.Ragdoll)
        {
            PlayerState = playerState.Idle;
        }
    }

    void EditorMouseInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            haveInput = true;
            if(PlayerState != playerState.Bomb && PlayerState != playerState.Ragdoll)
            {
                PlayerState = playerState.Run;
            }
            if(anim.GetBool("run")== false)
            {
                anim.SetBool("run", true);
            }
            StartPos = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            EndPos = Input.mousePosition;
            float delta = (StartPos.x - EndPos.x) / Screen.width * 100;
            StartPos = EndPos;
            tempRotSpeed = RotSpeed * delta;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            tempRotSpeed = 0;
            haveInput = false;
            anim.SetBool("run", false);
        }

        CheckBomb();
    }

    void EditorMouseInput2()
    {
        if (Input.GetMouseButtonDown(0))
        {
            haveInput = true;
            if (PlayerState != playerState.Bomb && PlayerState != playerState.Ragdoll)
            {
                PlayerState = playerState.Run;
            }
            if (anim.GetBool("run") == false)
            {
                anim.SetBool("run", true);
            }
            StartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            EndPos = Input.mousePosition;

            Vector2 dir = EndPos - StartPos;
            if (Vector2.Distance(EndPos, StartPos) > 3)
            {
                angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            }
            else
            {
                angle = 0;
            }
            tempRotSpeed = RotSpeed;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            tempRotSpeed = 0;
            angle = 0;
            haveInput = false;
            anim.SetBool("run", false);
        }

        CheckBomb();
    }

    void TouchInput()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                haveInput = true;
                if (PlayerState != playerState.Bomb && PlayerState != playerState.Ragdoll)
                {
                    PlayerState = playerState.Run;
                }
                if (anim.GetBool("run") == false)
                {
                    anim.SetBool("run", true);
                }
                StartPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                EndPos = touch.position;
                Vector2 dir = EndPos - StartPos;
                if (Vector2.Distance(EndPos, StartPos) > 3)
                {
                    angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
                }
                else
                {
                    angle = 0;
                }
                tempRotSpeed = RotSpeed;
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                tempRotSpeed = 0;
                haveInput = false;
                anim.SetBool("run", false);
            }

            CheckBomb();
        }
    }

    void CheckBomb()
    {
        if(PlayerState == playerState.Ragdoll && b != null)
        {
            b.SetParent(null);
            b.GetComponent<Rigidbody>().useGravity = true;
            b.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            b = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            if (GetComponent<CharactersData>().isAlive)
            {
                FinishPlayer();
            }
        }
    }

    public void FinishPlayer()
    {
        gameObject.GetComponent<CharactersData>().isAlive = false;
        if (gameObject.GetComponent<CharactersData>().LastHitEnemy != null)
        {
            gameObject.GetComponent<CharactersData>().LastHitEnemy.GetComponent<CharactersData>().Power += gameObject.GetComponent<CharactersData>().Power;
            UIFeed.instance.FeedText(gameObject.GetComponent<CharactersData>().LastHitEnemy.GetComponent<CharactersData>().CharacterName + " killed " + "<color=yellow>" + GetComponent<CharactersData>().CharacterName + "</color>");
        }
        else
        {
            UIFeed.instance.FeedText("<color=yellow>"+ GetComponent<CharactersData>().CharacterName + "</color>" + " committed suicide");
        }
        Ranking.instance.PlayerOut(gameObject.GetComponent<CharactersData>().CharacterName);
        Camera.main.transform.GetComponent<CamerMovement>().GoToStartPos = true;
    }

    private void FixedUpdate()
    {
        movements(PlayerState);

    }

    void movements(playerState p)
    {
        switch(p)
        {
            case playerState.Run:
                if (haveInput)
                {
                    //transform.Rotate(Vector3.up * tempRotSpeed * Time.fixedDeltaTime);
                    if(angle != 0)
                    {
                        //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0,angle,0), .3f);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), .4f);
                    }
                    Vector3 v = transform.forward * MoveSpeed *(1+(GetComponent<CharactersData>().Power-1)*0.2f) * Time.deltaTime;
                    v.y = rb.velocity.y;
                    rb.velocity = v;
                }
                break;
            case playerState.Bomb:
                if (haveInput)
                {
                    if (angle != 0)
                    {
                        //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0,angle,0), .3f);
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), .4f);
                    }
                    //transform.Rotate(Vector3.up * tempRotSpeed * Time.fixedDeltaTime);
                    Vector3 v2 = transform.forward * MoveSpeed * (1 + (GetComponent<CharactersData>().Power - 1) * 0.2f) * SpeedReduction * Time.deltaTime;
                    v2.y = rb.velocity.y;
                    rb.velocity = v2;

                    if(bombHitIndicator.activeInHierarchy == false)
                    {
                        bombHitIndicator.SetActive(true);
                        RightRot = Random.Range(0, 2) == 0 ? 1 : -1;
                    }
                }
                else
                {
                    if (b != null && BombThrowed == false)
                    {
                        BombThrowed = true;
                        anim.SetTrigger("throw"); // ANIMATION :- Triggering bomb carry animation to throw animation
                    }
                }
                if(b != null)
                {
                    Vector3 bombhitpos = transform.forward * ThrowDistance + transform.position;
                    bombhitpos.y = .05f;
                    bombHitIndicator.transform.position = bombhitpos;
                    bombHitIndicator.transform.Rotate(Vector3.forward * RotSpeed * RightRot * Time.fixedDeltaTime);
                }
                break;
            case playerState.Ragdoll:
                rt -= Time.deltaTime;
                if(rt < 0)
                {
                    RagToNormal();
                    Camera.main.GetComponent<CamerMovement>().followRagdoll = false;
                }
                break;
        }
    }


    public void ThrowBomb()
    {
        if (b != null)
        {
            b.SetParent(null);
            b.GetComponent<Rigidbody>().useGravity = true;
            b.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //b.GetComponent<Rigidbody>().AddForce(transform.forward * BombThrowForce + Vector3.up * UpwardThrowForce, ForceMode.VelocityChange);
            b.GetComponent<Rigidbody>().velocity = CalculateThrowVelocity(b.position, transform.position + transform.forward * ThrowDistance,ThrowTime);
        }

        if (PlayerState != playerState.Ragdoll)
        {
            PlayerState = playerState.Run;
        }
        b = null;
        anim.speed = 1;
        anim.SetLayerWeight(1, 0);
        BombThrowed = false;

        StopCoroutine(BombtoNormal());
    }

    Vector3 CalculateThrowVelocity(Vector3 StartPos, Vector3 endPos, float Time)
    {
        Vector3 distance = endPos - StartPos;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        //Calculate the total x and y distances of the projectile
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        //Calculating Initial Velocity
        float Vxz = Sxz / Time;
        float Vy = Sy / Time + 0.5f * Mathf.Abs(Physics.gravity.y) * Time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;

    }

    public void RagToNormal()
    {
        //anim.SetBool("bomb", false);
        rt = MaxRagdollTime;
        PlayerState = playerState.Ragdoll;
        gameObject.GetComponent<RagdollManager>().DeactivateRagdoll();
        transform.position = Hip.position;
    }

    public void startRun() // Animation CallBack from wakeup to run
    {
        PlayerState = playerState.Run;
    }


    void CheckForBomb()
    {
        t -= Time.deltaTime;
        if(t <= 0)
        {
            t = SpawnRate;
            anim.SetTrigger("bomb"); //Player animation changes to Bomb Holding animation
        }
    }

    public void SpawnBomb()
    {
        if (GetComponent<CharactersData>().Power <= BombsArray.instance.Bombs.Length)
        {
            b = Instantiate(BombsArray.instance.Bombs[GetComponent<CharactersData>().Power - 1], transform.position + Vector3.up * (transform.localScale.y * 2 + (BombsArray.instance.Bombs[GetComponent<CharactersData>().Power - 1].localScale.y - .5f) / 2) + transform.forward * GetComponent<CharactersData>().Power * .1f, Quaternion.identity);
        }
        else
        {
            b = Instantiate(BombsArray.instance.Bombs[BombsArray.instance.Bombs.Length - 1], transform.position + Vector3.up * (transform.localScale.y * 2 + (BombsArray.instance.Bombs[BombsArray.instance.Bombs.Length - 1].localScale.y - .5f) / 2) + transform.forward * BombsArray.instance.Bombs.Length * .1f, Quaternion.identity);
        }
        //b = Instantiate(Bomb, transform.position + Vector3.up * 2 + transform.forward * .1f, Quaternion.identity);
        b.SetParent(transform);
        b.GetComponent<Bomb>().TriggerTime = ExplosionTime;
        b.GetComponent<Bomb>().ThrowingPlayer = gameObject;
        Physics.IgnoreCollision(b.GetComponent<Collider>(), this.GetComponent<Collider>());
        for(int i=0; i < GetComponent<RagdollManager>().BodyParts.Length; i++)
        {
            Physics.IgnoreCollision(GetComponent<RagdollManager>().BodyParts[i].GetComponent<Collider>(),b.GetComponent<Collider>());
        }

        anim.speed = anim.speed * SpeedReduction;
        anim.SetLayerWeight(1, 1);
        PlayerState = playerState.Bomb;  // Player state changed to Bomb
        StartCoroutine(BombtoNormal());
    }

    IEnumerator BombtoNormal()
    {
        yield return new WaitForSeconds(.4f + ExplosionTime);
        b = null;
        if (PlayerState != playerState.Ragdoll)
        {
            PlayerState = playerState.Run;
        }
        anim.speed = 1;
        anim.SetLayerWeight(1, 0);
    }

    public enum playerState
    {
        Idle,
        Run,
        Bomb,
        Ragdoll
    }
}
