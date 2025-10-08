using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Messenger.UI;
using UniRx;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class PlayerOptionsMessageView : MessageView<PlayerChatMessage>
{
	private ResponseOptionView.Manager viewManager;

	private IDisposable selectResponseStream;

	public IEnumerable<ResponseOptionView> ResponseViews { get; private set; }

	[Inject]
	internal void Init(ResponseOptionView.Manager viewManager)
	{
		this.viewManager = viewManager;
	}

	public override void Set(PlayerChatMessage message)
	{
		base.Set(message);
		viewManager.HideAll();
		ResponseViews = viewManager.Display(message.GetAvailableOptions);
		selectResponseStream?.Dispose();
		selectResponseStream = ObservableExtensions.Subscribe<ResponseOption>(Observable.Merge<ResponseOption>(ResponseViews.Select(OnSelectResponse)), (Action<ResponseOption>)TryToSelectResponse);
	}

	private void TryToSelectResponse(ResponseOption option)
	{
		bool flag = true;
		if (option.NecessaryItems.Any())
		{
			IEnumerator<IItemLot> enumerator = option.NecessaryItems.GetEnumerator();
			while (flag && enumerator.MoveNext())
			{
				flag = enumerator.Current.CheckIsEnough();
			}
			if (flag)
			{
				foreach (IItemLot necessaryItem in option.NecessaryItems)
				{
					necessaryItem.Buy();
				}
			}
		}
		if (flag)
		{
			base.Source.SetChoice(option.ID);
			Display(display: false);
		}
	}

	private static IObservable<ResponseOption> OnSelectResponse(ResponseOptionView view)
	{
		return Observable.Select<Unit, ResponseOption>(UnityUIComponentExtensions.OnClickAsObservable((Button)view), (Func<Unit, ResponseOption>)((Unit _) => view.Source));
	}

	protected virtual void OnDisable()
	{
		selectResponseStream?.Dispose();
	}
}
