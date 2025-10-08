using System;
using StripClub.UI.Collections.Promote;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class AddSoulsCheat : MonoBehaviour
{
	[Inject]
	private PromoteWindow promoteWindow;

	private void Start()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.Where<long>(Observable.Interval(TimeSpan.FromSeconds(0.2)), (Func<long, bool>)((long _) => promoteWindow.IsOpened)), (Func<long, bool>)((long _) => Input.GetKey(KeyCode.S))), (Action<long>)delegate
		{
			AddSouls();
		}), (Component)this);
	}

	private void AddSouls()
	{
		promoteWindow.Character.Promote.AddSoul(1);
	}
}
