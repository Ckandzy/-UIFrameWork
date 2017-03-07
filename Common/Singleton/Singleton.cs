using System;
using UnityEngine;

namespace XHFrameWork
{
	public abstract class Singleton<T> where T : class, new()
	{
		protected static T _Instance = null;

		public static T Instance{
			get{ 
				if (_Instance == null) {
					_Instance = new T ();
				}
				return _Instance;
			}
		}
		protected Singleton (){
			if (_Instance != null)
				throw new SingletonException("This" + (typeof(T).ToString() + "Singleton is not null !!!"));
			Init ();
		}

		public virtual void Init(){
			
		}
	}
}

