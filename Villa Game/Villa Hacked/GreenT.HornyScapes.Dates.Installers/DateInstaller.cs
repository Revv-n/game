using GreenT.HornyScapes.Dates.Services;
using GreenT.HornyScapes.Dates.Views;
using GreenT.HornyScapes.Dates.Windows;
using GreenT.HornyScapes.Stories.UI;
using StripClub.Stories;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Dates.Installers;

public class DateInstaller : MonoInstaller
{
	[SerializeField]
	private BackgroundView _backgroundView;

	[SerializeField]
	private PhraseView _phraseView;

	[SerializeField]
	private SpeakersView _speakersView;

	[SerializeField]
	private DateWindow _dateWindow;

	[SerializeField]
	private OverView _overView;

	[SerializeField]
	private EyeView _eyeView;

	[SerializeField]
	private TapChecker _tapChecker;

	public override void InstallBindings()
	{
		((FromBinderGeneric<BackgroundView>)(object)((MonoInstallerBase)this).Container.Bind<BackgroundView>()).FromInstance(_backgroundView);
		((FromBinderGeneric<EyeView>)(object)((MonoInstallerBase)this).Container.Bind<EyeView>()).FromInstance(_eyeView);
		((FromBinderGeneric<TapChecker>)(object)((MonoInstallerBase)this).Container.Bind<TapChecker>()).FromInstance(_tapChecker);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DateController>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DateSoundController>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DateFlowController>()).AsSingle()).WithArguments<PhraseView, SpeakersView, DateWindow, OverView>(_phraseView, _speakersView, _dateWindow, _overView);
	}
}
