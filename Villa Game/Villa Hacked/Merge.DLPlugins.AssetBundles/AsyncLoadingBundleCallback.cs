namespace Merge.DLPlugins.AssetBundles;

public delegate void AsyncLoadingBundleCallback(int index, int bundle_count, float progress, float file_length, string unit, string name);
