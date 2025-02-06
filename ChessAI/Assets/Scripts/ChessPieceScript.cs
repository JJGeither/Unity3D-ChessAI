using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// This script acts as class declaration and definitions for chess pieces
// It stores all information regarding pieces such as their potential moves, their models, color affiliation, etc.
// It also contains a class to represent the entire board
public class ChessPieceScript : MonoBehaviour
{
    public class ChessBoard : ICloneable
    {
        private ChessPiece[,] _board = new ChessPiece[8, 8];
        private List<ChessPiece>[] _colorPieces = new List<ChessPiece>[2];
        private bool _gameOver;

        public bool IsGameOver() => _gameOver;

        public void SetGameOver(bool status)
        {
            _gameOver = status;
        }

        public ChessBoard(string FENString)
        {
            // create lists for black and white pieces
            _colorPieces[0] = new List<ChessPiece>(); // black pieces
            _colorPieces[1] = new List<ChessPiece>(); // white pieces
            CreatePieces(FENString);
        }

        public object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            ChessBoard clonedPiece = new ChessBoard();

            clonedPiece._board = _board;

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public ChessBoard()
        {
            // create lists for black and white pieces
            _colorPieces[0] = new List<ChessPiece>(); // black pieces
            _colorPieces[1] = new List<ChessPiece>(); // white pieces
        }

        public ChessPiece[,] GetBoard() => _board;

        public List<ChessPiece>[] GetColorList() => _colorPieces;

        public List<ChessPiece> GetColorList(int color) => _colorPieces[color];

        public bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                return false;
            return true;
        }

        public ChessPiece GetKing(int color)
        {
            foreach (var piece in _colorPieces[color])
            {
                if (piece.GetName() != "empty" && piece.GetName() == "king")
                    return piece;
            }
            Debug.Log("Error: No King found");
            Time.timeScale = 0;
            return null;
        }

        public bool CanMoveTo(ChessPiece piece, int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            ChessPiece pieceAtDesiredPosition = GetPiece(x, y);
            if (pieceAtDesiredPosition != null)
                return pieceAtDesiredPosition.GetPieceColor() != piece.GetPieceColor();

            return true;
        }

        // Will not assume that you can move straight and capture a piece as a pawn
        public bool PawnCanMoveTo(ChessPiece piece, int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            ChessPiece pieceAtDesiredPosition = GetPiece(x, y);
            if (pieceAtDesiredPosition.GetName() != "empty")
                return false;

            return true;
        }

        public bool EnPassant(Pawn pawn, int x, int y, int turn)
        {
            if (!IsInBounds(x, y))
            {
                return false;
            }

            ChessPiece pieceAtDesiredPosition = GetPiece(x, y);
            if (pieceAtDesiredPosition != null && pieceAtDesiredPosition.GetName() == "pawn")
            {
                bool hasMovedForwardTwoTurnsAgo = pieceAtDesiredPosition.GetTurnMovedForwardTwo() == turn - 1;
                bool isDifferentColor = pieceAtDesiredPosition.GetPieceColor() != pawn.GetPieceColor();
                return hasMovedForwardTwoTurnsAgo && isDifferentColor;
            }

            return false;
        }

        public bool PawnCanCaptureDiagonals(ChessPiece piece, int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            ChessPiece pieceAtDesiredPosition = GetPiece(x, y);
            if (pieceAtDesiredPosition.GetName() != "empty")
            {
                return pieceAtDesiredPosition.GetPieceColor() != piece.GetPieceColor();
            }

            return false;
        }

