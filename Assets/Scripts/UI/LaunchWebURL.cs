using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LaunchWebURL : MonoBehaviour {

    public void OpenURL(string _url)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            LaunchURL(_url);
        }
        else
        {
            Application.OpenURL(_url);
        }
    }

    public void OpenTwitter()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            LaunchURL("https://www.twitter.com/theshapeofclay");

            return;
        }

        float startTime = Time.timeSinceLevelLoad;

        //Application.OpenURL("twitter:///user?screen_name=johnnyjacques");

        if (Time.timeSinceLevelLoad - startTime <= 1f)
        {
            Application.OpenURL("https://www.twitter.com/johnnyjacques");
        }
    }

    public void OpenLinkedIn()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            LaunchURL("https://www.linkedin.com/in/johnnyjacques/");

            return;
        }
        else
        {
            Application.OpenURL("https://www.linkedin.com/in/johnnyjacques/");
        }
    }

    [DllImport("__Internal")]
    private static extern void LaunchURL(string url);
}
