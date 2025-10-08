using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

public class CompleteResizing : MonoBehaviour
{
	public Vector3 DefaultSize = new Vector3(342.88f, 119.95f, 0f);

	public Vector3 CompleteSize = new Vector3(342.88f, 169.95f, 0f);

	public GameObject Separator;

	public RectTransform Target;

	private void OnEnable()
	{
		Separator.SetActive(value: true);
		Target.sizeDelta = CompleteSize;
	}

	private void OnDisable()
	{
		Separator.SetActive(value: false);
		Target.sizeDelta = DefaultSize;
	}
}
