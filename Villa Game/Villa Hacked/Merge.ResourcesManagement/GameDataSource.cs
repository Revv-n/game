using System;
using System.Collections.Generic;

namespace Merge.ResourcesManagement;

public static class GameDataSource
{
	public enum TypeContent
	{
		DefaultFields,
		GameItems,
		Levels,
		Art,
		RoomConfigs,
		Tasks,
		Dialogs,
		Localization,
		ShopConfigs,
		Tutorials,
		EventConfigs,
		Recipes
	}

	public enum Place
	{
		Auto,
		Editor,
		Bundles,
		Resources
	}

	public static Place GetAutoPlaceByTypeContent(ref Place place, TypeContent typeContent)
	{
		if (place == Place.Auto)
		{
			place = GetPlaceFromPrefs(typeContent);
		}
		if (place == Place.Auto)
		{
			place = Place.Editor;
		}
		return place;
	}

	public static Place GetPlaceFromPrefs(TypeContent typeContent)
	{
		return Place.Auto;
	}

	public static void SetPlaceToPrefs(TypeContent typeContent, Place place)
	{
	}

	public static void ResetAll()
	{
		int num = Enum.GetNames(typeof(TypeContent)).Length;
		for (int i = 0; i < num; i++)
		{
			SetPlaceToPrefs((TypeContent)i, Place.Auto);
		}
	}

	public static void SetAll(List<TypeContent> typeContents, Place place)
	{
		foreach (TypeContent typeContent in typeContents)
		{
			SetPlaceToPrefs(typeContent, place);
		}
	}
}