        public void CreatePieces(string FENString)
        {
            List<Move>[] moves = new List<Move>[2];
            string characterToRemove = "/";
            FENString = FENString.Replace(characterToRemove, string.Empty);
            for (int i = 0, j = 0; i < FENString.Length; i++)
            {
                char pieceChar = FENString[i];
                // White color == 1, black is 0
                int color = char.IsUpper(pieceChar) ? 1 : 0;
                int x = j % 8;
                int y = j / 8;

                if (char.IsDigit(pieceChar))
                {
                    //j += pieceChar - '0';
                    for (int k = 0; k < pieceChar - '0'; k++)
                    {
                        _board[x, y] = new Empty(x, y);
                        j++;
                        x = j % 8;
                        y = j / 8;
                    }
                }
                else
                {
                    j++;
                    pieceChar = char.ToLower(pieceChar);
                    switch (pieceChar)
                    {
                        case 'p':
                            _board[x, y] = new Pawn(x, y, color);

                            break;

                        case 'n':
                            _board[x, y] = new Knight(x, y, color);
                            break;

                        case 'b':
                            _board[x, y] = new Bishop(x, y, color);
                            break;

                        case 'r':
                            _board[x, y] = new Rook(x, y, color);
                            break;

                        case 'k':
                            _board[x, y] = new King(x, y, color);
                            break;

                        case 'q':
                            _board[x, y] = new Queen(x, y, color);
                            break;

                        default:
                            break;
                    }
                    _colorPieces[color].Add(_board[x, y]);
                }
            }
        }

        public void UpdateBoardMove(int[] fromCoordinates, int[] toCoordinates)
        {
            ChessPiece toPiece = GetPiece(toCoordinates[0], toCoordinates[1]);
            ChessPiece fromPiece = GetPiece(fromCoordinates[0], fromCoordinates[1]);

            //From should never be null, so remove this maybe

            int toColor = -1;
            if (toPiece.GetName() != "empty")
                toColor = toPiece.GetPieceColor();


            MoveBoardPiece(fromPiece, toCoordinates[0], toCoordinates[1]);
            ClearCoordinate(fromCoordinates[0], fromCoordinates[1]);
        }

        public void RevertBoardMove(ChessPiece fromPiece, ChessPiece toPiece)
        {
            int[] toCoordinates = toPiece.GetCoordinates();
            int[] fromCoordinates = fromPiece.GetCoordinates();

            _board[toCoordinates[0], toCoordinates[1]] = toPiece;
            if (toPiece.GetName() != "empty")
                _colorPieces[toPiece.GetPieceColor()].Add(toPiece);

            _board[fromCoordinates[0], fromCoordinates[1]] = fromPiece;
            if (fromPiece.GetName() != "empty")
                _colorPieces[fromPiece.GetPieceColor()].Add(fromPiece);
        }

        // Primarilly used for EnPassant
        public void RevertBoardPiece(ChessPiece fromPiece)
        {
            int[] fromCoordinates = fromPiece.GetCoordinates();

            _board[fromCoordinates[0], fromCoordinates[1]] = fromPiece;
            if (fromPiece.GetName() != "empty")
                _colorPieces[fromPiece.GetPieceColor()].Add(fromPiece);
        }

        public void RemovePieceInList(ChessPiece toPiece)
        {
            if (toPiece.GetName() != "empty")
            {
                int toColor = toPiece.GetPieceColor();
                _colorPieces[toColor].Remove(toPiece);
            }
        }

        public void ReplacePieceInList(int color, ChessPiece replacementPiece, ChessPiece piece)
        {
            _colorPieces[color].Remove(piece);
            _colorPieces[color].Add(replacementPiece);
        }

        public void MoveBoardPiece(ChessPiece piece, int toX, int toY)
        {
            int[] toCoordinates = { toX, toY };
            RemovePiece(toCoordinates);
            _board[toX, toY] = piece;
        }

        public void ClearCoordinate(int x, int y)
        {
            _board[x, y] = new Empty(x, y);
        }

