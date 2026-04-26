using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum NPCMouthState { neutral, talking, angry, sad, happy };

[Serializable]
public class MouthAnimState
{
    public NPCMouthState state;
    public List<Sprite> animFrames;
}
public class NPCMouth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public List<MouthAnimState> mouthStateAnimations;
    private MouthAnimState currentAnimState;
    private int animIndex = 0;
    private NPCMouthState currentState = NPCMouthState.neutral;
    //TODO: roll this into the anim states
    private float frameDuration = 0.125f;
    private float currentFrameTime;
    private void Awake()
    {
        currentAnimState = null;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetState(NPCMouthState.neutral);
    }
    public void SetState(NPCMouthState newState)
    {
        animIndex = 0;
        currentFrameTime = 0;
        currentAnimState = null;
        foreach(MouthAnimState mas in mouthStateAnimations)
        {
            if (mas.state == newState)
                currentAnimState = mas;
        }
    }

    //TODO: roll in the sound sense tech to flap the mouth
    private void Update()
    {
        if (currentAnimState == null)
            return;

        currentFrameTime += Time.deltaTime;
        if(currentFrameTime>frameDuration)
        {
            currentFrameTime = 0;
            animIndex += 1;
            if (animIndex >= currentAnimState.animFrames.Count)
                animIndex = 0;

        }
        spriteRenderer.sprite = currentAnimState.animFrames[animIndex];
    }
}
