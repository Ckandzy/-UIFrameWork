using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace XHFrameWork
{
	public class AssetInfo{
		private UnityEngine.Object _Object;
		public Type AssetType{ get; set; }
		public string Path { get; set; }
		public int RefCount{ get; set; }
		public bool isLoaded{
			get{ 
				return _Object != null;
			}
		}

		public UnityEngine.Object AssetObject{
			get{ 
				if (_Object == null) {
					_ResourceLoad ();
				}
				return _Object;
			}
		}

		//Coroutine 协程
		public IEnumerator GetCoroutineObject(Action<UnityEngine.Object> _loaded){
			while (true) {
				yield return null;
				if (_Object == null) {
					_ResourceLoad ();
					yield return null;
				}
				if (_loaded != null)
					_loaded (_Object);	
				yield break;
			}
		}

		private void _ResourceLoad(){
			try{
				_Object = Resources.Load (Path);
				if(_Object == null)
					Debug.Log("Resources Load Failure! Path: " + Path);
			}
			catch(Exception e){
				Debug.Log (e.ToString ());
			}
		}

		#region asynchronously loads an asset stored at path in a resource folder
		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded){
			return GetAsyncObject (_loaded, null);
		}

		public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded, Action<float> _progress){
			
			if (_Object != null) {
				_loaded (_Object);
				yield break;
			}

			ResourceRequest _resRequest = Resources.LoadAsync (Path);

			//what's the operation's progress
			while (_resRequest.progress < 0.9) {
				if (_progress != null)
					_progress (_resRequest.progress);
				yield return null;
			}

			//
			while (!_resRequest.isDone) {
				if (_progress != null)
					_progress (_resRequest.progress);
				yield return null;
			}

			// ???
			_Object = _resRequest.asset;
			if (_loaded != null)
				_loaded (_Object);
			yield return _resRequest;
		}
	}
	#endregion

	public class ResManager : Singleton<ResManager>
	{
		private Dictionary<string, AssetInfo> dicAssetInfo = null;

		public override void Init(){
			dicAssetInfo = new Dictionary<string,AssetInfo> ();
		}

		#region Load Resources & Instantiate Object
		public void LoadCoroutineInstance(string _path, Action<UnityEngine.Object> _loaded){
			LoadCoroutine (
				_path,
				(_obj) => {
					Instantiate(_obj, _loaded);
				}
			);
		}

		public void LoadAsyncInstance(string _path, Action<UnityEngine.Object> _loaded){
			LoadAsync (
				_path,
				(_obj) => {
					Instantiate(_obj, _loaded);
				}
			);
		}

		public void LoadAsyncInstance(string _path, Action<UnityEngine.Object> _loaded,Action<float> _progress){
			LoadAsync (
				_path,
				(_obj) => {
					Instantiate(_obj, _loaded);
				},
				_progress
			);
		}

		public UnityEngine.Object LoadInstance(string _path){
			UnityEngine.Object _retObj = null;
			UnityEngine.Object _obj = Load (_path);
			if(_obj != null){
				_retObj = MonoBehaviour.Instantiate(_obj);
				//是否实例化成功
				if(_retObj != null){
					return _retObj;
				}
				else{
					Debug.LogError("Error: null Instantiate _retObj.");
				}
			}
			//资源加载失败
			else{
				Debug.LogError("Error: null Resources Load return _obj.");
			}
			return null;
		}

		public void LoadInstance(string _path, Action<UnityEngine.Object> _loaded){
			LoadInstance (_path, _loaded, null);
		}

		public void LoadInstance(string _path,Action<UnityEngine.Object> _loaded, Action<float> _progress){
			LoadAsync(_path,
				(_obj) =>{
					UnityEngine.Object _retObj = null;
					//资源加载成功
					if(_obj != null){
						_retObj = MonoBehaviour.Instantiate(_obj);
						//是否实例化成功
						if(_retObj != null){
							if(_loaded != null)
								_loaded(_retObj);
							else
								Debug.LogError("null _loaded.");
						}
						else{
							Debug.LogError("Error: null Instantiate _retObj.");
						}
					}
					//资源加载失败
					else{
						Debug.LogError("Error: null Resources Load return _obj.");
					}
				},
				_progress
			);
		}
		#endregion

		#region Lod Resources
		public UnityEngine.Object Load(string _path){
			AssetInfo _assetInfo = GetAssetInfo (_path);
			if (_assetInfo != null)
				return _assetInfo.AssetObject;
			return null;
		}
		#endregion

		#region Load Coroutine Resources
		public void LoadCoroutine(string _path,Action<UnityEngine.Object> _loaded){
			AssetInfo _assetInfo = GetAssetInfo (_path, _loaded);
			if (_assetInfo != null)
				CoroutineController.Instance.StartCoroutine (_assetInfo.GetCoroutineObject (_loaded));
		}
		#endregion

		#region Load Async Resources
		public void LoadAsync(string _path,Action<UnityEngine.Object> _loaded){
			LoadAsync (_path, _loaded, null);
		}

		public void LoadAsync(string _path,Action<UnityEngine.Object> _loaded, Action<float> _progress){
			AssetInfo _assetInfo = GetAssetInfo (_path, _loaded);
			if (_assetInfo != null)
				CoroutineController.Instance.StartCoroutine (_assetInfo.GetAsyncObject (_loaded, _progress));
		}
		#endregion

		#region Get AssetInfo & Instantiate Object
		private AssetInfo GetAssetInfo(string _path){
			return GetAssetInfo (_path, null);
		}

		private AssetInfo GetAssetInfo(string _path, Action<UnityEngine.Object> _loaded){
			if (string.IsNullOrEmpty (_path)) {
				Debug.LogError ("Error: null _path name.");
				if (_loaded != null)
					_loaded (null);
			}
			//Load Res ...
			AssetInfo _assetInfo = null;
			if (!dicAssetInfo.TryGetValue (_path, out _assetInfo)) {
				_assetInfo = new AssetInfo ();
				_assetInfo.Path = _path;
				dicAssetInfo.Add (_path, _assetInfo);
			}
			_assetInfo.RefCount++;
			return _assetInfo;
		}

		private UnityEngine.Object Instantiate(UnityEngine.Object _obj){
			return Instantiate (_obj, null);
		}

		private UnityEngine.Object Instantiate(UnityEngine.Object _obj, Action<UnityEngine.Object> _loaded){
			UnityEngine.Object _retObj = null;
			if (_obj != null) {
				_retObj = MonoBehaviour.Instantiate (_obj);
				//是否实例化成功
				if (_retObj != null) {
					if (_loaded != null)
						_loaded (_retObj);
//					else
//						Debug.LogError ("Error: null _loaded.");
					return _retObj;
				}
				else {
					Debug.LogError ("Error: null Instantiate _retObj.");
				}
			}
			else {
				Debug.LogError ("Error: null Resources Load return _obj");
			}
			return null;
		}
		#endregion
	}
}

