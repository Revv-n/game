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
		_gameStarter.IsGameActive.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			InitializationAfterGameStart();
		});
	}

	private void InitializationAfterGameStart()
	{
		_cluster.OnDataReady.SelectMany((CreateData createData) => _bundleProvider.Get(createData)).Subscribe(SendRequest);
		_cluster.Initialization();
	}

	private void SendRequest(CreateData createData)
	{
		_bannerNotificationService.NotifyBannerLoaded(createData);
	}
}
