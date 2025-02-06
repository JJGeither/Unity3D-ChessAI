using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    private BoardHandler _bhScriptRef;
    private ChessPieceScript.ChessBoard _chessBoard;
    public bool ActiveAI;
    public int DEPTH_MAX = 1;
    private int debugNumber = 0;

    private void Start()
    {
        _bhScriptRef = this.GetComponent<BoardHandler>();
    }

    public void Update()
    {
        // Plays for white pieces
        if (_bhScriptRef.GetTurn() % 2 == 1 && ActiveAI)
        {
            var (bestCoords, bestMove) = FindBestMove(DEPTH_MAX);
            if (bestCoords != null && bestMove != null)
            {
                ChessPieceScript.ChessPiece bestPiece = _chessBoard.GetPiece(bestCoords);
                int[] combinedCoordinates = bestMove.GetCombinedCoordinates(bestPiece);
                _bhScriptRef.PerformSpecialMove(bestPiece, bestMove);
                _bhScriptRef.MovePieceTo(bestPiece, combinedCoordinates);
                _bhScriptRef.PawnPromotion(combinedCoordinates);
            }
        }
    }

    public void SetBoard(ChessPieceScript.ChessBoard chessBoard) => _chessBoard = chessBoard;

    public int MiniMax(int depth, bool maximizingPlayer)
    {
        List<ChessPieceScript.ChessPiece> list;
        if (maximizingPlayer)
            list = _chessBoard.GetColorList()[1];
        else
            list = _chessBoard.GetColorList()[0];

        ChessPieceScript.ChessPiece[] colorList = new ChessPieceScript.ChessPiece[list.Count];
        list.CopyTo(colorList);

        if (depth == 0 || _chessBoard.IsGameOver())
        {
            return EvaluateBoard();
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            for (int i = 0; i < colorList.Length; i++)
            {
                ChessPieceScript.ChessPiece piece = colorList[i];
                List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
                _bhScriptRef.CalculateLegalMovesQ(piece, ref possibleMoves);
                foreach (var possibleMove in possibleMoves.Skip(1))
                {
                    _bhScriptRef.PerformMoveAction(piece, possibleMove);
                    debugNumber++;
                    Debug.Log(debugNumber);
                    int eval = MiniMax(depth - 1, false);
                    maxEval = Mathf.Max(eval, maxEval);
                    _bhScriptRef.RevertMoveQ(ref piece);
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int i = 0; i < colorList.Length; i++)
            {
                ChessPieceScript.ChessPiece piece = colorList[i];
                List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
                _bhScriptRef.CalculateLegalMovesQ(piece, ref possibleMoves);
                foreach (var possibleMove in possibleMoves.Skip(1))
                {
                    //?HERE?
                    _bhScriptRef.PerformMoveAction(piece, possibleMove);
                    debugNumber++;
                    Debug.Log(debugNumber);
                    int eval = MiniMax(depth - 1, true);
                    minEval = Mathf.Min(eval, minEval);
                    _bhScriptRef.RevertMoveQ(ref piece);
                }
            }
            return minEval;
        }
    }

    public (int[], ChessPieceScript.Move) FindBestMove(int depth)
    {
        List<ChessPieceScript.ChessPiece> list = _chessBoard.GetColorList()[1];
        ChessPieceScript.ChessPiece[] colorList = new ChessPieceScript.ChessPiece[list.Count];
        list.CopyTo(colorList);

        int[] bestCoords = null;
        ChessPieceScript.Move bestMove = null;
        int maxEval = int.MinValue;
        for (int i = 0; i < colorList.Length; i++)
        {
            ChessPieceScript.ChessPiece piece = colorList[i];

            List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
            _bhScriptRef.CalculateLegalMovesQ(piece, ref possibleMoves);
            foreach (var possibleMove in possibleMoves.Skip(1))
            {
                int obj = piece.GetHashCode();
                int[] currentCoordinates = piece.GetCoordinates();
                int[] tileCoordinates = possibleMove.GetCombinedCoordinates(piece);

                _bhScriptRef.PerformSpecialMove(piece, possibleMove);
                _bhScriptRef.MovePieceTo(piece, tileCoordinates);
                _bhScriptRef.PawnPromotion(tileCoordinates);
                int obj3 = piece.GetHashCode();
                int eval = MiniMax(depth - 1, false);
                int obj4 = piece.GetHashCode();

                _bhScriptRef.RevertMoveQ(ref piece);

                if (eval > maxEval)
                {
                    bestCoords = piece.GetCoordinates();  //The knight gets removed, and then added back which casues issues with this
                    bestMove = possibleMove;
                    maxEval = eval;
                }
                int obj2 = piece.GetHashCode();
            }
        }
        return (bestCoords, bestMove);
    }

    private int EvaluateBoard()
    {
        int whiteMaterialValue = 0;
        int blackMaterialValue = 0;
        float centerOfBoard = 3; // calculate the center of the board

        foreach (ChessPieceScript.ChessPiece piece in _chessBoard.GetBoard())
        {
            int materialValue = piece.GetMaterialValue();
            float distanceFromCenter = Mathf.Abs(piece.GetCoordinates()[0] - centerOfBoard) + Mathf.Abs(piece.GetCoordinates()[1] - centerOfBoard); // calculate the Manhattan distance from the center of the board
            int score = (materialValue * 6) - Mathf.RoundToInt(distanceFromCenter); // calculate the score for the piece based on its material value and distance from the center

            if (piece.GetPieceColor() == 1)
                whiteMaterialValue += score;
            else if (piece.GetPieceColor() == 0)
                blackMaterialValue += score;
        }

        return whiteMaterialValue - blackMaterialValue;
    }
}