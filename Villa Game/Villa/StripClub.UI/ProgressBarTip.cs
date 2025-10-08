using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class ProgressBarTip : MonoBehaviour
{
	[SerializeField]
	private Image fillMask;

	[SerializeField]
	private float angleShift = 180f;

	private RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void RotateAroundAnchor()
	{
		float num = fillMask.fillAmount * 360f - angleShift;
		Vector3 euler = new Vector3(rectTransform.rotation.eulerAngles.x, rectTransform.rotation.eulerAngles.y, 0f - num);
		rectTransform.rotation = Quaternion.Euler(euler);
	}

	private void Update()
	{
		RotateAroundAnchor();
	}
}
