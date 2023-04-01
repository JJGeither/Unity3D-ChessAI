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

    }

    public void ClearPossibleMoves()
    {
        possibleCurrentMoves.Clear();
    }

    public bool CanMoveToTile(TileController tile)
    {
        int[] tileCoordinates = tile.GetTileCoordinates();

        int[] holdMovement = GetHeldCoordinates();
        foreach (CPS.Move move in possibleCurrentMoves)
        {
            // If the tile is within the list of moveable spaces, you can move
            
            if (tileCoordinates[0] == move.GetMoveCoordinates()[0] + holdMovement[0] && tileCoordinates[1] == move.GetMoveCoordinates()[1] + holdMovement[1])
            {
                return true;
            }
        }
        return false;
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
            Debug.Log("Holding: " + GetHeldCoordinates()[0] + ", " + GetHeldCoordinates()[1]);

        else
            Debug.Log("Piece placed");
    }

    public PieceController GetHold()
    {
        return _pieceHoldingScriptRef;
    }

    public int[] GetHeldCoordinates()
    {
        return _pieceHoldingScriptRef.GetPieceCoordinates();
    }

    public void EndTurn()
    {
        // Write to switch turns
    }

    // !! Might need to fix this at some point to remove all the dang move functions !!
    public void PlaceHeldPieceAtCoordinate(int[] tileCoordiantes)
    {
        int[] holdMovement = GetHeldCoordinates();
        int toX = holdMovement[0], toY = holdMovement[1];
        if (toX != tileCoordiantes[0] || toY != tileCoordiantes[1])
        {
            _chessBoard.UpdateBoardMove(toX, toY, tileCoordiantes[0], tileCoordiantes[1]);
        }

        // If you didn't just cancel a move by placing it on residing tile it will end the turn
        if (GetHeldCoordinates() != tileCoordiantes)
            EndTurn();

        _pieceHoldingScriptRef.GetPiece().MoveTo(tileCoordiantes[0], tileCoordiantes[1]);

        _pieceHoldingScriptRef.SetPieceHold(false);
        SetHold(null);   //clears the holding piece
        ClearPossibleMoves();



    }
}