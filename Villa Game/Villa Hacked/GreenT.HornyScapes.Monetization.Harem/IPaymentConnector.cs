using System;

namespace GreenT.HornyScapes.Monetization.Harem;

public interface IPaymentConnector
{
	void Connect(int monetizationID, string itemName, string price, string description, string imageUrl, Action<int> success, Action<int> failure);
}
