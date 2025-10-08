using System;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.Services;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnyGirlAnyPresentObjective : PresentObjective
{
	public AnyGirlAnyPresentObjective(Func<Sprite> iconProvider, SavableObjectiveData data, PresentsNotifier presentsNotifier)
		: base(iconProvider, presentsNotifier, data)
	{
	}

	protected override void AddProgress(Present present)
	{
		Data.Progress++;
		base.AddProgress(present);
	}
}
