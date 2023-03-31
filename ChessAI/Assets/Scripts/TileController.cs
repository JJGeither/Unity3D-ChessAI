using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script deals with the tiles
// Whenever an interaction with a tile happens, such as selecting moves, this script will handle it and pass the information to the BoardHandler script
public class TileController : MonoBehaviour
{
    private BoardHandler bhScriptRef;
    private int _tileCoordX, _tileCoordY;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject obj_gameboard = GameObject.Find("Obj_GameBoard");
        bhScriptRef = obj_gameboard.GetComponent<BoardHandler>();
    }
    public void SetTileCoordinate(int xCoord, int yCoord)
    {
        _tileCoordX = xCoord;
        _tileCoordY = yCoord;
    }

    private void OnMouseDown()
    {
        // If the board handler is actually holding a piece
        if (bhScriptRef.GetHold() != null)
        {
            bhScriptRef.PlacePieceAtCoordinate(_tileCoordX, _tileCoordY);
        }
    }
}