using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script handles the contents that make up the entire board, including the tiles and pieces.
// All other "Controller" scripts will communicate with this script for storage and processing of game states.

public class BoardHandler : MonoBehaviour
{
    [SerializeField] private Transform pieceParentObj;
    [SerializeField] private GameObject[] tileBorderRef;
    private int _turn = 1;
    private ChessPieceScript.ChessBoard _chessBoard;
    private MoveHistory _moveHistoryScriptRef;
    private MoveHistory.MoveHistoryStack _moveHistoryStackObject;
    private PieceController _selectedPieceController;
    public List<GameObject> TileBorderArray { get; private set; }
    public List<ChessPieceScript.Move> PossibleMoves { get; private set; }
    public bool NoTurnOrder { get; set; } = false;

    int debugNumber = 0;

    public void Start()
    {
        _moveHistoryScriptRef = this.GetComponent<MoveHistory>();
        _moveHistoryStackObject = _moveHistoryScriptRef.GetStackObject();

        _turn = 1;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !GetSelected())
        {
            RevertMove();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SetStack(MoveHistory.MoveHistoryStack stack) => _moveHistoryStackObject = stack;

    public MoveHistory.MoveHistoryStack GetStack() => _moveHistoryStackObject;

    public void SetBoard(ChessPieceScript.ChessBoard chessBoard) => _chessBoard = chessBoard;

    public ChessPieceScript.ChessBoard GetBoard() => _chessBoard;

    public bool IsNoTurnOrder() => NoTurnOrder;

    public void IncrementTurn() => _turn++;

    public int GetTurn() => _turn;

    public void SetSelected(PieceController pieceController)
    {
        // If it is currently not holding a piece and clicks on a piece it will pick up the passed variable.
        if (pieceController != null)
        {
            _selectedPieceController = pieceController;
            //Debug.Log($"Selected: {GetHeldCoordinates()[0]}, {GetHeldCoordinates()[1]}");
        }
        else
        // Else it will place down the piece.
        {
            _selectedPieceController.SetPieceSelected(false);
            _selectedPieceController = null;
            //Debug.Log("Piece placed");
        }
    }

    public PieceController GetSelected() => _selectedPieceController;

    public ChessPieceScript.ChessPiece GetSelectedPiece() => _selectedPieceController.GetPiece();

    public int[] GetHeldCoordinates() => _selectedPieceController.GetPieceCoordinates();

    public int GetSelectedColor() => _selectedPieceController.GetPiece().GetPieceColor();

    public void DrawMoveBorders(List<ChessPieceScript.Move> possibleMoves)
    {
        TileBorderArray = new List<GameObject>();
        var pieceCoordinates = _selectedPieceController.GetPieceCoordinates();
        foreach (var move in possibleMoves)
        {
            var x = move.GetMoveCoordinates()[0] + pieceCoordinates[0];
            var y = move.GetMoveCoordinates()[1] + pieceCoordinates[1];
            var borderObject = move.GetMoveCoordinates()[0] == 0 && move.GetMoveCoordinates()[1] == 0 ? tileBorderRef[1] : tileBorderRef[0];
            var border = Instantiate(borderObject, new Vector3(x, .05f, y), Quaternion.identity);
            border.transform.Rotate(90f, 0f, 0f);
            TileBorderArray.Add(border);
        }
    }

    // Creates the available moves that a piece can make in relation to its location.
    // i.e [0,1] means it can move one space up

    public void CalculateLegalMoves(ChessPieceScript.ChessPiece piece)
    {
        List<ChessPieceScript.Move> possibleMoves;
        if (piece.GetName() == "pawn")
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard, _turn);
        else
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard);

