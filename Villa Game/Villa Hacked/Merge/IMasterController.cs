using System.Collections.Generic;

namespace Merge;

public interface IMasterController
{
	void LinkControllers(IList<BaseController> controllers);
}
