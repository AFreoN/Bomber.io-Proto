using UnityEngine;

public class BombsArray : MonoBehaviour
{
    public static BombsArray instance;
    public Transform[] Bombs;

    private void Awake()
    {
        instance = this;
    }
}
