using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.Meta.RoomObjects;
using UniRx;
using UnityEngine;
using Zenject;

namespace Merge.Meta.RoomObjects;

public class RoomObject : GameRoomObject<RoomObjectConfig>
{
	[SerializeField]
	private RoomObjectViewFactory roomObjectViewFactory;

	[SerializeField]
	private Material normalMaterial;

	[SerializeField]
	private Material outlineMaterial;

	[SerializeField]
	private float delayBetweenViewsAnimations = 1f;

	private RoomManager house;

	private GameStarter gameStarter;

	private IDisposable disposable;

	private List<RoomObjectReference> references;

	private bool isAnimate;

	public List<RoomObjectView> Views { get; } = new List<RoomObjectView>();


	public float PositionView => ViewRoot.GetChild(0).transform.localPosition.y;

	public int ViewNumber { get; private set; }

	[Inject]
	public void Constructor(RoomManager house, GameStarter gameStarter)
	{
		this.house = house;
		this.gameStarter = gameStarter;
	}

	public override void Init(RoomStateData data, RoomObjectConfig config)
	{
		CreateViews(config.Views);
		base.Init(data, config);
		references = base.Config.RoomObjectReferences;
		if (config.BordPosition == Vector3.zero && Views.Any())
		{
			base.Bord.transform.localPosition = Views[0].transform.localPosition;
		}
		if (base.Config.Behaviour != 0)
		{
			for (int i = 0; i < Views.Count; i++)
			{
				Views[i].BeforeChangeAnimation.Delay = (float)i * delayBetweenViewsAnimations;
			}
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (config.Number == 0 || gameStarter.IsGameActive.Value)
		{
			isAnimate = true;
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			isAnimate = true;
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (Component)this);
	}

	private void CreateViews(IEnumerable<RoomObjectViewInfo> viewInfos)
	{
		RemoveViews();
		foreach (RoomObjectViewInfo viewInfo in viewInfos)
		{
			RoomObjectView item = roomObjectViewFactory.Create(IsClickAllowed, OnTapAnyView, viewInfo);
			Views.Add(item);
		}
	}

	private void RemoveViews()
	{
		foreach (RoomObjectView view in Views)
		{
			UnityEngine.Object.Destroy(view.gameObject);
		}
		Views.Clear();
	}

	public override void Highlight(HighlightType highlightType)
	{
		Material material = ((highlightType == HighlightType.None) ? normalMaterial : new Material(outlineMaterial));
		foreach (RoomObjectView view in Views)
		{
			view.Renderer.material = material;
		}
		Views.ForEach(delegate(RoomObjectView x)
		{
			x.PlayAnimation(highlightType);
		});
	}

	public override void SetView(int viewNumber)
	{
		ViewNumber = viewNumber;
		if (ViewNumber != 0 && references.Count > 0)
		{
			SetReferencedObjectsView();
		}
		if (viewNumber == 0 || !isAnimate)
		{
			foreach (RoomObjectView view2 in Views)
			{
				base.Data.SelectedSkin = viewNumber;
				view2.SetView(viewNumber);
			}
			base.OnViewChanged.Value = viewNumber;
			return;
		}
		RoomObjectView roomObjectView = Views.Last();
		disposable?.Dispose();
		disposable = ObservableExtensions.Subscribe<Animation>(Observable.First<Animation>(roomObjectView.AfterChangeAnimation.OnAnimationEnd), (Action<Animation>)delegate
		{
			base.OnViewChanged.Value = viewNumber;
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		});
		foreach (RoomObjectView view in Views)
		{
			view.BeforeChangeAnimation.Play().OnComplete(delegate
			{
				base.Data.SelectedSkin = viewNumber;
				view.SetView(viewNumber);
				view.AfterChangeAnimation.Play();
			});
		}
	}

	private void SetReferencedObjectsView()
	{
		foreach (RoomObjectReference reference in references)
		{
			house.GetRoomObject(reference.ID).SetView((int)reference.NextState);
		}
	}

	public override void SetVisible(bool visible)
	{
		if (visible && base.Config.Behaviour == Behaviour.Normal)
		{
			SetView(1);
		}
		else if (!visible && base.Config.Behaviour == Behaviour.Trash)
		{
			SetView(0);
		}
		else if (base.Config.Behaviour == Behaviour.Preset)
		{
			SetView((base.Data.Status == EntityStatus.Rewarded) ? 1 : 0);
		}
		ViewRoot.gameObject.SetActive(visible);
	}

	public override IObservable<Bounds> GetBounds()
	{
		Bounds bounds = default(Bounds);
		for (int i = 0; i < Views.Count; i++)
		{
			Renderer renderer = Views[i].Renderer;
			if (i == 0)
			{
				bounds = renderer.bounds;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		return Observable.Return<Bounds>(bounds);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		disposable?.Dispose();
	}

	protected virtual void OnTapAnyView()
	{
		onTap.OnNext((IRoomObject<RoomObjectConfig>)this);
	}

	public override void SetSkin(int skinID)
	{
		throw new NotImplementedException();
	}
}
