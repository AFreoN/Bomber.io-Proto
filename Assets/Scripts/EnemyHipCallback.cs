using UnityEngine;
using System.Collections;

public class EnemyHipCallback : MonoBehaviour
{
    GameObject Player;
    Enemy e;

    void Start()
    {
        Player = transform.parent.gameObject;
        e = Player.GetComponent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            StartCoroutine(HipCallDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            Player.GetComponent<Enemy>().FinishPlayer();
        }
    }

    IEnumerator HipCallDelay()
    {
        yield return new WaitForSeconds(.5f);
        e.RagToNormal();
    }
}
