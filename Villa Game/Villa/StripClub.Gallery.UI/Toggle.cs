using System;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Gallery.UI;

public class Toggle : MonoBehaviour
{
	[SerializeField]
	protected Button Button;

	[SerializeField]
	protected Sprite defaultSprite;

	[SerializeField]
	protected SpriteState sprite;

	[SerializeField]
	protected Text Text;

	public bool IsSelected { get; protected set; }

	public bool IsInteractable { get; protected set; }

	protected virtual void Awake()
	{
		Assert.IsNotNull(Button);
		Assert.IsNotNull(defaultSprite);
		IsInteractable = Button.interactable;
	}

	public virtual void Init(string text, Action<Toggle> clickAction)
	{
		if (Text != null)
		{
			Text.text = text;
		}
		Button.onClick.RemoveAllListeners();
		Button.onClick.AddListener(delegate
		{
			clickAction(this);
		});
	}

	public virtual void SetSelected(bool isSelected)
	{
		IsSelected = isSelected;
		UpdateSprite();
	}

	public virtual void SetInteractable(bool isInteractable)
	{
		IsInteractable = isInteractable;
		Button.interactable = IsInteractable;
		UpdateSprite();
	}

	public void UpdateSprite()
	{
		Button.image.sprite = ((!IsInteractable) ? sprite.disabledSprite : (IsSelected ? sprite.selectedSprite : defaultSprite));
	}
}
