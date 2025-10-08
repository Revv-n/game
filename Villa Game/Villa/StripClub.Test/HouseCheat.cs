using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Cheats;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta;
using Merge.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace StripClub.Test;

public class HouseCheat : IInitializable, IDisposable
{
	private int objId;

	private int roomId;

	private bool IsCheat = true;

	private readonly RoomManager _house;

	private RoomObjectsBuilder _roomObjectsBuilder;

	private readonly InputSettingCheats _inputSetting;

	private IDisposable _disposable;

	private EventSystem m_EventSystem;

	public HouseCheat(RoomManager house, RoomObjectsBuilder roomObjectsBuilder, InputSettingCheats inputSetting)
	{
		_house = house;
		_roomObjectsBuilder = roomObjectsBuilder;
		_inputSetting = inputSetting;
	}

	public void Update()
	{
		if (IsCheat)
		{
			RoomObjectsCheat();
			if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
			{
				PrintId();
			}
		}
	}

	public void Initialize()
	{
		_disposable = _roomObjectsBuilder.OnCreated.Subscribe(delegate(IRoomObject<BaseObjectConfig> _roomObject)
		{
			if (_roomObject.Config is RoomObjectConfig roomObjectConfig)
			{
				foreach (RoomObjectReference roomObjectReference in roomObjectConfig.RoomObjectReferences)
				{
					_ = roomObjectReference;
				}
			}
		});
	}

	private void RoomObjectsCheat()
	{
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Room roomOrDefault = _house.GetRoomOrDefault(roomId);
			if (objId == roomOrDefault.RoomObjects.Count())
			{
				roomId++;
				roomOrDefault = _house.GetRoomOrDefault(roomId);
				objId = 0;
			}
			IRoomObject<BaseObjectConfig> roomObject = roomOrDefault.RoomObjects.OrderBy((IRoomObject<BaseObjectConfig> rj) => rj.Config.Number).ToArray()[objId];
			if (roomObject.Config.TrashBehaviour)
			{
				roomObject.SetView(1);
			}
			else
			{
				roomObject?.SetStatus(EntityStatus.InProgress);
				roomObject?.SetStatus(EntityStatus.Complete);
				roomObject?.SetStatus(EntityStatus.Rewarded);
			}
			objId++;
		}
		if (!Input.GetKeyDown(KeyCode.LeftArrow))
		{
			return;
		}
		Room roomOrDefault2 = _house.GetRoomOrDefault(roomId);
		if (objId == roomOrDefault2.RoomObjects.Count())
		{
			roomId++;
			roomOrDefault2 = _house.GetRoomOrDefault(roomId);
			objId = 0;
		}
		else if (objId == 0 && roomId > 0)
		{
			roomId--;
			roomOrDefault2 = _house.GetRoomOrDefault(roomId);
			objId = 1;
		}
		if (objId > 0)
		{
			objId--;
		}
		IRoomObject<BaseObjectConfig> obj = roomOrDefault2.RoomObjects.OrderBy((IRoomObject<BaseObjectConfig> rj) => rj.Config.Number).ToArray()[objId];
		obj.SetView(0);
		obj.SetStatus(EntityStatus.InProgress);
		foreach (RoomObjectView view in ((RoomObject)obj).Views)
		{
			if ((bool)view.GetComponent<SpriteRenderer>())
			{
				SpriteRenderer component = view.GetComponent<SpriteRenderer>();
				component.material = new Material(component.material);
				component.material.SetFloat("_Lerp_Fade_1", 0f);
				component.material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
			}
		}
	}

	public void PrintId()
	{
		PointerEventData pointerEventData = new PointerEventData(m_EventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerEventData, list);
		RoomObject roomObject = null;
		foreach (RaycastResult item in list)
		{
			roomObject = item.gameObject.transform.GetComponentInParent<RoomObject>();
			if (!(roomObject == null))
			{
				roomObject.Config.ID.ToString();
			}
		}
	}

	public void Dispose()
	{
		_disposable?.Dispose();
	}
}
