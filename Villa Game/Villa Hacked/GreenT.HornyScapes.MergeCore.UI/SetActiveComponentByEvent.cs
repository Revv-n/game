using System;
using GreenT.Types;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore.UI;

public class SetActiveComponentByEvent : MonoBehaviour
{
	public MonoBehaviour component;

	protected void Awake()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ContentType>(ObserveExtensions.ObserveEveryValueChanged<ReadOnlyReactiveProperty<ContentType>, ContentType>(Controller<GameItemController>.Instance.CurrentContentType, (Func<ReadOnlyReactiveProperty<ContentType>, ContentType>)((ReadOnlyReactiveProperty<ContentType> contentType) => contentType.Value), (FrameCountType)0, false), (Action<ContentType>)delegate(ContentType value)
		{
			component.enabled = value == ContentType.Event;
		}), (Component)this);
	}
}
