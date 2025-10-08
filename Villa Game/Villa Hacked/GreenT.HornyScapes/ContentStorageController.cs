using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

public class ContentStorageController : IInitializable, IDisposable, IContentSelector, ISelector<ContentType>
{
	private readonly Subject<Unit> onAddPlayer = new Subject<Unit>();

	private ContentType currentType;

	private readonly ContentStorageProvider storageProvider;

	private readonly GameStarter gameStarter;

	private readonly EventMergeScreenIndicator eventMergeScreenIndicator;

	private readonly MainScreenIndicator _mainScreenIndicator;

	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable trackStream = new CompositeDisposable();

	public IObservable<Unit> OnAddPlayer => Observable.AsObservable<Unit>((IObservable<Unit>)onAddPlayer);

	public ContentStorageController(ContentStorageProvider storageProvider, GameStarter gameStarter, EventMergeScreenIndicator eventMergeScreenIndicator, MainScreenIndicator mainScreenIndicator, SignalBus signalBus)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_signalBus = signalBus;
		this.storageProvider = storageProvider;
		this.gameStarter = gameStarter;
		this.eventMergeScreenIndicator = eventMergeScreenIndicator;
		_mainScreenIndicator = mainScreenIndicator;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_mainScreenIndicator.IsVisible, (Func<bool, bool>)((bool _visible) => gameStarter.IsGameActive.Value && _visible)), (Action<bool>)delegate
		{
			storageProvider.UpdateState();
		}), (ICollection<IDisposable>)trackStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)eventMergeScreenIndicator.IsVisible, (Func<bool, bool>)((bool _visible) => gameStarter.IsGameActive.Value && _visible)), (Action<bool>)delegate
		{
			storageProvider.UpdateState();
		}), (ICollection<IDisposable>)trackStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_mainScreenIndicator.IsVisible, (Func<bool, bool>)((bool _visible) => gameStarter.IsGameActive.Value && _visible && currentType == storageProvider.ContentType)), (Action<bool>)delegate
		{
			AddToPlayer();
		}), (ICollection<IDisposable>)trackStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)eventMergeScreenIndicator.IsVisible, (Func<bool, bool>)((bool _visible) => gameStarter.IsGameActive.Value && _visible && currentType == storageProvider.ContentType)), (Action<bool>)delegate
		{
			AddToPlayer();
		}), (ICollection<IDisposable>)trackStream);
	}

	private void AddToPlayer()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (storageProvider.HasStoredContent())
		{
			storageProvider.AddToPlayer();
			onAddPlayer.OnNext(default(Unit));
		}
	}

	public void Select(ContentType currentType)
	{
		this.currentType = currentType;
	}

	public void Dispose()
	{
		onAddPlayer?.Dispose();
		CompositeDisposable obj = trackStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
