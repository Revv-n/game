using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.Tools;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Meta.RoomObjects;

public class CharacterObjectVisibleController : MonoBehaviour, IDisposable
{
	[SerializeField]
	private CharacterShowHideConfig config;

	private ICameraChanger cameraChanger;

	private INavigation navigation;

	private RoomManager roomManager;

	private VisibilityChecker visibilityChecker;

	private IDisposable onCameraMoveStream;

	private LinkedList<CharacterObject> loadList;

	private LinkedList<CharacterObject> unloadList;

	private float loadTime;

	private float releaseTime;

	private Camera Camera => cameraChanger.MainCamera;

	[Inject]
	private void Construct(ICameraChanger cameraChanger, INavigation navigation, RoomManager roomManager)
	{
		this.cameraChanger = cameraChanger;
		this.navigation = navigation;
		this.roomManager = roomManager;
	}

	private void Awake()
	{
		if (!config.enabled)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		visibilityChecker = new VisibilityChecker();
		loadList = new LinkedList<CharacterObject>();
		unloadList = new LinkedList<CharacterObject>();
		navigation.OnDrag().Subscribe(OnCameraMove);
	}

	private void Update()
	{
		if (loadList.Count == 0)
		{
			ReleaseGirls();
			return;
		}
		releaseTime = 0f;
		HandleGirlsLoading();
	}

	private void HandleGirlsLoading()
	{
		releaseTime = 0f;
		loadTime += Time.deltaTime;
		if (!(loadTime < config.loadTimeStep))
		{
			loadList.First.Value.ReDownloadAndShow();
			loadList.RemoveFirst();
			loadTime = 0f;
		}
	}

	private void ReleaseGirls()
	{
		if (unloadList.Count == 0)
		{
			releaseTime = 0f;
			return;
		}
		if (releaseTime < config.releaseDelay)
		{
			releaseTime += Time.deltaTime;
			return;
		}
		unloadList.First.Value.HideAndUnload();
		unloadList.RemoveFirst();
	}

	private void OnCameraMove(Vector2 delta)
	{
		foreach (Room item in roomManager.Collection)
		{
			foreach (IRoomObject<BaseObjectConfig> roomObject in item.RoomObjects)
			{
				if (roomObject is CharacterObject characterObject && characterObject.IsCharacterAvailable())
				{
					CheckGirl(characterObject);
				}
			}
		}
	}

	private void CheckGirl(CharacterObject characterObject)
	{
		float cameraExtentsCoef = (characterObject.IsSetAsVisible ? config.extentsCoefWhenVisible : config.extentsCoefWhenNonVisible);
		Vector3 position = characterObject.Position;
		Vector3 extents = characterObject.Extents;
		if (visibilityChecker.IsVisible(Camera, cameraExtentsCoef, position, extents))
		{
			if (unloadList.Contains(characterObject))
			{
				unloadList.Remove(characterObject);
			}
			if (!characterObject.IsSetAsVisible && !loadList.Contains(characterObject))
			{
				loadList.AddLast(characterObject);
			}
		}
		else
		{
			if (loadList.Contains(characterObject))
			{
				loadList.Remove(characterObject);
			}
			if (characterObject.IsSetAsVisible && !unloadList.Contains(characterObject))
			{
				unloadList.AddLast(characterObject);
			}
		}
	}

	public void Dispose()
	{
		onCameraMoveStream?.Dispose();
	}
}
