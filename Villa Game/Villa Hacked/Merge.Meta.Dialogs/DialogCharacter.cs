using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Meta.Dialogs;

public class DialogCharacter : MonoBehaviour
{
	[Serializable]
	public struct CharacterAnchors
	{
		[SerializeField]
		private RectTransform normal;

		[SerializeField]
		private RectTransform hidden;

		public RectTransform Normal => normal;

		public RectTransform Hidden => hidden;

		public Transform GetAnchor(bool visible)
		{
			if (!visible)
			{
				return hidden;
			}
			return normal;
		}

		public Vector2 GetPosition(bool visible)
		{
			if (!visible)
			{
				return hidden.anchoredPosition;
			}
			return normal.anchoredPosition;
		}
	}

	[SerializeField]
	private Image image;

	[SerializeField]
	private float visibleTime;

	[SerializeField]
	private float sizeTime;

	[SerializeField]
	private float smallScale;

	[SerializeField]
	private CharacterAnchors anchors;

	[SerializeField]
	private Transform nameBlock;

	[SerializeField]
	private Text nameText;

	private Tween visibleTween;

	private Tween sizeTween;

	public Image Image => image;

	public bool IsActive => base.gameObject.activeSelf;

	public bool IsBig { get; private set; } = true;


	public bool IsVisible { get; private set; }

	public Sprite CharacterSprite
	{
		get
		{
			return image.sprite;
		}
		set
		{
			image.sprite = value;
		}
	}

	public string CharacterName
	{
		get
		{
			return nameText.text;
		}
		set
		{
			nameText.text = value;
		}
	}

	private RectTransform CharTr => Image.transform as RectTransform;

	public DialogCharacter SetCharacter(Sprite sprite, string name)
	{
		CharacterSprite = sprite;
		CharacterName = name;
		return this;
	}

	public Tween DoBig(bool isBig)
	{
		sizeTween?.Kill();
		IsBig = isBig;
		sizeTween = CharTr.DOScale(isBig ? 1f : smallScale, sizeTime);
		return sizeTween;
	}

	public Tween DoVisible(bool visible)
	{
		IsVisible = visible;
		visibleTween?.Kill();
		CharTr.anchoredPosition = anchors.GetPosition(!visible);
		Sequence s = DOTween.Sequence();
		s.Join(DOTweenModuleUI.DOAnchorPos(CharTr, anchors.GetPosition(visible), visibleTime));
		s.Join(nameBlock.DOScale(visible ? 1 : 0, visibleTime));
		visibleTween = s;
		return visibleTween;
	}

	public Tween DoSwap(Action swap)
	{
		IsVisible = true;
		visibleTween?.Kill();
		CharTr.anchoredPosition = anchors.GetPosition(visible: true);
		Sequence s = DOTween.Sequence();
		s.Join(DOTweenModuleUI.DOAnchorPos(CharTr, anchors.GetPosition(visible: false), visibleTime));
		s.Join(DoBig(isBig: false));
		s.AppendCallback(delegate
		{
			swap();
		});
		s.Append(DOTweenModuleUI.DOAnchorPos(CharTr, anchors.GetPosition(visible: true), visibleTime));
		visibleTween = s;
		return visibleTween;
	}

	public void Prepare()
	{
		base.gameObject.SetActive(value: true);
		CharTr.anchoredPosition = anchors.Hidden.anchoredPosition;
		nameBlock.localScale = Vector3.zero;
		CharTr.localScale = Vector3.one * smallScale;
	}

	private void OnEnable()
	{
		nameBlock.gameObject.SetActive(value: true);
	}

	private void OnDisable()
	{
		nameBlock.gameObject.SetActive(value: false);
	}
}
