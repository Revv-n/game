using Merge.Core.Windows;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Merge.Core.Events;

public class EventWindow : PopupWindow
{
	[SerializeField]
	private Transform parent;

	[SerializeField]
	private Image previewImage;

	[SerializeField]
	private Text titleText;

	[SerializeField]
	private Text[] desctiprionText;

	[SerializeField]
	private Text progressCountText;

	[SerializeField]
	private Text buttonText;

	[SerializeField]
	private Button mainButton;

	[SerializeField]
	private Button closeButton;

	[SerializeField]
	private GameObject partStart;

	[SerializeField]
	private GameObject partProgress;

	[SerializeField]
	private GameObject backStart;

	[SerializeField]
	private GameObject backProgress;

	private void Start()
	{
		closeButton.SetClickCallback(AtNoClick);
	}

	protected override void AtBecomeHide()
	{
	}

	protected override void AtBecomeShow()
	{
		base.AtBecomeShow();
	}

	public EventWindow SetData(EventConfig config, EventStatus eventStatus)
	{
		switch (eventStatus)
		{
		case EventStatus.available:
			ValidateElements("event/StartButton", AtOkClick, allowClose: true, isStart: true, isProgress: false);
			break;
		case EventStatus.inProgress:
			ValidateElements("event/ShopButton", AtShopClick, allowClose: true, isStart: false, isProgress: true);
			break;
		case EventStatus.completed:
			ValidateElements("event/ShopButton", AtShopClick, allowClose: true, isStart: false, isProgress: true);
			break;
		}
		titleText.text = config.Title;
		Text[] array = desctiprionText;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = config.Description;
		}
		previewImage.sprite = config.Preview(Controller<EventsController>.Instance.CurrentEvent);
		return this;
	}

	private void AtOkClick()
	{
		Controller<EventsController>.Instance.StartEvent();
		Hide();
	}

	private void AtNoClick()
	{
		Hide();
	}

	private void AtShopClick()
	{
	}

	private void ValidateElements(string buttonStr, UnityAction buttonCallback, bool allowClose, bool isStart, bool isProgress)
	{
		mainButton.SetClickCallback(buttonCallback);
		buttonText.text = buttonStr.ToUpper();
		partStart.SetActive(isStart);
		partProgress.SetActive(isProgress);
		backStart.SetActive(isStart);
		backProgress.SetActive(isProgress);
		closeButton.SetActive(allowClose);
	}
}