        public void RevertTest(MoveHistory.PriorMove priorMove)
        {
            int fromX = priorMove.FromPiece.GetCoordinates()[0];
            int fromY = priorMove.FromPiece.GetCoordinates()[1];
            int toX = priorMove.ToPiece.GetCoordinates()[0];
            int toY = priorMove.ToPiece.GetCoordinates()[1];

            // Resets the moved piece back to it's original position
            // The move: [P] -> [ ] ==> [ ] -> [P]
            // The revert: [ ] <- [P] ==> [P] [ ]
            ChessPieceScript.ChessPiece movedPiece = _board[toX, toY];
            movedPiece.SetCoordinates(fromX, fromY);
            if (movedPiece.GetGameObject() == null)
                Debug.LogError(movedPiece.GetName() + "GameObject for piece is null.");
           // movedPiece.SetGameObject(ref priorMove.ToPieceGameObject);
            movedPiece.GetGameObject().transform.position = new Vector3(fromX, 0.05f, fromY);
            movedPiece.GetGameObject().GetComponent<PieceController>().SetPiece(movedPiece);
            movedPiece.SetHasMoved(priorMove.FromPiece.GetHasMoved());
            _board[fromX, fromY] = movedPiece;


            if (priorMove.ToPiece.GetName() != "empty")
            {
                int pieceColor = priorMove.ToPiece.GetPieceColor();
                _colorPieces[pieceColor].Add(priorMove.ToPiece);
                priorMove.ToPieceGameObject.SetActive(true);
                priorMove.ToPiece.SetGameObject(ref priorMove.ToPieceGameObject);

                if (priorMove.ToPiece.GetGameObject() == null)
                {
                    Debug.LogError("GameObject for piece is null.");
                    return;
                }

                priorMove.ToPiece.GetGameObject().GetComponent<PieceController>().SetPiece(priorMove.ToPiece);
                //TODO:: apply new gameobject
            }

            //priorMove.ToPiece.SetCoordinates(toX, toY);
            priorMove.ToPiece.SetHasMoved(priorMove.ToPiece.GetHasMoved());
            _board[toX, toY] = priorMove.ToPiece;
        }
        
        public void RevertTestQ(MoveHistory.PriorMove priorMove, ref ChessPiece fromPieceOriginal)
        {
            int fromX = priorMove.FromPiece.GetCoordinates()[0];
            int fromY = priorMove.FromPiece.GetCoordinates()[1];
            int toX = priorMove.ToPiece.GetCoordinates()[0];
            int toY = priorMove.ToPiece.GetCoordinates()[1];

            // Resets the moved piece back to it's original position
            // The move: [P] -> [ ] ==> [ ] -> [P]
            // The revert: [ ] <- [P] ==> [P] [ ]
            ChessPieceScript.ChessPiece movedPiece = _board[toX, toY];
            movedPiece.SetCoordinates(fromX, fromY);
           // Debug.Log(debugNum);
            movedPiece.GetGameObject().transform.position = new Vector3(fromX, 0.05f, fromY);
            movedPiece.GetGameObject().GetComponent<PieceController>().SetPiece(movedPiece);
            movedPiece.SetHasMoved(priorMove.FromPiece.GetHasMoved());
            _board[fromX, fromY] = movedPiece;
            fromPieceOriginal = movedPiece;


            if (priorMove.ToPiece.GetName() != "empty")
            {
                int pieceColor = priorMove.ToPiece.GetPieceColor();
                _colorPieces[pieceColor].Add(priorMove.ToPiece);
                priorMove.ToPieceGameObject.SetActive(true);
                priorMove.ToPiece.SetGameObject(ref priorMove.ToPieceGameObject);
                priorMove.ToPiece.GetGameObject().transform.position = new Vector3(toX, 0.05f, toY);
                priorMove.ToPiece.GetGameObject().GetComponent<PieceController>().SetPiece(priorMove.ToPiece);
                //TODO:: apply new gameobject
            }

            priorMove.ToPiece.SetCoordinates(toX, toY);
            priorMove.ToPiece.SetHasMoved(priorMove.ToPiece.GetHasMoved());
            _board[toX, toY] = priorMove.ToPiece;
        }

        public void RemovePiece(int[] coordinates)
        {
            ChessPiece deletePiece = _board[coordinates[0], coordinates[1]];
            if (deletePiece.GetName() != "empty")
            {
                RemovePieceInList(deletePiece);
                GameObject deleteObject = deletePiece.GetGameObject();
                deleteObject.SetActive(false);
            }

            _board[coordinates[0], coordinates[1]] = new Empty(coordinates[0], coordinates[1]);
        }

