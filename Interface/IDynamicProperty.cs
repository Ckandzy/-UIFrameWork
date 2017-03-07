using System;

namespace XHFrameWork
{
	public interface IDynamicProperty
	{
		void DoChangeProperty(int id, object oldValue, object newValue);
		PropertyItem GetProperty(int id);
	}
}

