using System.Reflection;
using GreenT.HornyScapes.Cheats.UI;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatPurchase : CheatButtonWithInputField
{
	[Inject]
	private LotManager _lotManager;

	[Inject]
	private Purchaser _purchaser;

	private int _currentId;

	public void TryCheatPurchase(int monetizationId)
	{
		if (_lotManager.GetLotByMonetizationID(monetizationId) is ValuableLot<decimal> valuableLot)
		{
			string paymentID = GetPaymentID(valuableLot);
			_purchaser.TryPurchase(valuableLot, paymentID, valuableLot.LocalizationKey);
		}
	}

	public static string GetPaymentID(Lot lot)
	{
		return lot.GetType().GetProperty("PaymentID", BindingFlags.Instance | BindingFlags.Public).GetValue(lot)
			.ToString();
	}

	public override bool IsValid(string param)
	{
		if (!int.TryParse(param, out var _))
		{
			return false;
		}
		_currentId = int.Parse(param);
		return true;
	}

	public override void Apply()
	{
		TryCheatPurchase(_currentId);
		inputField.text = string.Empty;
	}
}
