using GreenT.Data;
using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class SettingsInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		BindRegistrationRewardState();
		BindRequests();
	}

	private void BindRegistrationRewardState()
	{
		if (PlatformHelper.IsEpochaMonetization())
		{
			base.Container.Bind<SavableVariable<bool>>().WithArguments("RegistrationReward", param2: false).OnInstantiated(delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			})
				.WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsNutakuMonetization())
		{
			base.Container.Bind<SavableVariable<bool>>().WithArguments("RegistrationReward", param2: true).OnInstantiated(delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			})
				.WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsHaremMonetization())
		{
			base.Container.Bind<SavableVariable<bool>>().WithArguments("RegistrationReward", param2: false).OnInstantiated(delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			})
				.WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsSteamMonetization())
		{
			base.Container.Bind<SavableVariable<bool>>().WithArguments("RegistrationReward", param2: true).OnInstantiated(delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			})
				.WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsErolabsMonetization())
		{
			base.Container.Bind<SavableVariable<bool>>().WithArguments("RegistrationReward", param2: false).OnInstantiated(delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			})
				.WhenInjectedInto<SettingsWindow>();
		}
	}

	private void BindRequests()
	{
		base.Container.BindPostRequest<PromocodeActivationRequest>(PostRequestType.Promocodes);
	}
}
