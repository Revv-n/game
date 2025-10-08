using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public class TasksRoomConfig
{
	[SerializeField]
	public int RoomID { get; private set; }

	[SerializeField]
	public List<TaskConfig> Tasks { get; private set; }

	public TasksRoomConfig(int roomID, List<TaskConfig> tasks)
	{
		RoomID = roomID;
		Tasks = tasks;
	}
}
