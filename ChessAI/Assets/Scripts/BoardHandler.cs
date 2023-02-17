using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

// This script will deal with the contents that make up the entire board
// This includes the tiles and pieces, so all "Controller" scripts will answer and send information to this script for storage and processing of game states
public class BoardHandler : MonoBehaviour
{
    private CPS.ChessBoard _chessBoard;

    private PieceController _pieceHoldingScriptRef;

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void SetBoard(ref CPS.ChessBoard cb)
    {
        _chessBoard = cb;
    }

    // Determines which piece is currently being held in order to move
    // Set to null to clear
    public void SetHold(PieceController heldPieceScriptRef)
    {
        _pieceHoldingScriptRef = heldPieceScriptRef;
        if (heldPieceScriptRef != null)
            Debug.Log("Holding: " + _pieceHoldingScriptRef.GetHeldCoordinateX() + ", " + _pieceHoldingScriptRef.GetHeldCoordinateY());
        else
            Debug.Log("Piece placed");
    }

    public PieceController GetHold()
    {
        return _pieceHoldingScriptRef;
    }

    public void PlacePieceAtCoordinate(int tileCoordX, int tileCoordY)
    {
        int toX = _pieceHoldingScriptRef.GetHeldCoordinateX(), toY = _pieceHoldingScriptRef.GetHeldCoordinateY();
        //  if (toX != tileCoordX && toY != tileCoordY)
        {
            _chessBoard.UpdateBoardMove(toX, toY, tileCoordX, tileCoordY);
        }

        _pieceHoldingScriptRef.GetPiece().Move(tileCoordX, tileCoordY);

        _pieceHoldingScriptRef.SetHold(false);
        SetHold(null);   //clears the holding piece
    }
}