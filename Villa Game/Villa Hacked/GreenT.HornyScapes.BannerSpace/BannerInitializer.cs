using System;
using GreenT.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerInitializer : IInitializable
{
	private readonly DataManagerCluster _cluster;

	private readonly BundleProvider _bundleProvider;

	private readonly GameStarter _gameStarter;

	private readonly BannerNotificationService _bannerNotificationService;

	private readonly BannerController _bannerController;

	private readonly SaveDataManager _saveDataManager;

	private readonly ISaver _saver;

	public BannerInitializer(DataManagerCluster cluster, BundleProvider bundleProvider, GameStarter gameStarter, BannerNotificationService bannerNotificationService, BannerController bannerController, SaveDataManager saveDataManager, ISaver saver)
	{
		_cluster = cluster;
		_bundleProvider = bundleProvider;
		_gameStarter = gameStarter;
		_bannerNotificationService = bannerNotificationService;
		_bannerController = bannerController;
		_saveDataManager = saveDataManager;
		_saver = saver;
	}

	public void Initialize()
	{
		_saver.Add(_saveDataManager);
		_bannerController.Initialization();
		ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
		{
			InitializationAfterGameStart();
		});
	}

	private void InitializationAfterGameStart()
	{
		ObservableExtensions.Subscribe<CreateData>(Observable.SelectMany<CreateData, CreateData>(_cluster.OnDataReady, (Func<CreateData, IObservable<CreateData>>)((CreateData createData) => _bundleProvider.Get(createData))), (Action<CreateData>)SendRequest);
		_cluster.Initialization();
	}

	private void SendRequest(CreateData createData)
	{
		_bannerNotificationService.NotifyBannerLoaded(createData);
	}
}
