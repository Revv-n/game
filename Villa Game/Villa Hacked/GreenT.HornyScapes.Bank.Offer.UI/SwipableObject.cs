using System;
using GreenT.HornyScapes.Sounds;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public sealed class SwipableObject : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IEndDragHandler, IDragHandler
{
	[Serializable]
	public class SwipeEvent : UnityEvent<Vector2Int>
	{
	}

	public UnityEvent OnClick;

	public SwipeEvent OnSwipe;

	public ButtonSoundSO Click;

	public ButtonSoundSO Hover;

	private bool isSwipped;

	private int dragTrashhold;

	private IAudioPlayer audioPlayer;

	[Inject]
	public void Init(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
	}

	private void Awake()
	{
		dragTrashhold = Mathf.Min(Screen.height, Screen.width) * 8 / 100;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		audioPlayer.PlayOneShotAudioClip2D(Hover.Sound);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!isSwipped && (eventData.position - eventData.pressPosition).magnitude > (float)dragTrashhold)
		{
			isSwipped = true;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ((eventData.position - eventData.pressPosition).magnitude > (float)dragTrashhold)
		{
			Vector2Int arg = EvaluateSwipeSide(eventData);
			OnSwipe?.Invoke(arg);
		}
		isSwipped = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!isSwipped)
		{
			audioPlayer.PlayAudioClip2D(Click.Sound);
			OnClick?.Invoke();
		}
	}

	private Vector2Int EvaluateSwipeSide(PointerEventData eventData)
	{
		Vector2Int zero = Vector2Int.zero;
		Vector2 position = eventData.position;
		Vector2 pressPosition = eventData.pressPosition;
		float num = Mathf.Abs(position.x - pressPosition.x);
		float num2 = Mathf.Abs(position.y - pressPosition.y);
		if (num > num2)
		{
			if (position.x > pressPosition.x)
			{
				return Vector2Int.right;
			}
			return Vector2Int.left;
		}
		if (position.y > pressPosition.y)
		{
			return Vector2Int.up;
		}
		return Vector2Int.down;
	}
}
