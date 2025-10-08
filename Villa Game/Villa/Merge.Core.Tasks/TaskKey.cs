using System;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public struct TaskKey
{
	[SerializeField]
	private int room;

	[SerializeField]
	private int id;

	public int Room => room;

	public int ID => id;

	public TaskKey(int room, int id)
	{
		this.room = room;
		this.id = id;
	}

	public static bool operator ==(TaskKey a, TaskKey b)
	{
		if (a.id == b.id)
		{
			return a.room == b.room;
		}
		return false;
	}

	public static bool operator !=(TaskKey a, TaskKey b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj is TaskKey taskKey && room == taskKey.room)
		{
			return id == taskKey.id;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (-1526070678 * -1521134295 + room.GetHashCode()) * -1521134295 + id.GetHashCode();
	}
}
