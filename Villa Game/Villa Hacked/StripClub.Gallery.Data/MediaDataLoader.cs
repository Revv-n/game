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
		return Observable.ToArray<IMediaInfo>(LoadMediaInfo(mediaIDs));
	}

	IObservable<Media> ILoader<IEnumerable<int>, Media>.Load(IEnumerable<int> mediaIDs)
	{
		if (!mediaIDs.Any())
		{
			return Observable.Return<Media>(Array.Empty<Media>().FirstOrDefault());
		}
		IConnectableObservable<Media> obj = Observable.Publish<Media>(Observable.Catch<Media, Exception>(Observable.ContinueWith<IMediaInfo, Media>(LoadMediaInfo(mediaIDs), (Func<IMediaInfo, IObservable<Media>>)DownloadMediaAsset), (Func<Exception, IObservable<Media>>)delegate(Exception ex)
		{
			throw ex.SendException("Error on loading media files with ids: " + mediaIDs.CollectionToString());
		}));
		obj.Connect();
		return (IObservable<Media>)obj;
	}

	public void ReleaseBundle(IEnumerable<int> mediaIDs)
	{
		releaseMediaDisposable.Add(ObservableExtensions.Subscribe<IMediaInfo>(Observable.Do<IMediaInfo>(LoadMediaInfo(mediaIDs), (Action<IMediaInfo>)ReleaseMedia)));
	}

	IObservable<IEnumerable<Media>> ILoader<IEnumerable<IMediaInfo>, IEnumerable<Media>>.Load(IEnumerable<IMediaInfo> mediaInfos)
	{
		if (!mediaInfos.Any())
		{
			return Observable.Return<IEnumerable<Media>>((IEnumerable<Media>)Array.Empty<Media>());
		}
		return Observable.Catch<IEnumerable<Media>, Exception>(LoadAllMediaByInfo(Observable.ToObservable<IMediaInfo>(mediaInfos)), (Func<Exception, IObservable<IEnumerable<Media>>>)delegate(Exception ex)
		{
			throw ex.SendException("Error on loading media files with ids: " + mediaInfos.Select((IMediaInfo _info) => _info.ID).CollectionToString());
		});
	}

	public void ReleaseBundle(IEnumerable<IMediaInfo> mediaInfos)
	{
		releaseMediaDisposable.Add(ObservableExtensions.Subscribe<IMediaInfo>(Observable.Do<IMediaInfo>(Observable.ToObservable<IMediaInfo>(mediaInfos), (Action<IMediaInfo>)ReleaseMedia)));
	}

	private IObservable<IMediaInfo> LoadMediaInfo(IEnumerable<int> mediaIDs)
	{
		return Observable.Select<MediaMapper, IMediaInfo>(Observable.SelectMany<IEnumerable<MediaMapper>, MediaMapper>(Observable.Select<IEnumerable<MediaMapper>, IEnumerable<MediaMapper>>(mediaMapperLoader.Load(), (Func<IEnumerable<MediaMapper>, IEnumerable<MediaMapper>>)((IEnumerable<MediaMapper> _mappers) => _mappers.Where((MediaMapper _mapper) => mediaIDs.Any((int _id) => _id.Equals(_mapper.id))))), (Func<IEnumerable<MediaMapper>, IEnumerable<MediaMapper>>)((IEnumerable<MediaMapper> x) => x)), (Func<MediaMapper, IMediaInfo>)mediaInfoFactory.Create);
	}

	private IObservable<IEnumerable<Media>> LoadAllMediaByInfo(IObservable<IMediaInfo> observableMediaInfo)
	{
		return Observable.ToArray<Media>(Observable.SelectMany<IMediaInfo, Media>(observableMediaInfo, (Func<IMediaInfo, IObservable<Media>>)DownloadMediaAsset));
	}

	private IObservable<Media> DownloadMediaAsset(IMediaInfo mediaInfo)
	{
		string mediaPath = GetMediaPath(mediaInfo.ID);
		return Observable.Select<IAssetBundle, Media>(assetBundlesLoader.DownloadAssetBundle(mediaPath), (Func<IAssetBundle, Media>)((IAssetBundle x) => mediaFactory.Create(mediaInfo, x)));
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
		CompositeDisposable obj = releaseMediaDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}
