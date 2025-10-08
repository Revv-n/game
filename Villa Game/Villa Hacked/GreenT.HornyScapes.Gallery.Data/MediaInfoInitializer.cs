using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Gallery;
using StripClub.Gallery.Data;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Gallery.Data;

public class MediaInfoInitializer : StructureInitializerViaArray2<MediaMapper, IMediaInfo>
{
	private readonly MapperStructureInitializer<MediaMapper> mapperStructureInitializer;

	private IDisposable stream;

	public MediaInfoInitializer(ICollectionSetter<IMediaInfo> lockerSetter, IFactory<MediaMapper, IMediaInfo> factory, MapperStructureInitializer<MediaMapper> mapperStructureInitializer, IEnumerable<IStructureInitializer> others = null)
		: base(lockerSetter, factory, others)
	{
		this.mapperStructureInitializer = mapperStructureInitializer;
	}

	public override IObservable<bool> Initialize(IEnumerable<MediaMapper> mappers)
	{
		stream?.Dispose();
		stream = ObservableExtensions.Subscribe<bool>(mapperStructureInitializer.Initialize(mappers));
		return base.Initialize(mappers);
	}
}
