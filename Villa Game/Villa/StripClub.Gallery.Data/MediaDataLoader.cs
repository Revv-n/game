using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GreenT;
using GreenT.AssetBundles;
using GreenT.Settings;
using GreenT.Settings.Data;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace StripClub.Gallery.Data;

public sealed class MediaDataLoader : ILoader<IEnumerable<int>, IEnumerable<IMediaInfo>>, IBundlesLoader<IEnumerable<int>, Media>, ILoader<IEnumerable<int>, Media>, IBundlesLoader<IEnumerable<IMediaInfo>, IEnumerable<Media>>, ILoader<IEnumerable<IMediaInfo>, IEnumerable<Media>>, IDisposable
{
	private const RequestType mediaRequestType = RequestType.MediaInfo;

	private const BundleType mediaBundleType = BundleType.Media;

	private readonly IProjectSettings projectSettings;

	private readonly IAssetBundlesLoader assetBundlesLoader;

	private readonly IFactory<MediaMapper, IMediaInfo> mediaInfoFactory;

	private readonly IFactory<IMediaInfo, IAssetBundle, Media> mediaFactory;

	private readonly ILoader<IEnumerable<MediaMapper>> mediaMapperLoader;

	private CompositeDisposable releaseMediaDisposable;

	public MediaDataLoader(IProjectSettings projectSettings, IAssetBundlesLoader assetBundlesLoader, IFactory<MediaMapper, IMediaInfo> mediaInfoFactory, IFactory<IMediaInfo, IAssetBundle, Media> mediaFactory, ILoader<IEnumerable<MediaMapper>> mediaMapperLoader)
	{
		this.projectSettings = projectSettings;
		this.assetBundlesLoader = assetBundlesLoader;
		this.mediaInfoFactory = mediaInfoFactory;
		this.mediaFactory = mediaFactory;
		this.mediaMapperLoader = mediaMapperLoader;
	}

	public IObservable<IEnumerable<IMediaInfo>> Load(IEnumerable<int> mediaIDs)
	{
		if (!mediaIDs.Any())
		{
			return Observable.Empty<IEnumerable<MediaInfo>>();
		}
		return LoadMediaInfo(mediaIDs).ToArray();
	}

	IObservable<Media> ILoader<IEnumerable<int>, Media>.Load(IEnumerable<int> mediaIDs)
	{
		if (!mediaIDs.Any())
		{
			return Observable.Return(Array.Empty<Media>().FirstOrDefault());
		}
		IConnectableObservable<Media> connectableObservable = LoadMediaInfo(mediaIDs).ContinueWith(DownloadMediaAsset).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Error on loading media files with ids: " + mediaIDs.CollectionToString());
		}).Publish();
		connectableObservable.Connect();
		return connectableObservable;
	}

	public void ReleaseBundle(IEnumerable<int> mediaIDs)
	{
		releaseMediaDisposable.Add(LoadMediaInfo(mediaIDs).Do(ReleaseMedia).Subscribe());
	}

	IObservable<IEnumerable<Media>> ILoader<IEnumerable<IMediaInfo>, IEnumerable<Media>>.Load(IEnumerable<IMediaInfo> mediaInfos)
	{
		if (!mediaInfos.Any())
		{
			return Observable.Return((IEnumerable<Media>)Array.Empty<Media>());
		}
		return LoadAllMediaByInfo(mediaInfos.ToObservable()).Catch(delegate(Exception ex)
		{
			throw ex.SendException("Error on loading media files with ids: " + mediaInfos.Select((IMediaInfo _info) => _info.ID).CollectionToString());
		});
	}

	public void ReleaseBundle(IEnumerable<IMediaInfo> mediaInfos)
	{
		releaseMediaDisposable.Add(mediaInfos.ToObservable().Do(ReleaseMedia).Subscribe());
	}

	private IObservable<IMediaInfo> LoadMediaInfo(IEnumerable<int> mediaIDs)
	{
		return (from _mappers in mediaMapperLoader.Load()
			select from _mapper in _mappers
				where mediaIDs.Any((int _id) => _id.Equals(_mapper.id))
				select _mapper).SelectMany((IEnumerable<MediaMapper> x) => x).Select(mediaInfoFactory.Create);
	}

	private IObservable<IEnumerable<Media>> LoadAllMediaByInfo(IObservable<IMediaInfo> observableMediaInfo)
	{
		return observableMediaInfo.SelectMany((Func<IMediaInfo, IObservable<Media>>)DownloadMediaAsset).ToArray();
	}

	private IObservable<Media> DownloadMediaAsset(IMediaInfo mediaInfo)
	{
		string mediaPath = GetMediaPath(mediaInfo.ID);
		return from x in assetBundlesLoader.DownloadAssetBundle(mediaPath)
			select mediaFactory.Create(mediaInfo, x);
	}

	private string GetMediaPath(int mediaID)
	{
		string path = projectSettings.BundleUrlResolver.BundleUrl(BundleType.Media);
		string path2 = mediaID.ToString();
		return Path.Combine(path, path2);
	}

	private void ReleaseMedia(IMediaInfo mediaInfo)
	{
		string mediaPath = GetMediaPath(mediaInfo.ID);
		assetBundlesLoader.Release(mediaPath);
	}

	public void Dispose()
	{
		releaseMediaDisposable?.Dispose();
	}
}
