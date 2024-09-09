using UnityEngine;

public class CamerMovement : MonoBehaviour
{
    public Transform Target;
    PlayerController pc;
    public Transform Hip;
    [Range(0,1)]
    public float LerpSpeed = .3f;
    Vector3 offset;

    [HideInInspector]
    public bool followRagdoll = false;

    public static bool ShouldShake = false;
    public float ShakePower = .1f;
    public float Duration = .3f;
    float tempDuration;

    Vector3 StartPosition;
    [HideInInspector]
    public bool GoToStartPos = false;

    private void Awake()
    {
        StartPosition = new Vector3(0, 15.7f, -22.7f);
    }

    void Start()
    {
        pc = Target.GetComponent<PlayerController>();
        offset = transform.position - Target.position;
        tempDuration = Duration;
        ShouldShake = false;
    }

    void LateUpdate()
    {
        if (GoToStartPos == false)
        {
            CameraFollow();
        }
        else if(transform.position != StartPosition)
        {
            transform.position = Vector3.Lerp(transform.position, StartPosition, .1f);
        }
    }

    void CameraFollow()
    {
        if (pc.PlayerState != PlayerController.playerState.Ragdoll && followRagdoll == false)
        {
            transform.position = Vector3.Lerp(transform.position, offset + Target.position, LerpSpeed);
        }
        else
        {
            followRagdoll = true;
            transform.position = Hip.position + offset;
        }

        if (ShouldShake)
        {
            tempDuration -= Time.deltaTime;
            if (tempDuration > 0)
            {
                transform.position = transform.position + Random.insideUnitSphere * ShakePower;
            }
            else
            {
                ShouldShake = false;
                tempDuration = Duration;
            }
        }
    }
}
