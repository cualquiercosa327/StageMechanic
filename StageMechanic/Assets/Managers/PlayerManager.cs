﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public GameObject Player1Prefab;
    public GameObject Player2Prefab;
    public GameObject Player3Prefab;

    public static List<Cathy1PlayerStartLocation> PlayerStartLocations { get; set; } = new List<Cathy1PlayerStartLocation>();
    public static List<Cathy1PlayerCharacter> Avatars { get; set; } = new List<Cathy1PlayerCharacter>();
    internal static PlayerManager Instance;

    internal static bool _playMode = false;
    public bool PlayMode
    {
        get
        {
            return _playMode;
        }
        set
        {
            _playMode = value;
            if(_playMode)
            {
                SpawnPlayers();
            }
            else
            {
                HidePlayers();
            }
        }
    }

    public static int PlayerCount()
    {
        if (Avatars != null && Avatars.Count > 0)
            return Avatars.Count;
        return PlayerStartLocations.Count;
    }

    public static Cathy1PlayerCharacter Player1()
    {
        if (Avatars.Count > 0)
            return Avatars[0];
        return null;
    }

    public static void Clear()
    {
        HidePlayers();
        PlayerStartLocations.Clear();
    }


    public static void Player1PlayDieSound()
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
            Avatars[0].PlayDieSound();
    }

    public static void Player1PlayGameOverSound()
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
            Avatars[0].PlayGameOverSound();
    }

    public static void Player1PlayThudSound()
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
            Avatars[0].PlayThudSound();
    }

    public static void PlayersReset()
    {
        HidePlayers();
        BlockManager.ReloadStartState();
        SpawnPlayers();
        UIManager.RefreshButtonMappingDialog();
        LogController.Log("YOU DIED");
    }

    public static void PlayersThudReset()
    {
        HidePlayers();
        //BlockManager.ReloadStartState();
        //SpawnPlayers();
        Player1PlayThudSound();
        LogController.Log("YOU DIED");
    }

    public static void OnUndoStart()
    {
        HidePlayers();
    }

    public static void OnUndoFinish()
    {
        SpawnPlayers();
    }

    public static void HidePlayers()
    {
        Debug.Log("Hiding");
        foreach (Cathy1PlayerCharacter player in Avatars) {
            Destroy(player.gameObject);
        }
        Avatars.Clear();
    }

    public static void SpawnPlayers()
    {
        Debug.Assert(PlayerStartLocations != null);
        if (PlayerStartLocations.Count == 0)
            return;
        if (PlayerStartLocations.Count > 0)
        {
            if (Avatars.Count == 0)
                Avatars.Add(Instantiate(Instance.Player1Prefab, PlayerStartLocations[0].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[0].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>());
            else
                Avatars[0] = Instantiate(Instance.Player1Prefab, PlayerStartLocations[0].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[0].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>();
            Debug.Log("Spawning player 1 at " + PlayerStartLocations[0].transform.position);
            LoadKeybindings(0);
        }
        if (PlayerStartLocations.Count > 1)
        {
            if (Avatars.Count == 1)
                Avatars.Add(Instantiate(Instance.Player2Prefab, PlayerStartLocations[1].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[1].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>());
            else
                Avatars[1] = Instantiate(Instance.Player2Prefab, PlayerStartLocations[1].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[1].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>();
            Debug.Log("Spawning player 2 at " + PlayerStartLocations[1].transform.position);
            LoadKeybindings(1);
        }
        if (PlayerStartLocations.Count > 2)
        {
            if (Avatars.Count == 2)
                Avatars.Add(Instantiate(Instance.Player3Prefab, PlayerStartLocations[2].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[2].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>());
            else
                Avatars[1] = Instantiate(Instance.Player3Prefab, PlayerStartLocations[2].transform.position + new Vector3(0f, 0.5f, 0f), PlayerStartLocations[2].transform.rotation, Instance.transform).GetComponent<Cathy1PlayerCharacter>();
            Debug.Log("Spawning player32 at " + PlayerStartLocations[2].transform.position);
            LoadKeybindings(2);
        }
    }


    // Use this for initialization
    void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Vector3 Player1Location()
    {
        if(Avatars.Count>0 && Avatars[0] != null)
        {
            return Avatars[0].transform.position;
        }
        //TODO not do it this way
        return new Vector3(-255, -255, -255);
    }

    public static Vector3 Player1FacingDirection()
    {
        Debug.Assert(Avatars.Count > 0 && Avatars[0] != null);
        return Avatars[0].FacingDirection;
    }

    public static void SetPlayer1FacingDirection(Vector3 direction)
    {
        Debug.Assert(Avatars.Count > 0 && Avatars[0] != null);
        Avatars[0].FacingDirection = direction;
    }

    public static string Player1StateName()
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
        {
            return Avatars[0].CurrentMoveState.ToString();
        }
        return "Hiding";
    }

    public static Cathy1PlayerCharacter.State Player1State()
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
        {
            return Avatars[0].CurrentMoveState;
        }
        return Cathy1PlayerCharacter.State.Idle;
    }

    public static void SetPlayer1State(Cathy1PlayerCharacter.State state)
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
            Avatars[0].CurrentMoveState = state;
    }

    public static void SetPlayer1Location(Vector3 location)
    {
        if (Avatars.Count > 0 && Avatars[0] != null)
            Avatars[0].Teleport(location);
    }

    public static void Player1BoingyTo( Vector3 location )
    {
        Avatars[0].Boingy(location);
    }

    public static void Player1SlideForward()
    {
        Avatars[0].SlideForward();
    }

    public static float Player1ApplyInput(List<string> inputs, Dictionary<string,string> parameters = null)
    {
        if (Avatars == null || Avatars.Count == 0)
            return 0f;
        return Avatars[0].ApplyInput(inputs, parameters);
    }

    public static float Player2ApplyInput(List<string> inputs, Dictionary<string, string> parameters = null)
    {
        if (Avatars == null || Avatars.Count < 2)
            return 0f;
        return Avatars[1].ApplyInput(inputs, parameters);
    }

    public static float Player3ApplyInput(List<string> inputs, Dictionary<string, string> parameters = null)
    {
        if (Avatars == null || Avatars.Count < 3)
            return 0f;
        return Avatars[2].ApplyInput(inputs, parameters);
    }

    public static float PlayerApplyInput(int playerNumber, List<string> inputs, Dictionary<string, string> parameters = null)
    {
        if (playerNumber == 0)
            return Player1ApplyInput(inputs, parameters);
        else if (playerNumber == 1)
            return Player2ApplyInput(inputs, parameters);
        else
            return Player3ApplyInput(inputs, parameters);
    }

    private static List<Dictionary<string, string[]>> keybindings = new List<Dictionary<string, string[]>>();

    private static void LoadKeybindings( int playerNumber )
    {
        Debug.Assert(playerNumber >= 0);
        Debug.Assert(Avatars != null);
        Debug.Assert(Avatars.Count > playerNumber);
        Debug.Assert(Avatars[playerNumber] != null);
        Debug.Assert(keybindings != null);

        Dictionary<string, string[]> actual = new Dictionary<string, string[]>();

        Dictionary<string, string[]> suggested = Avatars[playerNumber].SuggestedInputs;
        foreach(KeyValuePair<string,string[]> item in suggested) {
            if (PlayerPrefs.HasKey("Player" + playerNumber + "_Input_" + item.Key))
                actual.Add(item.Key, PlayerPrefs.GetString("Input_Mapping_Player" + playerNumber + "_" + item.Key).Split(','));
            else
                actual.Add(item.Key, item.Value);
        }

        while (keybindings.Count <= playerNumber)
            keybindings.Add(null);

        keybindings[playerNumber] = actual;
    }

    public static Dictionary<string,string[]> Player1InputOptions
    {
        get
        {
            if (Avatars == null || Avatars.Count == 0)
                return null;
            Debug.Assert(keybindings != null);
            Debug.Assert(keybindings.Count >= 1);
            return keybindings[0];
        }
    }

    public static Dictionary<string, string[]> Player2InputOptions
    {
        get
        {
            if (Avatars == null || Avatars.Count == 0)
                return null;
            Debug.Assert(keybindings != null);
            Debug.Assert(keybindings.Count >= 2);
            return keybindings[1];
        }
    }

    public static Dictionary<string, string[]> Player3InputOptions
    {
        get
        {
            if (Avatars == null || Avatars.Count == 0)
                return null;
            Debug.Assert(keybindings != null);
            Debug.Assert(keybindings.Count >= 3);
            return keybindings[2];
        }
    }

    public static Dictionary<string,string[]> PlayerInputOptions(int playerNumber)
    {
        if (playerNumber == 0)
            return Player1InputOptions;
        else if (playerNumber == 1)
            return Player2InputOptions;
        else
            return Player3InputOptions;
    }

    public static void RemoveKeyBinding(int playerNumber, string action = null, string keyName = null)
    {
        Dictionary<string, string[]> newBindings = new Dictionary<string,string[]>(keybindings[playerNumber]);
        foreach (KeyValuePair<string, string[]> bindings in keybindings[playerNumber])
        {
            List<string> newList = new List<string>();
            foreach(string key in bindings.Value)
            {
                if ((action != null || action != bindings.Key)  && key != keyName)
                    newList.Add(key);
            }
            newBindings[bindings.Key] = newList.ToArray();
        }
        keybindings[playerNumber] = newBindings;
    }

    public static void AddKeyBinding(int playerNumber, string action, string keyName)
    {
        Dictionary<string, string[]> newBindings = new Dictionary<string, string[]>(keybindings[playerNumber]);
        foreach (KeyValuePair<string, string[]> bindings in keybindings[playerNumber])
        {
            List<string> newList = new List<string>(bindings.Value);
            if(bindings.Key == action || action == null)
                newList.Add(keyName);
            newBindings[bindings.Key] = newList.ToArray();
        }
        keybindings[playerNumber] = newBindings;
    }
}
