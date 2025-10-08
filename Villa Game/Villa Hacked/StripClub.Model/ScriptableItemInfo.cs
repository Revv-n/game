using System;
using StripClub.Utility;
using UnityEngine;

namespace StripClub.Model;

public abstract class ScriptableItemInfo : ScriptableObject, IItemInfo
{
	[ReadOnly]
	[SerializeField]
	protected string guidString;

	[SerializeField]
	protected Sprite icon;

	private const string duplicate = "Duplicate";

	private const string paste = "Paste";

	public Guid Guid { get; protected set; }

	public abstract string LocalizationKey { get; }

	public Sprite Icon => icon;

	public override bool Equals(object other)
	{
		if (other.GetType() != GetType())
		{
			return false;
		}
		return ((IItemInfo)other)?.Guid.Equals(Guid) ?? false;
	}

	public override int GetHashCode()
	{
		return GetInstanceID();
	}

	public void OnEnable()
	{
		SetGuid();
	}

	public virtual void OnValidate()
	{
		Event current = Event.current;
		if (current != null && (current.commandName.Equals("Duplicate") || current.commandName.Equals("Paste")))
		{
			Guid = default(Guid);
			guidString = null;
		}
		if (guidString != null)
		{
			SetGuid();
		}
	}

	public void SetGuid()
	{
		if (Guid.Equals(default(Guid)))
		{
			if (Guid.TryParse(guidString, out var result))
			{
				Guid = result;
				return;
			}
			Guid = Guid.NewGuid();
			guidString = Guid.ToString();
		}
		else
		{
			guidString = Guid.ToString();
		}
	}
}
