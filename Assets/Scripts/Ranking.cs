using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public static Ranking instance;

    public Text[] NameText;

    List<string> LeaderBoardNames = new List<string>();

    GameObject[] AllPlayers;

    int t = 0;

    private void Awake()
    {
        instance = this;

        AllPlayers = GameObject.FindGameObjectsWithTag("Player");
    }

    public void PlayerOut(string playerName)
    {
        t++;
        LeaderBoardNames.Add(playerName);
        Debug.Log(t + "  " +playerName);
    }

    private void Update()
    {
        if(LeaderBoardNames.Count == AllPlayers.Length-1 && GameManager.GameFinished == false)
        {
            GameManager.GameFinished = true;

            for(int j=0; j < AllPlayers.Length; j++)
            {
                if(AllPlayers[j].GetComponent<CharactersData>().isAlive)
                {
                    LeaderBoardNames.Add(AllPlayers[j].GetComponent<CharactersData>().CharacterName);
                }
            }

            for(int i=0; i < AllPlayers.Length; i++)
            {
                NameText[i].text = LeaderBoardNames[Mathf.Abs(i -(AllPlayers.Length-1))];
                //LeaderBoardNames.Remove(LeaderBoardNames[LeaderBoardNames.Count-1]);
            }

            GameManager.instance.ShowResults();
        }
    }
}
