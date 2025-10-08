using System;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Subscription;
using GreenT.UI;
using StripClub.Model;
using StripClub.UI.Rewards;
using UniRx;

namespace GreenT.HornyScapes.Lootboxes;

public class LootboxOpener : ILootboxOpener, IDisposable
{
	private Subject<Lootbox> onOpen = new Subject<Lootbox>();

	private RewardsWindow rewardsWindow;

	private CompositeDisposable disposables = new CompositeDisposable();

	private readonly IWindowsManager windowsManager;

	private readonly LootboxCollection lootboxCollection;

	public IObservable<Lootbox> OnOpen => onOpen;

	public LootboxOpener(IWindowsManager windowsManager, LootboxCollection lootboxCollection)
	{
		this.windowsManager = windowsManager;
		this.lootboxCollection = lootboxCollection;
	}

	public void Open(int lootboxID, CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		Lootbox lootbox;
		try
		{
			lootbox = lootboxCollection.Collection.First((Lootbox _lootbox) => _lootbox.ID == lootboxID);
		}
		catch (InvalidOperationException innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't find lootbox with ID: {lootboxID}");
		}
		try
		{
			Open(lootbox, sourceType);
		}
		catch (InvalidOperationException innerException2)
		{
			throw innerException2.SendException($"{GetType().Name}: Can't open lootbox with ID: {lootboxID}");
		}
	}

	public void Open(Lootbox lootbox, CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		if (rewardsWindow == null)
		{
			rewardsWindow = windowsManager.Get<RewardsWindow>();
		}
		lootbox.SetSource(sourceType);
		LinkedContent preopenedContent;
		try
		{
			preopenedContent = lootbox.GetPreopenedContent();
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't get lootbox content");
		}
		SetSource(preopenedContent, sourceType);
		Launch(lootbox, preopenedContent);
	}

	public void OpenSubscriptionDaily(Lootbox lootbox, CurrencyAmplitudeAnalytic.SourceType sourceType, SubscriptionModel subscriptionModel)
	{
		if (rewardsWindow == null)
		{
			rewardsWindow = windowsManager.Get<RewardsWindow>();
		}
		lootbox.SetSource(sourceType);
		LinkedContent preopenedContent;
		try
		{
			preopenedContent = lootbox.GetPreopenedContent();
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Can't get lootbox content");
		}
		SetSource(preopenedContent, sourceType);
		Launch(lootbox, preopenedContent, subscriptionModel);
	}

	private void Launch(Lootbox lootbox, LinkedContent lootboxContent, SubscriptionModel subscriptionModel = null)
	{
		disposables.Clear();
		rewardsWindow.OnOpenWithLootbox.Where((Lootbox _lootbox) => lootbox == _lootbox).Take(1).ContinueWith(rewardsWindow.OnCloseWithLootbox)
			.Subscribe(onOpen.OnNext)
			.AddTo(disposables);
		if (subscriptionModel == null)
		{
			rewardsWindow.Init(lootbox, lootboxContent);
		}
		else
		{
			rewardsWindow.Init(lootbox, lootboxContent, subscriptionModel);
		}
		rewardsWindow.Open();
	}

	private void SetSource(LinkedContent linkedLootbox, CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		if (linkedLootbox is LootboxLinkedContent)
		{
			for (LootboxLinkedContent next = linkedLootbox.GetNext<LootboxLinkedContent>(checkThis: true); next != null; next = next.GetNext<LootboxLinkedContent>())
			{
				next.SetSource(sourceType);
			}
		}
	}

	public void Dispose()
	{
		onOpen.OnCompleted();
		onOpen.Dispose();
		disposables.Dispose();
	}
}
