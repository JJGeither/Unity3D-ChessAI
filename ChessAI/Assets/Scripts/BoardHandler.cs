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

    public List<CPS.Move> possibleCurrentMoves;

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void SetBoard(ref CPS.ChessBoard cb)
    {
        _chessBoard = cb;
    }

    // Creates the available moves that a piece can make in relation to it's location
    public void CalculateMoves(PieceController heldPieceScriptRef)
    {
        CPS.ChessPiece piece = heldPieceScriptRef.GetPiece();
        List<CPS.Move> possibleMoves = piece.GetPossibleMoves(ref _chessBoard);
        int[] debugMoveArray;
        string debugStringForMoves = "";
        foreach (CPS.Move i in possibleMoves)
        {
            debugMoveArray = i.GetMoveCoordinates() ;
            debugStringForMoves += "(" + debugMoveArray[0] + ", " + debugMoveArray[1] + ") ";
        }
        possibleCurrentMoves = possibleMoves;
        Debug.Log("Available Moves: " + debugStringForMoves);

    }

    public void ClearPossibleMoves()
    {
        possibleCurrentMoves.Clear();
    }

    public bool CanMoveToTile(ref TileController tile)
    {
        return true;
        // ***** Continue from here *****
        // You are currently debating whether to hold a list of possible tiles here and having a tile reference this whenever clicked or whether or not you should
        // set a boolean whenever you click on  a tile
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

    // !! Might need to fix this at some point to remove all the dang move functions !!
    public void PlacePieceAtCoordinate(int tileCoordX, int tileCoordY)
    {
        int toX = _pieceHoldingScriptRef.GetHeldCoordinateX(), toY = _pieceHoldingScriptRef.GetHeldCoordinateY();
        if (toX != tileCoordX || toY != tileCoordY)
        {
            _chessBoard.UpdateBoardMove(toX, toY, tileCoordX, tileCoordY);
        }

        _pieceHoldingScriptRef.GetPiece().MoveTo(tileCoordX, tileCoordY);

        _pieceHoldingScriptRef.SetPieceHold(false);
        SetHold(null);   //clears the holding piece
        ClearPossibleMoves();
    }
}