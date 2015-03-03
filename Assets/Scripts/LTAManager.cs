using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Handles all the important logics
/// </summary>
public class LTAManager : MonoBehaviour {

	// Settings of the game
	public int rowNumber;
	public int columnNumber;
	public Vector2 tileSize;
	public int pairNumber;	// Number of tokens in total
	
	public LTATile tilePrefab;


	private LTATile[, ] _tiles;

	// Singleton
	private static LTAManager _instance;
	public static LTAManager instance {get {return _instance;}}


	// Initialation method in Unity3d
	void Awake () {
		_instance = this;

		pairNumber = Mathf.Max(pairNumber, 1);
		pairNumber = Mathf.Min((int)(rowNumber * columnNumber / 2.0f), pairNumber);

		_tiles = new LTATile[rowNumber, columnNumber];

		Transform canvasTrans = FindObjectOfType<Canvas>().transform;

		// Create all the tiles
		for (int r=0; r<rowNumber; r++) {
			for (int c=0; c<columnNumber; c++) {
				Vector3 position = new Vector3(tileSize.y * c, tileSize.x * r, 0);
				LTATile tile = (LTATile) Instantiate(tilePrefab, position, Quaternion.identity);
				tile.name = "Tile r:" + r + " c:" + c;
				tile.SetRowAndColumn(r, c);

				tile.transform.SetParent(canvasTrans);
				tile.transform.localScale = new Vector3(1, 1, 1);

				_tiles[r, c] = tile;
			}
		}

		Shuffle();
	}

	// Shuffle the tiles
	public void Shuffle () {
		for (int r=0; r<rowNumber; r++) {
			for (int c=0; c<columnNumber; c++) {
				LTATile tile = _tiles[r, c];

				if (!tile.isEmpty && tile.paringId == -1) {
					int pairId = Random.Range(0, pairNumber - 1);

					tile.paringId = pairId;

					LTATile paringTile = GetRandomNotEmptyNoPairTile();
					paringTile.paringId = pairId;
				}
			}
		}
	}

	public LTATile GetRandomNotEmptyNoPairTile () {
		List<LTATile> notEmptyNoPairTiles = new List<LTATile>();

		for (int r=0; r<rowNumber; r++) {
			for (int c=0; c<columnNumber; c++) {
				LTATile tile = _tiles[r, c];
				
				if (!tile.isEmpty && tile.paringId == -1) {
					notEmptyNoPairTiles.Add(tile);
				}
			}
		}

		return notEmptyNoPairTiles[Random.Range(0, notEmptyNoPairTiles.Count-1)];
	}



	// --- Check Linkage ---

	/// <summary>
	/// Checks any valid linkage exists.
	/// </summary>
	/// <returns><c>true</c>, if any valid linkage exists, <c>false</c> otherwise.</returns>
	/// <remarks>Not tested. But should work.</remarks>
	public bool CheckValidLinkExists () {
		List<LTATile> visitedTiles = new List<LTATile>();

		for (int r=0; r<rowNumber; r++) {
			for (int c=0; c<columnNumber; c++) {
				LTATile tile = _tiles[r, c];

				if (tile.isEmpty) continue;

				for (int r2=0; r2<rowNumber; r2++) {
					for (int c2=0; c2<columnNumber; c2++) {
						LTATile tile2 = _tiles[r2, c2];

						if (tile2.isEmpty) continue;

						if (visitedTiles.Contains(tile2)) continue;

						if (CheckCanLink(tile, tile2)) {
							return true;
						}
					}
				}

				visitedTiles.Add(tile);
			}
		}

		return false;
	}

	/// <summary>
	/// Try if this particular linkage is valid or not.
	/// If valid, both tiles will disappear.
	/// </summary>
	/// <returns><c>true</c>, if the linkage was valid, <c>false</c> otherwise.</returns>
	/// <param name="tileA">Tile a.</param>
	/// <param name="tileB">Tile b.</param>
	public bool TryLink (LTATile tileA, LTATile tileB) {
		if (CheckCanLink(tileA, tileB)) {
			tileA.ChangeToEmpty();
			tileB.ChangeToEmpty();
			return true;
		}
		return false;
	}

	private bool CheckSameTile (LTATile tileA, LTATile tileB) {
		return tileA.row == tileB.row && tileA.column == tileB.column;
	}

	private bool CheckCanLink (LTATile tileA, LTATile tileB) {
		bool result = !CheckSameTile(tileA, tileB) && CheckIsPair(tileA, tileB);
		result = result && (CheckCanLinkWithTurn0(tileA, tileB) || CheckCanLinkWithTurn1(tileA, tileB) || CheckCanLinkWithTurn2(tileA, tileB));
		return result;
	}

	private bool CheckIsPair (LTATile tileA, LTATile tileB) {
		return tileA.paringId == tileB.paringId;
	}

	private bool CheckTileEmpty (int row, int column) {
		bool result = true;
		if (row >= 0 && row < rowNumber && column >= 0 && column < columnNumber) {
			result = LTATile.CheckIsEmpty(_tiles[row, column]);
		}

		return result;
	}

	private bool CheckCanLinkWithTurn0 (LTATile tileA, LTATile tileB) {
		bool result = true;

		if (tileA.row == tileB.row) {
			int min = Mathf.Min(tileA.column, tileB.column);
			int max = Mathf.Max(tileA.column, tileB.column);

			for (int c=min+1; c<max; c++) {
				if (!CheckTileEmpty(tileA.row, c)) {
					result = false;
					break;
				}
			}
		}
		else if (tileA.column == tileB.column) {
			int min = Mathf.Min(tileA.row, tileB.row);
			int max = Mathf.Max(tileA.row, tileB.row);
			
			for (int r=min+1; r<max; r++) {
				if (!CheckTileEmpty(r, tileA.column)) {
					result = false;
					break;
				}
			}
		}
		else {
			result = false;
		}

		return result;
	}

