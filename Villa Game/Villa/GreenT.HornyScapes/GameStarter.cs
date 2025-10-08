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
	private readonly ReactiveProperty<bool> isDataLoaded = new ReactiveProperty<bool>(initialValue: false);

	public IReadOnlyReactiveProperty<bool> IsConfigsLoaded;

	private readonly ReactiveProperty<bool> isGameReadyToStart = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> isGameActive = new ReactiveProperty<bool>(initialValue: false);

	public readonly ICompletable<float> LoadingProgress;

	private readonly WindowOpener windowOpener;

	private readonly IWindowsManager windowsManager;

	private readonly IProjectSettings projectSettings;

	private readonly IAssetBundlesCache assetBundlesCache;

	private readonly IDisposable loadingScenesStream;

	private IReadOnlyReactiveProperty<bool> isDataReady;

	public IReadOnlyReactiveProperty<bool> IsDataLoaded => isDataLoaded;

	public IReadOnlyReactiveProperty<bool> IsGameReadyToStart => isGameReadyToStart;

	public IReadOnlyReactiveProperty<bool> IsGameActive => isGameActive;

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
		isDataReady = isDataLoaded.CombineLatest(IsConfigsLoaded, (bool x, bool y) => x && y).ToReactiveProperty();
		isDataReady.Where((bool x) => !x).Do(delegate
		{
			isGameReadyToStart.Value = false;
		}).Skip(1)
			.SelectMany(UnloadGameScenes())
			.SelectMany((Unit _) => SceneLoader.LoadSceneAsObservable(projectSettings.LoginScene))
			.Subscribe();
		isDataReady.Where((bool x) => x).SelectMany(LoadGameScenes()).Subscribe(delegate
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
		return projectSettings.GameScenes.LevelScenesLoadOrder.Where((SerializedScene _scene) => SceneManager.GetSceneByBuildIndex(_scene.BuildIndex).isLoaded).ToObservable().SelectMany((Func<SerializedScene, IObservable<AsyncOperation>>)Unload)
			.AsSingleUnitObservable();
		static IObservable<AsyncOperation> Unload(SerializedScene scene)
		{
			return SceneManager.UnloadSceneAsync(scene.BuildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).AsObservable();
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
