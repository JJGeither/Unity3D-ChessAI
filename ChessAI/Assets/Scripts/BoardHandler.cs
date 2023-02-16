using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

// This script will deal with the contents that make up the entire board
// This includes the tiles and pieces, so all "Controller" scripts will answer and send information to this script for storage and processing of game states
public class BoardHandler : MonoBehaviour
{
    private CPS.ChessBoard _chessBoard;

    private CPS.ChessPiece _pieceHolding;

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
    public void SetHold(CPS.ChessPiece piece)
    {
        if (piece != null)
        {
            Debug.Log("Holding: " + piece.X + ", " + piece.Y);
        }
        else
            Debug.Log("Piece placed");

        _pieceHolding = piece;
    }

    public CPS.ChessPiece GetHold()
    {
        return _pieceHolding;
    }

    public void PlacePieceAtCoordinate(int tileCoordX, int tileCoordY)
    {
        Debug.Log("Placed piece at coordinate" + _pieceHolding.X + _pieceHolding.Y + " to coordinate" + tileCoordX + ", " + tileCoordY);
        _pieceHolding.Move(tileCoordX, tileCoordY);
        SetHold(null);   //clears the holding piece
    }
}