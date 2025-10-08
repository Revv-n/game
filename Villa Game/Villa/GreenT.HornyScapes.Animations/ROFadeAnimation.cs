using DG.Tweening;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using Merge.Meta;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ROFadeAnimation : Animation
{
	[SerializeField]
	private float duration = 0.5f;

	[SerializeField]
	private RoomObjectConfig[] fadeObjects;

	[SerializeField]
	private Color fadedColor;

	[SerializeField]
	private Color targetColor = UnityEngine.Color.white;

	private RoomManager house;

	private static readonly int Color = Shader.PropertyToID("_Color");

	public void SetHouse(RoomManager house)
	{
		this.house = house;
	}

	public override Sequence Play()
	{
		Room roomOrDefault = house.GetRoomOrDefault(fadeObjects[0].RoomID);
		RoomObjectConfig[] array = fadeObjects;
		foreach (RoomObjectConfig roomObjectConfig in array)
		{
			if (!(roomOrDefault.GetObjectOrDefault(roomObjectConfig.Number) is RoomObject roomObject))
			{
				continue;
			}
			foreach (RoomObjectView view in roomObject.Views)
			{
				view.Renderer.material.DOColor(targetColor, Color, duration);
			}
		}
		return base.Play();
	}

	public override void Init()
	{
		RoomObjectConfig[] array = fadeObjects;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (RoomObjectViewInfo view in array[i].Views)
			{
				if ((bool)view.Material)
				{
					view.Material.SetColor(Color, fadedColor);
				}
				if ((bool)view.BlendMaterial)
				{
					view.BlendMaterial.SetColor(Color, fadedColor);
				}
			}
		}
	}

	public override void ResetToAnimStart()
	{
		Init();
	}
}
