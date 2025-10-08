using System;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class GainObjective : SavableObjective
{
	private readonly Func<Sprite> iconProvider;

	public GainObjective(Func<Sprite> iconProvider, SavableObjectiveData data)
		: base(data)
	{
		this.iconProvider = iconProvider;
	}

	public override Sprite GetIcon()
	{
		return iconProvider();
	}

	public override int GetProgress()
	{
		return Data.Progress;
	}

	public override int GetTarget()
	{
		return Data.Required;
	}

	public override void Track()
	{
	}

	public override void OnRewardTask()
	{
	}
}
