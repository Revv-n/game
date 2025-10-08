using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.StarShop.Story;
using GreenT.HornyScapes.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.StarShop.UI;

public class StarShopSubwindow : AnimatedSubwindow
{
	private const string titleKey = "story.roomart.title.";

	private const string storyKey = "story.roomart.body.";

	[SerializeField]
	private LocalizedTextMeshPro storyTitle;

	[SerializeField]
	private LocalizedTextMeshPro storyDescription;

	[SerializeField]
	private Image storyImage;

	[SerializeField]
	private GameObject comingSoon;

	private StarShopManager starShopManager;

	private StarShopViewManager starShopViewManager;

	private StarShopArtManager artManager;

	private CompositeDisposable storyUnlockStream = new CompositeDisposable();

	private IDisposable comingSoonStream;

	[Inject]
	private void Init(StarShopManager starShopManager, StarShopViewManager starShopViewManager, StarShopArtManager artManager)
	{
		this.starShopManager = starShopManager;
		this.starShopViewManager = starShopViewManager;
		this.artManager = artManager;
	}

	private void Start()
	{
		UpdateLeftPage();
	}

	public override void Open()
	{
		base.Open();
		StarShopLoader();
		ManageComingSoonPlaceHolder();
	}

	public override void Close()
	{
		base.Close();
		comingSoonStream?.Dispose();
	}

	private void UpdateLeftPage()
	{
		IEnumerable<StarShopArt> openedStories = artManager.Collection.Where((StarShopArt _story) => _story.Locker.IsOpen.Value);
		storyUnlockStream.Clear();
		SubscribeOnUnlockStory();
		UpdateStoryImage();
		if (!openedStories.Any())
		{
			new Exception().SendException("Doesnt have a opened starStory");
			return;
		}
		StarShopArt story = openedStories.Max((Func<StarShopArt, StarShopArt>)Selector);
		SetStory(story);
		StarShopArt Selector(StarShopArt starStory)
		{
			int lastOpenedID = openedStories.Max((StarShopArt _story) => _story.ID);
			return openedStories.Where((StarShopArt _story) => _story.ID == lastOpenedID).First();
		}
	}

	private void SetStory(StarShopArt lastOpenedStory)
	{
		storyTitle.Init("story.roomart.title." + lastOpenedStory.ID);
		storyDescription.Init("story.roomart.body." + lastOpenedStory.ID);
	}

	private void UpdateStoryImage()
	{
		artManager.Current.Subscribe(delegate(Sprite _current)
		{
			storyImage.sprite = _current;
		}).AddTo(storyUnlockStream);
	}

	private void SubscribeOnUnlockStory()
	{
		artManager.Collection.Where((StarShopArt _story) => !_story.Locker.IsOpen.Value).ToObservable().SelectMany((StarShopArt _story) => from _isOpen in _story.Locker.IsOpen
			where _isOpen
			select _isOpen into _
			select _story)
			.Subscribe(SetStory)
			.AddTo(storyUnlockStream);
	}

	private void ManageComingSoonPlaceHolder()
	{
		comingSoonStream?.Dispose();
		comingSoonStream = starShopManager.Collection.Select((StarShopItem _item) => _item.OnUpdate.StartWith(_item)).CombineLatest().Select(IsAnyTaskDisplayed)
			.Subscribe(delegate(bool _anyTaskIsDisplayed)
			{
				comingSoon.SetActive(!_anyTaskIsDisplayed);
			});
	}

	private bool IsAnyTaskDisplayed(IList<StarShopItem> items)
	{
		return items.Any(IsTaskDisplayed);
	}

	private static bool IsTaskDisplayed(StarShopItem _item)
	{
		if (_item.State != EntityStatus.InProgress)
		{
			return _item.State == EntityStatus.Complete;
		}
		return true;
	}

	private void StarShopLoader()
	{
		starShopViewManager.HideAll();
		IEnumerable<StarShopItem> enumerable = starShopManager.Collection.Where(IsTaskDisplayed);
		IEnumerable<StarShopItem> source = starShopManager.Collection.Where((StarShopItem _item) => _item.State == EntityStatus.Blocked);
		foreach (StarShopItem item in enumerable)
		{
			DisplayStarShopItem(item);
		}
		(from _item in source.Select((StarShopItem _item) => _item.OnUpdate).Merge()
			where _item.State == EntityStatus.InProgress
			select _item).TakeUntilDisable(this).Subscribe(delegate(StarShopItem _item)
		{
			DisplayStarShopItem(_item);
		}).AddTo(this);
	}

	private void DisplayStarShopItem(IStarShopItem item)
	{
		starShopViewManager.GetView().Set(item);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		storyUnlockStream?.Dispose();
		comingSoonStream?.Dispose();
	}
}
