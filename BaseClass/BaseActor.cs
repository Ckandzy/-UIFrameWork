using System;
using XHFrameWork;
using System.Collections.Generic;
using UnityEngine;

namespace XHFrameWork
{
	public class BaseActor : IDynamicProperty
	{
		protected Dictionary<int, PropertyItem> dicProperty = null;

		public event PropertyChangedHandle PropertyChanged;

		public EnumActorType ActorType{ set; get; }

		public int ID { set; get; }

		private BaseScene currentScene;

		public BaseScene CurrentScene{
			set{ 
				//add Change Scene
				CurrentScene = value;
			}
			get{ 
				return currentScene;
			}
		}

		public virtual void AddProperty(EnumPropertyType propertyType, object content){
			AddProperty ((int)propertyType, content);
		}

		public virtual void AddProperty(int id, object content){
			PropertyItem property = new PropertyItem (id, content);
			AddProperty (property);
		}

		public virtual void AddProperty(PropertyItem property){
			if (dicProperty == null) {
				dicProperty = new Dictionary<int, PropertyItem> ();
			}
			if (dicProperty.ContainsKey (property.ID)) {
				//remove the same property
			}
			dicProperty.Add (property.ID, property);
			property.Owner = this;
		}

		public void RemoveProperty(EnumPropertyType propertyType){
			RemoveProperty ((int)propertyType);
		}

		public void RemoveProperty(int id){
			if (dicProperty != null && dicProperty.ContainsKey(id)) {
				dicProperty.Remove (id);
			}
		}

		public void ClearProperty(){
			if (dicProperty != null) {
				dicProperty.Clear ();
				dicProperty = null;
			}
		}

		public virtual PropertyItem GetProperty(EnumPropertyType propertyType){
			return GetProperty ((int)propertyType);
		}

		protected virtual void OnPropertyChanged(int id, object oldValue, object newValue){
			//add update
//			if(id == (int)EnumPropertyType.HP){
//				
//			}
		}

		#region IDynamicProperty implementation

		public void DoChangeProperty (int id, object oldValue, object newValue)
		{
			OnPropertyChanged (id, oldValue, newValue);
			if (PropertyChanged != null)
				PropertyChanged (this, id, oldValue, newValue);
		}

		public PropertyItem GetProperty (int id)
		{
			if (dicProperty == null) {
				return null;
			}

			if (dicProperty.ContainsKey (id))
				return dicProperty [id];
			else {
				Debug.LogWarning ("Can't get the property form the dicProperty by id :" + id);
				return null;
			}
		}

		#endregion

		public BaseActor ()
		{
		}
	}
}

