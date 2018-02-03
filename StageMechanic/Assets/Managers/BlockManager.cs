﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    #region Serialization

    public void ClearForUndo()
    {
        foreach (IBlock block in BlockCache)
        {
            Destroy(block.GameObject);
        }
        BlockCache.Clear();
        EventManager.Clear();
    }

    public PlatformJsonDelegate GetPlatformJsonDelegate()
    {
        return new PlatformJsonDelegate(ActiveFloor);
    }
    public PlatformBinaryDelegate GetPlatformBinaryDelegate()
    {
        return new PlatformBinaryDelegate(ActiveFloor);
    }



    #endregion

    // Unity Inspector variables
    public GameObject Stage;
    public GameObject CursorPrefab;
    public GameObject BasicPlatformPrefab;
    public GameObject StartLocationIndicator;
    public GameObject GoalLocationIndicator;
    public Cathy1BlockFactory Cathy1BlockFactory;
    public ParticleSystem Fog;


    /// <summary>
    /// There should only ever be one BlockManager in the scene - it manages all blocks for all platforms and loaded stages. As such it
    /// can be used statically via this property when accessing methods that are not already marked static.
    /// </summary>
    /// TODO Singleton flamewar
    public static BlockManager Instance { get; private set; }


    /// <summary>
    /// Used by many classes to determine current game state, eventually this will be moved into
    /// a seperate GameManager class or something but for now this is pretty much THE way to
    /// determine if the application is currently in PlayMode or EditMode. Note that this property
    /// is read-pnly. Use BlockManager.TogglePlayMode() to change the application state to/from
    /// PlayMode and EditMode. This too will change in an upcoming revision.
    /// </summary>
    /// TODO: Move this out to a separate GameManager class
    public static bool PlayMode { get; private set; } = false;

    /// <summary>
    /// Toggles the application between PlayMode and EditMode. This will show/hide the cursor,
    /// inform PlayerManager of the new mode, and if entering play mode record the starting state
    /// of the blocks to facilitate player death and test-playing while creating (restore on exiting
    /// PlayMode)
    /// </summary>
    /// TODO Combine this with the PlayMode property and move it to GameManager class
    /// TODO change the way the button mapping box behaves
    public void TogglePlayMode()
    {
        PlayMode = !PlayMode;
        if (PlayMode)
        {
            LogController.Log("Start!");
            UIManager.Instance.BlockInfoBox.gameObject.SetActive(false);
            Serializer.RecordStartState();
            Fog.gameObject.SetActive(PlayerPrefs.GetInt("Fog", 0) == 1);
        }
        else
        {
            //Reset blocks to their pre-PlayMode state
            if (Serializer.HasStartState())
                Serializer.ReloadStartState();
            //UIManager.Instance.BlockInfoBox.gameObject.SetActive(true);
            Fog.gameObject.SetActive(false);
        }
        UIManager.RefreshButtonMappingDialog();
        PlayerManager.Instance.PlayMode = PlayMode;
        Cursor.SetActive(!PlayMode);

    }

    #region BlockAccounting
    internal static List<IBlock> BlockCache = new List<IBlock>();

    /// <summary>
    /// Read-only property that technically returns the number of blocks
    /// in the internal cache.
    /// </summary>
    public static int BlockCount
    {
        get
        {
            return BlockCache.Count;
        }
    }

    /// <summary>
    /// When in EditMode, returns the block, if any, that is currently under the cursor,
    /// or null if the cursor is not on a block. Setting this property in EditMode will
    /// move the cursor to the position of the block.
    /// 
    /// When in PlayMode, returns the block associated with player 1, this will be either
    /// the block the player is standing on or sidled on (if sidling)
    /// </summary>
    /// TODO Support IBlock interface
    public Cathy1Block ActiveBlock
    {
        get
        {
            if (PlayMode)
                return PlayerManager.Player(0)?.GameObject?.GetComponent<Cathy1PlayerCharacter>()?.CurrentBlock?.GameObject?.GetComponent<Cathy1Block>();
            else
                return GetBlockAt(Cursor.transform.position)?.GameObject?.GetComponent<Cathy1Block>();
        }
        set
        {
            if (!PlayMode)
                Cursor.transform.position = value.Position;
        }
    }

    public static void Clear()
    {
        //Clear all cached data
        foreach (IBlock block in BlockCache)
        {
            Destroy(block.GameObject);
        }
        BlockCache.Clear();
        blockGroups.Clear();
        blockToGroupMapping.Clear();
        PlayerManager.Clear();
        EventManager.Clear();
        Serializer.ClearUndoStates();
        Serializer.LastAccessedFileName = null;

        ActiveFloor.transform.position = Vector3.zero;
        Cursor.transform.position = new Vector3(0f, 1f, 0f);
    }

    // Create a basic block at the current cursor position

    public static IBlock CreateBlockAtCursor(Cathy1Block.BlockType type = Cathy1Block.BlockType.Basic)
    {
        Debug.Assert(Instance != null);
        Debug.Assert(Cursor != null);
        Cathy1Block block = Instance.Cathy1BlockFactory.CreateBlock(Cursor.transform.position, Cursor.transform.rotation, type, ActiveFloor) as Cathy1Block;
        BlockCache.Add(block);
        Serializer.AutoSave();
        return block;
    }

    public static IBlock CreateBlockAtCursor(string palette, string type)
    {
        return CreateBlockAt(Cursor.transform.position, palette, type);
    }

    public static IBlock CreateBlockAt(Vector3 position, string palette, string type)
    {
        Debug.Assert(Instance != null);
        Debug.Assert(Cursor != null);
        if (palette == "Cathy1 Internal")
        {
            Cathy1Block block = Instance.Cathy1BlockFactory.CreateBlock(position, Cursor.transform.rotation, type, ActiveFloor) as Cathy1Block;
            BlockCache.Add(block);
            Serializer.AutoSave();
            return block;
        }
        return null;
    }

    public static void DestroyBlock(IBlock block)
    {
        if (blockToGroupMapping.ContainsKey(block))
        {
            Debug.Assert(blockGroups.ContainsKey(blockToGroupMapping[block]));
            blockGroups[blockToGroupMapping[block]].Remove(block);
            blockToGroupMapping.Remove(block);
        }
        BlockCache.Remove(block);
        block.GameObject.SetActive(false);
        Destroy(block.GameObject);
    }

    #endregion

    /// <summary>
    /// This is a hacky method from early on in the implementation. In theory it should return
    /// the GameObject of whatever is under the cursor. In actuality it usually only does this
    /// correctly if its a block.
    /// </summary>
    /// TODO: Move cursor when set
    public GameObject ActiveObject
    {
        get
        {
            return GetBlockAt(Cursor.transform.position)?.GameObject;
        }
        set
        {
        }
    }

    /// <summary>
    /// The Cursor used in Edit Mode.
    /// </summary>
    /// TODO Right now this gets moved from the top level scene, do we want this behavior?
    private static GameObject _cursor;
    public static GameObject Cursor
    {
        get
        {
            return _cursor;
        }
        set
        {
            _cursor = value;
        }
    }

    /// <summary>
    /// Used for cycling the default block type while in EditMode. This will be moved to
    /// Cathy1BlockFactory or similar class later.
    /// </summary>
    private static Cathy1Block.BlockType _blockCycleType = Cathy1Block.BlockType.Basic;
    public static Cathy1Block.BlockType BlockCycleType
    {
        get
        {
            return _blockCycleType;
        }
        set
        {
            _blockCycleType = value;
        }
    }

    /// <summary>
    /// Used for cycling the default block type while in EditMode. This will be moved to
    /// Cathy1BlockFactory or similar class later.
    /// </summary>
    public static Cathy1Block.BlockType NextBlockType()
    {
        if (BlockCycleType >= Cathy1Block.BlockType.Goal)
        {
            BlockCycleType = Cathy1Block.BlockType.Basic;
            return BlockCycleType;
        }
        return ++BlockCycleType;
    }

    /// <summary>
    /// Used for cycling the default block type while in EditMode. This will be moved to
    /// Cathy1BlockFactory or similar class later.
    /// </summary>
    public static Cathy1Block.BlockType PrevBlockType()
    {
        if (BlockCycleType <= Cathy1Block.BlockType.Basic)
        {
            BlockCycleType = Cathy1Block.BlockType.Goal;
            return BlockCycleType;
        }
        return --BlockCycleType;
    }

    /// <summary>
    /// Used to support Cathy-2 style rotatable floors and other multi-platform implementations
    /// This is to be implemented in the future
    /// Right now it only contains the BlockManager.ActiveFloor
    /// </summary>
    private List<GameObject> _rotatableFloors = new List<GameObject>();
    public List<GameObject> RotatableFloors
    {
        get
        {
            return _rotatableFloors;
        }
        set
        {
            _rotatableFloors = value;
        }
    }

    /// <summary>
    /// Currently this will always be the platform on which the stage rests. When BlockManager.RotatableFloors
    /// is implemented later this will be set to the platform currently selected by the cursor or occupied by
    /// the player.
    /// </summary>
    private static GameObject _activeFloor;
    public static GameObject ActiveFloor
    {
        get
        {
            return _activeFloor;
        }
        set
        {
            _activeFloor = value;
        }
    }

    public static void RotatePlatform(int x, int y, int z)
    {
        ActiveFloor.transform.Rotate(x, y, z, Space.Self);
        Instance.StartCoroutine(Instance.rotateCleanup());
    }

    IEnumerator rotateCleanup()
    {
        yield return new WaitForEndOfFrame();
        IBlock[] blocks = ActiveFloor.GetComponentsInChildren<IBlock>();
        yield return new WaitForEndOfFrame();
        Debug.Log("rotating " + blocks.Length + " blocks");
        foreach (IBlock block in blocks)
        {
            block.GameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            block.Rotation = Quaternion.identity;
            (block as AbstractBlock).SetGravityEnabledByMotionState();
        }
        yield return new WaitForEndOfFrame();
    }

    private void Awake()
    {
        Instance = this;
    }

    // Called when the BlockManager is intantiated, when the Level Editor is loaded
    void Start()
    {
        // Create the cursor
        ActiveFloor = Instantiate(BasicPlatformPrefab, new Vector3(0, 0f, 3f), new Quaternion(0, 0, 0, 0));
        ActiveFloor.name = "Platform";
        ActiveFloor.transform.SetParent(Stage.transform, false);
        RotatableFloors.Add(ActiveFloor);
        Cursor = CursorPrefab;
        Cursor.transform.SetParent(Stage.transform, false);
    }

    public void DestroyActiveObject()
    {
        if (ActiveObject != null)
        {
            if (ActiveObject.GetComponent<IBlock>() != null)
                DestroyBlock(ActiveObject.GetComponent<IBlock>());
            else
                Destroy(ActiveObject);
            Serializer.AutoSave();
        }
    }

    public static AbstractBlock GetBlockAt(Vector3 position, float radius = 0.01f)
    {

        return Utility.GetGameObjectAt<AbstractBlock>(position, radius);
    }

    public static AbstractBlock GetBlockNear(Vector3 position, float radius = 0.01f)
    {
        return Utility.GetGameObjectNear<AbstractBlock>(position, radius);
    }

    public static List<AbstractBlock> GetBlocskNear(Vector3 position, float radius = 0.01f)
    {
        return Utility.GetGameObjectsNear<AbstractBlock>(position, radius);
    }

    public static List<IBlock> GetBlocksOfType(string type = null)
    {
        List<IBlock> ret = new List<IBlock>();
        foreach (Transform child in ActiveFloor.transform)
        {
            IBlock block = child.gameObject.GetComponent<IBlock>();
            if (block != null && (type == null || block.TypeName == type))
                ret.Add(block);
        }
        return ret;
    }

    #region Block groups
    static Dictionary<IBlock, int> blockToGroupMapping = new Dictionary<IBlock, int>();
    static Dictionary<int, List<IBlock>> blockGroups = new Dictionary<int, List<IBlock>>();

    public static List<IBlock> BlockGroup(int groupNumber)
    {
        if (groupNumber < 0)
            return new List<IBlock>();
        Debug.Assert(blockGroups.ContainsKey(groupNumber));
        return blockGroups[groupNumber];
    }

    public static void AddBlockToGroup(IBlock block, int groupNumber)
    {
        Debug.Assert(block != null);
        if (groupNumber < 0)
        {
            if (blockToGroupMapping.ContainsKey(block))
            {
                blockGroups[blockToGroupMapping[block]].Remove(block);
                blockToGroupMapping.Remove(block);
                return;
            }
            return;
        }
        if (blockToGroupMapping.ContainsKey(block) && blockToGroupMapping[block] != groupNumber)
        {
            Debug.Assert(blockGroups.ContainsKey(blockToGroupMapping[block]));
            blockGroups[blockToGroupMapping[block]].Remove(block);
            if (!blockGroups.ContainsKey(groupNumber))
                blockGroups.Add(groupNumber, new List<IBlock>());
            if (blockGroups[groupNumber] == null)
                blockGroups[groupNumber] = new List<IBlock>();
            blockGroups[groupNumber].Add(block);
            blockToGroupMapping[block] = groupNumber;
            cakeslice.Outline outline = block.GameObject.GetComponent<cakeslice.Outline>();
            if (outline == null)
                outline = block.GameObject.GetComponentInChildren<cakeslice.Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.color = groupNumber;
            }
        }
        else if (!blockToGroupMapping.ContainsKey(block))
        {
            blockToGroupMapping.Add(block, groupNumber);
            if (!blockGroups.ContainsKey(groupNumber))
                blockGroups.Add(groupNumber, new List<IBlock>());
            if (blockGroups[groupNumber] == null)
                blockGroups[groupNumber] = new List<IBlock>();
            blockGroups[groupNumber].Add(block);
            cakeslice.Outline outline = block.GameObject.GetComponent<cakeslice.Outline>();
            if (outline == null)
                outline = block.GameObject.GetComponentInChildren<cakeslice.Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.color = groupNumber;
            }
        }
    }

    public static void RemoveBlockFromGroup(IBlock block)
    {
        AddBlockToGroup(block, -1);
    }

    public static int BlockGroupNumber(IBlock block)
    {
        if (blockToGroupMapping.ContainsKey(block))
            return blockToGroupMapping[block];
        return -1;
    }

    public static bool CanMoveGroup(int groupNumber, Vector3 direction, int distance = 1)
    {
        Debug.Assert(groupNumber >= 0);
        Debug.Assert(blockGroups.ContainsKey(groupNumber));
        foreach (IBlock block in BlockGroup(groupNumber))
        {
            if (!block.CanBeMoved(direction, distance))
                return false;
        }
        return true;
    }

    public static bool MoveGroup(int groupNumber, Vector3 direction, int distance = 1)
    {
        if (!CanMoveGroup(groupNumber, direction, distance))
            return false;

        foreach (IBlock block in BlockGroup(groupNumber))
        {

            IBlock neighbor = GetBlockAt(block.Position + direction);
            if (neighbor != null)
            {
                if (BlockGroupNumber(neighbor) < 0)
                    neighbor.Move(direction, distance);
                else if (BlockGroupNumber(neighbor) != groupNumber)
                    MoveGroup(BlockGroupNumber(neighbor), direction, distance);
            }
            AbstractBlock ab = block.GameObject?.GetComponent<AbstractBlock>();
            if (ab == null)
                block.Position += direction;
            else
                ab.StartCoroutine(ab.AnimateMove(ab.Position, ab.Position + direction, 0.2f * ab.MoveWeight(direction, distance),false));

        }
        return true;
    }

    public static bool CanBeMoved(IBlock block, Vector3 direction, int distance = 1)
    {
        if (BlockGroupNumber(block) < 0)
            return block.CanBeMoved(direction, distance);
        return CanMoveGroup(BlockGroupNumber(block), direction, distance);
    }

    public static bool Move(IBlock block, Vector3 direction, int distance = 1, bool push = false)
    {
        if (BlockGroupNumber(block) < 0)
            return block.Move(direction, distance, push);
        return MoveGroup(BlockGroupNumber(block), direction, distance);
    }
    #endregion

}