	private bool CheckCanLinkWithTurn1 (LTATile tileA, LTATile tileB) {
		LTATile tileARowBColumn = gameObject.AddComponent<LTATile>();
		tileARowBColumn.SetRowAndColumn(tileA.row, tileB.column);
		if (CheckTileEmpty(tileARowBColumn.row, tileARowBColumn.column) && CheckCanLinkWithTurn0(tileARowBColumn, tileA) && CheckCanLinkWithTurn0(tileARowBColumn, tileB)) {
			Destroy(tileARowBColumn);
			return true;
		}

		LTATile tileAColumnBRow = gameObject.AddComponent<LTATile>();
		tileAColumnBRow.SetRowAndColumn(tileB.row, tileA.column);
		if (CheckTileEmpty(tileAColumnBRow.row, tileAColumnBRow.column) && CheckCanLinkWithTurn0(tileAColumnBRow, tileA) && CheckCanLinkWithTurn0(tileAColumnBRow, tileB)) {
			Destroy(tileAColumnBRow);
			return true;
		}

		Destroy(tileARowBColumn);
		Destroy(tileAColumnBRow);

		return false;
	}

	private bool CheckCanLinkWithTurn2 (LTATile tileA, LTATile tileB) {
		bool result = false;

		List<LTATile> tilesToDestroy = new List<LTATile>();
	
		// Direction row max
		for (int r=tileA.row+1; r<=rowNumber; r++) {
			LTATile tile = gameObject.AddComponent<LTATile>();
			tile.SetRowAndColumn(r, tileA.column);
			tilesToDestroy.Add(tile);

			if (!CheckTileEmpty(tile.row, tile.column)) break;

			if (CheckCanLinkWithTurn0(tile, tileA) && CheckCanLinkWithTurn1(tile, tileB)) {
				result = true;
			}
		}

		if (!result) {
			for (int r=tileB.row+1; r<=rowNumber; r++) {
				LTATile tile = gameObject.AddComponent<LTATile>();
				tile.SetRowAndColumn(r, tileB.column);
				tilesToDestroy.Add(tile);
				
				if (!CheckTileEmpty(tile.row, tile.column)) break;
				
				if (CheckCanLinkWithTurn0(tile, tileB) && CheckCanLinkWithTurn1(tile, tileA)) {
					result = true;
				}
			}
		}

		// Direction row min
		if (!result) {
			for (int r= tileA.row-1; r>=-1; r--) {
				LTATile tile = gameObject.AddComponent<LTATile>();
				tile.SetRowAndColumn(r, tileA.column);
				tilesToDestroy.Add(tile);

				if (!CheckTileEmpty(tile.row, tile.column)) break;

				if (CheckCanLinkWithTurn0(tile, tileA) && CheckCanLinkWithTurn1(tile, tileB)) {
					result = true;
				}
			}

			if (!result) {
				for (int r= tileB.row-1; r>=-1; r--) {
					LTATile tile = gameObject.AddComponent<LTATile>();
					tile.SetRowAndColumn(r, tileB.column);
					tilesToDestroy.Add(tile);
					
					if (!CheckTileEmpty(tile.row, tile.column)) break;
					
					if (CheckCanLinkWithTurn0(tile, tileB) && CheckCanLinkWithTurn1(tile, tileA)) {
						result = true;
					}
				}
			}
		}

		// Direction column max
		if (!result) {
			for (int c=tileA.column+1; c<=columnNumber; c++) {
				LTATile tile = gameObject.AddComponent<LTATile>();
				tile.SetRowAndColumn(tileA.row, c);
				tilesToDestroy.Add(tile);

				if (!CheckTileEmpty(tile.row, tile.column)) break;

				if (CheckCanLinkWithTurn0(tile, tileA) && CheckCanLinkWithTurn1(tile, tileB)) {
					result = true;
				}
			}

			if (!result) {
				for (int c=tileB.column+1; c<=columnNumber; c++) {
					LTATile tile = gameObject.AddComponent<LTATile>();
					tile.SetRowAndColumn(tileB.row, c);
					tilesToDestroy.Add(tile);
					
					if (!CheckTileEmpty(tile.row, tile.column)) break;
					
					if (CheckCanLinkWithTurn0(tile, tileB) && CheckCanLinkWithTurn1(tile, tileA)) {
						result = true;
					}
				}
			}
		}

		// Direction column min
		if (!result) {
			for (int c=tileA.column-1; c>=-1; c--) {
				LTATile tile = gameObject.AddComponent<LTATile>();
				tile.SetRowAndColumn(tileA.row, c);
				tilesToDestroy.Add(tile);

				if (!CheckTileEmpty(tile.row, tile.column)) break;
				
				if (CheckCanLinkWithTurn0(tile, tileA) && CheckCanLinkWithTurn1(tile, tileB)) {
					result = true;
				}
			}

			if (!result) {
				for (int c=tileB.column-1; c>=-1; c--) {
					LTATile tile = gameObject.AddComponent<LTATile>();
					tile.SetRowAndColumn(tileB.row, c);
					tilesToDestroy.Add(tile);
					
					if (!CheckTileEmpty(tile.row, tile.column)) break;
					
					if (CheckCanLinkWithTurn0(tile, tileB) && CheckCanLinkWithTurn1(tile, tileA)) {
						result = true;
					}
				}
			}
		}

		foreach (LTATile tile in tilesToDestroy) {
			Destroy(tile);
		}

		return result;
	}

}
