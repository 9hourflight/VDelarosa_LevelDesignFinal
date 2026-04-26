using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager  
{	
	private static bool isInitialized = false;
	public static GameObject player;
	public static GAME1304PlayerController playerController;
	
	public static bool isPaused;

    public static bool isPlayingFromCam
    {
        set
        {
            init();
            _isPlayingFromCam = value;
        }
        get
        {
            init();
            return _isPlayingFromCam;
        }
    }

	

    private static bool _isPlayingFromCam = false;

	public static void reinit()
	{
		isInitialized = false;
		init ();
	}

	public static void init() 
	{
		if(isInitialized)
			return;
		isInitialized = true;
		
	}

	public static void registerPlayer(GameObject p)
	{
		init();
		player = p;
		playerController = p.GetComponent<GAME1304PlayerController>();
		NPCManager.updatePlayerRef();
	}

	
		
	

	public static void pause()
	{
		isPaused = true;
		Time.timeScale = 0;
	}

	public static void unPause()
	{		
		isPaused = false;
		Time.timeScale = 1;
	}

	public static void togglePause()
	{
		if(isPaused)
			unPause();
		else
			pause();
	}

	public static Inventory GetPlayerInventory()
    {
		return player.GetComponent<ObjectInteractionBehavior>().inventory;
    }
}
