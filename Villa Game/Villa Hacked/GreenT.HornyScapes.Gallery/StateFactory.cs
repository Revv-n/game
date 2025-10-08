using GreenT.Data;
using Zenject;

namespace GreenT.HornyScapes.Gallery;

public class StateFactory : IFactory<GalleryState>, IFactory
{
	private readonly IGallery gallery;

	private readonly ISaver saver;

	public StateFactory(IGallery gallery, ISaver saver)
	{
		this.gallery = gallery;
		this.saver = saver;
	}

	public GalleryState Create()
	{
		GalleryState galleryState = new GalleryState(gallery);
		saver.Add(galleryState);
		return galleryState;
	}
}
