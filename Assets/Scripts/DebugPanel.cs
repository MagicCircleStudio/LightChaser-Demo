using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct watchingObject {
	public object obj;
	public string label;
	public watchingObject(object _obj, string _label) {
		this.obj = _obj;
		this.label = _label;
	}
};

public class DebugPanel : MonoBehaviour {
	public static DebugPanel instance;

	[Tooltip("Press F1 to active/disactive debug panel")]
	public bool showDebugPanel = false;
	public GameObject panel;
	public Text debugText;

	List<watchingObject> watchingList = new List<watchingObject>();

	public static void Watch(ref object obj, string label) {
		foreach (var item in instance.watchingList) {
			if(item.label == label){
				return;
			}
		}
		instance.watchingList.Add(new watchingObject(obj, label));
	}

	public static void Unwatch(object obj) {
		var list = instance.watchingList;
		for(int i=list.Count; i>0; i--) {
			if (list[i].obj == obj)
				list.RemoveAt(i);
		}
	}

	public static void Unwatch(string label) {
		var list = instance.watchingList;
		for(int i=list.Count; i>0; i--) {
			if (list[i].label == label) {
				list.RemoveAt(i);
				return;
			}	
		}
	}

	string Convert() {
		var str = "";
		foreach (var item in watchingList) {
			str += item.label + ": " + item.obj.ToString() + "\n";
		}

		return str;
	}

	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			instance = this;

		debugText.text = "";
		panel.SetActive(showDebugPanel);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.F1)) {
			if (showDebugPanel)
				Debug.Log("Close Debug Panel");
			else
				Debug.Log("Open Debug Panel");
			showDebugPanel = !showDebugPanel;
			panel.SetActive(showDebugPanel);
		}

		debugText.text = Convert();
	}
}