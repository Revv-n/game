using GreenT.HornyScapes.Cheats;
using GreenT.HornyScapes.Cheats.UI;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public class CheatBuyProduct : CheatButtonWithInputField
{
	[Inject]
	private IMonetizationAdapter _monetization;

	[Inject]
	private LotManager _lotManager;

	private int _currentId;

	public void TryCheatBuyProduct(int monetizationId)
	{
		Lot lotByMonetizationID = _lotManager.GetLotByMonetizationID(monetizationId);
		if (lotByMonetizationID != null)
		{
			string paymentID = CheatPurchase.GetPaymentID(lotByMonetizationID);
			_monetization.BuyProduct(paymentID, lotByMonetizationID.MonetizationID, lotByMonetizationID.ID);
		}
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
		TryCheatBuyProduct(_currentId);
		inputField.text = string.Empty;
	}
}
