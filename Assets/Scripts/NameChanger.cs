using System.Collections.Generic;
using UnityEngine;

public class NameChanger : MonoBehaviour
{
    public CharactersData[] Enemies;

    public List<string> Names;

    // Start is called before the first frame update
    void Start()
    {
        foreach(CharactersData cd in Enemies)
        {
            int r = Random.Range(0, Names.Count);
            cd.CharacterName = Names[r];
            Names.Remove(Names[r]);
        }
    }
}
