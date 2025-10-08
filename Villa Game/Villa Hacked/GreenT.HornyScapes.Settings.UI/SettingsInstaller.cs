using System;
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
			((ArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SavableVariable<bool>>()).WithArguments<string, bool>("RegistrationReward", false).OnInstantiated<ISavableState>((Action<InjectContext, ISavableState>)delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			}).WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsNutakuMonetization())
		{
			((ArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SavableVariable<bool>>()).WithArguments<string, bool>("RegistrationReward", true).OnInstantiated<ISavableState>((Action<InjectContext, ISavableState>)delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			}).WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsHaremMonetization())
		{
			((ArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SavableVariable<bool>>()).WithArguments<string, bool>("RegistrationReward", false).OnInstantiated<ISavableState>((Action<InjectContext, ISavableState>)delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			}).WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsSteamMonetization())
		{
			((ArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SavableVariable<bool>>()).WithArguments<string, bool>("RegistrationReward", true).OnInstantiated<ISavableState>((Action<InjectContext, ISavableState>)delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			}).WhenInjectedInto<SettingsWindow>();
		}
		else if (PlatformHelper.IsErolabsMonetization())
		{
			((ArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SavableVariable<bool>>()).WithArguments<string, bool>("RegistrationReward", false).OnInstantiated<ISavableState>((Action<InjectContext, ISavableState>)delegate(InjectContext _context, ISavableState savable)
			{
				_context.Container.Resolve<ISaver>().Add(savable);
			}).WhenInjectedInto<SettingsWindow>();
		}
	}

	private void BindRequests()
	{
		((MonoInstallerBase)this).Container.BindPostRequest<PromocodeActivationRequest>(PostRequestType.Promocodes);
	}
}
