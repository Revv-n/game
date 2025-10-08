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
		base.Container.Bind<BackgroundView>().FromInstance(_backgroundView);
		base.Container.Bind<EyeView>().FromInstance(_eyeView);
		base.Container.Bind<TapChecker>().FromInstance(_tapChecker);
		base.Container.BindInterfacesAndSelfTo<DateController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateSoundController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DateFlowController>().AsSingle().WithArguments(_phraseView, _speakersView, _dateWindow, _overView);
	}
}
