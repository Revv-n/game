using System;
using System.Collections.Generic;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes.Cheats;

public class BankTabCheat : MonoBehaviour, IDisposable
{
	[SerializeField]
	private InputSettingCheats _inputSettings;

	private IDisposable _inputStream;

	private EventSystem _eventSystem;

	private void Start()
	{
		_eventSystem = EventSystem.current;
		_inputStream = (from _ in Observable.EveryUpdate()
			where Input.GetKey(_inputSettings.ShowBankTabID) && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			select _).Subscribe(delegate
		{
			ShowBankTabID();
		});
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void ShowBankTabID()
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		_eventSystem.RaycastAll(pointerEventData, list);
		BankTabView bankTabView = null;
		foreach (RaycastResult item in list)
		{
			bankTabView = item.gameObject.transform.GetComponentInParent<BankTabView>();
			if (bankTabView != null)
			{
				break;
			}
		}
		if (!(bankTabView == null))
		{
			CopyUtil.Copy(bankTabView.Source.ID.ToString());
		}
	}

	public void Dispose()
	{
		_inputStream.Dispose();
	}
}
