using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float MoveSpeed = 250f;
    public float RotSpeed = 250f;

    [Header("For Bomb")]
    public Transform Bomb;
    public float SpawnRate = 4f;
    [Range(0,1)]
    public float SpeedReduction = .5f;
    public float ExplosionTime = 2f;
    float t = 0;

    [Header("For Throwing Bomb")]  // Parameters for throwing boms
    public float BombThrowForce = 10f;
    public float BombUpwardForce = 2f;
    Transform b;
    bool BombThrowed = false;

    Animator anim;  //Local accessible parameters
    RagdollManager rag;
    Rigidbody rb;

    [Header("For Ragdoll")] //For Setting up ragdoll settings
    public Transform Hip;
    public float MaxRagdollTime = 4f;
    float rt;

    [HideInInspector]
    public EnemyState _enemyState = EnemyState.Run;

    //For Enemy Movements
    [Header("For Movement based on Ground")]
    public Transform Front, Right, Left;
    public float RotTimeDelayMin = .5f;
    public float RotTimeDelayMax = 1f;

    //For Targeting Oppenents
    Transform Target;
    [Header("For Targetting Movement")]
    public float LookRadius = 5f;
    public float TargetingTime = 2f;
    public float RestTime = 3f;
    LayerMask mask;
    float tempTargetingTime, tempRestTime;
    int elementTarget = 0;
    bool firstTargeted = false;

    //For Temp Values
    float tempRotSpeed = 0;
    bool haveInput = false, corStarted = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rag = GetComponent<RagdollManager>();
        rb = GetComponent<Rigidbody>();

        _enemyState = EnemyState.Run; // STATE :- Set the State from Idle to Run
        anim.SetBool("run", true); // ANIMATION :- Set the running animation from Idle Animation
        t = SpawnRate;
        rt = MaxRagdollTime;

        mask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (_enemyState != EnemyState.Ragdoll)
        {
            TargetBasedMovement();
        }

        if(_enemyState != EnemyState.Ragdoll)
        {
            rt = MaxRagdollTime;
        }

        bombChecker();
    }

    void TargetBasedMovement()
    {
        if (Target == null)
        {
            if (firstTargeted == true)
            {
                tempRestTime -= Time.deltaTime;
                if (tempRestTime < 0)
                {
                    GetTarget();
                }
            }
            else
            {
                GetTarget();
            }
            DetectGround();
        }
        else if (Target != null)
        {
            if(Target.GetComponent<CharactersData>().isAlive == false)
            {
                if(Target.GetComponent<PlayerController>() != null && Target.GetComponent<PlayerController>().PlayerState == PlayerController.playerState.Ragdoll)
                {
                    Target = null;
                    return;
                }
                else if(Target.GetComponent<Enemy>() != null && Target.GetComponent<Enemy>()._enemyState == Enemy.EnemyState.Ragdoll)
                {
                    Target = null;
                    return;
                }
                Target = null;
                return;
            }
            tempTargetingTime -= Time.deltaTime;
            if (tempTargetingTime > 0)
            {
                if (Vector3.Distance(transform.position, Target.position) > LookRadius)
                {
                    tempTargetingTime = 0;
                    tempRestTime = RestTime;
                    Target = null;
                    return;
                }
                transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
            }
            else
            {
                tempRestTime = RestTime;
                Target = null;
            }
        }
    }

    void GetTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, LookRadius/2, mask);
        if (cols.Length < 1)
        {
            elementTarget = 0;
            Target = null;
        }
        else if(cols.Length == 1)
        {
            if(cols[0].transform == this.transform)
            {
                elementTarget = 0;
                Target = null;
                return;
            }
            else
            {
                Target = cols[0].transform;
            }
        }
        else
        {
            Transform[] enemies = new Transform[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                enemies[i] = cols[i].transform;
            }
            elementTarget = Random.Range(0, cols.Length);
            Target = enemies[elementTarget];
            if(Target == this.transform)
            {
                if(elementTarget > 0)
                {
                    elementTarget = elementTarget - 1;
                    Target = enemies[elementTarget];
                }
                else
                {
                    elementTarget = Random.Range(1, enemies.Length);
                    Target = enemies[elementTarget];
                }
            }
        }
        if(Target != null && Target != this.transform)
        {
            tempTargetingTime = TargetingTime;
            if(firstTargeted == false)
            {
                firstTargeted = true;
            }
        }
    }

    void DetectGround()
    {
        if (Physics.Raycast(Front.position, Vector3.down, 1))
        {
            if (corStarted == false)
            {
                haveInput = false;
            }
        }
        else if(Physics.Raycast(Right.position , Vector3.down , 1) && corStarted == false)
        {
            haveInput = true;
            tempRotSpeed = RotSpeed;

            corStarted = true;
            StartCoroutine(RotationDelay());
        }
        else if(Physics.Raycast(Left.position , Vector3.down , 1) && corStarted == false)
        {
            haveInput = true;
            tempRotSpeed = -RotSpeed;

            corStarted = true;
            StartCoroutine(RotationDelay());
        }
    }

    IEnumerator RotationDelay()
    {
        yield return new WaitForSeconds(Random.Range(RotTimeDelayMin, RotTimeDelayMax));
        corStarted = false;
        haveInput = false;
        tempRotSpeed = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            if (gameObject.GetComponent<CharactersData>().isAlive == true)
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
            if (gameObject.GetComponent<CharactersData>().LastHitEnemy.GetComponent<PlayerController>() == null)
            {
                UIFeed.instance.FeedText(gameObject.GetComponent<CharactersData>().LastHitEnemy.GetComponent<CharactersData>().CharacterName + " killed " + GetComponent<CharactersData>().CharacterName);
            }
            else
            {
                UIFeed.instance.FeedText("<color=yellow>"+ gameObject.GetComponent<CharactersData>().LastHitEnemy.GetComponent<CharactersData>().CharacterName + "</color>" + " killed " + GetComponent<CharactersData>().CharacterName);
            }
        }
        else
        {
            UIFeed.instance.FeedText(gameObject.GetComponent<CharactersData>().CharacterName + " committed suicide");
        }
        Ranking.instance.PlayerOut(gameObject.GetComponent<CharactersData>().CharacterName);
    }

    private void FixedUpdate()
    {
        movements(_enemyState);
    }

    void movements(EnemyState e)
    {
        switch(e)
        {
            case EnemyState.Run:
                CheckforBomb();  //For Starting Bomb Counting

                if(haveInput)
                {
                    transform.Rotate(Vector3.up * tempRotSpeed * Time.fixedDeltaTime);
                }
                Vector3 v1 = transform.forward * MoveSpeed * (1 + (GetComponent<CharactersData>().Power - 1) * 0.2f) * Time.deltaTime;
                v1.y = rb.velocity.y;
                rb.velocity = v1;

                break;

            case EnemyState.Bomb:
                if (haveInput)
                {
                    transform.Rotate(Vector3.up * tempRotSpeed * Time.fixedDeltaTime);
                }
                Vector3 v2 = transform.forward * MoveSpeed * (1 + (GetComponent<CharactersData>().Power - 1) * 0.2f) * SpeedReduction * Time.deltaTime;
                v2.y = rb.velocity.y;
                rb.velocity = v2;

                if(Target != null)
                {
                    if(b != null && BombThrowed == false)
                    {
                        BombThrowed = true;
                        anim.SetTrigger("throw");  // ANIMATION :- Triggering from bomb carrying animation to throwing animation
                    }
                }
                else
                {
                    if(b != null && BombThrowed == false)
                    {
                        BombThrowed = true;
                        StartCoroutine(throwDelay());
                    }
                }
                break;

            case EnemyState.Ragdoll:
                rt -= Time.deltaTime;
                if(rt < 0)
                {
                    rt = MaxRagdollTime;
                    RagToNormal();
                }
                break;
        }
    }

    void bombChecker()
    {
        if(_enemyState == EnemyState.Ragdoll && b != null)
        {
            b.SetParent(null);
            b.GetComponent<Rigidbody>().useGravity = true;
            b.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            b = null;
        }
    }

    IEnumerator throwDelay()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1.1f));
        anim.SetTrigger("throw");
    }

    public void ThrowBomb()
    {
        if (b != null)
        {
            b.SetParent(null);
            b.GetComponent<Rigidbody>().useGravity = true;
            b.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            b.GetComponent<Rigidbody>().AddForce(transform.forward * BombThrowForce + Vector3.up * BombUpwardForce, ForceMode.Impulse);
        }

        _enemyState = EnemyState.Run; // STATE :- Changing from Bomb state to Run STate
        b = null;
        anim.speed = 1;
        anim.SetLayerWeight(1, 0);
        BombThrowed = false;

        StopCoroutine(BombToNormal());
    }

    public void RagToNormal()
    {
        rt = MaxRagdollTime;
        _enemyState = EnemyState.Idle;  // STATE :- Changing from Run to Idle State
        rag.DeactivateRagdoll();
        transform.position = Hip.position;
    }

    public void startRun()
    {
        _enemyState = EnemyState.Run;  // STATE :- Changing from Ragdoll to Run State
    }

    void CheckforBomb()
    {
        t -= Time.deltaTime;
        if(t <= 0)
        {
            t = SpawnRate;
            anim.SetTrigger("bomb");  // ANIMATION :- Triggers enemy animations to bomb lifting animation
        }
    }

    public void SpawnBomb()
    {
        if (GetComponent<CharactersData>().Power <= BombsArray.instance.Bombs.Length)
        {
            b = Instantiate(BombsArray.instance.Bombs[GetComponent<CharactersData>().Power - 1], transform.position + Vector3.up * (transform.localScale.y*2 + (BombsArray.instance.Bombs[GetComponent<CharactersData>().Power - 1].localScale.y - .5f) / 2) + transform.forward * GetComponent<CharactersData>().Power * .1f, Quaternion.identity);
        }
        else
        {
            b = Instantiate(BombsArray.instance.Bombs[BombsArray.instance.Bombs.Length - 1], transform.position + Vector3.up * (transform.localScale.y * 2 + (BombsArray.instance.Bombs[BombsArray.instance.Bombs.Length - 1].localScale.y - .5f) / 2) + transform.forward * BombsArray.instance.Bombs.Length * .1f, Quaternion.identity);
        }
        //b = Instantiate(Bomb, transform.position + Vector3.up * 1.8f + transform.forward * .1f, Quaternion.identity);
        b.SetParent(transform);
        b.GetComponent<Bomb>().TriggerTime = ExplosionTime;
        b.GetComponent<Bomb>().ThrowingPlayer = gameObject;
        Physics.IgnoreCollision(b.GetComponent<Collider>(), this.GetComponent<Collider>());
        for(int i= 0; i < rag.BodyParts.Length; i++)
        {
            Physics.IgnoreCollision(rag.BodyParts[i].GetComponent<Collider>(), b.GetComponent<Collider>());
        }

        anim.speed = anim.speed * SpeedReduction;
        anim.SetLayerWeight(1, 1);
        _enemyState = EnemyState.Bomb;  // STATE :- Set the Enemy state to Bomb State
        StartCoroutine(BombToNormal());
    }

    IEnumerator BombToNormal()
    {
        yield return new WaitForSeconds(.4f + ExplosionTime);
        if (_enemyState != EnemyState.Ragdoll)
        {
            _enemyState = EnemyState.Run;
        }
        anim.speed = 1;
        anim.SetLayerWeight(1, 0);
        b = null;
        BombThrowed = false;
    }

    public enum EnemyState
    {
        Idle,
        Run,
        Bomb,
        Ragdoll
    }
}
