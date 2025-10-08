using Zenject;

namespace GreenT.HornyScapes.Net;

public class IndependentUrlInstaller : Installer<IndependentUrlInstaller>
{
	private const string redirectUrl = "https://discord.gg/E6Cdz74jzn";

	private const string redirectUrlErolabs = "https://l.hyenadata.com/s/1IzyKV";

	public override void InstallBindings()
	{
		base.Container.BindInstance("https://discord.gg/E6Cdz74jzn").WithId("SupportUrl").AsSingle();
	}
}
