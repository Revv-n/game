using System;
using System.Collections.Generic;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public class TaskConfig
{
	[SerializeField]
	public TaskKey Key { get; private set; }

	[SerializeField]
	public string Name { get; private set; }

	[SerializeField]
	public string RoomObject { get; private set; }

	[SerializeField]
	public List<TaskKey> RequiredTasks { get; private set; } = new List<TaskKey>();


	[SerializeField]
	public List<ObjectiveInfo> ObjectivesInfo { get; private set; } = new List<ObjectiveInfo>();


	[SerializeField]
	public List<GIData> Rewards { get; private set; } = new List<GIData>();


	[SerializeField]
	public string TaskIconName { get; private set; }

	public TaskConfig(TaskKey key, string name, string roomObj, List<TaskKey> requiredTasks, List<ObjectiveInfo> objectivesInfo, List<GIData> rewards, string taskIconName)
	{
		Key = key;
		Name = name;
		RoomObject = roomObj;
		RequiredTasks = requiredTasks;
		ObjectivesInfo = objectivesInfo;
		Rewards = rewards;
		TaskIconName = taskIconName;
	}
}
