using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class SubTileSystem : Controller<SubTileSystem>
{
	public TileController TileController;

	public BackTile HighlightSubTile;

	public void SetLight(Tile tile, bool isLock)
	{
		if (isLock)
		{
			tile.SubTile.SpriteRen.sprite = ChooseView(HighlightSubTile, isLock: true);
		}
		else
		{
			tile.SubTile.SpriteRen.sprite = ChooseView(HighlightSubTile, isLock: false);
		}
	}

	public void SetLight(Point point, bool isLock)
	{
		if (TryGetTile(point, out var tile))
		{
			SetLight(tile, isLock);
		}
	}

	public void SetLight(GameItem gameItem, bool isLock)
	{
		if (TryGetTile(gameItem, out var tile))
		{
			SetLight(tile, isLock);
		}
	}

	private void Show(Tile tile)
	{
		tile.SubTile.SetActive(active: true);
	}

	public void Show(Point point)
	{
		if (TryGetTile(point, out var tile))
		{
			Show(tile);
		}
	}

	public void Show(GameItem gameItem)
	{
		if (TryGetTile(gameItem, out var tile))
		{
			Show(tile);
		}
	}

	private void Hide(Tile tile)
	{
		tile.SubTile.SetActive(active: false);
	}

	public void Hide(Point point)
	{
		if (TryGetTile(point, out var tile))
		{
			Hide(tile);
		}
	}

	public void Hide(GameItem gameItem)
	{
		if (TryGetTile(gameItem, out var tile))
		{
			Hide(tile);
		}
	}

	private bool TryGetTile(Point point, out Tile tile)
	{
		return TileController.TryGetTile(point, out tile);
	}

	private bool TryGetTile(GameItem gameItem, out Tile tile)
	{
		return TileController.TryGetTile(gameItem, out tile);
	}

	private Sprite ChooseView(BackTile set, bool isLock)
	{
		if (!isLock)
		{
			return set.TileUnlock;
		}
		return set.TileLock;
	}
}
