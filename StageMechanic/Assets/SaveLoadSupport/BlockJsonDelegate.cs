﻿/*  
 * Copyright (C) Catherine. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System;

[DataContract(Name="Block")]
public class BlockJsonDelegate {

	Block _block;
	public Block Block {
		get {
			return _block;
		}
		set {
			_block = value;
		}
	}

	public BlockJsonDelegate( Block block ) {
		_block = block;
	}

	public BlockJsonDelegate() {
		_block = new Block ();
	}

	[DataMember(Name="Name",Order=1)]
	public string Name {
		get {
			Debug.Assert (_block != null);
			return _block.Name;
		}
		set {
			Debug.Assert (_block != null);
			_block.Name = value;
		}
	}

	[DataMember(Name="Type",Order=2)]
	public string Type {
		get {
			Debug.Assert (_block != null);
			return _block.Type.ToString ();
		}
		set {
			try {
				Block.BlockType type = (Block.BlockType)Enum.Parse (typeof(Block.BlockType), value);
				if (Enum.IsDefined (typeof(Block.BlockType), type))
					_block.Type = type;
			} catch (ArgumentException e) {
				Debug.Log (e.Message);
			}
		}
	}

	[DataMember(Name="RelativePosition",Order=4)]
	public Vector3 RelativePosition {
		get {
			Debug.Assert (_block != null);
			return _block.transform.localPosition;
		}
		set {
			Debug.Assert (_block != null);
			_block.transform.localPosition = value;
		}
	}
}