using System;
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

	public IObservable<Unit> OnAddPlayer => onAddPlayer.AsObservable();

	public ContentStorageController(ContentStorageProvider storageProvider, GameStarter gameStarter, EventMergeScreenIndicator eventMergeScreenIndicator, MainScreenIndicator mainScreenIndicator, SignalBus signalBus)
	{
		_signalBus = signalBus;
		this.storageProvider = storageProvider;
		this.gameStarter = gameStarter;
		this.eventMergeScreenIndicator = eventMergeScreenIndicator;
		_mainScreenIndicator = mainScreenIndicator;
	}

	public void Initialize()
	{
		_mainScreenIndicator.IsVisible.Where((bool _visible) => gameStarter.IsGameActive.Value && _visible).Subscribe(delegate
		{
			storageProvider.UpdateState();
		}).AddTo(trackStream);
		eventMergeScreenIndicator.IsVisible.Where((bool _visible) => gameStarter.IsGameActive.Value && _visible).Subscribe(delegate
		{
			storageProvider.UpdateState();
		}).AddTo(trackStream);
		_mainScreenIndicator.IsVisible.Where((bool _visible) => gameStarter.IsGameActive.Value && _visible && currentType == storageProvider.ContentType).Subscribe(delegate
		{
			AddToPlayer();
		}).AddTo(trackStream);
		eventMergeScreenIndicator.IsVisible.Where((bool _visible) => gameStarter.IsGameActive.Value && _visible && currentType == storageProvider.ContentType).Subscribe(delegate
		{
			AddToPlayer();
		}).AddTo(trackStream);
	}

	private void AddToPlayer()
	{
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
		trackStream?.Dispose();
	}
}
