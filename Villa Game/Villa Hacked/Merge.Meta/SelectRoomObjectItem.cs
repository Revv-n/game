using System;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Meta;

public class SelectRoomObjectItem : MonoBehaviour
{
	[SerializeField]
	private int state;

	[SerializeField]
	private Image back;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Button btn;

	public Sprite SelectedSprite => icon.sprite;

	public int State => state;

	public event Action<SelectRoomObjectItem> OnItemClick;

	private void Start()
	{
		btn.AddClickCallback(delegate
		{
			this.OnItemClick?.Invoke(this);
		});
	}

	public SelectRoomObjectItem SetSprite(Sprite sprt)
	{
		icon.sprite = sprt;
		return this;
	}

	public SelectRoomObjectItem SetBackgroundSprite(Sprite sprt)
	{
		back.sprite = sprt;
		return this;
	}
}
