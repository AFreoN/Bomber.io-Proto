using UnityEngine;
using System.Collections.Generic;

public class MaterialAssiger : MonoBehaviour
{
    public List<GoArray> EnemyBodyParts;
    public Material[] materials;

    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GoArray ga in EnemyBodyParts)
        {
            for(int i=0; i < ga.allParts.Length; i++)
            {
                ga.allParts[i].GetComponent<Renderer>().sharedMaterial = materials[index];
            }
            index++;
        }
    }
}

[System.Serializable]
public class GoArray
{
    public GameObject[] allParts;
}
