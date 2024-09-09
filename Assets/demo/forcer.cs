using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forcer : MonoBehaviour
{
    public float forceApplied = 10f;
    public float UpwardForce = 1f;
    public float Radius;
    public Rigidbody Mainrb;
    public Rigidbody HipsRB;

    public Transform ForcePoint;

    Rigidbody[] allBodyParts;
    
    // Start is called before the first frame update
    void Start()
    {
        Mainrb.isKinematic = false;
        HipsRB.isKinematic = true;
        HipsRB.detectCollisions = false;

        allBodyParts = Mainrb.GetComponent<RagdollManager>().BodyParts;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Mainrb.isKinematic = true;
            Mainrb.GetComponent<Animator>().enabled = false;
            Mainrb.GetComponent<RagdollManager>().ActivateRagdoll();
            HipsRB.isKinematic = false;
            HipsRB.detectCollisions = true;

            HipsRB.AddExplosionForce(forceApplied * HipsRB.mass, ForcePoint.position, Radius, UpwardForce);
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            Mainrb.GetComponent<RagdollManager>().ActivateRagdoll();

            foreach(Rigidbody rb in allBodyParts)
            {
                rb.AddExplosionForce(forceApplied * rb.mass * .1f, ForcePoint.position, Radius, UpwardForce);
            }
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            Mainrb.GetComponent<RagdollManager>().ActivateRagdoll();

            Collider[] collider = Physics.OverlapSphere(ForcePoint.position, Radius);

            foreach(Collider col in collider)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();

                if(rb != null)
                {
                    rb.AddExplosionForce(forceApplied * rb.mass * .1f, ForcePoint.position, Radius, UpwardForce);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            Mainrb.GetComponent<RagdollManager>().ActivateRagdoll();

            Collider[] collider = Physics.OverlapSphere(ForcePoint.position, Radius);

            Rigidbody[] rb = new Rigidbody[collider.Length];

            for (int i=0; i < collider.Length; i++)
            {
                rb[i] = collider[i].GetComponent<Rigidbody>();
            }

            foreach(Rigidbody r in rb)
            {
                if (r != null)
                {
                    r.AddExplosionForce(forceApplied * r.mass * .1f, ForcePoint.position, Radius, UpwardForce);
                }
            }
        }
    }
}
