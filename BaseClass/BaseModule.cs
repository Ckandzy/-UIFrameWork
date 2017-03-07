using System;

namespace XHFrameWork
{
	public class BaseModule
	{
		public enum EnumRegisterMode{
			NotRegister,
			AutoRegister,
			AlreadyRegister
		}

		private EnumObjectState state = EnumObjectState.Initial;

		public EnumObjectState State {
			get{ 
				return state;	
			}
			set{
				if (state == value)
					return;
				EnumObjectState oldState = state;
				state = value;

				if (StateChanged != null) {
					StateChanged (this, state, oldState);
				}
				OnStateChanged (state, oldState);
			}
		}

		public event StateChangeEvent StateChanged;

		protected virtual void OnStateChanged(EnumObjectState newState, EnumObjectState oldState){
			
		}

		private EnumRegisterMode registerMode = EnumRegisterMode.NotRegister;

		public bool HasRegistered{
			get{ 
				return registerMode == EnumRegisterMode.AlreadyRegister;
			}
		}

		public bool AutoRegister{
			get{
				return registerMode == EnumRegisterMode.AutoRegister ? false : true;
			}
			set{ 
				if (registerMode == EnumRegisterMode.NotRegister || registerMode == EnumRegisterMode.AutoRegister)
					registerMode = value ? EnumRegisterMode.AutoRegister : EnumRegisterMode.NotRegister;
					
			}
		}

		public void Load(){
			if (state != EnumObjectState.Initial)
				return;
			State = EnumObjectState.Loading;

			//...
			if(registerMode == EnumRegisterMode.AutoRegister){
				//register
				ModuleManager.Instance.Register(this);
				registerMode = EnumRegisterMode.AlreadyRegister;
			}

			OnLoad ();
			State = EnumObjectState.Ready;
		}

		protected virtual void OnLoad(){
			
		}

		public void Release(){
			if (State != EnumObjectState.Disabled) {
				State = EnumObjectState.Disabled;
				//...
				if(registerMode == EnumRegisterMode.AlreadyRegister){
					//unregister
					ModuleManager.Instance.UnRegister(this);
					registerMode = EnumRegisterMode.AutoRegister;
				}
			}
			OnRelease ();
		}

		public virtual void OnRelease(){
			
		}
	}
}

