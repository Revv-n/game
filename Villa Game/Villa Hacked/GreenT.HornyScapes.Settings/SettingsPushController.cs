using System;
using GreenT.HornyScapes.Tutorial;
using GreenT.HornyScapes.UI;
using Merge;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Settings;

public class SettingsPushController : IInitializable, IDisposable
{
	private readonly PCUserInputDetector _inputDetector;

	private readonly MainScreenIndicator _mainScreenIndicator;

	private readonly MergeScreenIndicator _mergeScreenIndicator;

	private readonly EventMergeScreenIndicator _eventMergeScreenIndicator;

	private readonly SettingsPopupOpener _settingsPopupOpener;

	private readonly TutorialSystem _tutorialSystem;

	private IDisposable _stream;

	public SettingsPushController(PCUserInputDetector inputDetector, MainScreenIndicator mainScreenIndicator, MergeScreenIndicator mergeScreenIndicator, EventMergeScreenIndicator eventMergeScreenIndicator, SettingsPopupOpener settingsPopupOpener, TutorialSystem tutorialSystem)
	{
		_inputDetector = inputDetector;
		_mainScreenIndicator = mainScreenIndicator;
		_mergeScreenIndicator = mergeScreenIndicator;
		_eventMergeScreenIndicator = eventMergeScreenIndicator;
		_settingsPopupOpener = settingsPopupOpener;
		_tutorialSystem = tutorialSystem;
	}

	public void Initialize()
	{
		_stream = ObservableExtensions.Subscribe<UserInputDetector>(Observable.Where<UserInputDetector>(_inputDetector.OnEsc(), (Func<UserInputDetector, bool>)((UserInputDetector x) => CanShow())), (Action<UserInputDetector>)delegate
		{
			ShowPopup(value: true);
		});
	}

	private void ShowPopup(bool value)
	{
		_settingsPopupOpener.OpenAdditive();
	}

	private bool CanShow()
	{
		if (_mainScreenIndicator.IsVisible.Value || _mergeScreenIndicator.IsVisible.Value || _eventMergeScreenIndicator.IsVisible.Value)
		{
			return !_tutorialSystem.IsActive.Value;
		}
		return false;
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
