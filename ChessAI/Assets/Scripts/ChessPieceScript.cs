using UnityEngine;

// This script acts as class declaration and definitions for chess pieces
// It stores all information regarding pieces such as their potential moves, their models, color affiliation, etc.
// It also contains a class to represent the entire board
public class ChessPieceScript : MonoBehaviour
{
    public class ChessBoard
    {
        public ChessBoard(string FENString) //the constructor creates the board
        {
            CreatePieces(FENString);
        }

        private ChessPiece[,] _board = new ChessPiece[8, 8];

        public ChessPiece[,] GetBoard()
        {
            return _board;
        }

        public void CreatePieces(string FENString)
        {
            string characterToRemove = "/";
            FENString = FENString.Replace(characterToRemove, string.Empty);
            for (int i = 0, j = 0; i < FENString.Length; i++)
            {
                char pieceChar = FENString[i];
                int color = char.IsUpper(pieceChar) ? 1 : 0;
                //Debug.Log(color);
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

        // Moves a piece at coordinate (fromX, fromY) to coordinate (toX, toY)
        public void UpdateBoardMove(int fromX, int fromY, int toX, int toY)
        {
            // Gets the piece that will be moved
            ChessPiece fromPiece = GetPiece(fromX, fromY);

            // Sets the to coordinate to the piece from the from coordinate
            // This destroys the TO piece
            SetPiece(fromPiece, toX, toY);

            // Clears the form coordinate so that there are not duplicate pieces
            ClearCoordinate(fromX, fromY);
        }

        public void SetPiece(ChessPiece piece, int x, int y)
        {
            _board[x, y] = piece;
            piece.X = x;
            piece.Y = y;
        }

        public void ClearCoordinate(int x, int y)
        {
            _board[x, y] = null;
        }

        public ChessPiece GetPiece(int x, int y)
        {
            return _board[x, y];
        }
    }

    public abstract class ChessPiece
    {
        public GameObject[] pieceFolder = Resources.LoadAll<GameObject>("Pieces");
        public Material[] materialFolder = Resources.LoadAll<Material>("Materials");

        public GameObject pieceObject;
        public Material pieceMaterial;
        public int X { get; set; }
        public int Y { get; set; }

        public ChessPiece(int x, int y)
        {
            X = x;
            Y = y;
        }

        public abstract void Move(int x, int y);
    }

    public class Pawn : ChessPiece
    {
        public Pawn(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[3 + (6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Pawn moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[2 + (6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Knight moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[(6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Knight moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[5 + (6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Knight moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }

    public class King : ChessPiece
    {
        public King(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[1 + (6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Knight moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(int x, int y, int color) : base(x, y)
        {
            pieceObject = pieceFolder[4 + (6 * color)];
        }

        public override void Move(int x, int y)
        {
            Debug.Log("Knight moves from" + X + "," + Y + " to " + x + "," + y);
            X = x;
            Y = y;
        }
    }
}