using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Messenger;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class MessageTaskView : MonoView<Task>, IView
{
	public class Factory : PlaceholderFactory<MessageTaskView>
	{
	}

	[SerializeField]
	private Transform taskItemContainer;

	[SerializeField]
	private CompletableView completeElements;

	[SerializeField]
	private Button start;

	[SerializeField]
	private Button complete;

	[SerializeField]
	private WindowOpener openMessenger;

	[SerializeField]
	private WindowOpener closeBook;

	[SerializeField]
	private WindowOpener closeMain;

	[SerializeField]
	private WindowOpener openMerge;

	[SerializeField]
	private LocalizedTextMeshPro title;

	[SerializeField]
	private Image icon;

	private string nameKey;

	private Sprite avatar;

	private ResponseOption option;

	private List<ObjectiveView> bookTaskItemViews = new List<ObjectiveView>();

	private IWindowsManager windowsManager;

	private ObjectiveViewManager manager;

	public bool IsEnough { get; protected set; } = true;


	public new void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public new bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	[Inject]
	private void Init(IWindowsManager windowsManager, ObjectiveViewManager manager)
	{
		this.windowsManager = windowsManager;
		this.manager = manager;
		SubscribeButtons();
	}

	public void Set(string nameKey, Sprite avatar, ResponseOption option)
	{
		if (option != null)
		{
			this.nameKey = nameKey;
			this.avatar = avatar;
			this.option = option;
			SetView();
		}
	}

	private void SubscribeButtons()
	{
		complete.onClick.AddListener(OpenMessenger);
		start.onClick.AddListener(OpenMergeField);
	}

	private void OpenMessenger()
	{
		openMessenger.Click();
	}

	private void OpenMergeField()
	{
		if ((from _window in windowsManager.GetOpened()
			where _window is MergeWindow
			select _window).Any())
		{
			closeBook.Close();
			return;
		}
		closeBook.Close();
		closeMain.Close();
		openMerge.Click();
	}

	private void SetView()
	{
		manager.HideAll();
		bookTaskItemViews.Clear();
		SetLocalization();
		SetAvatar();
		IsEnough = true;
		foreach (IItemLot necessaryItem in option.NecessaryItems)
		{
			_ = necessaryItem;
		}
		SetElementToComplete(IsEnough);
		SetButton(IsEnough);
	}

	private void SetLocalization()
	{
		title.Init(nameKey);
	}

	private void SetAvatar()
	{
		icon.sprite = avatar;
	}

	private void SetElementToComplete(bool isEnough)
	{
		completeElements.SetComplete(isEnough);
	}

	private void SetButton(bool isComplete)
	{
		complete.gameObject.SetActive(isComplete);
		start.gameObject.SetActive(!isComplete);
	}
}
