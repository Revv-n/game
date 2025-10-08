using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Merge.Meta;

public class SceneController : Controller<SceneController>
{
	private AsyncOperation asyncOperation;

	private bool loadComplete;

	private bool animationComplete;

	private bool isLoadingNow;

	public static Scenes ActiveScene => SceneByName(SceneManager.GetActiveScene().name);

	public static bool IsCoreNow => ActiveScene == Scenes.Core;

	public static bool IsMetaNow => ActiveScene == Scenes.Meta;

	public static bool IsStartNow => ActiveScene == Scenes.Start;

	public static bool IsLoadingNow => Controller<SceneController>.Instance.isLoadingNow;

	public void LoadScene(string scene)
	{
		if (!isLoadingNow)
		{
			isLoadingNow = true;
			DOTween.KillAll();
			StartCoroutine(CRT_KillTweensNextFrame());
			Sounds.Play("Switch_Screen");
			StartCoroutine(CRT_LoadScene(scene));
			StartCoroutine(Controller<UIMaster>.Instance.HideObjects());
		}
	}

	private IEnumerator CRT_KillTweensNextFrame()
	{
		yield return new WaitForEndOfFrame();
		DOTween.KillAll();
	}

	private static Scenes SceneByName(string name)
	{
		if (name == "Core")
		{
			return Scenes.Core;
		}
		if (name == "Meta")
		{
			return Scenes.Meta;
		}
		_ = name == "Start";
		return Scenes.Start;
	}

	private IEnumerator CRT_LoadScene(string scene)
	{
		yield return null;
		asyncOperation = SceneManager.LoadSceneAsync(scene);
		asyncOperation.allowSceneActivation = false;
		while (!asyncOperation.isDone && !loadComplete)
		{
			if (asyncOperation.progress >= 0.9f)
			{
				loadComplete = true;
			}
			if (loadComplete && animationComplete)
			{
				asyncOperation.allowSceneActivation = true;
			}
			yield return null;
		}
	}

	public static void LoadScene(Scenes scene)
	{
		Controller<SceneController>.Instance.LoadScene(scene.ToString());
	}
}
