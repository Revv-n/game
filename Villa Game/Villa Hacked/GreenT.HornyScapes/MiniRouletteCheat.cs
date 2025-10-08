using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreenT.HornyScapes.Cheats;
using GreenT.HornyScapes.MiniEvents;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes;

public class MiniRouletteCheat : MonoBehaviour, IDisposable
{
	[SerializeField]
	private InputSettingCheats _inputSetting;

	private CompositeDisposable _inputStream = new CompositeDisposable();

	private EventSystem _eventSystem;

	private void Start()
	{
		_eventSystem = EventSystem.current;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.EveryUpdate(), (Func<long, bool>)((long _) => Input.GetKey(_inputSetting.Roulette) && Input.GetMouseButtonDown(0))), (Action<long>)delegate
		{
			ShowRouletteInfo();
		}), (ICollection<IDisposable>)_inputStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.EveryUpdate(), (Func<long, bool>)((long _) => Input.GetKey(_inputSetting.Roulette) && Input.GetMouseButtonDown(1))), (Action<long>)delegate
		{
			SetMaxRollCount();
		}), (ICollection<IDisposable>)_inputStream);
	}

	private void ShowRouletteInfo()
	{
		ShowRouletteID();
		ShowRouletteChance();
	}

	private void ShowRouletteID()
	{
		if (TryGetRouletteLot(out var rouletteLot))
		{
			CopyUtil.Copy(rouletteLot.ID.ToString());
		}
	}

	private void ShowRouletteChance()
	{
		if (TryGetRouletteLot(out var rouletteLot))
		{
			_ = (int)typeof(RouletteLot).GetField("_rollCount", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rouletteLot);
			RouletteDropService obj = typeof(RouletteLot).GetField("_rouletteDropService", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rouletteLot) as RouletteDropService;
			typeof(RouletteDropService).GetField("_garantChance", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
		}
	}

	private void SetMaxRollCount()
	{
		if (TryGetRouletteLot(out var rouletteLot))
		{
			RouletteDropService obj = typeof(RouletteLot).GetField("_rouletteDropService", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rouletteLot) as RouletteDropService;
			GarantChance obj2 = typeof(RouletteDropService).GetField("_garantChance", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj) as GarantChance;
			int num = (typeof(GarantChance).GetField("_chances", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj2) as Dictionary<int, int>).Keys.Last();
			typeof(RouletteLot).GetField("_rollCount", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(rouletteLot, num);
		}
	}

	private bool TryGetRouletteLot(out RouletteLot rouletteLot)
	{
		PointerEventData pointerEventData = new PointerEventData(_eventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		_eventSystem.RaycastAll(pointerEventData, list);
		RouletteLotBackgroundView rouletteLotBackgroundView = null;
		rouletteLot = null;
		foreach (RaycastResult item in list)
		{
			rouletteLotBackgroundView = item.gameObject.transform.parent.GetComponentInChildren<RouletteLotBackgroundView>();
			if (rouletteLotBackgroundView != null)
			{
				rouletteLot = rouletteLotBackgroundView.Source;
				return true;
			}
		}
		return false;
	}

	private void OnDestroy()
	{
		Dispose();
	}

	public void Dispose()
	{
		_inputStream.Dispose();
	}
}
