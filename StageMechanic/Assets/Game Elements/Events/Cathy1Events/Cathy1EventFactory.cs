﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cathy1EventFactory : MonoBehaviour, IEventFactory {

    public GameObject PlayerStartPrefab;
    public GameObject PlayerGoalPrefab;
    public GameObject EnemySpawnPrefab;

    public Cathy1AbstractEvent CreateEvent(Vector3 globalPosition, Quaternion globalRotation, Cathy1AbstractEvent.EventType type, Cathy1Block parent = null)
    {
        string oldName = String.Empty;

        GameObject[] collidedGameObjects =
            Physics.OverlapSphere(globalPosition, 0.1f)
                .Except(new[] { GetComponent<BoxCollider>() })
                .Select(c => c.gameObject)
                .ToArray();


        foreach (GameObject obj in collidedGameObjects)
        {
            Cathy1Block bl = obj.GetComponent<Cathy1Block>();
            // In this case the event is being created at the same location that
            // a block currently exists. Instead the system will create the event
            // as being located above the block by one unit and will make the event
            // a child of the block.
            if (bl != null)
            {
                if (parent == null)
                    parent = bl;
                globalPosition = bl.Position + new Vector3(0, 0.5f, 0);
                globalRotation = bl.Rotation;
            }

            Cathy1AbstractEvent oldEvent = obj.GetComponent<Cathy1AbstractEvent>();
            if(oldEvent != null) {
                oldName = oldEvent.Name;
                Destroy(oldEvent);
            }

        }

        GameObject newEvent = null;

        switch (type)
        {
            case Cathy1AbstractEvent.EventType.PlayerStart:
                newEvent = Instantiate(PlayerStartPrefab, globalPosition, globalRotation, parent.transform);
                break;
            case Cathy1AbstractEvent.EventType.Goal:
                newEvent = Instantiate(PlayerGoalPrefab, globalPosition, globalRotation, parent.transform);
                break;
            //TODO checkpoint, cutom, enemies
            
        }

        Debug.Assert(newEvent != null);
        Cathy1AbstractEvent ev = newEvent.GetComponent<Cathy1AbstractEvent>();
        Debug.Assert(ev != null);
        //TODO some kind of Parent property in Event
        if (parent != null)
        {
            ev.transform.parent = parent.transform;
            Cathy1Block bl = parent as Cathy1Block;
            if(bl != null)
            {
                bl.FirstEvent = ev;
            }
        }
        if (oldName != String.Empty)
            ev.Name = oldName;
        return ev;
    }

    public IEvent CreateEvent(Vector3 globalPosition, Quaternion globalRotation, int eventTypeIndex, GameObject parent = null)
    {
        throw new System.NotImplementedException();
    }

    public IEvent CreateEvent(Vector3 globalPosition, Quaternion globalRotation, string eventTypeName, GameObject parent = null)
    {
        throw new System.NotImplementedException();
    }

    public IEvent CreateEvent(int eventTypeIndex, IBlock parent)
    {
        throw new System.NotImplementedException();
    }

    public IEvent CreateEvent(string eventTypeName, IBlock parent)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
