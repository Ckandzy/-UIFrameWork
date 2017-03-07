using System;
using UnityEngine;

namespace XHFrameWork
{
	static public class MethodExtension
	{
		static public T GetOrAddComponent<T>(this GameObject go) where T : Component{
			T ret = go.GetComponent<T> ();
			if (ret == null)
				ret = go.AddComponent<T> ();
			return ret;
		}
	}
}