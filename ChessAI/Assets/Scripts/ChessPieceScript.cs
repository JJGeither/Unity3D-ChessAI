using UnityEngine;
using System.Collections.Generic;

// This script acts as class declaration and definitions for chess pieces
// It stores all information regarding pieces such as their potential moves, their models, color affiliation, etc.
// It also contains a class to represent the entire board
public class ChessPieceScript : MonoBehaviour
{
    public class ChessBoard
    {
        private readonly ChessPiece[,] _board = new ChessPiece[8, 8];

        public ChessBoard(string FENString)
        {
            CreatePieces(FENString);
        }

        public ChessPiece[,] GetBoard() => _board;

        public bool CanMoveTo(ChessPiece piece, int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                return false;

            ChessPiece pieceAtDesiredPosition = GetPiece(x, y);
            if (pieceAtDesiredPosition != null)
            {
                return pieceAtDesiredPosition.GetPieceColor() != piece.GetPieceColor();
            }

            return true;
        }

        public void CreatePieces(string FENString)
        {
            string characterToRemove = "/";
            FENString = FENString.Replace(characterToRemove, string.Empty);
            for (int i = 0, j = 0; i < FENString.Length; i++)
            {
                char pieceChar = FENString[i];
                int color = char.IsUpper(pieceChar) ? 1 : 0;
                int x = j % 8;
                int y = j / 8;

                if (char.IsDigit(pieceChar))
                {
                    j += pieceChar - '0';
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
                }
            }
        }

        public void UpdateBoardMove(int fromX, int fromY, int toX, int toY)
        {
            ChessPiece fromPiece = GetPiece(fromX, fromY);
            SetPiece(fromPiece, toX, toY);
            ClearCoordinate(fromX, fromY);
        }

        public void SetPiece(ChessPiece piece, int x, int y)
        {
            DeletePiece(x, y);
            _board[x, y] = piece;
        }

        public void ClearCoordinate(int x, int y)
        {
            _board[x, y] = null;
        }

        public void DeletePiece(int coordinateX, int coordinateY)
        {
            ChessPiece deletePiece = _board[coordinateX, coordinateY];
            if (deletePiece != null)
                Destroy(deletePiece.GetGameObject());
        }

        public ChessPiece GetPiece(int x, int y) => _board[x, y];

    }

    // Each move that a piece can make will be stored in its own Move class
    // If a white piece could move up one, it would be (0, 1)
    public class Move
    {
        private readonly int _relativeX;
        private readonly int _relativeY;

        public Move(int x, int y)
        {
            _relativeX = x;
            _relativeY = y;
        }

        public int[] GetMoveCoordinates()
        {
            return new int[] { _relativeX, _relativeY };
        }
    }

    public abstract class ChessPiece
    {
        protected GameObject[] _pieceFolder = Resources.LoadAll<GameObject>("Pieces");
        protected Material[] _materialFolder = Resources.LoadAll<Material>("Materials");

        protected Material _pieceMaterial;
        protected int _color;
        protected int[] _initialPosition;
        protected GameObject _pieceGameObject;
        protected int _coordinateX;
        protected int _coordinateY;

        // Keeps track of what moves it can make in relation to its current position depending on circumstance. ex: en passant, castling, etc.
        protected int[] _specialRelativeMoves;

        public ChessPiece(int x, int y)
        {
            _coordinateX = x;
            _coordinateY = y;
            _initialPosition = new int[] { x, y };
        }

        public int[] GetCoordinates()
        {
            return new int[] { _coordinateX, _coordinateY };
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

        public void MoveTo(int x, int y)
        {
            _pieceGameObject.transform.position = new Vector3(x, .05f, y);

            // If selecting current tile
            if (x == _coordinateX && y == _coordinateY)
            {
                Debug.Log("Cancelling move");
                return;
            }

            Debug.Log("Pawn moves from " + _coordinateX + "," + _coordinateY + " to " + x + "," + y);
            _coordinateX = x;
            _coordinateY = y;
        }

        // Determines if the piece is currently at the given position
        public bool IsAtInitialPosition()
        {
            return _coordinateX == _initialPosition[0] && _coordinateY == _initialPosition[1];
        }

        // Returns a list of all possible moves for this piece
        virtual public List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            return new List<Move>();
        }
    }

    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[3 + (6 * pColor)];
            _color = pColor;
        }

        public override List<Move> GetPossibleMoves(ref ChessBoard board)
        {
            List<Move> relativeMoves = new List<Move>();

            relativeMoves.Add(new Move(0, 0));

            // Normal case
            if (board.CanMoveTo(this, 0 + _coordinateX, 1 + _coordinateY))
                relativeMoves.Add(new Move(0, 1));

            // Moving two spaces forward 
            if (IsAtInitialPosition() && board.CanMoveTo(this, 0 + _coordinateX, 2 + _coordinateY))
                relativeMoves.Add(new Move(0, 2));

            // En passant

            // Capturing

            return relativeMoves;
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[2 + (6 * pColor)];
            _color = pColor;
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[(6 * pColor)];
            _color = pColor;
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[5 + (6 * pColor)];
            _color = pColor;
        }
    }

    public class King : ChessPiece
    {
        public King(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[1 + (6 * pColor)];
            _color = pColor;
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(int x, int y, int pColor) : base(x, y)
        {
            _pieceGameObject = _pieceFolder[4 + (6 * pColor)];
            _color = pColor;
        }
    }
}