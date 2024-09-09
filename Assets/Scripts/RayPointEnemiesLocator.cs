using UnityEngine;

public class RayPointEnemiesLocator : MonoBehaviour
{
    public Transform Front, Right, Left;
    Vector3 frontPos, rightPos, leftPos;

    Vector3 curScale;

    void Start()
    {
        curScale = transform.localScale;
        frontPos = Front.localPosition;
        rightPos = Right.localPosition;
        leftPos = Left.localPosition;
    }

    void LateUpdate()
    {
        if(curScale != transform.localScale)
        {
            curScale = transform.localScale;
            Front.localPosition = new Vector3(frontPos.x / transform.localScale.x, frontPos.y / transform.localScale.y, frontPos.z / transform.localScale.z);
            Right.localPosition = new Vector3(rightPos.x / transform.localScale.x, rightPos.y / transform.localScale.y, rightPos.z / transform.localScale.z);
            Left.localPosition = new Vector3(leftPos.x / transform.localScale.x, leftPos.y / transform.localScale.y, leftPos.z / transform.localScale.z);
        }
    }
}
