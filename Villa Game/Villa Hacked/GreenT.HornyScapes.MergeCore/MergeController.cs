using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Constants;
using Merge;
using Merge.MotionDesign;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergeController : Controller<MergeController>
{
	public class PulsationManager : IDisposable
	{
		private class PulsationNode
		{
			public GameItem gameItem;

			public Tween tween;

			public PulsationNode(GameItem gameItem, Tween tween)
			{
				this.gameItem = gameItem;
				this.tween = tween;
			}
		}

		private bool serchPairRequest;

		private bool somethingHappensWhileRequestDely = true;

		private List<PulsationNode> recomendatedForMerge = new List<PulsationNode>();

		private PulsationTweenBuilder Tweener => Controller<MergeController>.Instance.pulsationTweenBuilder;

		public PulsationManager()
		{
			Controller<MergeController>.Instance.Field.OnItemCreated += AtItemCreated;
			Controller<MergeController>.Instance.Field.OnItemTakenFromSomethere += AtItemCreated;
			DOVirtual.DelayedCall(3f, delegate
			{
				recomendatedForMerge = FindRecomendationForMerge();
			});
			Controller<LockedController>.Instance.OnItemActionUnlock += AtItemCreated;
			MotionController.OnItemBecomesDrag += AtBecomesDrag;
			MotionController.OnItemDragReleased += AtItemPlaced;
			Controller<ChestController>.Instance.OnBecomeOpening += AtBecomeOpening;
		}

		public void Dispose()
		{
			recomendatedForMerge.Clear();
			if ((bool)Controller<MergeController>.Instance.Field)
			{
				Controller<MergeController>.Instance.Field.OnItemCreated -= AtItemCreated;
				Controller<MergeController>.Instance.Field.OnItemTakenFromSomethere -= AtItemCreated;
			}
			if ((bool)Controller<LockedController>.Instance)
			{
				Controller<LockedController>.Instance.OnItemActionUnlock -= AtItemCreated;
			}
			MotionController.OnItemBecomesDrag -= AtBecomesDrag;
			MotionController.OnItemDragReleased -= AtItemPlaced;
			if ((bool)Controller<ChestController>.Instance)
			{
				Controller<ChestController>.Instance.OnBecomeOpening -= AtBecomeOpening;
			}
		}

		private void AtItemCreated(GameItem created)
		{
			ValidateSomethingHappenFlag();
			if (recomendatedForMerge.Count <= 0)
			{
				SerchPairRequest();
			}
		}

		private void AtBecomesDrag(GameItem dragging)
		{
			if (!recomendatedForMerge.Any((PulsationNode x) => x.gameItem == dragging))
			{
				return;
			}
			foreach (PulsationNode item in recomendatedForMerge)
			{
				item.tween?.Kill();
				Sequence sequence = DOTween.Sequence();
				sequence.Append(item.gameItem.IconRenderer.transform.DOScale(1f, 0.4f));
				sequence.Join(item.gameItem.IconRenderer.transform.DOLocalMove(Vector3.zero, 0.4f));
				item.tween = sequence;
				item.gameItem.AppendOuterTween(item.tween);
				item.gameItem.OnRemoving -= AtItemRemoved;
			}
			recomendatedForMerge.Clear();
		}

		private void AtItemPlaced(GameItem placed)
		{
			ValidateSomethingHappenFlag();
			if (recomendatedForMerge.Count <= 0 || recomendatedForMerge.Any((PulsationNode x) => x.gameItem == placed))
			{
				SerchPairRequest();
			}
		}

		private void AtItemRemoved(GameItem removed)
		{
			ClearPulsation();
		}

		private void ClearPulsation()
		{
			foreach (PulsationNode item in recomendatedForMerge)
			{
				item.gameItem.OnRemoving -= AtItemRemoved;
				item.tween?.Kill(complete: true);
			}
			recomendatedForMerge.Clear();
			SerchPairRequest();
		}

		private void AtBecomeOpening(GIBox.Chest chest)
		{
			if (recomendatedForMerge == null || recomendatedForMerge.Count == 0)
			{
				return;
			}
			using List<PulsationNode>.Enumerator enumerator = recomendatedForMerge.GetEnumerator();
			GIBox.Chest box;
			while (enumerator.MoveNext() && enumerator.Current.gameItem.TryGetBox<GIBox.Chest>(out box))
			{
				if (box == chest)
				{
					ClearPulsation();
					break;
				}
			}
		}

		private List<PulsationNode> FindRecomendationForMerge()
		{
			List<PulsationNode> list = new List<PulsationNode>();
			Dictionary<GIKey, List<GameItem>> dictionary = new Dictionary<GIKey, List<GameItem>>();
			foreach (GameItem item in Controller<MergeController>.Instance.Field.CurrentField.Field)
			{
				if (item == null || !Controller<MergeController>.Instance.IsMergable(item))
				{
					continue;
				}
				List<GameItem> list2 = (dictionary.ContainsKey(item.Key) ? dictionary[item.Key] : new List<GameItem>());
				if (item.AllowMotion || !item.IsAnimatingMotion || !item.HasTaskMark)
				{
					list2.Add(item);
				}
				if (dictionary.ContainsKey(item.Key))
				{
					List<GameItem> list3 = dictionary[item.Key];
					if (list3.Count <= 1)
					{
						continue;
					}
					foreach (GameItem item2 in list3)
					{
						if (Controller<MergeController>.Instance.IsMergable(item2) && (item.AllowMotion || item2.AllowMotion) && !item.IsAnimatingMotion && !item2.IsAnimatingMotion && !item.HasTaskMark && !item2.HasTaskMark && item.IsLocked != item2.IsLocked)
						{
							list.Add(new PulsationNode(item, null));
							list.Add(new PulsationNode(item2, null));
							CreateTweenForMergePair(list);
							return list;
						}
					}
				}
				else
				{
					dictionary.Add(item.Key, list2);
				}
			}
			foreach (GIKey key in dictionary.Keys)
			{
				List<GameItem> list4 = dictionary[key];
				if (list4.Count > 1)
				{
					List<GameItem> list5 = list4.Where((GameItem x) => !x.IsLocked).ToList();
					if (list5 != null && list5.Count > 1)
					{
						list.Add(new PulsationNode(list5[0], null));
						list.Add(new PulsationNode(list5[1], null));
						CreateTweenForMergePair(list);
						return list;
					}
				}
			}
			if (somethingHappensWhileRequestDely)
			{
				SerchPairRequest(1f, withRepeatFlag: false);
			}
			else
			{
				somethingHappensWhileRequestDely = false;
			}
			return list;
		}

		private void CreateTweenForMergePair(List<PulsationNode> list)
		{
			list.ForEach(delegate(PulsationNode x)
			{
				x.tween?.Kill();
			});
			list[0].tween = BuildPulsationTween(list[0].gameItem, list[1].gameItem);
			list[1].tween = BuildPulsationTween(list[1].gameItem, list[0].gameItem);
			list.ForEach(delegate(PulsationNode x)
			{
				x.gameItem.OnRemoving += AtItemRemoved;
			});
			list[0].gameItem.AppendOuterTween(list[0].tween);
			list[1].gameItem.AppendOuterTween(list[1].tween);
			Tween BuildPulsationTween(GameItem gi, GameItem attraction)
			{
				return Tweener.BuildTween(gi.IconRenderer.transform, attraction.transform);
			}
		}

		private IEnumerator CRT_SerchRequest(float delay = 0f)
		{
			if (delay == 0f)
			{
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return new WaitForSeconds(delay);
			}
			serchPairRequest = false;
			recomendatedForMerge = FindRecomendationForMerge();
		}

		private void SerchPairRequest(float delay = 3f, bool withRepeatFlag = true)
		{
			if (!serchPairRequest)
			{
				serchPairRequest = true;
				Controller<MergeController>.Instance.StartCoroutine(CRT_SerchRequest(delay));
			}
		}

		private void ValidateSomethingHappenFlag()
		{
			if (serchPairRequest)
			{
				somethingHappensWhileRequestDely = true;
			}
		}
	}

	private class DragManager
	{
		private ShineCircleCreator mergeEffectCreator;

		private Point startPoint;

		private Point? overlapedPoint;

		private GameItem draggingItem;

		public DragManager()
		{
			mergeEffectCreator = Controller<MergeController>.Instance.mergeEffectCreator;
			mergeEffectCreator.Init(Controller<MergeController>.Instance.transform);
			MotionController.OnItemBecomesDrag += AtBecomesDrag;
		}

		public void OnDestroy()
		{
			mergeEffectCreator.Pool.ClearPool();
			MotionController.OnItemBecomesDrag -= AtBecomesDrag;
		}

		private void AtBecomesDrag(GameItem gi)
		{
			SetSubscribeDrag(subscribe: true);
			overlapedPoint = gi.Coordinates;
			startPoint = gi.Coordinates;
			draggingItem = gi;
		}

		private void AtDrag(GameItem gi, Vector3 pos)
		{
			if (!TileController.TryGetPoint(pos, out var point))
			{
				if (overlapedPoint.HasValue)
				{
					AtOverlapedPointChange(null);
				}
			}
			else if (!overlapedPoint.HasValue || point != overlapedPoint.Value)
			{
				AtOverlapedPointChange(point);
			}
		}

		private void AtEndsDrag(GameItem obj)
		{
			mergeEffectCreator.Pool.ActiveElements.ForEach(delegate(MergeAllownEffect x)
			{
				x.Hide();
			});
			draggingItem = null;
			SetSubscribeDrag(subscribe: false);
		}

		private void AtOverlapedPointChange(Point? newPoint)
		{
			List<MergeAllownEffect> activeElements = mergeEffectCreator.Pool.ActiveElements;
			if (activeElements.Count > 0)
			{
				foreach (MergeAllownEffect item in activeElements)
				{
					if (item != null)
					{
						item.Hide();
					}
				}
			}
			overlapedPoint = newPoint;
			if (newPoint.HasValue && !(newPoint == startPoint) && !(Controller<GameItemController>.Instance.CurrentField.Field[newPoint.Value] == null) && Controller<MergeController>.Instance.CanBeMerged(Controller<GameItemController>.Instance.CurrentField.Field[newPoint.Value], draggingItem))
			{
				mergeEffectCreator.Pool.Pop().transform.position = TileController.GetPosition(newPoint.Value);
			}
		}

		private void SetSubscribeDrag(bool subscribe)
		{
			if (subscribe)
			{
				MotionController.OnItemDragReleased += AtEndsDrag;
				MotionController.OnItemDrag += AtDrag;
			}
			else
			{
				MotionController.OnItemDragReleased -= AtEndsDrag;
				MotionController.OnItemDrag -= AtDrag;
			}
		}
	}

	[SerializeField]
	private PulsationTweenBuilder pulsationTweenBuilder;

	[SerializeField]
	private MergeCreateTweenBuilder creationTweenBuilder;

	[SerializeField]
	private ShineCircleCreator mergeEffectCreator;

	private DragManager dragManager;

	private PulsationManager pulsationManager;

	private List<BlockMergeNode> blockMergeNodes = new List<BlockMergeNode>();

	private MergeNotifier _mergeNotifier;

	private ILocker _mergeBonusLocker;

	public bool IsTestBubble;

	private GameItemController Field => Controller<GameItemController>.Instance;

	private float BubbleChanceTh
	{
		get
		{
			if (!IsTestBubble)
			{
				return UnityEngine.Random.value;
			}
			return 0.001f;
		}
	}

	public event Action<GameItem> OnMerge;

	public event Action<GameItem, GameItem> OnStartMerge;

	public bool CanBeMerged(GameItem gi1, GameItem gi2)
	{
		if (gi1.Config.Key == gi2.Config.Key && IsMergable(gi1))
		{
			return IsMergable(gi2);
		}
		return false;
	}

	[Inject]
	public void Construct(LockerFactory lockerFactory, MergeNotifier mergeNotifier, IConstants<ILocker> lockerConstants)
	{
		_mergeBonusLocker = lockerConstants["merge_bonus_locker"];
		_mergeNotifier = mergeNotifier;
	}

	public override void Init()
	{
		dragManager = new DragManager();
		pulsationManager = new PulsationManager();
	}

	public void Merge(GameItem giFrom, GameItem giTo)
	{
		if (giFrom == null || giTo == null)
		{
			return;
		}
		if (IsMergeBlockedByNode(giFrom.Key, out var node))
		{
			node.callback(giFrom, giTo);
			giFrom.DoFlyTo(TileController.GetPosition(giFrom.Coordinates));
			return;
		}
		this.OnStartMerge?.Invoke(giFrom, giTo);
		GIGhost ghost1 = GIGhost.CreateGhost(giFrom);
		GIGhost ghost2 = GIGhost.CreateGhost(giTo);
		GIMaster.Field.RemoveItemsParam(giFrom, giTo);
		ModuleConfigs.Merge module = giTo.Config.GetModule<ModuleConfigs.Merge>();
		GIData giData = module.MergeResult.Copy().SetCoordinates(giTo.Coordinates);
		GameItem created = GIMaster.Field.CreateItem(giData);
		Tween tween = creationTweenBuilder.BuildTweeen(ghost1.IconRenderer, ghost2.IconRenderer, created);
		Tween tween2 = tween;
		tween2.onComplete = (TweenCallback)Delegate.Combine(tween2.onComplete, new TweenCallback(OnTweenComplete));
		created.OnRemoving += AtItemDestroyWhileTween;
		Controller<SelectionController>.Instance.SetSelection(created);
		if (_mergeBonusLocker.IsOpen.Value)
		{
			foreach (GIData bonu in module.Bonus)
			{
				if (GIMaster.Field.IsFull)
				{
					break;
				}
				Point nearEmptyPoint = GIMaster.Field.GetNearEmptyPoint(created.Coordinates);
				GIData giData2 = bonu.Copy().SetCoordinates(nearEmptyPoint);
				GIMaster.Field.CreateItem(giData2).DoCreateFrom(giTo.Position);
			}
		}
		if (module.BonusChance > BubbleChanceTh && !GIMaster.Field.IsFull)
		{
			Point nearEmptyPoint2 = GIMaster.Field.GetNearEmptyPoint(giTo.Coordinates);
			GIData giData3 = Weights.GetWeightObject((IList<WeightNode<GIData>>)module.BubbleMergeList).Copy().SetCoordinates(nearEmptyPoint2);
			GIMaster.Field.CreateItem(giData3).DoCreateFrom(created.Position);
		}
		this.OnMerge?.Invoke(created);
		_mergeNotifier.Notify(created);
		global::Merge.Sounds.Play($"merge_{Mathf.Clamp(created.Key.ID, 1, 5)}");
		void AtItemDestroyWhileTween(GameItem sender)
		{
			if (tween.IsActive())
			{
				tween.Kill(complete: true);
			}
		}
		void OnTweenComplete()
		{
			ghost1.Destroy();
			ghost2.Destroy();
			created.OnRemoving -= AtItemDestroyWhileTween;
		}
	}

	public void AddBlockMergeNode(BlockMergeNode node)
	{
		blockMergeNodes.Add(node);
	}

	public void RemoveBlockMergeNode(BlockMergeNode node)
	{
		blockMergeNodes.Remove(node);
	}

	public void SetTestState(bool isTest)
	{
		IsTestBubble = isTest;
	}

	private bool IsMergeBlockedByNode(GIKey key, out BlockMergeNode node)
	{
		node = null;
		if (blockMergeNodes.Count == 0)
		{
			return false;
		}
		node = blockMergeNodes.FirstOrDefault((BlockMergeNode x) => x.keysList.Contains(key));
		if (node != null && node.allowIfAtLeastOneLeft && Field.FindItems(key).Count > 2)
		{
			return false;
		}
		return node != null;
	}

	public bool IsMergable(GameItem gi)
	{
		if (gi.Config.HasModule<ModuleConfigs.Merge>() && gi.AllowInteraction(GIModuleType.Merge))
		{
			return !gi.IsRemovingNow;
		}
		return false;
	}

	protected override void OnDestroy()
	{
		dragManager?.OnDestroy();
		pulsationManager?.Dispose();
		base.OnDestroy();
	}
}
