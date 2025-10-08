using System;
using UnityEngine;

[Serializable]
public class EventConfig
{
	[SerializeField]
	[TextArea(1, 10)]
	private string title;

	[SerializeField]
	[TextArea(1, 10)]
	private string description;

	[SerializeField]
	private string previewName;

	public string Title => title;

	public string Description => description;

	public Sprite Preview(string key)
	{
		return null;
	}
}
