using UnityEngine;
using UnityEngine.EventSystems;

namespace StripClub.UI;

[RequireComponent(typeof(Animator))]
public class CustomAnimationTriggersHandler : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	private Animator _animator;

	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_animator.SetTrigger("Highlighted");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_animator.SetTrigger("Normal");
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_animator.SetTrigger("Pressed");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_animator.SetTrigger("Selected");
	}
}
