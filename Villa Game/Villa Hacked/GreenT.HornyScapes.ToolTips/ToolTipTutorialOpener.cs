using System;
using GreenT.HornyScapes.Tutorial;
using GreenT.UI;
using UniRx;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipTutorialOpener : IDisposable
{
	private ToolTipTutorialHintOpener hintOpener;

	private ToolTipTutorialArrowOpener arrowOpener;

	public ReactiveProperty<bool> IsPlaying = new ReactiveProperty<bool>();

	private IWindowsManager windowsManager;

	private TutorialWindow tutorialWindow;

	private IDisposable followStream;

	private TutorialStepSO settings;

	public ToolTipTutorialOpener(ToolTipTutorialHintOpener hintOpener, ToolTipTutorialArrowOpener arrowOpener, IWindowsManager windowsManager)
	{
		this.hintOpener = hintOpener;
		this.arrowOpener = arrowOpener;
		this.windowsManager = windowsManager;
		IsPlaying.Value = hintOpener.IsPlaying.Value;
		followStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)hintOpener.IsPlaying, (Action<bool>)delegate(bool _value)
		{
			IsPlaying.Value = _value;
		});
	}

	public void Init(TutorialStepSO settings)
	{
		if (!tutorialWindow)
		{
			tutorialWindow = windowsManager.Get<TutorialWindow>();
		}
		this.settings = settings;
		settings.HintSettings.OpenHint = settings.OpenHint;
		settings.HintSettings.CloseHint = settings.CloseHint;
		settings.HintSettings.StepID = settings.StepID;
		settings.ArrowSettings.StepID = settings.StepID;
		hintOpener.Init(settings.HintSettings);
		arrowOpener.Init(settings.ArrowSettings);
	}

	public void Open()
	{
		IsPlaying.Value = true;
		tutorialWindow.Open(settings);
		if (settings.ShowHint)
		{
			hintOpener.Open();
		}
		else
		{
			hintOpener.Close();
		}
		if (settings.ShowArrow)
		{
			arrowOpener.Open();
		}
		else
		{
			arrowOpener.Close();
		}
	}

	public void Close(TutorialStepSO step)
	{
		tutorialWindow.Close();
		if (step.CloseHint)
		{
			hintOpener.Close();
		}
		arrowOpener.Close();
	}

	public void Dispose()
	{
		followStream?.Dispose();
	}
}
