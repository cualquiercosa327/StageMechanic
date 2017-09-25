﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public GameObject Stage;
	public GameObject Cursor;
	public GameObject Camera;

	public BlockManager GetBlockManager() {
		return (BlockManager)Stage.GetComponent (typeof(BlockManager));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float vert = Input.GetAxis ("Vertical");
		float hori = Input.GetAxis ("Horizontal");

		// Buttons for creating blocks
		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
			GetBlockManager().CreateBlockAtCursor (Cursor, Block.BlockType.Basic);
		} else if (Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Joystick1Button1)) {
			GetBlockManager().CreateBlockAtCursor (Cursor, Block.BlockType.Ice);
		} else if (Input.GetKeyDown (KeyCode.Alpha3 )|| Input.GetKeyDown(KeyCode.Joystick1Button2)) {
			GetBlockManager().CreateBlockAtCursor (Cursor, Block.BlockType.Heavy1);
		} else if (Input.GetKeyDown (KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Joystick1Button3)) {
			GetBlockManager().CreateBlockAtCursor (Cursor, Block.BlockType.Bomb1);
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			GetBlockManager().CreateBlockAtCursor (Cursor, Block.BlockType.Bomb2);
		} else if (Input.GetKeyDown (KeyCode.Delete) || Input.GetKeyDown(KeyCode.Joystick1Button7)) {
			if(GetBlockManager().ActiveObject != null)
				//TODO use a method of BlockManager to do this
				Destroy (GetBlockManager().ActiveObject);
		} else if (Input.GetKeyDown (KeyCode.Home)  || Input.GetKeyDown(KeyCode.Joystick1Button6)) {
			if (GetBlockManager().ActiveObject != null) {
				//TODO use a method of BlockManager to do this
				Block block = (Block)GetBlockManager().ActiveObject.GetComponent (typeof(Block));
				block.Item = Instantiate (GetBlockManager().StartLocationIndicator, Cursor.transform.position + new Vector3 (0, 0.5F, 0), Quaternion.Euler (0, 180, 0)) as GameObject;
			}
		} else if (Input.GetKeyDown (KeyCode.End)) {
			if (GetBlockManager().ActiveObject != null) {
				//TODO use a method of BlockManager to do this
				Block block = (Block)GetBlockManager().ActiveObject.GetComponent (typeof(Block));
				block.Item = Instantiate (GetBlockManager().GoalLocationIndicator, Cursor.transform.position + new Vector3 (0, 0.5F, 0), Quaternion.Euler (0, 180, 0)) as GameObject;
			}
		}

		// Cursor movement cotrol
		// Keyboard & XBox 360 Input
		// TODO update ActiveObject based on cursor position using colliders
		else if (Input.GetKeyDown (KeyCode.UpArrow) || vert > 0) {
			Cursor.transform.position += new Vector3 (0, 1, 0);
			Input.ResetInputAxes ();
		} else if (Input.GetKeyDown (KeyCode.DownArrow) || vert < 0) {
			Cursor.transform.position += new Vector3 (0, -1, 0);
			Input.ResetInputAxes ();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow) || hori < 0) {
			Cursor.transform.position += new Vector3 (-1, 0, 0);
			Input.ResetInputAxes ();
		} else if (Input.GetKeyDown (KeyCode.RightArrow) || hori > 0) {
			Cursor.transform.position += new Vector3 (1, 0, 0);
			Input.ResetInputAxes ();
		} else if (Input.GetKeyDown (KeyCode.Comma) || Input.GetKeyDown(KeyCode.Joystick1Button4)) {
			Input.ResetInputAxes ();
			Cursor.transform.position += new Vector3 (0, 0, -1);
		} else if (Input.GetKeyDown (KeyCode.Period) || Input.GetKeyDown(KeyCode.Joystick8Button5)) {
			Input.ResetInputAxes ();
			Cursor.transform.position += new Vector3 (0, 0, 1);
		}
	}
}
