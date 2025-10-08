using System;
using GreenT.HornyScapes.Animations;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class GameItemFactory : IFactory<GIData, MergeField, GameItem>, IFactory
{
	private readonly DiContainer diContainer;

	private readonly GameItem prefab;

	private readonly GameItemConverter gameItemConverter;

	private readonly LightningSystem grayScaleSystem;

	private readonly IMergeIconProvider iconProvider;

	public GameItemFactory(IMergeIconProvider iconProvider, LightningSystem grayScaleSystem, GameItemConverter gameItemConverter, GameItem prefab, DiContainer diContainer)
	{
		this.iconProvider = iconProvider;
		this.grayScaleSystem = grayScaleSystem;
		this.gameItemConverter = gameItemConverter;
		this.prefab = prefab;
		this.diContainer = diContainer;
	}

	public GameItem Create(GIData giData, MergeField field)
	{
		try
		{
			GIConfig gIConfig = gameItemConverter.TryConvert(giData);
			GameItem gameItem = diContainer.InstantiatePrefabForComponent<GameItem>(prefab, field.FieldMediator.FieldParent);
			gameItem.Init(giData, gIConfig);
			gameItem.Position = TileController.GetPosition(giData.Coordinates);
			gameItem.Icon = iconProvider.GetSprite(gIConfig.Key);
			gameItem.GrayScaleSystem = grayScaleSystem;
			gameItem.PropBlock = new MaterialPropertyBlock();
			field.SetGameItem(giData.Coordinates, gameItem);
			return gameItem;
		}
		catch (Exception ex)
		{
			throw ex.SendException($"Failed creating GameItem with Key: {giData.Key}. Exception is: {ex.Message}");
		}
	}

	public bool IsItemBroken(GIData giData)
	{
		return gameItemConverter.TryConvert(giData) == null;
	}
}
