using UnityEngine;
using System.Collections;

public class HipCallback : MonoBehaviour
{
    GameObject Player;
    PlayerController pc;

    CamerMovement cam;

    void Start()
    {
        Player = transform.parent.gameObject;
        pc = Player.GetComponent<PlayerController>();
        cam = Camera.main.gameObject.GetComponent<CamerMovement>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            StartCoroutine(hipCallDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            Player.GetComponent<PlayerController>().FinishPlayer();
        }
    }

    IEnumerator hipCallDelay()
    {
        yield return new WaitForSeconds(1.2f);
        pc.RagToNormal();
        cam.followRagdoll = false;
    }
}
