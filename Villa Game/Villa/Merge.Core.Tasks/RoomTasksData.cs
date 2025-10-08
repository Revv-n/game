using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public class RoomTasksData
{
	[SerializeField]
	public int RoomID { get; private set; }

	[SerializeField]
	public List<TaskData> Tasks { get; private set; } = new List<TaskData>();


	public RoomTasksData(int id)
	{
		RoomID = id;
	}
}
