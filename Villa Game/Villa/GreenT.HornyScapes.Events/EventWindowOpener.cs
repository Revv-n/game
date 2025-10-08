using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventWindowOpener : IDisposable
{
	private readonly EventProgressView eventProgressView;

	private readonly IWindowsManager windowsManager;

	private readonly WindowID startWindowID;

	private readonly WindowID progressWindowID;

	private readonly WindowOpener metaWindowOpenerFromMerge;

	private readonly ContentSelectorGroup contentSelectorGroup;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	private IWindow startWindow;

	private IWindow progressWindow;

	private readonly SignalBus _signalBus;

	private IWindow StartWindow => startWindow ?? (startWindow = windowsManager.GetWindow(startWindowID));

	private IWindow ProgressWindow => progressWindow ?? (progressWindow = windowsManager.GetWindow(progressWindowID));

	public EventWindowOpener(EventProgressView eventProgressView, IWindowsManager windowsManager, WindowID startWindowID, WindowID progressWindowID, WindowOpener metaWindowOpenerFromMerge, ContentSelectorGroup contentSelectorGroup, SignalBus signalBus)
	{
		_signalBus = signalBus;
		this.eventProgressView = eventProgressView;
		this.windowsManager = windowsManager;
		this.startWindowID = startWindowID;
		this.progressWindowID = progressWindowID;
		this.metaWindowOpenerFromMerge = metaWindowOpenerFromMerge;
		this.contentSelectorGroup = contentSelectorGroup;
	}

	public void OpenProgress()
	{
		ProgressWindow.Open();
	}

	public void PrepareViewToEndEvent(EntityStatus status)
	{
		switch (status)
		{
		case EntityStatus.Complete:
			eventProgressView.SetViewState(2);
			OpenMeta();
			break;
		case EntityStatus.Rewarded:
			OpenMeta();
			break;
		default:
			throw new ArgumentOutOfRangeException(status.ToString());
		}
	}

	private void OpenMeta()
	{
		if (!TryCloseStartWindow())
		{
			GoToMain();
		}
	}

	private bool TryCloseStartWindow()
	{
		_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.EventStart));
		if (!StartWindow.IsOpened)
		{
			return false;
		}
		StartWindow.Close();
		return true;
	}

	private void GoToMain()
	{
		if (contentSelectorGroup.Current == ContentType.Event)
		{
			metaWindowOpenerFromMerge.OpenOnly();
		}
	}

	public void Dispose()
	{
		_compositeDisposable.Dispose();
	}
}
