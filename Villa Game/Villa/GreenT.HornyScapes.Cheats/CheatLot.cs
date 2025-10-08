using System.Collections.Generic;
using StripClub.Model.Shop;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes.Cheats;

public class CheatLot : MonoBehaviour
{
	[Header("And Use RightMouseButton")]
	[SerializeField]
	private InputSettingCheats inputSetting;

	private Lot _lot;

	private EventSystem _eventSystem;

	protected void Awake()
	{
		_eventSystem = Object.FindObjectOfType<EventSystem>();
	}

	private void Start()
	{
		(from _ in Observable.EveryUpdate()
			where Input.GetMouseButtonDown(1) && Input.GetKey(inputSetting.CopyTaskId)
			select _).Subscribe(delegate
		{
			CopyLotID();
		}).AddTo(this);
	}

	private void CopyLotID()
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerEventData, list);
		LotView lotView = null;
		foreach (RaycastResult item in list)
		{
			lotView = item.gameObject.transform.GetComponentInParent<LotView>();
			if (lotView != null)
			{
				break;
			}
		}
		if (!(lotView == null))
		{
			CopyUtil.Copy(lotView.Source.ID.ToString());
		}
	}
}
