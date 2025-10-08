using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.Presents.Models;

public class Present
{
	private const string uniqueKeyPrefix = "Present.";

	private readonly string _uniqueKey;

	public string Id { get; private set; }

	public int LovePoints { get; private set; }

	public float[] LongtapStageTime { get; private set; }

	public float[] LongtapSpeed { get; private set; }

	public CurrencyType CurrencyType { get; private set; }

	public ICurrenciesActionContainer Container { get; private set; }

	public Sprite Icon { get; private set; }

	public Present(string id, int lovePoints, float[] longtapStageTime, float[] longtapSpeed, CurrencyType currencyType)
	{
		Id = id;
		LovePoints = lovePoints;
		LongtapStageTime = longtapStageTime;
		LongtapSpeed = longtapSpeed;
		CurrencyType = currencyType;
		_uniqueKey = "Present." + Id;
	}

	public bool AddCount(int value)
	{
		if (0 <= value)
		{
			return Container.TryAdd(value);
		}
		return Container.TrySpend(-value);
	}

	public void SetIcon(Sprite icon)
	{
		Icon = icon;
	}

	public void SetContainer(ICurrenciesActionContainer container)
	{
		Container = container;
	}
}
