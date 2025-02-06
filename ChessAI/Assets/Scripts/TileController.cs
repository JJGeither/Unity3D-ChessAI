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
        PieceController heldPiece = bhScriptRef.GetSelected();
        ChessPieceScript.Move move = bhScriptRef.GetTileMove(this);
        if (heldPiece != null && move != null)
        {
            ChessPieceScript.ChessPiece selectedPiece = bhScriptRef.GetSelectedPiece();
            if (bhScriptRef.CanMoveToTile(this, move))
            {
                bhScriptRef.PerformSpecialMove(selectedPiece, move);
                bhScriptRef.PlaceSelectedPieceAtCoordinate(GetTileCoordinates());
                bhScriptRef.PawnPromotion(GetTileCoordinates());

                int checkColor = bhScriptRef.GetTurn() % 2;
                if (bhScriptRef.IsColorInCheck(checkColor))
                {
                    bool checkmate = bhScriptRef.IsColorInCheckmate(checkColor);
                    Debug.Log("Is in checkmate?: " + checkmate);
                }
            }
        }
        else
        {
            Debug.Log("Cannot move to tile (" + _tileCoordX + ',' + _tileCoordY + ")");
        }
    }
}