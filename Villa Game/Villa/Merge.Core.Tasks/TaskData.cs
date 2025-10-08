using System;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public class TaskData
{
	[SerializeField]
	public TaskKey Key { get; private set; }

	[SerializeField]
	public bool IsCompleted { get; set; }

	public TaskData(TaskKey key)
	{
		Key = key;
	}
}
