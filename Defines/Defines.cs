using System;
using UnityEngine;
using XHFrameWork;

namespace XHFrameWork
{
	#region Global delegate
	public delegate void StateChangeEvent(object ui,EnumObjectState newState,EnumObjectState oldState);

	public delegate void MessageEvent(Message message);

	public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);

	public delegate void PropertyChangedHandle(BaseActor actor, int id, object oldValue, object newValue);
	#endregion

	#region 全局枚举对象
	public enum EnumObjectState{
		None,//未知状态
		Initial,//初始化
		Loading,//异步加载
		Ready,//就绪
		Disabled,//隐藏
		Closing,//关闭
	};
	#endregion

	public enum EnumUIType : int{
		None = -1,
		TestOne = 0,
		TestTwo = 1,
	};

	public enum EnumSceneType{
		None = 0,
		StartGame,
		LoadingScene,
		LoginScene,
		MainScene,
		CopyScene,
		PVPScene,
		PVEScene,
	};

	public enum EnumTouchEventType{
		OnClick,
		OnDoubleClick,
		OnDown,
		OnUp,
		OnEnter,
		OnExit,
		OnSelect,
		OnUpdateSelect,
		OnDeSelect,
		OnDrag,
		OnDragEnd,
		OnDrop,
		OnScroll,
		OnMove,
	};

	public enum EnumActorType{
		None = 0,
		Role,
		Monster,
		NPC,
	};

	public enum EnumPropertyType : int{
		RoleName = 1,
		Sex,
		RoleID,
		Gold,
		Coin,
		Level,
		Exp,

		AttackSpeed,
		HP,
		HPMax,
		Attack,
		Water,
		Fire,
	};

	public class UIPathDefines{
		//UI预设。
		public const string UI_PREFAB = "Prefabs/";
		//UI小控件预设。
		public const string UI_CONTROLS_PREFAB = "Prefabs/Control/";
		//UI子页面预设。
		public const string UI_SUBUI_PREFAB = "Prefabs/SubUI/";
		//icon路径
		public const string UI_ICON_PATH = "UI/Icon/";

		public static string GetPrefabsPathByType(EnumUIType _uiType){
			string _path = string.Empty;
			switch (_uiType) {
			case EnumUIType.TestOne:
				_path = UI_PREFAB + "TestOne";
				break;
			case EnumUIType.TestTwo:
				_path = UI_PREFAB + "TestTwo";
				break;
			default:
				Debug.Log ("Not Find EnumUIType! Type: " + _uiType.ToString ());
				break;
			}
			return _path;
		}

		public static System.Type GetUIScriptByType(EnumUIType _uiType){
			System.Type _scriptType = null;
			switch (_uiType) {
			case EnumUIType.TestOne:
				_scriptType = typeof(TestOne);
				break;
			case EnumUIType.TestTwo:
				_scriptType = typeof(TestTwo);
				break;
			default:
				Debug.Log ("Not Find EnumUIType! Type: " + _uiType.ToString ());
				break;
			}
			return _scriptType;
		}
	}

	public class Defines
	{
		public Defines ()
		{
		}
	}
}

