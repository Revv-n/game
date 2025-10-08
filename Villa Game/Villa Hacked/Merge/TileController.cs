using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.MergeCore;
using MISC.Resolution;
using UnityEngine;
using Zenject;

namespace Merge;

public class TileController : Controller<TileController>, IResolutionAdapter
{
	[SerializeField]
	private List<TilesConfig> configs;

	[Header("Initial tile view")]
	[SerializeField]
	private bool inverseTileBacks;

	private PointMatrix<Tile> matrix;

	private Vector3 cachedStartPos;

	private Vector3 half;

	private Rect cachedBounds;

	private TilesConfig correctConfig;

	[SerializeField]
	private FieldMonoMediator[] fieldMediators;

	[Inject]
	private MergeFieldProvider mergeFieldProvider;

	private MergeField mergeField => Controller<GameItemController>.Instance.CurrentField;

	public Point Size => Config.FieldSize;

	private TilesConfig Config
	{
		get
		{
			if (correctConfig == null)
			{
				correctConfig = configs.First((TilesConfig x) => x.Resolution == ResolutionAdapterMaster.Resolution);
			}
			return correctConfig;
		}
	}

	[EditorButton]
	[ContextMenu("PlaceOld")]
	private void PlaceOld()
	{
		Adaptate(ResolutionType.Old);
	}

	[EditorButton]
	[ContextMenu("PlaceIPad")]
	private void PlaceIPad()
	{
		Adaptate(ResolutionType.IPad);
	}

	[EditorButton]
	[ContextMenu("PlaceWide")]
	private void PlaceWide()
	{
		Adaptate(ResolutionType.Wide);
	}

	public void Adaptate(ResolutionType type)
	{
		correctConfig = configs.First((TilesConfig x) => x.Resolution == type);
		Preload();
	}

	[EditorButton]
	[ContextMenu("Place Tiles")]
	public override void Preload()
	{
		base.Preload();
		FieldMonoMediator[] array = fieldMediators;
		foreach (FieldMonoMediator mediator in array)
		{
			InitField(mediator);
		}
	}

	public void InitField(FieldMonoMediator mediator)
	{
		Tile[] tiles = mediator.Tiles;
		mediator.TilesParent.parent.position = Config.RootOffset;
		Vector3 vector = new Vector3(Config.Spacing / 2f, Config.Spacing / 2f, 0f);
		cachedStartPos = (Vector3)Config.FieldSize * (Config.TileSize + Config.Spacing) / -2f + vector + mediator.TilesParent.position;
		cachedBounds = new Rect(cachedStartPos, (Vector2)Config.FieldSize * (Config.TileSize + Config.Spacing));
		half = Vector3.one * Config.TileSize / 2f;
		matrix = PointMatrix<Tile>.CreateBySizeFromUnpointed(Config.FieldSize, tiles, ValidationFunс);
		SetTileFieldBack(mediator);
		mediator.FieldBackground.sprite = mediator.OutframeSp;
		mediator.FieldBackground.size = new Vector2((Config.TileSize + Config.Spacing) * (float)Config.FieldSize.X + Config.OuterFrameSize, (Config.TileSize + Config.Spacing) * (float)Config.FieldSize.Y + Config.OuterFrameSize);
	}

	private void ValidationFunс(Tile tile, Point pnt)
	{
		tile.IsBackEnabled = true;
		tile.Coordinates = pnt;
		tile.transform.position = GetMinPositonByPoint(pnt);
		tile.SpriteRen.size = new Vector2(Config.TileSize, Config.TileSize);
		tile.name = $"Tile {pnt}";
	}

	private void SetTileFieldBack(FieldMonoMediator mediator)
	{
		Tile[] tiles = mediator.Tiles;
		foreach (Tile tile in tiles)
		{
			ChooseBack(tile, tile.Coordinates, mediator);
		}
	}

	private void ChooseBack(Tile tile, Point pnt, FieldMonoMediator mediator)
	{
		bool flag = (pnt.X + pnt.Y) % 2 == 0;
		BackTile backSetting = ((inverseTileBacks != flag) ? mediator.LightTile : mediator.DarkTile);
		tile.SetBackSetting(backSetting);
		SetTileBack(tile, isLock: false);
	}

	public static Point GetPoint(Vector3 position)
	{
		return Controller<TileController>.Instance.GetPointByPositon(position);
	}

	public static bool TryGetPoint(Vector3 position, out Point point)
	{
		point = GetPoint(position);
		return IsPointInField(position);
	}

	public static Vector3 GetPosition(Point pnt)
	{
		return Controller<TileController>.Instance.GetPositonByPoint(pnt);
	}

	public static bool IsPointInField(Vector3 position)
	{
		return Controller<TileController>.Instance.cachedBounds.Contains(position);
	}

	private Vector3 GetMinPositonByPoint(Point pnt)
	{
		return cachedStartPos + (Vector3)pnt * (Config.TileSize + Config.Spacing);
	}

	private Vector3 GetPositonByPoint(Point pnt)
	{
		return GetMinPositonByPoint(pnt) + half;
	}

	private Point GetPointByPositon(Vector3 position)
	{
		return new Point(Mathf.FloorToInt((position.x - cachedStartPos.x) / (Config.TileSize + Config.Spacing)), Mathf.FloorToInt((position.y - cachedStartPos.y) / (Config.TileSize + Config.Spacing)));
	}

	public void SetTileBack(Tile tile, bool isLock)
	{
		tile.SpriteRen.sprite = ChooseTileBack(tile.BackSetting, isLock);
	}

	private Sprite ChooseTileBack(BackTile set, bool isLock)
	{
		if (!isLock)
		{
			return set.TileUnlock;
		}
		return set.TileLock;
	}

	public bool TryGetTile(GameItem gameItem, out Tile tile)
	{
		tile = null;
		if (!mergeFieldProvider.TryGetFieldWithItem(gameItem, out var data))
		{
			return false;
		}
		return TryGetTile(gameItem.Coordinates, out tile, data);
	}

	public bool TryGetTile(Point coord, out Tile tile, MergeField field = null)
	{
		field = field ?? mergeField;
		Tile[] tiles = field.FieldMediator.Tiles;
		foreach (Tile tile2 in tiles)
		{
			if (tile2.Coordinates == coord)
			{
				tile = tile2;
				return true;
			}
		}
		tile = null;
		throw new Exception().SendException("Tile doesn't exist " + coord.ToString());
	}
}
