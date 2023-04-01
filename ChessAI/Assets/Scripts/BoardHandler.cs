using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;

// This script handles the contents that make up the entire board, including the tiles and pieces.
// All other "Controller" scripts will communicate with this script for storage and processing of game states.

public class BoardHandler : MonoBehaviour
{
    private CPS.ChessBoard _chessBoard;

    private PieceController _selectedPieceController;
    public List<CPS.Move> PossibleMoves { get; private set; }

    public GameObject[] tileBorderRef;
    public List<GameObject> TileBorderArray { get; private set; }

    public void SetBoard(CPS.ChessBoard chessBoard)
    {
        _chessBoard = chessBoard;
    }


    public void DrawMoveBorders(List<CPS.Move> possibleMoves)
    {
        TileBorderArray = new List<GameObject>();
        int[] pieceCoordinates = GetSelected().GetPieceCoordinates();
        foreach (CPS.Move move in possibleMoves)
        {
            int x = move.GetMoveCoordinates()[0] + pieceCoordinates[0];
            int y = move.GetMoveCoordinates()[1] + pieceCoordinates[1];
            GameObject borderObject = move.GetMoveCoordinates()[0] == 0 && move.GetMoveCoordinates()[1] == 0 ? tileBorderRef[1] : tileBorderRef[0];
            GameObject border = Instantiate(borderObject, new Vector3(x, .05f, y), Quaternion.identity);
            border.transform.Rotate(90f, 0f, 0f);
            TileBorderArray.Add(border);
        }
    }

    // Creates the available moves that a piece can make in relation to its location.
    // i.e [0,1] means it can move one space up

    public void CalculateMoves(PieceController pieceController)
    {
        var piece = pieceController.GetPiece();
        var possibleMoves = piece.GetPossibleMoves(ref _chessBoard);
        DrawMoveBorders(possibleMoves);
        PossibleMoves = possibleMoves;
    }

    public bool CanMoveToTile(TileController tile)
    {
        var tileCoordinates = tile.GetTileCoordinates();
        var heldCoordinates = _selectedPieceController.GetPieceCoordinates();
        foreach (var move in PossibleMoves)
        {
            // If the tile is within the list of moveable spaces, you can move.
            if (tileCoordinates[0] == move.GetMoveCoordinates()[0] + heldCoordinates[0] &&
                tileCoordinates[1] == move.GetMoveCoordinates()[1] + heldCoordinates[1])
            {
                return true;
            }
        }
        return false;
    }

    public void SetSelected(PieceController pieceController)
    {
        // If it is currently not holding a piece and clicks on a piece it will pick up the passed variable.
        if (pieceController != null)
        {
            _selectedPieceController = pieceController;
            Debug.Log($"Selected: {GetHeldCoordinates()[0]}, {GetHeldCoordinates()[1]}");
        }
        else
        // Else it will place down the piece.
        {
            _selectedPieceController.SetPieceSelected(false);
            _selectedPieceController = null;
            Debug.Log("Piece placed");
        }
    }

    public void MoveSelectedTo(int x, int y)
    {
        var heldCoordinates = GetHeldCoordinates();
        var selectedPiece = _selectedPieceController.GetPiece();
        selectedPiece.MoveTo(x, y);
        // The if statement will go through if you don't just place down a piece at it's original positions
        if (heldCoordinates[0] != x || heldCoordinates[1] != y)
        {
            _chessBoard.UpdateBoardMove(heldCoordinates[0], heldCoordinates[1], x, y);
            EndTurn();
        }
    }

    public PieceController GetSelected()
    {
        return _selectedPieceController;
    }

    public int[] GetHeldCoordinates()
    {
        return _selectedPieceController.GetPieceCoordinates();
    }

    public void EndTurn()
    {
        // To be implemented
    }

    public void DeleteMoveBorders()
    {
        foreach (GameObject border in TileBorderArray)
        {
            Destroy(border);
        }
        TileBorderArray.Clear();
    }

    public void PlaceSelectedPieceAtCoordinate(int[] tileCoordinates)
    {
        MoveSelectedTo(tileCoordinates[0], tileCoordinates[1]);
        SetSelected(null);
        PossibleMoves.Clear();
        DeleteMoveBorders();
    }
}
