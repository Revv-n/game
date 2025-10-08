using System;

namespace GreenT.HornyScapes;

[Serializable]
public struct BuildMainInfo
{
	public string path;

	public string hash;

	public BuildMainInfo(string path, string hash)
	{
		this.path = path;
		this.hash = hash;
	}
}
