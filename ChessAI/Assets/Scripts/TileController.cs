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

    public int[] GetTileCoordinates()
    {
        return new int[] { _tileCoordX, _tileCoordY };
    }

    private void OnMouseDown()
    {
        // If the board handler is actually holding a piece
        PieceController heldPiece = bhScriptRef.GetHold();
        if (heldPiece != null && bhScriptRef.CanMoveToTile(this))
        {
            bhScriptRef.PlaceHeldPieceAtCoordinate(GetTileCoordinates());
                
            
        } else
        {
            Debug.Log("Cannot move to tile (" + _tileCoordX + ',' + _tileCoordY + ")");
        }
    }
}