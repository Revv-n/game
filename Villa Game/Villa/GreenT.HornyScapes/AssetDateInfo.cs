using System;

namespace GreenT.HornyScapes;

[Serializable]
public struct AssetDateInfo : IEquatable<AssetDateInfo>
{
	public string path;

	public long update;

	public AssetDateInfo(string path, DateTimeOffset update)
	{
		this.path = path;
		this.update = update.Ticks;
	}

	public AssetDateInfo(string path, long update)
	{
		this.path = path;
		this.update = update;
	}

	public bool Equals(AssetDateInfo other)
	{
		if (path == other.path)
		{
			return update == other.update;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is AssetDateInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(path, update);
	}
}
