using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GreenT.HornyScapes;
using GreenT.HornyScapes.MergeCore;
using GreenT.UI;
using Merge.Core.Masters;
using Merge.MotionDesign;
using MISC.Resolution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Merge;

public class PocketController : Controller<PocketController>, IDataController, IResolutionAdapter
{
	[Serializable]
	public class ItemDictionary : SerializableDictionary<int, string>
	{
	}

	[Serializable]
	public class PocketData : Data
	{
		[SerializeField]
		public Queue<GIData> PocketItemsQueue { get; set; } = new Queue<GIData>();


		[SerializeField]
		public Queue<GIData> PocketEventItemsQueue { get; set; } = new Queue<GIData>();


		public void Fix_1_3_0()
		{
			if (PocketEventItemsQueue == null)
			{
				PocketEventItemsQueue = new Queue<GIData>();
				Debug.Log("PocketData Fixed");
			}
		}
	}

	[Inject]
	private IWindowsManager windowsManager;

	private CoreWindow coreWindow;

	[SerializeField]
	private Transform anchorPocket;

	private PocketData data;

	private Tween imageScaleTween;

	private Button pocketButton;

	private Image pocketImage;

	private TextMeshProUGUI pocketName;

	[Inject]
	private ICameraChanger cameraChanger;

	public bool Test = true;

	public ItemDictionary Items = new ItemDictionary();

	public GameItemController Field => Controller<GameItemController>.Instance;

	public Vector3 AnchorPosition => anchorPocket.position;

	public GIData ItemToPop(PlayType playType = PlayType.story)
	{
		return GetQueue(playType).Peek();
	}

	public bool NotEmpty(PlayType playType = PlayType.story)
	{
		return !IsEmpty(playType);
	}

	public bool IsEmpty(PlayType playType = PlayType.story)
	{
		return CountItem(playType) == 0;
	}

	public int CountItem(PlayType playType = PlayType.story)
	{
		return GetQueue(playType).Count;
	}

	Data IDataController.GetSave()
	{
		return data;
	}

	public override void Preload()
	{
		base.Preload();
		coreWindow = windowsManager.Get<CoreWindow>();
	}

	public override void Init()
	{
		((IResolutionAdapter)this).Adaptate(ResolutionAdapterMaster.Resolution);
		pocketButton.AddClickCallback(AtButtonClick);
		bool flag = NotEmpty(EventsController.CurrentPlayType);
		SetViewActive(flag);
		if (flag)
		{
			GIKey key = ItemToPop(EventsController.CurrentPlayType).Key;
			pocketImage.sprite = IconProvider.GetGISprite(key);
			pocketName.text = key.Collection;
		}
		ValidateCountLabel();
		FixIn_1_2_0();
	}

	void IDataController.Load(Data baseData)
	{
		data = (baseData as PocketData) ?? new PocketData();
	}

	public void AddItemToQueue(GIData item, PlayType playType = PlayType.story)
	{
		bool num = IsEmpty(playType);
		GetQueue(playType).Enqueue(item);
		ValidateCountLabel(playType);
		if (num && playType == EventsController.CurrentPlayType)
		{
			SetViewActive(active: true);
			pocketImage.sprite = IconProvider.GetGISprite(item.Key);
			pocketName.text = item.Key.Collection;
			PingFirstItem();
		}
	}

	public void ClearEventData()
	{
		data.PocketEventItemsQueue.Clear();
	}

	private Queue<GIData> GetQueue(PlayType playType)
	{
		return playType switch
		{
			PlayType.story => data.PocketItemsQueue, 
			PlayType.events => data.PocketEventItemsQueue, 
			_ => throw new Exception("Неизвестный тип игры"), 
		};
	}

	private void ValidateCountLabel(PlayType playType = PlayType.story)
	{
	}

	private void AtButtonClick()
	{
		if (Field.TryGetFirstEmptyPoint(out var pnt))
		{
			GIData giData = null;
			if (EventsController.CurrentPlayType == PlayType.story)
			{
				giData = data.PocketItemsQueue.Dequeue().Copy().SetCoordinates(pnt);
			}
			else if (EventsController.CurrentPlayType == PlayType.events)
			{
				giData = data.PocketEventItemsQueue.Dequeue().Copy().SetCoordinates(pnt);
			}
			GameItem gameItem = Field.CreateItem(giData);
			ValidateCountLabel();
			gameItem.DoCreateFrom(GetStartPosition());
			NewItemHightlight.Create(gameItem.transform);
			Sounds.Play("spawn");
			if (!NotEmpty(EventsController.CurrentPlayType))
			{
				SetViewActive(active: false);
				return;
			}
			GIKey key = ItemToPop(EventsController.CurrentPlayType).Key;
			pocketImage.sprite = IconProvider.GetGISprite(key);
			PingFirstItem();
		}
	}

	public Vector3 GetStartPosition()
	{
		return cameraChanger.MergeCamera.ScreenToWorldPoint(anchorPocket.position);
	}

	private void SetViewActive(bool active)
	{
		if (pocketButton.gameObject.activeSelf != active)
		{
			if (active)
			{
				pocketButton.SetActive(active: true);
				pocketButton.transform.localScale = Vector3.zero;
				pocketButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
			}
			else
			{
				pocketButton.DOKill();
				TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = pocketButton.transform.DOScale(0f, 0.4f);
				tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, (TweenCallback)delegate
				{
					pocketButton.SetActive(active: false);
				});
			}
		}
		pocketImage.SetActive(active);
	}

	private void PingFirstItem()
	{
		imageScaleTween?.Kill();
		pocketImage.transform.SetScale(0f);
		imageScaleTween = pocketImage.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
	}

	void IResolutionAdapter.Adaptate(ResolutionType type)
	{
	}

	private void FixIn_1_2_0()
	{
		if (!(Application.version != "1.2.0"))
		{
			Debug.Log("BRIDGE >>> Fix missed brick");
			AddItemToQueue(new GIData(new GIKey(4, "Bricks")));
		}
	}
}
