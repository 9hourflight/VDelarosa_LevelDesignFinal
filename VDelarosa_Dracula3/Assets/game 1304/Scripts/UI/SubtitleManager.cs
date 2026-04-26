using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    //public static string subtitleText;
    public static Text instanceTextObject;
    public static SubtitleManager instance;
    public static Queue<subtitlePackage> subtitleQueue;
    public static bool isDisplayingSubtitle;

    private void Awake()
    {
        instance = this;
        instanceTextObject = GetComponent<Text>();
        if (subtitleQueue == null)
            subtitleQueue = new Queue<subtitlePackage>();
    }
    public static float DisplaySubtitle(string text, float duration) //, bool useAutoDuration)
    {
        instance.gameObject.SetActive(true);
        subtitlePackage tempPackage = new subtitlePackage();
        tempPackage.text = text;
        if (duration <= 0) //(useAutoDuration)
            tempPackage.duration = text.Length / 5f;
        else
            tempPackage.duration = duration;
        subtitleQueue.Enqueue(tempPackage);
        instance.UpdateSubtitles();
        if (duration == 0)
            return text.Length / 5f;
        else
            return duration;
    }

    public void UpdateSubtitles()
    {
        subtitlePackage tempPackage;
        if (!isDisplayingSubtitle)
        {
            if (subtitleQueue.Count > 0)
            {
                tempPackage = subtitleQueue.Dequeue();
                instanceTextObject.text = tempPackage.text;
                Invoke("subtitleTimeUp", tempPackage.duration);
                isDisplayingSubtitle = true;
            }
            else
            {
                instance.gameObject.SetActive(false);
            }
        }
    }

    private void subtitleTimeUp()
    {
        isDisplayingSubtitle = false;
        instanceTextObject.text = "";
        Invoke("UpdateSubtitles", 0.05f);
    }    
}
