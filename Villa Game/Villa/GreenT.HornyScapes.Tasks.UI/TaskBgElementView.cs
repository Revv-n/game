using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskBgElementView : MonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Sprite defaultSprite;

	[SerializeField]
	private Sprite completeSprite;

	public void SetBackground(bool isComplete)
	{
		background.sprite = (isComplete ? completeSprite : defaultSprite);
	}
}
