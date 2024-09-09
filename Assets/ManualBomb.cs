using UnityEngine;

public class ManualBomb : MonoBehaviour
{
    public float ExplosionForce = 2000f;
    public float UpwardForce = 2f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartExplode();
        }
    }


    void StartExplode()
    {
        Collider[] colliders = Physics.OverlapSphere(Vector3.zero, 30);

        foreach(Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();


            if(rb != null)
            {
                if (rb.gameObject.tag == "Player")
                {
                    rb.GetComponent<RagdollManager>().ActivateRagdoll();
                    if (rb.GetComponent<PlayerController>() != null)
                    {
                        rb.GetComponent<PlayerController>().PlayerState = PlayerController.playerState.Ragdoll;
                    }
                    else if (rb.GetComponent<Enemy>() != null)
                    {
                        rb.GetComponent<Enemy>()._enemyState = Enemy.EnemyState.Ragdoll;
                    }
                }

                rb.AddExplosionForce(ExplosionForce, Vector3.zero, 30, UpwardForce);
            }
        }
    }
}
