using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

public class UIObjectiveBlockSelector : MonoBehaviour
{
	[SerializeField]
	private LocalizedTextMeshPro description;

	[SerializeField]
	private LocalizedTextMeshPro descriptionComplete;

	[SerializeField]
	private Transform mergeItemBlock;

	[SerializeField]
	private Transform descriptionBlock;

	[SerializeField]
	private Transform mergeItemBlockComplete;

	[SerializeField]
	private Transform descriptionBlockComplete;

	public void Set(Task task)
	{
		ChooseBlock(task);
	}

	private void ChooseBlock(Task task)
	{
		bool flag = task.Goal.Objectives[0] is MergeItemObjective;
		mergeItemBlock.gameObject.SetActive(flag);
		mergeItemBlockComplete.gameObject.SetActive(flag);
		descriptionBlock.gameObject.SetActive(!flag);
		descriptionBlockComplete.gameObject.SetActive(!flag);
		if (!flag)
		{
			SetDescriptionBlock(task);
		}
	}

	private void SetDescriptionBlock(Task task)
	{
		description.Init(task.Goal.Description);
		descriptionComplete.Init(task.Goal.Description);
	}
}
