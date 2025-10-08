using System;
using GreenT.HornyScapes.Meta.RoomObjects;
using UnityEngine;
using Zenject;

namespace Merge.Meta.RoomObjects;

public class RoomObjectViewFactory : MonoBehaviour, IFactory<Func<bool>, Action, RoomObjectViewInfo, RoomObjectView>, IFactory
{
	[SerializeField]
	private RoomObjectView viewPrefab;

	[SerializeField]
	private Transform viewRoot;

	[Inject]
	private DiContainer container;

	public RoomObjectView Create(Func<bool> clickEnableDelegate, Action clickCallback, RoomObjectViewInfo info)
	{
		RoomObjectView roomObjectView;
		if (Application.isPlaying)
		{
			roomObjectView = container.InstantiatePrefabForComponent<RoomObjectView>(viewPrefab, viewRoot);
			roomObjectView.transform.SetParent(viewRoot);
		}
		else
		{
			roomObjectView = UnityEngine.Object.Instantiate(viewPrefab, viewRoot);
		}
		roomObjectView.Init(clickEnableDelegate, clickCallback, info);
		roomObjectView.transform.SetDefault();
		if (Application.isPlaying)
		{
			roomObjectView.CreateAnimations(info);
		}
		return roomObjectView;
	}
}
