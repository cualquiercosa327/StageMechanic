﻿using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Platforms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaScriptingManager : MonoBehaviour
{
	public LuaScriptingManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		Script.GlobalOptions.Platform = new LimitedPlatformAccessor();

		UserData.RegisterProxyType<LuaProxy_AbstractBlock, AbstractBlock>(r => new LuaProxy_AbstractBlock(r));
		UserData.RegisterProxyType<LuaProxy_BlockManager, BlockManager>(_ => new LuaProxy_BlockManager());
		UserData.RegisterType<AbstractPlayerCharacter>();
		UserData.RegisterType<Alert>();
		LuaCustomConverters.RegisterAll();
	}

	public static Script BaseScript
	{
		get
		{
			Script script = new Script(CoreModules.Preset_SoftSandbox);
			DynValue blockManager = UserData.Create(BlockManager.Instance);
			DynValue alert = UserData.Create(new Alert());
			script.Globals.Set("BlockManager", blockManager);
			script.Globals.Set("Alert", alert);
			return script;
		}
	}
}
