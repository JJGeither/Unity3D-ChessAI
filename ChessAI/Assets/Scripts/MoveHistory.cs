using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHistory : MonoBehaviour
{
    public MoveHistoryStack _moveHistoryStack;
    private BoardHandler _bhScriptRef;

    // Start is called before the first frame update
    private void Start()
    {
        _bhScriptRef = this.GetComponent<BoardHandler>();
        _moveHistoryStack = new MoveHistoryStack();
        _bhScriptRef.SetStack(_moveHistoryStack);
    }

    public Stack<PriorMove> GetStack() => _moveHistoryStack.moveStack;

    public MoveHistoryStack GetStackObject() => _moveHistoryStack;

    public class MoveHistoryStack
    {
        public Stack<PriorMove> moveStack;

        public MoveHistoryStack()
        {
            moveStack = new Stack<PriorMove>();
        }

        public void AddMove(ref ChessPieceScript.ChessPiece fromPiece, ref ChessPieceScript.ChessPiece toPiece)
        {
            moveStack.Push(new PriorMove(fromPiece, toPiece));
        }

        public void AddMove(ref ChessPieceScript.ChessPiece fromPiece, ref ChessPieceScript.ChessPiece toPiece, ChessPieceScript.SpecialMoveType specialMove)
        {
            moveStack.Push(new PriorMove(fromPiece, toPiece, specialMove));
        }

        public void AddMove(ref ChessPieceScript.ChessPiece fromPiece, ChessPieceScript.SpecialMoveType specialMove)
        {
            moveStack.Push(new PriorMove(fromPiece, specialMove));
        }

        public PriorMove GetLastMove()
        {
            if (moveStack.Count > 0)
            {
                return moveStack.Peek();
            }
            else
            {
                return null;
            }
        }

        public void UndoLastMove()
        {
            if (moveStack.Count > 0)
            {
                moveStack.Pop();
            }
        }

        public bool IsEmpty()
        {
            return moveStack.Count == 0;
        }

        public void Clear()
        {
            moveStack.Clear();
        }
    }

    // !! Make sure to account for special moves !!
    public class PriorMove
    {
        public ChessPieceScript.ChessPiece FromPiece { get; set; }
        public ChessPieceScript.ChessPiece ToPiece { get; set; }

        //From object can be recovered without storing it
        public GameObject ToPieceGameObject;

        public ChessPieceScript.SpecialMoveType SpecialMoveType { get; set; }

        public PriorMove(ChessPieceScript.ChessPiece fromPiece, ChessPieceScript.ChessPiece toPiece)
        {
            this.FromPiece = (ChessPieceScript.ChessPiece)fromPiece.Clone();
            ToPieceGameObject = toPiece.GetGameObject();
            this.ToPiece = (ChessPieceScript.ChessPiece)toPiece.Clone();
            this.SpecialMoveType = ChessPieceScript.SpecialMoveType.None;
        }

        public PriorMove(ChessPieceScript.ChessPiece fromPiece, ChessPieceScript.ChessPiece toPiece, ChessPieceScript.SpecialMoveType specialMoveType)
        {
            this.FromPiece = (ChessPieceScript.ChessPiece)fromPiece.Clone();
            ToPieceGameObject = toPiece.GetGameObject();
            this.ToPiece = (ChessPieceScript.ChessPiece)toPiece.Clone();
            this.SpecialMoveType = specialMoveType;
        }

        public PriorMove(ChessPieceScript.ChessPiece fromPiece, ChessPieceScript.SpecialMoveType specialMoveType)
        {
            this.FromPiece = (ChessPieceScript.ChessPiece)fromPiece.Clone();
            this.SpecialMoveType = specialMoveType;
        }
    }
}