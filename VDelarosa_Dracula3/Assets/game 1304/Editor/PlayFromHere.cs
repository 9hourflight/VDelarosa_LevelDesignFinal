using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayFromHere : MonoBehaviour
{
    [MenuItem("Tools/GAME1304/Play From Here")]
    static void playFromHere()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
        GAME1304PlayerController pc = player.GetComponent<GAME1304PlayerController>();
        if (pc == null)
            return;
        Camera sceneCam = SceneView.lastActiveSceneView.camera;
        EditorApplication.isPlaying = true;
        
        pc.setTransform(sceneCam.transform);
        //pc.spawn();
    }

}
