using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Analytics;
using StripClub.Model.Cards;
using UnityEngine;

namespace StripClub.Model;

public abstract class LinkedContent : ICloneable
{
	[Serializable]
	public abstract class Map
	{
		public LinkedContentAnalyticData AnalyticData;

		public Map(LinkedContent source)
		{
			AnalyticData = source.AnalyticData;
		}
	}

	public LinkedContentAnalyticData AnalyticData;

	protected LinkedContent next;

	public abstract Type Type { get; }

	protected LinkedContent CloneOfNext
	{
		get
		{
			if (next == null)
			{
				return null;
			}
			return next.Clone();
		}
	}

	public LinkedContent Next()
	{
		return next;
	}

	public LinkedContent(LinkedContentAnalyticData analyticData, LinkedContent next = null)
	{
		this.next = next;
		AnalyticData = analyticData;
	}

	public LinkedContent GetLast()
	{
		int num = 0;
		LinkedContent linkedContent = this;
		while (linkedContent.next != null)
		{
			linkedContent = linkedContent.next;
			if (num == 15)
			{
				throw new NotImplementedException("Endless cycle");
			}
			num++;
		}
		return linkedContent;
	}

	public void ReleaseNext()
	{
		next = null;
	}

	public int Count()
	{
		int num = 0;
		if (next != null)
		{
			num = next.Count();
		}
		return 1 + num;
	}

	public abstract Sprite GetIcon();

	public virtual bool TryGetAlternativeIcon(out Sprite sprite)
	{
		sprite = null;
		return false;
	}

	public abstract Sprite GetProgressBarIcon();

	public abstract Rarity GetRarity();

	public abstract string GetName();

	public abstract string GetDescription();

	public T GetNext<T>(bool checkThis = false) where T : LinkedContent
	{
		if (checkThis && this is T)
		{
			return (T)this;
		}
		if (next == null)
		{
			return null;
		}
		if (next is T)
		{
			return (T)next;
		}
		return next.GetNext<T>();
	}

	public virtual void Insert(LinkedContent content)
	{
		if (next != null)
		{
			next.Insert(content);
		}
		else
		{
			next = content;
		}
	}

	public List<Map> GetContentMap()
	{
		List<Map> list = new List<Map>();
		MapNestedContent(list, this);
		return list;
	}

	private void MapNestedContent(List<Map> listOfMaps, LinkedContent content)
	{
		Map map = content.GetMap();
		listOfMaps.Add(map);
		if (content.next != null)
		{
			MapNestedContent(listOfMaps, content.next);
		}
	}

	public void AddToPlayer()
	{
		AddCurrentToPlayer();
		if (next != null)
		{
			next.AddToPlayer();
		}
	}

	public virtual void AddCurrentToPlayer()
	{
	}

	public abstract Map GetMap();

	object ICloneable.Clone()
	{
		return Clone();
	}

	public abstract LinkedContent Clone();
}
