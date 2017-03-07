using System;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Collections;

namespace XHFrameWork
{
	#region SceneInfoData class
	public class SceneInfoData{

		public Type SceneType{ get; private set; }

		public string SceneName{ get; private set;}

		public object[] Params { get; private set; }

		public SceneInfoData(string _sceneName, Type _sceneType, params object[] _params){
			this.SceneType = _sceneType;
			this.SceneName = _sceneName;
			this.Params = _params;
		}
	}
	#endregion

	public class SceneManager : Singleton<SceneManager>
	{
		private Dictionary<EnumSceneType, SceneInfoData> dicSceneInfo = null;

		private BaseScene currentScene = new BaseScene ();

		public EnumSceneType LastSceneType {
			get;
			set;
		}

		public EnumSceneType ChangeSceneType {
			get;
			private set;
		}

		private EnumUIType SceneOpenUIType = EnumUIType.None;
		private object [] sceneOpenUIParams = null;

		public BaseScene CurrentScene {
			get { return currentScene; }
			set {
				currentScene = value; 
//				if (currentScene != null) {
//					currentScene.Load ();
//				}
			}
		}

		public override void Init ()
		{
			dicSceneInfo = new Dictionary<EnumSceneType, SceneInfoData> ();
		}

		public void RegisterAllScene(){
			RegisterScene (EnumSceneType.StartGame, "StartGameScene", null, null);
			RegisterScene (EnumSceneType.LoginScene, "LoginScene", typeof(BaseScene), null);
			RegisterScene (EnumSceneType.MainScene, "MainScene", null, null);
			RegisterScene (EnumSceneType.CopyScene, "CopyScene", null, null);

		}

		public void RegisterScene(EnumSceneType _sceneID, string _sceneName, Type _sceneType, params object[] _params){
			if (_sceneType == null || _sceneType.BaseType != typeof(BaseScene)) {
				throw new Exception ("Register scene type cannot be null and extends BaseScene!");
			}			
			if (dicSceneInfo.ContainsKey (_sceneID)) {
				SceneInfoData sceneInfo = new SceneInfoData (_sceneName, _sceneType, _params);
				dicSceneInfo.Add (_sceneID, sceneInfo);
			}
		}

		public void UnRegisterScene(EnumSceneType _sceneID){
			if (dicSceneInfo.ContainsKey (_sceneID)) {
				dicSceneInfo.Remove (_sceneID);
			}
		}

		public bool IsRegisterScene(EnumSceneType _sceneID){
			return  dicSceneInfo.ContainsKey (_sceneID);
		}

		internal BaseScene GetBaseScene(EnumSceneType _sceneType){
			Debug.Log ("GetBaseScene sceneId = " + _sceneType.ToString ());
			SceneInfoData sceneInfo = GetSceneInfo (_sceneType);
			if (sceneInfo == null || sceneInfo.SceneType == null) {
				return null;
			}
			BaseScene scene = System.Activator.CreateInstance (sceneInfo.SceneType) as BaseScene;
			return scene;
		}

		public SceneInfoData GetSceneInfo(EnumSceneType _sceneID){
			if(dicSceneInfo.ContainsKey(_sceneID)){
				return dicSceneInfo [_sceneID];
			}
			Debug.LogError ("This Scene hasn't registen! ID: " + _sceneID.ToString ());
			return null;
		}

		public string GetSceneName(EnumSceneType _sceneID){
			if(dicSceneInfo.ContainsKey(_sceneID)){
				return dicSceneInfo [_sceneID].SceneName;
			}
			Debug.LogError ("This Scene hasn't registen! ID: " + _sceneID.ToString ());
			return null;
		}

		public void ClearScene(){
			dicSceneInfo.Clear ();
		}

		#region Change scene

		public void ChangeSceneDirect(EnumSceneType _sceneType){
			UIManager.Instance.CloseUIAll ();
			if (CurrentScene != null) {
				CurrentScene.Release ();
				CurrentScene = null;
			}

			LastSceneType = ChangeSceneType;
			ChangeSceneType = _sceneType;
			string sceneName = GetSceneName (_sceneType);
			//change scene
			CoroutineController.Instance.StartCoroutine(AsyncLoadScene(sceneName));
		}

