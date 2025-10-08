using System;
using DG.Tweening;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore.GameItemBox;

public class Locked : GIBox.Casted<ModuleDatas.Locked, ModuleConfigs.Locked>, IMotionBlocker, IBlockModulesInteraction, IBlockModulesAction, IActionModule
{
	private GameObject moveLocker;

	private GameObject mergeLocker;

	private SpriteRenderer lockedIcon;

	private FilterNode<GIModuleType> interactionsFilter;

	private FilterNode<GIModuleType> actionsFilter;

	private Tween removeMoveLockTween;

	private Tween removeMergeLockTween;

	private Tile tile;

	private Vector2 imageFix = new Vector2(1.75f, 1.75f);

	private Vector2 imageBoxFix = new Vector2(1.64f, 1.64f);

	public SpriteRenderer LockedIcon => lockedIcon;

	public bool BlockMotion => true;

	private TileController TC => Controller<TileController>.Instance;

	bool IActionModule.IsActionEnable => true;

	int IActionModule.ActionPriority => 100;

	FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

	FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

	private Color LockedColor => new Color(1f, 1f, 1f);

	public event Action OnBlockInteractionChange;

	public event Action OnBlockActionChange;

	public Locked(ModuleDatas.Locked data, ModuleConfigs.Locked config)
		: base(data, config)
	{
	}

	public override void Kill()
	{
		removeMoveLockTween?.Kill(complete: true);
		removeMergeLockTween?.Kill(complete: true);
		SetBlockTileBack(isLock: false);
	}

	protected override void AtAttach()
	{
		base.AtAttach();
		interactionsFilter = (base.Data.BlocksMerge ? new FilterNode<GIModuleType>(FilterType.White) : new FilterNode<GIModuleType>(FilterType.White, default(GIModuleType)));
		actionsFilter = (base.Data.BlocksMerge ? new FilterNode<GIModuleType>(FilterType.White) : new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Locked));
		AddMoveLocker();
		if (base.Data.BlocksMerge)
		{
			AddMergeLocker();
		}
		SetBlockTileBack(isLock: true);
	}

	private void SetBlockTileBack(bool isLock)
	{
		TileController tC = TC;
		if ((object)tC != null && tC.TryGetTile(base.Parent.Coordinates, out tile))
		{
			TC.SetTileBack(tile, isLock);
		}
	}

	private void AddMoveLocker()
	{
		moveLocker = new GameObject("Web");
		base.Parent.AddChild(moveLocker.transform, atCentre: true);
		SpriteRenderer spriteRenderer = moveLocker.AddComponent<SpriteRenderer>();
		spriteRenderer.drawMode = SpriteDrawMode.Sliced;
		spriteRenderer.size = imageFix;
		spriteRenderer.SetSprite(IconProvider.GetSprite("Images/Web")).SetOrder("Game", 1);
		moveLocker.layer = 9;
		spriteRenderer.SetAlpha(0.7f);
		moveLocker.transform.localPosition = Vector3.zero;
		base.Parent.IconRenderer.color = LockedColor;
		moveLocker.transform.Translate(0f, 0f, -0.1f);
	}

	private void AddMergeLocker()
	{
		mergeLocker = new GameObject("LockBox");
		base.Parent.AddChild(mergeLocker.transform, atCentre: true);
		lockedIcon = mergeLocker.AddComponent<SpriteRenderer>();
		if (base.Data.RandomSkin == 0)
		{
			base.Data.RandomSkin = UnityEngine.Random.Range(1, 5);
		}
		lockedIcon.SetSprite(IconProvider.GetSprite($"Images/LockBox{base.Data.RandomSkin}")).SetOrder("Game", 1);
		lockedIcon.drawMode = SpriteDrawMode.Sliced;
		lockedIcon.size = imageBoxFix;
		mergeLocker.layer = 9;
		mergeLocker.transform.localPosition = Vector3.zero;
		base.Parent.IconRenderer.color = LockedColor;
		mergeLocker.transform.Translate(0f, 0f, -0.11f);
		moveLocker?.SetActive(value: false);
		base.Parent.IconRenderer.enabled = false;
	}

	public void RemoveMoveLocker()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(DOTweenModuleSprite.DOColor(base.Parent.IconRenderer, Color.white, 0.3f));
		sequence.Join(moveLocker.transform.DOScale(0f, 0.3f));
		sequence.OnComplete(delegate
		{
			UnityEngine.Object.Destroy(moveLocker);
		});
		removeMoveLockTween = sequence;
		interactionsFilter = FilterNode<GIModuleType>.Empty;
		actionsFilter = FilterNode<GIModuleType>.Empty;
		this.OnBlockInteractionChange?.Invoke();
		this.OnBlockActionChange?.Invoke();
		base.Parent.RemoveModule(GIModuleType.Locked);
	}

	public void RemoveMergeLocker()
	{
		base.Parent.IconRenderer.enabled = true;
		moveLocker?.SetActive(value: true);
		GIGhost ghost = GIGhost.CreateGhost(mergeLocker.GetComponent<SpriteRenderer>());
		ghost.transform.SetParent(base.Parent.transform.parent);
		base.Parent.transform.SetScale(0f);
		Sequence sequence = DOTween.Sequence();
		Tween tween = Controller<LockedController>.Instance.DestructionTweenBuilder.BuildTween(ghost);
		sequence.Join(tween);
		sequence.Join(base.Parent.transform.DOScale(1f, 0.6f)).SetEase(Ease.InSine);
		tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
		{
			ghost?.Destroy();
			ghost = null;
		});
		sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
		{
			ghost?.Destroy();
			ghost = null;
		});
		base.Parent.AppendOuterTween(sequence);
		UnityEngine.Object.Destroy(mergeLocker);
		removeMergeLockTween = tween;
		base.Data.BlocksMerge = false;
		interactionsFilter = new FilterNode<GIModuleType>(FilterType.White, default(GIModuleType));
		actionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Locked);
		this.OnBlockInteractionChange?.Invoke();
		this.OnBlockActionChange?.Invoke();
	}
}
