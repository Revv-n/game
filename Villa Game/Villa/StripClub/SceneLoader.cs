using System;
using System.Collections;
using System.Collections.Generic;
using GreenT.Settings;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StripClub;

public static class SceneLoader
{
	public static IEnumerator LoadScenes(IEnumerable<SerializedScene> sceneParameters, LoadSceneMode loadMode = LoadSceneMode.Additive)
	{
		foreach (SerializedScene sceneParameter in sceneParameters)
		{
			if (!SceneManager.GetSceneByBuildIndex(sceneParameter.BuildIndex).isLoaded)
			{
				AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneParameter.BuildIndex, loadMode);
				while (!loadSceneOperation.isDone)
				{
					yield return null;
				}
			}
		}
	}

	public static IObservable<Unit> LoadScenesAsObservable(IEnumerable<SerializedScene> sceneParameters, LoadSceneMode loadMode = LoadSceneMode.Additive)
	{
		return Observable.FromCoroutine(() => LoadScenes(sceneParameters, loadMode));
	}

	public static IEnumerator LoadScene(SerializedScene sceneInfo, LoadSceneMode loadMode = LoadSceneMode.Additive)
	{
		if (!SceneManager.GetSceneByBuildIndex(sceneInfo.BuildIndex).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(sceneInfo.BuildIndex, loadMode);
		}
	}

	public static IObservable<Unit> LoadSceneAsObservable(SerializedScene sceneInfo, LoadSceneMode loadMode = LoadSceneMode.Additive, bool returnEveryYield = false)
	{
		return Observable.FromCoroutine(() => LoadScene(sceneInfo, loadMode), returnEveryYield);
	}
}
