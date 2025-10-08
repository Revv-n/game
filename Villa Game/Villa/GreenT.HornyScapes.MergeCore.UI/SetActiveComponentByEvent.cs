using GreenT.Types;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore.UI;

public class SetActiveComponentByEvent : MonoBehaviour
{
	public MonoBehaviour component;

	protected void Awake()
	{
		Controller<GameItemController>.Instance.CurrentContentType.ObserveEveryValueChanged((ReadOnlyReactiveProperty<ContentType> contentType) => contentType.Value).Subscribe(delegate(ContentType value)
		{
			component.enabled = value == ContentType.Event;
		}).AddTo(this);
	}
}
