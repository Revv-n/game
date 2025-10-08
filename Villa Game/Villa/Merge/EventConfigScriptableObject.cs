using System;
using UnityEngine;

namespace Merge;

[Serializable]
[CreateAssetMenu(fileName = "Config", menuName = "DL/Events/EventConfigScriptableObject")]
public class EventConfigScriptableObject : ScriptableObject
{
	[SerializeField]
	private string key;

	[SerializeField]
	private EventConfig Start;

	[SerializeField]
	private EventConfig Progress;

	[SerializeField]
	private EventConfig Complete;

	public string Key => key;

	public EventConfig GetConfig(EventStatus eventStatus)
	{
		return eventStatus switch
		{
			EventStatus.available => Start, 
			EventStatus.inProgress => Progress, 
			EventStatus.completed => Complete, 
			_ => null, 
		};
	}

	public string GetTitle(EventStatus eventStatus)
	{
		return eventStatus switch
		{
			EventStatus.available => Start.Title, 
			EventStatus.inProgress => Progress.Title, 
			EventStatus.completed => Complete.Title, 
			_ => string.Empty, 
		};
	}

	public string GetDescription(EventStatus eventStatus)
	{
		return eventStatus switch
		{
			EventStatus.available => Start.Description, 
			EventStatus.inProgress => Progress.Description, 
			EventStatus.completed => Complete.Description, 
			_ => string.Empty, 
		};
	}

	public Sprite GetPreview(EventStatus eventStatus)
	{
		return eventStatus switch
		{
			EventStatus.available => Start.Preview(key), 
			EventStatus.inProgress => Progress.Preview(key), 
			EventStatus.completed => Complete.Preview(key), 
			_ => null, 
		};
	}
}
