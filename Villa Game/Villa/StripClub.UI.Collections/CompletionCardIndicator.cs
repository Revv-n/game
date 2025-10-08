using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Collections;

public class CompletionCardIndicator : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private List<GameObject> hideObjects;

	[SerializeField]
	private List<GameObject> showObjects;

	[SerializeField]
	private List<StatableComponent> statableComponents;

	private void Start()
	{
		Track(slider);
	}

	private void OnDestroy()
	{
		slider.onValueChanged.RemoveAllListeners();
	}

	private void Track(Slider slider)
	{
		slider.onValueChanged.AddListener(delegate(float value)
		{
			Set(value >= 1f);
		});
		Set(slider.value >= 1f);
	}

	private void Set(bool active)
	{
		foreach (GameObject hideObject in hideObjects)
		{
			hideObject.SetActive(!active);
		}
		foreach (GameObject showObject in showObjects)
		{
			showObject.SetActive(active);
		}
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(active ? 1 : 0);
		}
	}
}
