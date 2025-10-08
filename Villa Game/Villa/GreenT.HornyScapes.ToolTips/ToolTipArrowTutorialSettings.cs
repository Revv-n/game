using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipArrowTutorialSettings : ToolTipSettings
{
	public Vector3 Rotation;

	[Header("Спец. Анимация: Смещение пальца и бесконечный тап в конце")]
	public bool OneMoveEndlessClick;

	[Header("Пальчик будет двигаться в EndPosition")]
	public bool MoveToEndPos;

	public Vector3 EndPosition;

	[Header("Пальчик будет анимированно тапать")]
	public bool PlayClick;

	public int StepID { get; set; }
}
