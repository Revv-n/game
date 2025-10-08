using System;
using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(menuName = "StripClub/Shop/Time/Finite Sale Time", order = 1)]
public class FiniteSaleTime : SaleTime, ISerializationCallbackReceiver
{
	[SerializeField]
	protected string startTimeString;

	[SerializeField]
	protected string expirationTimeString;

	protected DateTime startTime;

	protected DateTime expirationTime;

	public override DateTime StartTime => startTime;

	public override DateTime ExpirationTime => expirationTime;

	public override bool IsInclude(DateTime currentTime)
	{
		if (StartTime <= currentTime)
		{
			return currentTime < ExpirationTime;
		}
		return false;
	}

	public void OnAfterDeserialize()
	{
		DateTime.TryParse(startTimeString, out startTime);
		DateTime.TryParse(expirationTimeString, out expirationTime);
	}

	public void OnBeforeSerialize()
	{
		startTimeString = startTime.ToString();
		expirationTimeString = expirationTime.ToString();
	}

	protected virtual void OnValidate()
	{
	}
}
