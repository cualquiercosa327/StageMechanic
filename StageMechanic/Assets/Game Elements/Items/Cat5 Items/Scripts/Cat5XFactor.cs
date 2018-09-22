﻿/*  
 * Copyright (C) 2018 You're Perfect Studio. All rights reserved.  
 * Licensed under the BSD 3-Clause License.
 * See LICENSE file in the project root for full license information.
 * See CONTRIBUTORS file in the project root for full list of contributors.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat5XFactor : Cat5AbstractItem {

	public int MaxClimbHeight = 2;
	public float Duration = 15f;
	private AudioClip Sound;
	private ParticleSystem Animation;

	public override void ApplyTheme(Cat5ItemTheme theme)
	{
		Debug.Assert(theme.XFactorPlaceholder != null);
		Model1 = theme.XFactorPlaceholder;
		Model2 = theme.XFactorObject;
		Sound = theme.XFactorActiveAudio;
		Animation = theme.XFactorActiveAnimation;
	}

	public override Dictionary<string, DefaultValue> DefaultProperties
	{
		get
		{
			Dictionary<string, DefaultValue> ret = base.DefaultProperties;
			ret.Add("Climb Height", new DefaultValue { TypeInfo = typeof(int), Value = "2" });
			ret.Add("Duration", new DefaultValue { TypeInfo = typeof(float), Value = "15" });
			return ret;
		}
	}

	public override Dictionary<string, string> Properties
	{
		get
		{
			Dictionary<string, string> ret = base.Properties;
			if (MaxClimbHeight != 2)
				ret.Add("Climb Height", MaxClimbHeight.ToString());
			if (Duration != 15f)
				ret.Add("Duration", Duration.ToString());
			return ret;
		}
		set
		{
			base.Properties = value;
			if (value.ContainsKey("Climb Height"))
				MaxClimbHeight = int.Parse(value["Climb Height"]);
			if (value.ContainsKey("Duration"))
				Duration = float.Parse(value["Duration"]);
		}
	}

	public override void OnPlayerActivate(IPlayerCharacter player)
	{
		base.OnPlayerActivate(player);
		if (Sound != null)
			AudioEffectsManager.PlaySound(Sound,0.25f);
		if (Animation != null)
			VisualEffectsManager.PlayEffect(player, Animation, 1, 15);
		(player as Cathy1PlayerCharacter).MaxClimbHeight = MaxClimbHeight;
		ItemManager.DelayedDestroyItem(this, Duration);
	}

	public override void OnGameModeChanged(GameManager.GameMode newMode, GameManager.GameMode oldMode)
	{
		base.OnGameModeChanged(newMode, oldMode);
		if (newMode == GameManager.GameMode.StageEdit)
			ShowModel(1);
		else if (newMode == GameManager.GameMode.Play)
			ShowModel(2);
	}
}
