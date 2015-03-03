using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LTATile : MonoBehaviour {

	public int row;
	public int column;

	private int _pairingId;
	public int paringId {
		get {
			return _pairingId;
		}
		set {
			_pairingId = value;
			displayText.text = "" + value;
		}
	}


	private static LTATile _empty;
	public static LTATile empty {
		get {
			if (_empty == null) {
				_empty = new LTATile();
				_empty.SetRowAndColumn(-1, -1);
			}
			return _empty;
		}
	}

	public bool isEmpty {get{return row == -1;}}

	public Text displayText;



	// Use this for initialization
	void Awake () {
		_pairingId = -1;
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void SetRowAndColumn (int row, int column) {
		this.row = row;
		this.column = column;
	}

	public static bool CheckIsEmpty (LTATile tile) {
		return tile.row == -1;
	}

	public void ChangeToEmpty () {
		SetRowAndColumn(-1, -1);
		displayText.enabled = false;
	}
}