        public ChessPiece GetPiece(int x, int y) => _board[x, y];

       public ChessPiece GetPiece(int[] coordinates) => _board[coordinates[0], coordinates[1]];
       
    }

    // Each move that a piece can make will be stored in its own Move class
    // If a white piece could move up one, it would be (0, 1)
    public class Move
    {
        private readonly int _relativeX;
        private readonly int _relativeY;
        private readonly SpecialMoveType _specialMoveType;

        public Move(int x, int y)
        {
            _relativeX = x;
            _relativeY = y;
            _specialMoveType = SpecialMoveType.None;
        }

        public Move(int x, int y, SpecialMoveType specialMoveType)
        {
            _relativeX = x;
            _relativeY = y;
            _specialMoveType = specialMoveType;
        }

        public int[] GetMoveCoordinates()
        {
            return new int[] { _relativeX, _relativeY };
        }

        public int[] GetCombinedCoordinates(ChessPiece piece)
        {
            int[] pieceCoordinates = piece.GetCoordinates();
            return new int[] { _relativeX + pieceCoordinates[0], _relativeY + pieceCoordinates[1] };
        }

        public SpecialMoveType GetSpecialMoveType()
        {
            return _specialMoveType;
        }
    }

    public class ChessPieceBasics
    {
        private string pieceName;
        private int color;
        private int[] coordinates;

        public ChessPieceBasics(ChessPiece piece)
        {
            pieceName = piece.GetName();
            color = piece.GetPieceColor();
            coordinates = piece.GetCoordinates();
        }

        public string GetName() => pieceName;

        public int GetPieceColor() => color;

        public int[] GetCoordinates() => coordinates;
    }

    public enum SpecialMoveType
    {
        None, EnPassant, Castling, Promotion
    }

    public abstract class ChessPiece : ICloneable
    {
        protected GameObject[] _pieceFolder = Resources.LoadAll<GameObject>("Pieces");
        protected Material[] _materialFolder = Resources.LoadAll<Material>("Materials");

        protected Material _pieceMaterial;
        protected int _color;
        protected int[] _initialPosition;
        protected GameObject _pieceGameObject;
        protected int _coordinateX;
        protected int _coordinateY;
        protected string _name;
        private int _turnMovedForwardTwo;
        private bool _hasMoved;

        protected int _materialValue;

        // Keeps track of what moves it can make in relation to its current position depending on circumstance. ex: en passant, castling, etc.
        protected int[] _specialRelativeMoves;

        public ChessPiece(int x, int y)
        {
            _coordinateX = x;
            _coordinateY = y;
            _initialPosition = new int[] { x, y };
            _name = "null";
            _turnMovedForwardTwo = -1;
            _hasMoved = false;
            _color = -1;
            _materialValue = 0;
        }

        public abstract object Clone();

        public int GetMaterialValue()
        {
            return _materialValue;
        }

        public int GetTurnMovedForwardTwo()
        {
            return _turnMovedForwardTwo;
        }

        public void SetTurnMovedForwardTwo(int value)
        {
            _turnMovedForwardTwo = value;
        }

        public bool GetHasMoved()
        {
            return _hasMoved;
        }

        public void SetHasMoved(bool hasMoved)
        {
            _hasMoved = hasMoved;
        }

        public string GetName()
        {
            return _name;
        }

        public int[] GetCoordinates()
        {
            return new int[] { _coordinateX, _coordinateY };
        }

        public void SetCoordinates(int[] coordinates)
        {
            _coordinateX = coordinates[0];
            _coordinateY = coordinates[1];
        }

        public void SetCoordinates(int x, int y)
        {
            _coordinateX = x;
            _coordinateY = y;
        }

        public void SetGameObject(ref GameObject gameObject)
        {
            _pieceGameObject = gameObject;
        }

        public ref GameObject GetGameObject()
        {
            return ref _pieceGameObject;
        }

        public int GetPieceColor()
        {
            return _color;
        }

        public void UpdatePieceMove(int[] coordinates)
        {
            _pieceGameObject.transform.position = new Vector3(coordinates[0], .05f, coordinates[1]);

            // If selecting current tile
            if (coordinates[0] == _coordinateX && coordinates[1] == _coordinateY)
            {
                //Debug.Log("Cancelling move");
                return;
            }

            //Debug.Log("Pawn moves from " + _coordinateX + "," + _coordinateY + " to " + coordinates[0] + "," + coordinates[1]);
            SetCoordinates(coordinates);
        }

        // Determines if the piece is currently at the given position
        public bool IsAtInitialPosition()
        {
            return _coordinateX == _initialPosition[0] && _coordinateY == _initialPosition[1];
        }

        // Returns a list of all possible moves for this piece
        public virtual List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            return new List<Move>();
        }

        public virtual List<Move> GetPossibleMoves(ref ChessBoard board, int currentTurn)
        {
            return new List<Move>();
        }

        public void CalculateDirectionalMoves(ref ChessBoard board, ref List<Move> relativeMoves, MoveType moveType)
        {
            bool[] cutoffs = new bool[8] { false, false, false, false, false, false, false, false };

            int maxIndex = 8;
            int[,] directions = new int[8, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };

            if (moveType == MoveType.Flat)
            {
                maxIndex = 4;
                directions = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
            }
            else if (moveType == MoveType.Diagonal)
            {
                maxIndex = 4;
                directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
            }

            for (int i = 1; i < 8; i++)
            {
                for (int j = 0; j < maxIndex; j++)
                {
                    if (!cutoffs[j])
                    {
                        int dx = directions[j, 0] * i, dy = directions[j, 1] * i;
                        int newX = _coordinateX + dx, newY = _coordinateY + dy;

                        if (board.CanMoveTo(this, newX, newY))
                        {
                            relativeMoves.Add(new Move(dx, dy, SpecialMoveType.None));

                            if (board.GetPiece(newX, newY).GetName() != "empty")
                            {
                                cutoffs[j] = true;
                            }
                        }
                        else
                        {
                            cutoffs[j] = true;
                        }
                    }
                }
            }
        }

        public enum MoveType
        {
            Flat,
            Diagonal,
            FlatAndDiagonal
        }
    }

    public class Empty : ChessPiece
    {
        public Empty(int x, int y) : base(x, y)
        {
            _name = "empty";
            _color = -1;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Empty clonedPiece = new Empty(_coordinateX, _coordinateY);

            // Return the cloned Pawn instance
            return clonedPiece;
        }
    }

    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[3 + (6 * pColor)];
            _color = pColor;
            _name = "pawn";
            _materialValue = 1;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Pawn clonedPiece = new Pawn(_coordinateX, _coordinateY, _color);

            clonedPiece._initialPosition = _initialPosition;
            clonedPiece.SetTurnMovedForwardTwo(GetTurnMovedForwardTwo());

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board, int currentTurn)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            int x = _coordinateX;
            int y = _coordinateY;
            int direction = (_color == 0 ? 1 : -1);

            // Normal case
            if (board.PawnCanMoveTo(this, x, y + direction))
                relativeMoves.Add(new Move(0, direction));

            // Moving two spaces forward
            if (IsAtInitialPosition() && board.PawnCanMoveTo(this, x, y + 2 * direction) && board.PawnCanMoveTo(this, x, y + 1 * direction))
                relativeMoves.Add(new Move(0, 2 * direction));

            // En passant
            if (board.EnPassant(this, x + 1, y, currentTurn))
                relativeMoves.Add(new Move(1, direction, SpecialMoveType.EnPassant));

            if (board.EnPassant(this, x - 1, y, currentTurn))
                relativeMoves.Add(new Move(-1, direction, SpecialMoveType.EnPassant));

            // Capturing
            if (board.PawnCanCaptureDiagonals(this, x + 1, y + direction))
                relativeMoves.Add(new Move(1, direction));

            if (board.PawnCanCaptureDiagonals(this, x - 1, y + direction))
                relativeMoves.Add(new Move(-1, direction));

            return relativeMoves;
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[2 + (6 * pColor)];
            _color = pColor;
            _name = "knight";
            _materialValue = 3;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Knight clonedPiece = new Knight(_coordinateX, _coordinateY, _color);

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            int[] dx = { 1, 1, 2, 2, -1, -1, -2, -2 };
            int[] dy = { 2, -2, 1, -1, 2, -2, 1, -1 };
            int xPos, yPos;

            for (int i = 0; i < dx.Length; i++)
            {
                xPos = _coordinateX + dx[i];
                yPos = _coordinateY + dy[i];

                if (board.CanMoveTo(this, xPos, yPos))
                {
                    relativeMoves.Add(new Move(dx[i], dy[i]));
                }
            }

            return relativeMoves;
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[(6 * pColor)];
            _color = pColor;
            _name = "bishop";
            _materialValue = 3;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Bishop clonedPiece = new Bishop(_coordinateX, _coordinateY, _color);

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            CalculateDirectionalMoves(ref board, ref relativeMoves, MoveType.Diagonal);

            return relativeMoves;
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[5 + (6 * pColor)];
            _color = pColor;
            _name = "rook";
            _materialValue = 5;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Rook clonedPiece = new Rook(_coordinateX, _coordinateY, _color);

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public void CalculateCastling(ref ChessBoard board, ref List<Move> relativeMoves)
        {
            // spaces between pieces have to be empty
            // King cannot be in check
            // king cannot pass through checked spaces
            int x = _coordinateX;
            int y = _coordinateY;
            //ChessPiece king = board.GetKing(_color);
            // If on the left

            ChessPiece king = board.GetKing(this._color);
            bool kingMoved = king.GetHasMoved();
            bool thisMoved = this.GetHasMoved();
            if (!thisMoved && !kingMoved)
            {
                if (x == 0)
                {
                    if (king.GetCoordinates()[0] != 3)
                        return;
                    for (int i = 1; i < 2; i++)
                    {
                        if (board.GetPiece(x + i, y).GetName() != "empty")
                        {
                            return;
                        }
                    }

                    relativeMoves.Add(new Move(2, 0, SpecialMoveType.Castling));
                }
                else if (x == 7)
                {
                    if (king.GetCoordinates()[0] != 3)
                        return;
                    for (int i = 1; i < 4; i++)
                    {
                        if (board.GetPiece(x - i, y).GetName() != "empty")
                        {
                            return;
                        }
                    }
                    relativeMoves.Add(new Move(-3, 0, SpecialMoveType.Castling));
                }
            }
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            CalculateCastling(ref board, ref relativeMoves);
            CalculateDirectionalMoves(ref board, ref relativeMoves, MoveType.Flat);

            return relativeMoves;
        }
    }

    public class King : ChessPiece
    {
        public King(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[1 + (6 * pColor)];
            _color = pColor;
            _name = "king";
            _materialValue = int.MaxValue;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            King clonedPiece = new King(_coordinateX, _coordinateY, _color);

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            int[] dx = { 1, 1, 1, 0, 0, -1, -1, -1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int xPos, yPos;

            for (int i = 0; i < dx.Length; i++)
            {
                xPos = _coordinateX + dx[i];
                yPos = _coordinateY + dy[i];

                if (board.CanMoveTo(this, xPos, yPos))
                    relativeMoves.Add(new Move(dx[i], dy[i]));
            }

            return relativeMoves;
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[4 + (6 * pColor)];
            _color = pColor;
            _name = "queen";
            _materialValue = 9;
        }

        public override object Clone()
        {
            // Create a new instance of Pawn with the same property values as the original instance
            Queen clonedPiece = new Queen(_coordinateX, _coordinateY, _color);

            // Return the cloned Pawn instance
            return clonedPiece;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();
            relativeMoves.Add(new Move(0, 0));

            CalculateDirectionalMoves(ref board, ref relativeMoves, MoveType.FlatAndDiagonal);

            return relativeMoves;
        }
    }
}