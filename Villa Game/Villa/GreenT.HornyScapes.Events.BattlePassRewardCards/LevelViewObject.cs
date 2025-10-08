using GreenT.HornyScapes.BattlePassSpace;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class LevelViewObject : MonoView
{
	public Image Image;

	public TMP_Text Text;

	public Sprite CompletedSprite;

	public Sprite StartSprite;

	public Color StartColor;

	public Color CompletedColor;

	public void SetCompleted()
	{
		Image.sprite = CompletedSprite;
		Text.color = CompletedColor;
	}

	public void Reset()
	{
		base.gameObject.SetActive(value: true);
		Image.sprite = StartSprite;
		Text.color = StartColor;
	}

	public void SetBattlePassSprites(BattlePass bp)
	{
		try
		{
			CompletedSprite = bp.Bundle.LevelBonusHolder;
			StartSprite = bp.Bundle.LevelHolder;
		}
		catch
		{
			Debug.LogError($"LevelViewObject: no battle pass data {bp.ID} or empty BP bundle {bp.Bundle.Type}");
		}
	}
}
