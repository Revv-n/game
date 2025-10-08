using System;
using GreenT.HornyScapes.BattlePassSpace;
using UnityEngine;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

[Serializable]
public class LevelViewObjectCase
{
	[SerializeField]
	private LevelViewObject start;

	[SerializeField]
	private LevelViewObject end;

	public bool IsProgress { get; private set; }

	public bool IsCompleted { get; private set; }

	public void Initialization(int targetLevel, int startLevel, bool isLast)
	{
		start.Text.text = startLevel.ToString();
		end.Text.text = targetLevel.ToString();
		start.Reset();
		end.Reset();
		end.gameObject.SetActive(isLast);
	}

	public void SetCompleted()
	{
		end.SetCompleted();
		IsCompleted = true;
	}

	public void SetStartProgress()
	{
		start.SetCompleted();
		IsProgress = true;
	}

	public void SetBattlePassSprites(BattlePass bp)
	{
		start.SetBattlePassSprites(bp);
		end.SetBattlePassSprites(bp);
	}
}
