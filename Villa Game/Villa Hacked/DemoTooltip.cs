using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoTooltip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[TextArea(4, 8)]
	public string TooltipText;

	private Text tooltipTextObject;

	private void Awake()
	{
		GameObject gameObject = GameObject.Find("Tooltip_Info");
		tooltipTextObject = gameObject.GetComponent<Text>();
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if (!(tooltipTextObject == null))
		{
			tooltipTextObject.text = TooltipText;
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (!(tooltipTextObject == null))
		{
			tooltipTextObject.text = "";
		}
	}
}
