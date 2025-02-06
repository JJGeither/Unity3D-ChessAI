using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;

public class GUIScript : MonoBehaviour
{
    public bool DisplayGUI;

    private BoardHandler _bhScriptRef;
    private CPS.ChessBoard _chessBoard;
    private MoveHistory _moveHistoryScriptRef;

    private MoveHistory.MoveHistoryStack _moveHistoryStack;

    private void Start()
    {
        _bhScriptRef = this.GetComponent<BoardHandler>();
        _chessBoard = _bhScriptRef.GetBoard();

        _moveHistoryScriptRef = this.GetComponent<MoveHistory>();
        _moveHistoryStack = _bhScriptRef.GetStack();
    }

    private void OnGUI()
    {
        if (_chessBoard.IsGameOver())
        {
            GUI.skin.label.fontSize = 100;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            int turn = _bhScriptRef.GetTurn();
            string labelText;
            if (turn % 2 == 0)
                labelText = "White Won";
            else
                labelText = "Black won";
            Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(labelText));
            float padding = 20.0f;
            Rect blackRect = new Rect(0, Screen.height / 2 - labelSize.y / 2 - padding / 2, Screen.width, labelSize.y + padding);
            Texture2D blackTexture = new Texture2D(1, 1);
            blackTexture.SetPixel(0, 0, Color.black);
            blackTexture.Apply();
            GUI.DrawTexture(blackRect, blackTexture);
            GUI.Label(new Rect(Screen.width / 2 - labelSize.x / 2, Screen.height / 2 - labelSize.y / 2, labelSize.x, labelSize.y), labelText);
        }

        if (DisplayGUI)
        {
            int turn = _bhScriptRef.GetTurn();
            GUI.Label(new Rect(10, 10, 200, 20), "Turn: " + turn);
            if (turn % 2 == 0)
            {
                GUI.Label(new Rect(10, 50, 200, 20), "Black's Move: ");
            }
            else
            {
                GUI.Label(new Rect(10, 50, 200, 20), "White's Move: ");
            }

            // Display the contents of a list
            List<CPS.ChessPiece>[] pieces = _chessBoard.GetColorList();
            int yPos = 80;
            int xPos = 10;

            // TODO: START FORM HERE
            for (int i = 0; i <= 1; i++)
            {
                foreach (CPS.ChessPiece piece in pieces[i])
                {
                    string pieceInfo = "Piece: " + piece.GetName() + ", Color: " + piece.GetPieceColor() + ", Position: " + piece.GetCoordinates()[0] + " " + piece.GetCoordinates()[1];
                    GUI.Label(new Rect(xPos, yPos, 500, 20), pieceInfo);
                    yPos += 20;
                }
                xPos += 800;
                yPos = 80;
            }

            DisplayStackContents(_moveHistoryScriptRef.GetStack());
        }
    }

    public static void DisplayStackContents(Stack<MoveHistory.PriorMove> stack)
    {
        int xPos = 200;
        int yPos = 10;
        if (stack == null || stack.Count == 0)
        {
            GUI.Label(new Rect(xPos, yPos, 500, 20), "Stack is null");
            return;
        }

        GUI.Label(new Rect(xPos, yPos - 80, 500, 20), "Stack contents: ");

        foreach (MoveHistory.PriorMove item in stack)
        {
            if (item.ToPiece != null)
                GUI.Label(new Rect(xPos, yPos, 1000, 20), item.FromPiece.GetName() + ":  [" + item.FromPiece.GetCoordinates()[0] + ", " + item.FromPiece.GetCoordinates()[1] + "]" + " --> " + item.ToPiece.GetName() + ":  [" + item.ToPiece.GetCoordinates()[0] + ", " + item.ToPiece.GetCoordinates()[1] + "] : " + item.SpecialMoveType);
            else
                GUI.Label(new Rect(xPos, yPos, 1000, 20), item.FromPiece.GetName() + ":  [" + item.FromPiece.GetCoordinates()[0] + ", " + item.FromPiece.GetCoordinates()[1] + "]" + " --> " + "Google EnPassant : " + item.SpecialMoveType);
            yPos += 30;
        }
    }
}