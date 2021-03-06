﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace XHFrameWork
{
	public class Message : IEnumerable<KeyValuePair<string,object>>
	{
		private Dictionary<string, object> dicDatas = null;

		public string Name { get; private set; }
		public object Sender { get; private set; }
		public object Content { get; set; }

		#region message[key] = value || data = message[key]

		public object this [string key] {
			get{ 
				if (dicDatas == null || !dicDatas.ContainsKey (key))
					return null;
				return dicDatas [key];
			}
			set{ 
				if (dicDatas == null)
					dicDatas = new Dictionary<string, object> ();
				if (dicDatas.ContainsKey (key))
					dicDatas [key] = value;
				else
					dicDatas.Add (key, value);
			}
		}
		#endregion

		#region IEnumerable implementation
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
			if (dicDatas == null)
				yield break;
			foreach (KeyValuePair<string, object> kvp in dicDatas) {
				yield return kvp;
			}
		}
		#endregion

		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator(){
			return dicDatas.GetEnumerator ();
		}
		#endregion

		#region Message Construction Function
		public Message (string name, object sender){
			Name = name;
			Sender = sender;
			Content = null;
		}

		public Message (string name, object sender, object content){
			Name = name;
			Sender = sender;
			Content = content;
		}

		public Message (string name, object sender, object content, params object[] _dicParams){
			Name = name;
			Sender = sender;
			Content = content;
			if (_dicParams.GetType () == typeof(Dictionary<string,object>)) {
				foreach(object _dicParam in _dicParams) {
					foreach (KeyValuePair<string, object> kvp in _dicParam as Dictionary<string, object>) {
//						dicDatas [kvp.Key] = kvp.Value;
						this[kvp.Key] = kvp.Value;
					}
				}
			}
		}

		public Message (Message message){
			Name = message.Name;
			Sender = message.Sender;
			Content = message.Content;
			foreach (KeyValuePair<string, object> kvp in message.dicDatas) {
				this [kvp.Key] = kvp.Value;  
			}
		}
		#endregion

		#region Add & Remove
		public void Add(string key, object value){
			this [key] = value;
		}
		public void Remove(string key){
			if(dicDatas != null && dicDatas.ContainsKey(key)){
				dicDatas.Remove (key);
			}
		}
		#endregion

		#region Send()
		public void Send(){
			//MesssageCenter Send Message
			MessageCenter.Instance.SendMessage(this);
		}

		#endregion
	}
}

