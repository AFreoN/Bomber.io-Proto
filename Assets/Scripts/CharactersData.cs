using UnityEngine;
using UnityEngine.UI;

public class CharactersData : MonoBehaviour
{
    public string CharacterName;
    public int Power = 1;
    public bool isAlive = true;
    [HideInInspector]
    public GameObject LastHitEnemy;

    int tempPower;
    bool StartScaling = false;
    public float LerpSpeed = .3f;
    Vector3 FinalScale;

    public Transform playerCanvas;
    Transform pc;
    Transform mainCamera;

    PlayerController plaCon;
    Enemy enem;

    private void Start()
    {
        tempPower = Power;
        pc = Instantiate(playerCanvas, transform.position + Vector3.up * transform.localScale.y * 2, playerCanvas.rotation);
        if (GetComponent<PlayerController>() != null)
        {
            pc.GetChild(0).GetComponent<Text>().text = "<color=yellow>" + CharacterName + "</color>";
            plaCon = GetComponent<PlayerController>();
        }
        else
        {
            pc.GetChild(0).GetComponent<Text>().text = CharacterName;
            enem = GetComponent<Enemy>();
        }

        mainCamera = Camera.main.transform;

    }
    private void Update()
    {
        if(tempPower != Power)
        {
            tempPower = Power;
            FinalScale = new Vector3(1 + Power * .2f, 1 + Power * .2f, 1 + Power * .2f);
            StartScaling = true;
        }

        if(StartScaling)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, FinalScale, LerpSpeed);
            if(transform.localScale == FinalScale)
            {
                StartScaling = false;
            }
        }

        if (isAlive)
        {
            SetPlayerCanvasPosition();
        }
        else if(pc != null)
        {
            Destroy(pc.gameObject);
            gameObject.SetActive(false);
        }
    }

    void SetPlayerCanvasPosition()
    {
        pc.LookAt(mainCamera);
        if (plaCon != null && plaCon.PlayerState != PlayerController.playerState.Ragdoll)
        {
            pc.position = transform.position + Vector3.up * transform.localScale.y * 1.8f;
        }
        else
        {
            pc.position = transform.GetChild(0).position + Vector3.up * transform.localScale.y * 1.8f;
        }

        if (enem != null && enem._enemyState != Enemy.EnemyState.Ragdoll)
        {
            pc.position = transform.position + Vector3.up * transform.localScale.y * 1.8f;
        }
        else
        {
            pc.position = transform.GetChild(0).position + Vector3.up * transform.localScale.y * 1.8f;
        }
    }
}
