using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.Data;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class GameMaster : MonoBehaviour
{
	[SerializeField]
	private List<BaseController> allControllers;

	private SoundController _soundController;

	private ISaver _saver;

	private MergePointsController _mergePointsController;

	[Inject]
	public void Init(SoundController soundController, ISaver saver, MergePointsController mergePointsController)
	{
		_mergePointsController = mergePointsController;
		_soundController = soundController;
		_saver = saver;
	}

	[ContextMenu("Validate")]
	private void Validate()
	{
		BaseController[] array = Object.FindObjectsOfType<BaseController>();
		if (array.Length != allControllers.Count)
		{
			allControllers.Clear();
			allControllers.AddRange(array);
		}
	}

	private void Awake()
	{
		UnityEngine.Resources.UnloadUnusedAssets();
		allControllers.Add(_soundController);
		Preload();
		LoadAll();
		InitAll();
		Screen.sleepTimeout = -1;
	}

	private void Preload()
	{
		foreach (BaseController item in allControllers.OrderBy((BaseController x) => x.PreloadOrder))
		{
			item.Preload();
		}
		_mergePointsController.Init();
	}

	private void LoadAll()
	{
		foreach (ISavableState item in allControllers.OfType<ISavableState>())
		{
			_saver.Add(item);
		}
	}

	private void InitAll()
	{
		foreach (IMasterController item in allControllers.OfType<IMasterController>())
		{
			item.LinkControllers(allControllers);
		}
		BaseController[] array = allControllers.OrderBy((BaseController x) => x.PreloadOrder).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}

	private void OnDestroy()
	{
		foreach (ISavableState item in allControllers.OfType<ISavableState>())
		{
			_saver.Remove(item);
		}
		foreach (BaseController allController in allControllers)
		{
			if (allController is IGameExitListener gameExitListener)
			{
				gameExitListener.BeforeExit();
			}
		}
		DOTween.KillAll();
	}
}
