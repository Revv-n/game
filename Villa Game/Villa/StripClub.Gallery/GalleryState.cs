using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace StripClub.Gallery;

[MementoHolder]
public class GalleryState : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int[] unlockedMediaIDCollection;

		public Memento(GalleryState galleryState)
			: base(galleryState)
		{
			unlockedMediaIDCollection = (from _media in galleryState.Source.GetMedia()
				select _media.Info.ID).ToArray();
		}
	}

	private const string uniqueKeyPrefix = "GalleryState";

	protected int[] loadedIDCollection = new int[0];

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public IGallery Source { get; }

	protected IEnumerable<int> CurrentGaleryMediaIDs => from _media in Source.GetMedia()
		select _media.Info.ID;

	public int[] MediaIDsToLoad => loadedIDCollection.Except(CurrentGaleryMediaIDs).ToArray();

	public string UniqueKey()
	{
		return "GalleryState";
	}

	public GalleryState(IGallery gallery)
	{
		Source = gallery;
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		loadedIDCollection = memento2.unlockedMediaIDCollection;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}
