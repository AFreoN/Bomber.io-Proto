using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    public float ExplosiomForce = 10f;
    public float UpwardForce = 2f;
    public float Radius = 4f;

    [Header("Blast Particles")]  // For Instantiating Particles on Blast
    public Transform BlastPS;
    public Transform BlastSplashPS;
    public Transform BlastCenterPS;

    public Transform BombDecal;

    [Header("For Radius Indication")]  // For Radius Indication
    public Transform RadiusIndicator;
    public float RadiusRotSpeed = 10f;
    int RotDir;

    [Header("For Showing Timer on Canvas")]  // For Instantiating canvas on top of the bomb
    public Transform BombCanvas;
    Transform curCanvas;
    Transform mainCamera;
    Image FillerImg;
    float t;

    Transform curIndicator;

    public float TriggerTime = 3f;
    [HideInInspector]
    public bool Blasted = false;

    [HideInInspector]
    public GameObject ThrowingPlayer;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        curCanvas = Instantiate(BombCanvas, new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z), Quaternion.identity);
        curCanvas.LookAt(mainCamera);

        FillerImg = curCanvas.GetChild(0).GetChild(0).GetComponent<Image>();
        FillerImg.fillAmount = 0;

        Blasted = false;
    }

    void Start()
    {
        curIndicator = Instantiate(RadiusIndicator, new Vector3(transform.position.x, .05f, transform.position.z), RadiusIndicator.rotation);
        curIndicator.localScale = new Vector3(Radius*2, Radius*2, 1);
        RotDir = Random.Range(0, 2) == 0 ? 1 : -1;
        t = TriggerTime;

        StartCoroutine(Explode());
    }

    private void Update()
    {
        curIndicator.position = new Vector3(transform.position.x, .05f, transform.position.z);
        curIndicator.transform.Rotate(Vector3.forward * RadiusRotSpeed * Time.deltaTime);

        curCanvas.position = new Vector3(transform.position.x, transform.position.y + .4f, transform.position.z);
        curCanvas.LookAt(mainCamera);

        ImageCountDown();
    }

    void ImageCountDown()
    {
        t -= Time.deltaTime;
        float fillValue = 1 - t / TriggerTime;
        FillerImg.fillAmount = fillValue;

        if(fillValue < .25f && fillValue >= 0)
        {
            FillerImg.color = Color.green;
        }
        else if(fillValue < .5f && fillValue >= .25f)
        {
            FillerImg.color = Color.yellow;
        }
        else if(fillValue < .75f && fillValue >= .5f)
        {
            FillerImg.color = new Color32(255, 82, 0, 255);
        }
        else
        {
            FillerImg.color = Color.red;
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(TriggerTime);
        ExplodeCall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "grund")
        {
            ExplodeCall();
        }
    }

    public void ExplodeCall()
    {
        Blasted = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

        Rigidbody[] rig = new Rigidbody[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            rig[i] = colliders[i].GetComponent<Rigidbody>();

            if (colliders[i].gameObject.tag == "Player")
            {
                if (colliders[i].gameObject.GetComponent<Enemy>() != null)
                {
                    colliders[i].GetComponent<Enemy>()._enemyState = Enemy.EnemyState.Ragdoll;
                    colliders[i].GetComponent<RagdollManager>().ActivateRagdoll();
                    if(colliders[i].gameObject != ThrowingPlayer)
                    {
                        colliders[i].GetComponent<CharactersData>().LastHitEnemy = ThrowingPlayer;
                    }
                    else
                    {
                        colliders[i].GetComponent<CharactersData>().LastHitEnemy = null;
                    }
                }
                if (colliders[i].gameObject.GetComponent<PlayerController>() != null)
                {
                    colliders[i].GetComponent<PlayerController>().PlayerState = PlayerController.playerState.Ragdoll;
                    colliders[i].GetComponent<RagdollManager>().ActivateRagdoll();
                    if (ThrowingPlayer != colliders[i].gameObject)
                    {
                        colliders[i].GetComponent<CharactersData>().LastHitEnemy = ThrowingPlayer;
                    }
                    else
                    {
                        colliders[i].GetComponent<CharactersData>().LastHitEnemy = null;
                    }
                }
            }

            if(i == colliders.Length - 1)
            {
                StartCoroutine(delayedBlast(rig));
            }
        }

        //StartCoroutine(delayedBlast(rig));
    }

    IEnumerator delayedBlast(Rigidbody[] rig)
    {
        yield return new WaitForFixedUpdate();
        foreach (Rigidbody rb in rig)
        {
            if (rb != null)
            {
                if (rb.gameObject.tag != "bomb")
                {
                    rb.AddExplosionForce(ExplosiomForce * rb.mass, transform.position, Radius, UpwardForce);
                }
                else if(rb.transform != this)
                {
                    if (rb.GetComponent<Bomb>().Blasted == false)
                    {
                        rb.GetComponent<Bomb>().ExplodeCall();
                    }
                }
            }
        }
        CamerMovement.ShouldShake = true; // For Shaking the Camera

        Instantiate(BlastSplashPS, transform.position, BlastSplashPS.rotation);
        Instantiate(BlastPS, transform.position, BlastPS.rotation);
        Instantiate(BlastCenterPS, transform.position, BlastCenterPS.rotation);

        if (transform.position.y < .2f)
        {
            float RanValue = Random.Range(.5f, 1.3f);
            CreateDecal(new Vector3(transform.position.x, .05f, transform.position.z), new Vector3(RanValue, RanValue, 1));
        }
        else if(transform.position.y < 1.2f)
        {
            float RanValue = Random.Range(.4f, .6f);
            CreateDecal(new Vector3(transform.position.x, .05f, transform.position.z), new Vector3(RanValue, RanValue,1));
        }
        else
        {
            CreateDecal(new Vector3(transform.position.x, 0.05f, transform.position.z), new Vector3(.3f, .3f, 1));
        }

        Destroy(curIndicator.gameObject);
        Destroy(curCanvas.gameObject);
        Destroy(gameObject);
    }

    void CreateDecal(Vector3 spawnPos, Vector3 scale)  //For creating Bomb decal to show the bomb effect on ground
    {
        Transform bd = Instantiate(BombDecal, spawnPos, BombDecal.rotation);
        bd.localScale = scale;
        Destroy(bd.gameObject, 3f);
    }

    private void OnDestroy()
    {
        if(curIndicator != null)
        {
            Destroy(curIndicator.gameObject);
        }
    }
}
