using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using Merge;
using Merge.MotionDesign;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class RewardContainer : MonoView
{
	public MonoDisplayStrategy displayStrategy;

	[SerializeField]
	private Transform anchorPocket;

	[SerializeField]
	private Button pocketButton;

	[SerializeField]
	private Image pocketImage;

	[SerializeField]
	private LocalizedTextMeshPro pocketName;

	[SerializeField]
	private ContentType contentType;

	private Tween imageScaleTween;

	[SerializeField]
	private Animation zoomInOut;

	private Sequence seq;

	private GreenT.HornyScapes.MergeCore.PocketController controller;

	private ICameraChanger cameraChanger;

	private ContentSelectorGroup contentSelectorGroup;

	private IMergeIconProvider _iconProvider;

	public GameItemController Field => Controller<GameItemController>.Instance;

	[Inject]
	private void InnerInit(GreenT.HornyScapes.MergeCore.PocketController controller, ICameraChanger cameraChanger, ContentSelectorGroup contentSelectorGroup, IMergeIconProvider iconProvider)
	{
		this.controller = controller;
		this.cameraChanger = cameraChanger;
		this.contentSelectorGroup = contentSelectorGroup;
		_iconProvider = iconProvider;
	}

	public override void Display(bool display)
	{
		bool display2 = controller.NotEmpty();
		displayStrategy.Display(display2);
	}

	private void Awake()
	{
		pocketButton.onClick.AddListener(controller.AtButtonClick);
		Subscribe();
		bool active = controller.NotEmpty();
		base.gameObject.SetActive(active);
	}

	private void OnEnable()
	{
		pocketButton.SetActive(active: false);
		bool flag = controller.NotEmpty();
		SetViewActive(flag);
		if (flag)
		{
			GIData view = controller.ItemToPop();
			SetView(view);
			seq = zoomInOut.Play();
		}
	}

	private void OnDisable()
	{
		seq?.Kill();
	}

	private void Subscribe()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GreenT.HornyScapes.MergeCore.PocketController.PocketMemento>(Observable.Where<GreenT.HornyScapes.MergeCore.PocketController.PocketMemento>(controller.OnClear, (Func<GreenT.HornyScapes.MergeCore.PocketController.PocketMemento, bool>)((GreenT.HornyScapes.MergeCore.PocketController.PocketMemento _) => contentSelectorGroup.Current == contentType)), (Action<GreenT.HornyScapes.MergeCore.PocketController.PocketMemento>)delegate
		{
			bool viewActive = controller.NotEmpty();
			SetViewActive(viewActive);
		}), (Component)this);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GIData>(Observable.Where<GIData>(controller.OnItemAdd, (Func<GIData, bool>)((GIData _) => contentSelectorGroup.Current == contentType)), (Action<GIData>)delegate
		{
			bool flag = controller.NotEmpty();
			SetViewActive(flag);
			if (flag)
			{
				GIData view2 = controller.ItemToPop();
				SetView(view2);
			}
		}), (Component)this);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<GIData>(Observable.Where<GIData>(controller.OnItemRemove, (Func<GIData, bool>)((GIData _) => contentSelectorGroup.Current == contentType)), (Action<GIData>)delegate(GIData _item)
		{
			GameItem gameItem = Field.CreateItem(_item);
			gameItem.DoCreateFrom(GetStartPosition());
			NewItemHightlight.Create(gameItem.transform);
			Merge.Sounds.Play("spawn");
			if (!controller.NotEmpty())
			{
				SetViewActive(active: false);
			}
			else
			{
				GIData view = controller.ItemToPop();
				SetView(view);
				PingFirstItem();
			}
		}), (Component)this);
	}

	private void SetViewActive(bool active)
	{
		Display(active);
		if (active)
		{
			PingFirstItem();
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
		pocketImage.SetActive(active);
	}

	private void SetView(GIData item)
	{
		pocketImage.sprite = _iconProvider.GetSprite(item.Key);
		pocketName.Init(GameItemName(item.Key));
	}

	private string GameItemName(GIKey key)
	{
		return $"item.{key}";
	}

	private void PingFirstItem()
	{
		imageScaleTween?.Kill();
		pocketImage.transform.SetScale(0f);
		imageScaleTween = pocketImage.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
	}

	public Vector3 GetStartPosition()
	{
		return cameraChanger.MergeCamera.ScreenToWorldPoint(anchorPocket.position);
	}
}
