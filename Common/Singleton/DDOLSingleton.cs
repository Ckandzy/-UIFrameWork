using UnityEngine;
using System.Collections;

public abstract class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{
	protected static T _Instance = null;

	public static T Instance{
		get{
			if (_Instance == null) {
				GameObject go = GameObject.Find ("DDOLGameObject");
				if (go == null) {
					go = new GameObject ("DDOLGameObject");
					DontDestroyOnLoad (go);
				}
				_Instance = go.AddComponent<T> ();
			}
			return _Instance;
		}
	}

	private void OnApplicationQuit(){
		_Instance = null;
	}
}

