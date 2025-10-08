using System;
using GreenT;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public class Objective
{
	public int Progress { get; private set; }

	public ObjectiveInfo Info { get; private set; }

	public Sprite Icon => IconProvider.GetGISprite(ItemKey);

	public GIKey ItemKey => Info.Item;

	public int Required => Info.Count;

	public bool IsCompleted => Progress >= Required;

	public event Action OnUpdate;

	public Objective(ObjectiveInfo info)
	{
		Info = info;
	}

	public void SetProgress(int current)
	{
		Progress = current;
		this.OnUpdate?.Invoke();
	}

	public static Objective[] ParseReqItems(string[] reqItems, int[] reqValue)
	{
		Objective[] array = new Objective[reqItems.Length];
		try
		{
			for (int i = 0; i < reqItems.Length; i++)
			{
				GIKey item = GIKey.Parse(reqItems[i]);
				array[i] = new Objective(new ObjectiveInfo(item, reqValue[i]));
			}
			return array;
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("ReqItem lenght doesn't equal ReqValue lenght");
		}
	}
}
