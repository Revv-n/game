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
		(from _ in Observable.Interval(TimeSpan.FromSeconds(0.2))
			where promoteWindow.IsOpened
			where Input.GetKey(KeyCode.S)
			select _).Subscribe(delegate
		{
			AddSouls();
		}).AddTo(this);
	}

	private void AddSouls()
	{
		promoteWindow.Character.Promote.AddSoul(1);
	}
}
