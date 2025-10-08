using GreenT.HornyScapes.Monetization.Windows.Steam;
using Steamworks;

namespace GreenT.Steam;

public class SteamPaymentAuthorization
{
	private readonly MonetizationSystem _monetizationSystem;

	public SteamPaymentAuthorization(MonetizationSystem monetizationSystem)
	{
		_monetizationSystem = monetizationSystem;
	}

	public void PaymentTransactionAuth()
	{
		Callback<MicroTxnAuthorizationResponse_t>.Create(_monetizationSystem.TxnAuthCallback);
	}
}
