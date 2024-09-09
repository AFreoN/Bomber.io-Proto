using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFeed : MonoBehaviour
{
    public static UIFeed instance;
    public Transform OverlayCanvas;

    public Transform KillFeedText;
    Text curText;

    float ti = 1;
    bool feeded = false;

    List<string> feedTextList = new List<string>();

    private void Awake()
    {
        instance = this;
    }

    public void FeedText(string t)
    {
        feedTextList.Add(t);
    }

    public void localFeedText(string t)
    {
        if (feeded == false)
        {
            feeded = true;
            Transform kft = Instantiate(KillFeedText, KillFeedText.position, Quaternion.identity);
            kft.SetParent(OverlayCanvas);
            kft.localScale = Vector3.one;
            kft.GetComponent<RectTransform>().localPosition = new Vector3(0, 400, 0);
            kft.GetComponent<Animator>().enabled = true;
            curText = kft.GetComponent<Text>();
            curText.text = t;

            ti = 1f;
            Destroy(kft.gameObject, 1.2f);
        }
    }

    private void Update()
    {
        if(feedTextList.Count > 0)
        {
            if(feeded == false)
            {
                localFeedText(feedTextList[0]);
                feedTextList.Remove(feedTextList[0]);
            }
            else if(feeded)
            {
                ti -= Time.deltaTime;
                if(ti < 0)
                {
                    feeded = false;
                }
            }
        }
    }

    IEnumerator DelayedFeed()
    {
        yield return new WaitForSeconds(1);

    }
}