		public void ChangeSceneDirect(EnumSceneType _sceneType, EnumUIType _uiType, params object[] _params){
			SceneOpenUIType = _uiType;
			sceneOpenUIParams = _params;
			if (LastSceneType == _sceneType) {
				if (SceneOpenUIType == EnumUIType.None) {
					return;
				}
				UIManager.Instance.OpenUI (SceneOpenUIType, sceneOpenUIParams);
				SceneOpenUIType = EnumUIType.None;
			}
			else {
				ChangeSceneDirect (_sceneType);
			}
		}










		private IEnumerator<AsyncOperation> AsyncLoadScene(string sceneName){
			AsyncOperation oper = Application.LoadLevelAsync (sceneName);
			yield return oper;

			//message send

			if(SceneOpenUIType != EnumUIType.None){
				UIManager.Instance.OpenUI (SceneOpenUIType, sceneOpenUIParams);
				SceneOpenUIType = EnumUIType.None;
			}
		}

		#endregion

		#region Change scene

		public void ChangeScene(EnumSceneType _sceneType){
			UIManager.Instance.CloseUIAll ();
			if (CurrentScene != null) {
				CurrentScene.Release ();
				CurrentScene = null;
			}

			LastSceneType = ChangeSceneType;
			ChangeSceneType = _sceneType;
			string sceneName = GetSceneName (_sceneType);
			//change loading scene
			CoroutineController.Instance.StartCoroutine(AsyncLoadOtherScene());
		}

		public void ChangeScene(EnumSceneType _sceneType, EnumUIType _uiType, params object[] _params){
			SceneOpenUIType = _uiType;
			sceneOpenUIParams = _params;
			if (LastSceneType == _sceneType) {
				if (SceneOpenUIType == EnumUIType.None) {
					return;
				}
				UIManager.Instance.OpenUI (SceneOpenUIType, sceneOpenUIParams);
				SceneOpenUIType = EnumUIType.None;
			}
			else {
				ChangeScene (_sceneType);
			}
		}










		private IEnumerator AsyncLoadOtherScene(){
			string sceneName = GetSceneName (EnumSceneType.LoadingScene);
			AsyncOperation oper = Application.LoadLevelAsync (sceneName);
			yield return oper;

			//message send

			if (oper.isDone) {
//				GameObject go = GameObject.Find ("LoadingScenePanel");
//				LoadingSceneUI loadingSceneUI = go.GetComponent<LoadingSceneUI> ();
//				BaseScene scene = CurrentScene;
//				if (scene != null) {
//					scene.currentSceneId = ChangeSceneId ();
//				}
//				//检查是否注册该场景
//				if(!SceneManager.Instance.IsRegisterScene(ChangeSceneId)){
//					Debug.Log ("没有注册该场景！ " + ChangeSceneId.ToString ());
//				}
//				loadingSceneUI.Load (ChangeSceneId);
//				loadingSceneUI.LoadComplete += SceneLoadCompleted;
			}
		}

		void SceneLoadCompleted(object sender, EventArgs e){
			Debug.Log ("场景切换成功: " + sender as String);
			//场景切换成功
//			MessageCenter.Instance.SendMessage();

//			有需要打开的UI
			if (SceneOpenUIType != EnumUIType.None) {
				UIManager.Instance.OpenUI (SceneOpenUIType, sceneOpenUIParams);
				SceneOpenUIType = EnumUIType.None;
			}
		}

		//加载场景
//		private IEnumerator AsyncLoadedNextScene(){
//			string fileName = SceneManager.Instance.GetSceneName (ChangeSceneId);
//			AsyncOperation oper = Application.LoadLevelAsync (fileName);
//			yield return oper;
//			if (oper.isDone) {
//				if (LoadCompleted != null) {
//					LoadCompleted (ChangeSceneId, null);
//				}
//			}
//		}



		#endregion
	}
}

