using System;
using UnityEngine;

namespace GreenT.Data;

[Serializable]
[Memento]
public abstract class Memento
{
	public string UniqueKey { get; private set; }

	public string Version { get; private set; }

	public Memento(ISavableState savableState)
	{
		UniqueKey = savableState.UniqueKey();
		Version = Application.version;
	}
}
