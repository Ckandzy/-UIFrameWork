using UnityEngine;
using System.Collections;

namespace XHFrameWork{
	public abstract class BaseUI : MonoBehaviour
	{
		#region Cache gameObject & transform
		//缓存
		private GameObject _cacheGameObject;
		public GameObject CacheGameObject{
			get{ 
				if (null == _cacheGameObject)
					_cacheGameObject = this.gameObject;
				return _cacheGameObject;
			}
		}
		
		//缓存
		private Transform _cacheTransform;
		public Transform CacheTransform{
			get{ 
				if (_cacheTransform == null)
					_cacheTransform = this.transform;
				return _cacheTransform;
			}
		}
		#endregion

		#region EnumObjectState & UIType
		protected EnumObjectState _state = EnumObjectState.None;

		public event StateChangeEvent StateChanged;

		public EnumObjectState State{
			protected get{ 
				return this._state;
			}
			set{ 
				EnumObjectState oldState = this._state;
					this._state = value;
				if (StateChanged != null) {
					StateChanged (this, this._state, oldState);
				}
			}
		}

		public abstract EnumUIType GetUIType();
		#endregion

		// Use this for initialization
		void Start ()
		{
			OnStart ();
		}

		void Awake(){
			this.State = EnumObjectState.Initial;
			OnAwake ();
		}

		// Update is called once per frame
		void Update ()
		{
			if (this._state == EnumObjectState.Ready) {
				OnUpdate (Time.deltaTime);
			}
		}

		public void Release(){
			this.State = EnumObjectState.Closing;
			GameObject.Destroy (this.CacheGameObject);
			OnRelease ();
		}

		void OnDestory(){
			this.State = EnumObjectState.None;
		}
		protected virtual void OnStart (){
			
		}
		protected virtual void OnAwake (){
			this.State = EnumObjectState.Loading;
			//播放音乐
			this.OnPlayOpenUIAudio();
		}
		protected virtual void OnRelease(){
			this.State = EnumObjectState.None;
			//关闭音乐
			this.OnPlayOpenUIAudio();
		}
		protected virtual void OnUpdate(float deltaTime){
		
		}
		protected virtual void OnPlayOpenUIAudio(){
		
		}
		protected virtual void OnPlayCloseUIAudio(){
		
		}
		protected virtual void SetUI(params object[] uiParams){
			this.State = EnumObjectState.Loading;
		}
		public virtual void SetUIparam(params object[] uiParams){
			
		}
		protected virtual void OnLoadData(){

		}
		public void SetUIWhenOpening(params object[] uiParams){
			SetUI (uiParams);
			CoroutineController.Instance.StartCoroutine (AsyncOnLoadData());
		}
		private IEnumerator AsyncOnLoadData(){
			yield return new WaitForSeconds (0);
			if (this.State == EnumObjectState.Loading) {
				this.OnLoadData ();
				this.State = EnumObjectState.Ready;
			}
		}
	}
}

