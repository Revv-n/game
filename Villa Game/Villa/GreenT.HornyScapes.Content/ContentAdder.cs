using System;
using System.Collections.Generic;
using GreenT.UI;
using StripClub.Model;
using StripClub.UI.Rewards;

namespace GreenT.HornyScapes.Content;

public class ContentAdder : IContentAdder, IDisposable
{
	private readonly IWindowsManager windowsManager;

	private RewardsWindow rewardsWindow;

	private Queue<LinkedContent> addQueue = new Queue<LinkedContent>();

	public ContentAdder(IWindowsManager windowsManager)
	{
		this.windowsManager = windowsManager;
	}

	public void AddContent(LinkedContent content)
	{
		if (content != null)
		{
			if (rewardsWindow == null)
			{
				rewardsWindow = windowsManager.Get<RewardsWindow>();
				rewardsWindow.OnChangeState += OnWindowChangeState;
			}
			if (!rewardsWindow.IsOpened)
			{
				DisplayContentAddition(content);
			}
			else
			{
				addQueue.Enqueue(content);
			}
		}
	}

	private void DisplayContentAddition(LinkedContent content)
	{
		SetSource(content);
		rewardsWindow.Init(content);
		rewardsWindow.Open();
	}

	private void OnWindowChangeState(object sender, EventArgs e)
	{
		if (e is WindowArgs { Active: false } && addQueue.Count != 0)
		{
			LinkedContent content = addQueue.Dequeue();
			DisplayContentAddition(content);
		}
	}

	private void SetSource(LinkedContent linkedLootBox)
	{
		if (linkedLootBox is LootboxLinkedContent)
		{
			for (LootboxLinkedContent next = linkedLootBox.GetNext<LootboxLinkedContent>(checkThis: true); next != null; next = next.GetNext<LootboxLinkedContent>())
			{
				next.SetSource(linkedLootBox.AnalyticData.SourceType);
			}
		}
	}

	public void Dispose()
	{
		if (rewardsWindow != null)
		{
			rewardsWindow.OnChangeState -= OnWindowChangeState;
		}
	}
}
