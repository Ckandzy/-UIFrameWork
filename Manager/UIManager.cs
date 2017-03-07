using System;
using UnityEngine;
using System.Collections.Generic;

namespace XHFrameWork{
	public class UIManager : Singleton<UIManager>{
		private class UIInfoData{
			public EnumUIType UIType{ get; private set; }
			public Type ScriptType { get; private set; }
			public string Path{ get; private set; }
			public object UIparams{ get; private set; }

			public UIInfoData(EnumUIType _uiType, string _path, params object[] _uIparams){
				UIType = _uiType;
				Path = _path;
				UIparams = _uIparams;
				this.ScriptType = UIPathDefines.GetUIScriptByType(this.UIType);
			}
		}

		private Dictionary<EnumUIType, GameObject> dicOpenUIs = null;

		private Stack<UIInfoData> stackOpenUIs = null;

		public override void Init(){
			dicOpenUIs = new Dictionary<EnumUIType, GameObject> ();
			stackOpenUIs = new Stack<UIInfoData> ();
		}

		public T GetUI<T>(EnumUIType _uiType) where T : BaseUI{
			GameObject _retObj = GetUIObject (_uiType);
			if (_retObj != null) {
				return _retObj.GetComponent<T> ();
			}
			return null;
		}

		//Get UI Object
		public GameObject GetUIObject(EnumUIType _uiType){
			GameObject _retObj = null;
			if (!dicOpenUIs.TryGetValue (_uiType, out _retObj))
				throw new Exception ("_dicOpenUIs TryGetValue Failure _uiType: " + _uiType.ToString ());
			return _retObj;
		}

		#region Preload UI Prefab By EnumUIType
		//预加载
		public void PreloadUI(EnumUIType[] _uiTypes){
			for (int i = 0; i < _uiTypes.Length; i++) {
				PreloadUI (_uiTypes [i]);
			}
		}
		public void PreloadUI(EnumUIType _uiType){
			string path = UIPathDefines.GetPrefabsPathByType (_uiType);
			Resources.Load (path);
		}
		#endregion

		#region Open UI By EnumUIType
		public void OpenUI(EnumUIType[] _uiTypes){
			OpenUI (false, _uiTypes, null);
		}

		public void OpenUI(EnumUIType _uiType, params object[] _uiParams){
			EnumUIType[] _uiTypes = new EnumUIType[1];
			_uiTypes [0] = _uiType;
			OpenUI (false, _uiTypes, _uiParams);
		}

		public void OpenUICloseOthers(EnumUIType[] _uiTypes){
			OpenUI (true, _uiTypes, null);
		}

		public void OpenUICloseOthers(EnumUIType _uiType, params object[] _uiParams){
			EnumUIType[] _uiTypes = new EnumUIType[1];
			_uiTypes [0] = _uiType;
			OpenUI (true, _uiTypes, _uiParams);
		}

		public void OpenUI(bool _isCloseOther, EnumUIType[] _uiTypes, params object[] _uiParams){
			//do or not close other UI.
			if (_isCloseOther) {
				CloseUIAll ();
			}
			//Push _uiTypes in stack.
			for (int i = 0; i < _uiTypes.Length; i++) {
				EnumUIType _uiType = _uiTypes [i];
				if (!dicOpenUIs.ContainsKey (_uiType)) {
					string _path = UIPathDefines.GetPrefabsPathByType (_uiType);
					stackOpenUIs.Push (new UIInfoData (_uiType, _path, _uiParams));
				}
			}

			//Async Load UI.
			if(stackOpenUIs.Count > 0){
				CoroutineController.Instance.StartCoroutine (AsyncLoadData ());
			}
		}
		#endregion

		#region Async Load Data
		private IEnumerator<int> AsyncLoadData(){
			UIInfoData _uiInfoData = null;
			UnityEngine.Object _prefabObj = null;
			GameObject _uiObject = null;

			if (stackOpenUIs != null && stackOpenUIs.Count > 0) {
				do {
					_uiInfoData = stackOpenUIs.Pop ();
					_prefabObj = Resources.Load(_uiInfoData.Path);
					if(_prefabObj != null){
						//_uiObject = NUGITools.AddChild(Game.Instance.mainUICamera.gameObject,_prefabObj);
						_uiObject = MonoBehaviour.Instantiate(_prefabObj) as GameObject;
						BaseUI _baseUI = _uiObject.GetComponent<BaseUI>();
						if(_baseUI == null){
							_baseUI = _uiObject.AddComponent(_uiInfoData.ScriptType) as BaseUI;
						} else{
							_baseUI.SetUIWhenOpening(_uiInfoData.UIparams);
						}
						dicOpenUIs.Add(_uiInfoData.UIType, _uiObject);
					}
				} while(stackOpenUIs.Count > 0);
			}
			yield return 0;
		}
		#endregion

		#region Close UI By EnumUIType
		public void CloseUI(EnumUIType _uiType){
			GameObject _uiObj = null;
			if (!dicOpenUIs.TryGetValue (_uiType, out _uiObj)) {
				Debug.Log ("dicOpenUIs TryGetValue Failure! _uiType :" + _uiType.ToString ());
				return;
			}
			CloseUI (_uiType, _uiObj);
		}
		public void CloseUI(EnumUIType[] _uiTypes){
			for(int i = 0; i < _uiTypes.Length; i++){
				CloseUI (_uiTypes [i]);
			}
		}
		public void CloseUIAll(){
			List<EnumUIType> _listKey = new List<EnumUIType> (dicOpenUIs.Keys);
//			for (int i = 0; i < _listKey.Count; i++) {
//				CloseUI (_listKey [i]);
//			}
			foreach (EnumUIType _uiType in _listKey){
				GameObject _uiObj = dicOpenUIs [_uiType];
				CloseUI (_uiType, _uiObj);
			}
			dicOpenUIs.Clear ();
		}
		private void CloseUI(EnumUIType _uiType, GameObject _uiObj){
			if (_uiObj == null) {
				dicOpenUIs.Remove (_uiType);
			}
			else {
				BaseUI _baseUI = _uiObj.GetComponent<BaseUI> ();
				if (_baseUI != null) {
					_baseUI.StateChanged += CloseUIHandler;
					_baseUI.Release ();
				}
				else {
					GameObject.Destroy (_uiObj);
					dicOpenUIs.Remove (_uiType);
				}
			}
		}
		public void CloseUIHandler(object sender, EnumObjectState _newState, EnumObjectState oldState){
			if (_newState == EnumObjectState.Closing) {
				BaseUI _baseUI = sender as BaseUI;
				dicOpenUIs.Remove (_baseUI.GetUIType ());
				_baseUI.StateChanged -= CloseUIHandler;
			}
		}
		#endregion

	}
}

