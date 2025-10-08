using UnityEngine;
using UnityEngine.UI;

namespace Merge.MotionDesign.TweenPanels.Utils;

public class ReflectionSlider : ReflectionViewElement
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private Text valueLabel;

	protected override void OnInit()
	{
		slider.onValueChanged.AddListener(SetValueInTarget);
		float fieldValue = SafeReflections.GetFieldValue<float>(target, property);
		slider.maxValue = fieldValue * 4f;
		slider.minValue = fieldValue / 4f;
		slider.value = fieldValue;
	}

	private void SetValueInTarget(float value)
	{
		SafeReflections.SetFieldValue(target, property, value);
		valueLabel.text = value.ToString("F2");
	}
}