        PossibleMoves = TrimMoves(piece, possibleMoves);
    }
    
    public void CalculateLegalMovesQ(ChessPieceScript.ChessPiece piece, ref List<ChessPieceScript.Move> possibleMoves)
    {
        //List<ChessPieceScript.Move> possibleMoves;
        if (piece.GetName() == "pawn")
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard, _turn);
        else
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard);

        //Debug.Log(++debugNumber);
        possibleMoves = TrimMoves(piece, possibleMoves);
    }

    public void CalculateMoves(ChessPieceScript.ChessPiece piece)
    {
        List<ChessPieceScript.Move> possibleMoves;
        if (piece.GetName() == "pawn")
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard, _turn);
        else
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard);

        PossibleMoves = possibleMoves;
    }

    public void CalculateMovesQ(ChessPieceScript.ChessPiece piece, ref List<ChessPieceScript.Move> possibleMoves)
    {
        if (piece.GetName() == "pawn")
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard, _turn);
        else
            possibleMoves = piece.GetPossibleMoves(ref _chessBoard);
    }

    // TODO: This is gross, refactor it
    public List<ChessPieceScript.Move> TrimMoves(ChessPieceScript.ChessPiece test, List<ChessPieceScript.Move> possibleMoves)
    {
        //Debug.Log("Object ID: " + objectID);
        List<ChessPieceScript.Move> possibleMoves2 = new List<ChessPieceScript.Move>();
        possibleMoves2.Add(possibleMoves[0]);
        foreach (var move in possibleMoves.Skip(1))
        {
            int[] currentCoordinates = test.GetCoordinates();
            int[] tileCoordinates = move.GetCombinedCoordinates(test);

            // make the move on the copied board
            PerformSpecialMove(test, move);
            MovePieceTo(test, tileCoordinates);
            PawnPromotion(tileCoordinates);

            if (!IsColorInCheck(test.GetPieceColor()))
            {
                possibleMoves2.Add(move);
            }

            RevertMove();
            test.SetCoordinates(currentCoordinates);
        }
        test.GetGameObject().GetComponent<PieceController>().SetPiece(test);

        return possibleMoves2;
    }

    // This returns an array of the pieces in between a rook trying to castle with a king
    public ChessPieceScript.ChessPiece[] GetCastlingGapPieces(ChessPieceScript.ChessPiece piece)
    {
        int[] coords = piece.GetCoordinates();
        if (coords[0] == 0)
        {
            return new ChessPieceScript.ChessPiece[] { _chessBoard.GetPiece(coords[0] + 1, coords[1]), _chessBoard.GetPiece(coords[0] + 2, coords[1]) };
        }
        else if (coords[0] == 7)
        {
            return new ChessPieceScript.ChessPiece[] { _chessBoard.GetPiece(coords[0] - 1, coords[1]), _chessBoard.GetPiece(coords[0] - 2, coords[1]), _chessBoard.GetPiece(coords[0] - 3, coords[1]) };
        }
        return new ChessPieceScript.ChessPiece[] { };
    }



    public bool CanMoveToTile(TileController tile, ChessPieceScript.Move move)
    {
        var tileCoordinates = tile.GetTileCoordinates();
        var heldCoordinates = _selectedPieceController.GetPieceCoordinates();

        if (tileCoordinates[0] == move.GetMoveCoordinates()[0] + heldCoordinates[0] &&
            tileCoordinates[1] == move.GetMoveCoordinates()[1] + heldCoordinates[1])
        {
            return true;
        }
        return false;
    }

    // Returns the move relating to a specific tile
    public ChessPieceScript.Move GetTileMove(TileController tile)
    {
        var tileCoordinates = tile.GetTileCoordinates();
        int[] heldCoordinates;
        if (_selectedPieceController != null)
        {
            heldCoordinates = _selectedPieceController.GetPieceCoordinates();
            foreach (var move in PossibleMoves)
            {
                if (tileCoordinates[0] == move.GetMoveCoordinates()[0] + heldCoordinates[0] &&
                    tileCoordinates[1] == move.GetMoveCoordinates()[1] + heldCoordinates[1])
                {
                    return move;
                }
            }
        }
        return null;
    }

    public void SetPawnHasMovedForwardTwo(ref ChessPieceScript.ChessPiece selectedPiece, int y)
    {
        if (selectedPiece.GetName() == "pawn" && Mathf.Abs(selectedPiece.GetCoordinates()[1] - y) == 2)
        {
            // Debug.Log("En Passant");
            selectedPiece.SetTurnMovedForwardTwo(GetTurn());
        }
    }

    public void EndTurn()
    {
        IncrementTurn();
        //Debug.Log("Turn: " + GetTurn().ToString());
    }

    public void DeleteMoveBorders()
    {
        foreach (GameObject border in TileBorderArray)
        {
            Destroy(border);
        }
        TileBorderArray.Clear();
    }

    public void PawnPromotion(int[] tileCoordinates)
    {
        ChessPieceScript.ChessPiece piece = _chessBoard.GetPiece(tileCoordinates[0], tileCoordinates[1]);

        if (piece.GetName() != "pawn")
            return;

        int pieceColor = -1;
        if (piece.GetName() != "empty")
            pieceColor = piece.GetPieceColor();

        // Pawn Promotion
        if ((pieceColor == 1 && tileCoordinates[1] == 0) || (pieceColor == 0 && tileCoordinates[1] == 7)) // Pawn is at the end of the board
        {
            //Debug.Log("---Pawn Promotion---");
            ChessPieceScript.ChessPiece queen = new ChessPieceScript.Queen(tileCoordinates[0], tileCoordinates[1], pieceColor);
            SetupPiece(queen, tileCoordinates);

            _chessBoard.MoveBoardPiece(queen, tileCoordinates[0], tileCoordinates[1]);
            _chessBoard.ReplacePieceInList(pieceColor, queen, piece);
        }
    }

    public void SetupPiece(ChessPieceScript.ChessPiece piece, int[] placementCoordinate)
    {
        if (piece.GetGameObject() != null)
        {
            GameObject myNewPiece = Instantiate(piece.GetGameObject(), new Vector3(placementCoordinate[0], .05f, placementCoordinate[1]), Quaternion.identity);
            piece.SetGameObject(ref myNewPiece);
            myNewPiece.transform.rotation = Quaternion.Euler(-90, 0, 0);
            myNewPiece.transform.localScale = new Vector3(1500, 1500, 1500);
            myNewPiece.transform.parent = pieceParentObj;

            // Add the BoxCollider component to myNewPiece
            BoxCollider boxCollider = myNewPiece.AddComponent<BoxCollider>();

            // Set the size of the BoxCollider to (0.0006, 0.0006, current_z_size)
            boxCollider.size = new Vector3(0.0006f, 0.0006f, 0.0001f);

            Rigidbody rb = myNewPiece.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            // Passes the information of the piece into the piece controller
            PieceController myNewPieceController = myNewPiece.AddComponent<PieceController>();
            myNewPieceController.SetPiece(piece);
        }
    }

    public void PerformSpecialMove(ChessPieceScript.ChessPiece piece, ChessPieceScript.Move move)
    {
        ChessPieceScript.SpecialMoveType specialMove = move.GetSpecialMoveType();

        if (specialMove == ChessPieceScript.SpecialMoveType.None)
        {
            return;
        }

        int[] heldCoords = piece.GetCoordinates();
        int[] moveCoords = move.GetMoveCoordinates();
        int color = piece.GetPieceColor();
        int direction = (color == 0) ? 1 : -1;
        int moveX = moveCoords[0];
        int moveY = moveCoords[1];

        switch (specialMove)
        {
            case ChessPieceScript.SpecialMoveType.EnPassant:
                Debug.Log("EnPassant");
                int[] enPassantCoordinates = { heldCoords[0] + moveX, heldCoords[1] + moveY - direction };
                ChessPieceScript.ChessPiece capturedPiece = _chessBoard.GetPiece(enPassantCoordinates[0], enPassantCoordinates[1]);
                int capturedColor = capturedPiece.GetPieceColor();
                _moveHistoryStackObject.AddMove(ref capturedPiece, ChessPieceScript.SpecialMoveType.EnPassant);
                _chessBoard.RemovePiece(enPassantCoordinates);
                break;

            case ChessPieceScript.SpecialMoveType.Castling:

                Debug.Log("Castling");
                ChessPieceScript.ChessPiece king = _chessBoard.GetKing(color);

                int[] kingCoords = king.GetCoordinates();

                int castleToX = (kingCoords[0] > heldCoords[0]) ? kingCoords[0] - 2 : kingCoords[0] + 2;
                int castleToY = heldCoords[1];
                int[] castleToCoords = { castleToX, castleToY };
                ChessPieceScript.ChessPiece toPiece = _chessBoard.GetPiece(castleToCoords);
                _moveHistoryStackObject.AddMove(ref king, ref toPiece, ChessPieceScript.SpecialMoveType.Castling);
                king.UpdatePieceMove(castleToCoords);
                _chessBoard.UpdateBoardMove(kingCoords, castleToCoords);

                break;

            default:
                break;
        }
    }

    public bool IsColorInCheck(int defendingColor)
    {
        int attackingColor = (defendingColor == 0) ? 1 : 0;
        ChessPieceScript.ChessPiece defendingKing = _chessBoard.GetKing(defendingColor);
        int[] defendingKingCoordinate = defendingKing.GetCoordinates();

        List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
        foreach (ChessPieceScript.ChessPiece piece in _chessBoard.GetColorList()[attackingColor])
        {
            CalculateMovesQ(piece, ref possibleMoves);
            foreach (ChessPieceScript.Move move in possibleMoves)
            {
                int[] moveCoordinates = move.GetMoveCoordinates();
                int[] pieceCoordinates = piece.GetCoordinates();
                int moveX = moveCoordinates[0] + pieceCoordinates[0];
                int moveY = moveCoordinates[1] + pieceCoordinates[1];

                if (moveX == defendingKingCoordinate[0] && moveY == defendingKingCoordinate[1])
                {
                    Debug.Log(defendingColor + " is in check");
                    return true;
                }
            }
            possibleMoves.Clear();
        }

        return false;
    }

    public bool IsPiecesInCheck(int defendingColor, ChessPieceScript.ChessPiece[] castlingPiecesToCheck)
    {
        int attackingColor = (defendingColor == 0) ? 1 : 0;

        List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
        foreach (ChessPieceScript.ChessPiece piece in _chessBoard.GetColorList()[attackingColor])
        {
            CalculateMovesQ(piece, ref possibleMoves);
            foreach (ChessPieceScript.Move move in possibleMoves)
            {
                int[] moveCoordinates = move.GetMoveCoordinates();
                int[] pieceCoordinates = piece.GetCoordinates();
                int moveX = moveCoordinates[0] + pieceCoordinates[0];
                int moveY = moveCoordinates[1] + pieceCoordinates[1];

                foreach (var castlingGapPiece in castlingPiecesToCheck)
                {
                    if (moveX == castlingGapPiece.GetCoordinates()[0] && moveY == castlingGapPiece.GetCoordinates()[1])
                    {
                        //Debug.Log("Cannot Castle");
                        return true;
                    }
                }
            }
            possibleMoves.Clear();
        }

        return false;
    }

    public bool IsTileInCheck(int defendingColor)
    {
        int attackingColor = (defendingColor == 0) ? 1 : 0;
        ChessPieceScript.ChessPiece defendingKing = _chessBoard.GetKing(defendingColor);
        int[] defendingKingCoordinate = defendingKing.GetCoordinates();

        List<ChessPieceScript.Move> possibleMoves = new List<ChessPieceScript.Move>();
        foreach (ChessPieceScript.ChessPiece piece in _chessBoard.GetColorList()[attackingColor])
        {
            CalculateMovesQ(piece, ref possibleMoves);
            foreach (ChessPieceScript.Move move in possibleMoves)
            {
                int[] moveCoordinates = move.GetMoveCoordinates();
                int[] pieceCoordinates = piece.GetCoordinates();
                int moveX = moveCoordinates[0] + pieceCoordinates[0];
                int moveY = moveCoordinates[1] + pieceCoordinates[1];

                if (moveX == defendingKingCoordinate[0] && moveY == defendingKingCoordinate[1])
                {
                    Debug.Log(defendingColor + " is in check");
                    return true;
                }
            }
            possibleMoves.Clear();
        }

        return false;
    }

    // Assumes that the piece is in check first

    public bool IsColorInCheckmate(int defendingColor)
    {
        int attackingColor = (defendingColor == 0) ? 1 : 0;
        List<ChessPieceScript.ChessPiece> list = _chessBoard.GetColorList(defendingColor);
        ChessPieceScript.ChessPiece[] chessList = new ChessPieceScript.ChessPiece[list.Count];
        list.CopyTo(chessList);

        for (int i = 0; i < chessList.Length; i++)
        {
            CalculateMoves(chessList[i]);
            for (int j = 1; j < PossibleMoves.Count; j++)
            {
                int[] currentCoordinates = chessList[i].GetCoordinates();
                int[] tileCoordinates = PossibleMoves[j].GetCombinedCoordinates(chessList[i]);

                // make the move on the copied board
                PerformSpecialMove(chessList[i], PossibleMoves[j]);
                MovePieceTo(chessList[i], tileCoordinates);
                PawnPromotion(tileCoordinates);

                if (!IsColorInCheck(defendingColor))
                {
                    Debug.Log("Can block with " + chessList[i]);
                    RevertMove();
                    return false;
                }
                RevertMove();
                chessList[i].SetCoordinates(currentCoordinates);
            }
        }
        _chessBoard.SetGameOver(true);
        return true;
    }

    public void RevertMove()
    {
        if (_moveHistoryStackObject.moveStack.Count == 0)
        {
            Debug.Log("Error: No moves to revert");
            return;
        }

        MoveHistory.PriorMove priorMove = _moveHistoryStackObject.moveStack.Pop();
        if (_chessBoard.GetPiece(priorMove.ToPiece.GetCoordinates()).GetGameObject() == null)
            Debug.Log("OOPSIE");

       _chessBoard.RevertTest(priorMove);

        if (_moveHistoryStackObject.moveStack.Count > 0)
        {
            MoveHistory.PriorMove nextMove = _moveHistoryStackObject.moveStack.Peek();
            if (nextMove.SpecialMoveType == ChessPieceScript.SpecialMoveType.EnPassant)
            {
                Debug.Log("EN PASSANT HAS HAPPENED");
                _moveHistoryStackObject.moveStack.Pop();
                RevertPiece(nextMove.FromPiece);
            }
            else if (nextMove.SpecialMoveType != ChessPieceScript.SpecialMoveType.None)
            {
                Debug.Log("SPECIAL MOVE HAS HAPPENED");
                RevertMove();
                return;
            }
        }

        _turn--;
    }
    
    public void RevertMoveQ(ref ChessPieceScript.ChessPiece fromPiece)
    {
        if (_moveHistoryStackObject.moveStack.Count == 0)
        {
            Debug.Log("Error: No moves to revert");
            return;
        }

        MoveHistory.PriorMove priorMove = _moveHistoryStackObject.moveStack.Pop();
        _chessBoard.RevertTestQ(priorMove, ref fromPiece);

        if (_moveHistoryStackObject.moveStack.Count > 0)
        {
            MoveHistory.PriorMove nextMove = _moveHistoryStackObject.moveStack.Peek();
            if (nextMove.SpecialMoveType == ChessPieceScript.SpecialMoveType.EnPassant)
            {
                //Debug.Log("EN PASSANT HAS HAPPENED");
                _moveHistoryStackObject.moveStack.Pop();
                RevertPiece(nextMove.FromPiece);
            }
            else if (nextMove.SpecialMoveType != ChessPieceScript.SpecialMoveType.None)
            {
                //Debug.Log("SPECIAL MOVE HAS HAPPENED");
                RevertMove();
                return;
            }
        }

        _turn--;
    }

    public void RevertPiece(ChessPieceScript.ChessPiece piece)
    {
        _chessBoard.RemovePiece(piece.GetCoordinates());
        _chessBoard.RevertBoardPiece(piece);
        SetupPiece(piece, piece.GetCoordinates());
    }

    public void MovePieceTo(ChessPieceScript.ChessPiece fromPiece, int[] toCoordinates)
    {
        var fromCoordinates = fromPiece.GetCoordinates();

        // The if statement will go through if you don't just place down a piece at it's original positions
        if (fromCoordinates[0] != toCoordinates[0] || fromCoordinates[1] != toCoordinates[1])
        {
            ChessPieceScript.ChessPiece toPiece = _chessBoard.GetPiece(toCoordinates);
            _moveHistoryStackObject.AddMove(ref fromPiece,ref toPiece);
            fromPiece.SetHasMoved(true);
            _chessBoard.UpdateBoardMove(fromCoordinates, toCoordinates);
            SetPawnHasMovedForwardTwo(ref fromPiece, toCoordinates[1]);
            EndTurn();
        }
        fromPiece.UpdatePieceMove(toCoordinates);
    }

    public void PlaceSelectedPieceAtCoordinate(int[] tileCoordinates)
    {
        MovePieceTo(_selectedPieceController.GetPiece(), tileCoordinates);
        SetSelected(null);
        DeleteMoveBorders();
    }

    public void PerformMoveAction(ChessPieceScript.ChessPiece piece, ChessPieceScript.Move move)
    {
        int[] coords = move.GetCombinedCoordinates(piece);
        PerformSpecialMove(piece, move);
        MovePieceTo(piece, coords);
        PawnPromotion(coords);

        int checkColor = GetTurn() % 2;
        if (IsColorInCheck(checkColor))
        {
            bool checkmate = IsColorInCheckmate(checkColor);
            Debug.Log("Is in checkmate?: " + checkmate);
        }
    }
}