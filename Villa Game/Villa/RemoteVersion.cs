using System;
using UnityEngine;

[Serializable]
public struct RemoteVersion
{
	[SerializeField]
	public string Block { get; private set; }

	[SerializeField]
	public string Version { get; private set; }

	public static RemoteVersion Default => new RemoteVersion("Default", "Default");

	public RemoteVersion(string block, string version)
	{
		Block = block;
		Version = version;
	}

	public override string ToString()
	{
		return "Block:" + Block + " Version:" + Version;
	}
}
