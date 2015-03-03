using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	private LTATile _tileSelected;

	void Start () {

	}
	
	void Update () {
		LTATile tile = null;

		if (Input.GetMouseButtonDown(0)) {
			Vector3 mousePosition = GetMousePositionInWorld();
			Vector3 farMousePosition = mousePosition;
			farMousePosition.z = -10;

			RaycastHit hit;
			if (Physics.Raycast(farMousePosition, mousePosition - farMousePosition, out hit)) {
				tile = hit.collider.gameObject.GetComponent<LTATile>();
				tile.displayText.color = Color.red;
			}
		}

		if (Input.GetKey(KeyCode.E) && tile) {
			tile.ChangeToEmpty();
			return;
		}

		if (tile != null) {
			if (_tileSelected == null) {
				_tileSelected = tile;
			}
			else {
				LTAManager.instance.TryLink(_tileSelected, tile);

				_tileSelected.displayText.color = Color.black;
				tile.displayText.color = Color.black;

				_tileSelected = null;
			}
		}
	}



	public static Vector3 GetMousePositionInWorld () {
		float distanceToCamera = Mathf.Abs(Camera.main.transform.position.z);
		Vector3 mousePosOnScreen = Input.mousePosition;
		Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePosOnScreen.x, mousePosOnScreen.y, distanceToCamera));
		mousePosInWorld.z = 0;
		return mousePosInWorld;
	}
}
