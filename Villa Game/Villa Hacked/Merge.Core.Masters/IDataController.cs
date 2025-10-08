namespace Merge.Core.Masters;

public interface IDataController
{
	Data GetSave();

	void Load(Data baseData);
}
