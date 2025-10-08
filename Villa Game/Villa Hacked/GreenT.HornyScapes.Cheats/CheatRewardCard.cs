using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.Cheats;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes.Cheats;

public class CheatRewardCard : MonoBehaviour
{
	[Header("And Use RightMouseButton")]
	[SerializeField]
	private InputSettingCheats inputSetting;

	private Lot _lot;

	private EventSystem _eventSystem;

	protected void Awake()
	{
		_eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
	}

	private void Start()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.EveryUpdate(), (Func<long, bool>)((long _) => Input.GetMouseButtonDown(1) && Input.GetKey(inputSetting.CopyTaskId))), (Action<long>)delegate
		{
			CopyCardData();
		}), (Component)this);
	}

	private void CopyCardData()
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerEventData, list);
		BattlePassRewardCard battlePassRewardCard = null;
		foreach (RaycastResult item in list)
		{
			battlePassRewardCard = item.gameObject.transform.GetComponentInParent<BattlePassRewardCard>();
			if (battlePassRewardCard != null)
			{
				break;
			}
		}
		if (!(battlePassRewardCard == null))
		{
			battlePassRewardCard.Source.Content.GetRewTypeFromLinked().ToString().ToLower();
			CopyUtil.Copy(battlePassRewardCard.Source.Selector.GetIDFromSelector());
		}
	}
}
