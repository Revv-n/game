using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using StripClub.Model.Quest;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatTask : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button taskBtn;

	[SerializeField]
	private StatableComponent imageState;

	private Task task;

	private TaskManagerCluster taskManagerCluster;

	private ContentSelectorGroup contentSelector;

	private EventSystem m_EventSystem;

	[Header("And Use LeftMouseButton")]
	[SerializeField]
	private InputSettingCheats inputSetting;

	[Inject]
	private void InnerInit(TaskManagerCluster taskManagerCluster, ContentSelectorGroup contentSelector)
	{
		this.taskManagerCluster = taskManagerCluster;
		this.contentSelector = contentSelector;
	}

	protected void Awake()
	{
		OnEnterValue(inputField.text);
		inputField.onValueChanged.AddListener(OnEnterValue);
		taskBtn.onClick.AddListener(AddItem);
		m_EventSystem = Object.FindObjectOfType<EventSystem>();
	}

	public void AddItemInPocketForTasksByKey()
	{
		MergeItemObjective[] array = taskManagerCluster[contentSelector.Current].ActiveObjectives.OfType<MergeItemObjective>().ToArray();
		foreach (MergeItemObjective objective in array)
		{
			AddItem(objective);
		}
	}

	[EditorButton]
	public void GetItemsForTasksByButton()
	{
		int.TryParse(inputField.text, out var result);
		Task[] array = (from _task in taskManagerCluster[contentSelector.Current].Collection.Take(result)
			where _task.State != StateType.Rewarded
			select _task).ToArray();
		foreach (Task task in array)
		{
			if (task.Goal.Objectives[0] is MergeItemObjective)
			{
				AddItem(task);
			}
		}
		inputField.text = string.Empty;
	}

	protected void OnEnterValue(string value)
	{
		if (int.TryParse(value, out var result) && taskManagerCluster.TryGetItem(result, out task))
		{
			imageState.Set(1);
			taskBtn.interactable = true;
		}
		else
		{
			taskBtn.interactable = false;
			imageState.Set(0);
		}
	}

	private void AddItem()
	{
		AddItem(task);
	}

	private void AddItem(MergeItemObjective objective)
	{
	}

	private void AddItem(Task task)
	{
		IObjective[] objectives = task.Goal.Objectives;
		foreach (IObjective objective in objectives)
		{
			AddItem((MergeItemObjective)objective);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(inputSetting.AddItemsForCurrentTasks))
		{
			AddItemInPocketForTasksByKey();
		}
		if (Input.GetMouseButtonDown(0) && Input.GetKey(inputSetting.CopyTaskId))
		{
			CopyTaskId();
		}
	}

	public void CopyTaskId()
	{
		PointerEventData pointerEventData = new PointerEventData(m_EventSystem);
		List<RaycastResult> list = new List<RaycastResult>();
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerEventData, list);
		TaskView taskView = null;
		foreach (RaycastResult item in list)
		{
			taskView = item.gameObject.transform.GetComponentInParent<TaskView>();
			if (taskView != null)
			{
				break;
			}
		}
		if (!(taskView == null))
		{
			CopyUtil.Copy(taskView.Source.ID.ToString());
		}
	}
}
