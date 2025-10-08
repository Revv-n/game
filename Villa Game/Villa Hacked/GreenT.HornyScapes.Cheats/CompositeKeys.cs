using System;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

[Serializable]
public class CompositeKeys
{
	public bool HasAdditional;

	public KeyCode MainKey;

	public KeyCode AdditionalKey;

	public bool IsPressedKeys
	{
		get
		{
			if (!HasAdditional)
			{
				return Input.GetKeyDown(MainKey);
			}
			if (Input.GetKey(MainKey))
			{
				return Input.GetKeyDown(AdditionalKey);
			}
			return false;
		}
	}

	public CompositeKeys()
	{
	}

	public CompositeKeys(KeyCode main)
	{
		MainKey = main;
	}

	public CompositeKeys(KeyCode main, KeyCode additional)
		: this(main)
	{
		AdditionalKey = additional;
		HasAdditional = true;
	}
}
