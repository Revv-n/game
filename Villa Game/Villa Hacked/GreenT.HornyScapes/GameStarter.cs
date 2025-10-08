using System;
using System.Linq;
using GreenT.AssetBundles;
using GreenT.HornyScapes.UI;
using GreenT.Settings;
using GreenT.UI;
using StripClub;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GreenT.HornyScapes;

public class GameStarter : IInitializable, IDisposable
{
	private readonly ReactiveProperty<bool> isDataLoaded = new ReactiveProperty<bool>(false);

	public IReadOnlyReactiveProperty<bool> IsConfigsLoaded;

	private readonly ReactiveProperty<bool> isGameReadyToStart = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> isGameActive = new ReactiveProperty<bool>(false);

	public readonly ICompletable<float> LoadingProgress;

	private readonly WindowOpener windowOpener;

	private readonly IWindowsManager windowsManager;

	private readonly IProjectSettings projectSettings;

	private readonly IAssetBundlesCache assetBundlesCache;

	private readonly IDisposable loadingScenesStream;

	private IReadOnlyReactiveProperty<bool> isDataReady;

	public IReadOnlyReactiveProperty<bool> IsDataLoaded => (IReadOnlyReactiveProperty<bool>)(object)isDataLoaded;

	public IReadOnlyReactiveProperty<bool> IsGameReadyToStart => (IReadOnlyReactiveProperty<bool>)(object)isGameReadyToStart;

	public IReadOnlyReactiveProperty<bool> IsGameActive => (IReadOnlyReactiveProperty<bool>)(object)isGameActive;

	public GameStarter(ICompletable<float> loadingProgress, IWindowsManager windowsManager, IProjectSettings projectSettings, WindowOpener windowOpener, IAssetBundlesCache assetBundlesCache)
	{
		this.windowsManager = windowsManager;
		this.projectSettings = projectSettings;
		this.windowOpener = windowOpener;
		this.assetBundlesCache = assetBundlesCache;
		LoadingProgress = loadingProgress;
	}

	public void SetConfigsLoaded(IReadOnlyReactiveProperty<bool> isConfigsLoaded)
	{
		IsConfigsLoaded = isConfigsLoaded;
	}

	public void SetState(bool state)
	{
		isDataLoaded.Value = state;
	}

	public void Initialize()
	{
		isDataReady = ReactivePropertyExtensions.ToReactiveProperty<bool>(Observable.CombineLatest<bool, bool, bool>((IObservable<bool>)isDataLoaded, (IObservable<bool>)IsConfigsLoaded, (Func<bool, bool, bool>)((bool x, bool y) => x && y)));
		ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<Unit, Unit>(Observable.SelectMany<bool, Unit>(Observable.Skip<bool>(Observable.Do<bool>(Observable.Where<bool>((IObservable<bool>)isDataReady, (Func<bool, bool>)((bool x) => !x)), (Action<bool>)delegate
		{
			isGameReadyToStart.Value = false;
		}), 1), UnloadGameScenes()), (Func<Unit, IObservable<Unit>>)((Unit _) => SceneLoader.LoadSceneAsObservable(projectSettings.LoginScene))));
		ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<bool, Unit>(Observable.Where<bool>((IObservable<bool>)isDataReady, (Func<bool, bool>)((bool x) => x)), LoadGameScenes()), (Action<Unit>)delegate
		{
			isGameReadyToStart.Value = true;
		});
	}

	private IObservable<Unit> LoadGameScenes()
	{
		return SceneLoader.LoadScenesAsObservable(projectSettings.GameScenes.LevelScenesLoadOrder.Where((SerializedScene _scene) => !SceneManager.GetSceneByBuildIndex(_scene.BuildIndex).isLoaded));
	}

	private IObservable<Unit> UnloadGameScenes()
	{
		return Observable.AsSingleUnitObservable<AsyncOperation>(Observable.SelectMany<SerializedScene, AsyncOperation>(Observable.ToObservable<SerializedScene>(projectSettings.GameScenes.LevelScenesLoadOrder.Where((SerializedScene _scene) => SceneManager.GetSceneByBuildIndex(_scene.BuildIndex).isLoaded)), (Func<SerializedScene, IObservable<AsyncOperation>>)Unload));
		static IObservable<AsyncOperation> Unload(SerializedScene scene)
		{
			return AsyncOperationExtensions.AsObservable(SceneManager.UnloadSceneAsync(scene.BuildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects), (System.IProgress<float>)null);
		}
	}

	public void Play()
	{
		if (!isGameActive.Value && isGameReadyToStart.Value)
		{
			CloseLoginScene();
			windowOpener.Click();
			isGameActive.Value = true;
		}
	}

	private void CloseLoginScene()
	{
		windowsManager.Get<LoadingWindow>().Close();
		SceneManager.UnloadSceneAsync(projectSettings.LoginScene.BuildIndex);
	}

	public void StopGame()
	{
		isGameActive.Value = false;
	}

	public void RestartApplication()
	{
		assetBundlesCache.ReleaseAll();
		SceneManager.LoadScene(0);
	}

	public void Dispose()
	{
		isGameReadyToStart?.Dispose();
		isGameActive?.Dispose();
		loadingScenesStream?.Dispose();
	}
}
