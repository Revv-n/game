using System;
using GreenT.HornyScapes.Monetization.Harem;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization.Webgl.Harem;

public class MonetizationPaymentConnector : MonoBehaviour, IPaymentConnector
{
	private Action<int> _success;

	private Action<int> _failure;

	public void Connect(int monetizationID, string itemName, string price, string description, string imageUrl, Action<int> success, Action<int> failure)
	{
		_success = success;
		_failure = failure;
	}

	public void OnPaymentSuccess(int monetizationId)
	{
		_success(monetizationId);
	}

	public void OnPaymentFailed(int monetizationId)
	{
		_failure(monetizationId);
	}
}
