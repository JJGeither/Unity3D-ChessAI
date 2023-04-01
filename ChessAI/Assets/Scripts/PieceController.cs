using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

// This script deals with the pieces
// Whenever an interaction with a piece happens, such as selecting one, this script will handle it and pass the information to the BoardHandler script
public class PieceController : MonoBehaviour
{
    private int _layerMask;
    private bool _isBeingSelected;
    private Rigidbody rb;
    private CPS.ChessPiece _piece;
    private BoardHandler _bhScriptRef;
    private CapsuleCollider _collisionComponent;

    // Start is called before the first frame update
    private void Start()
    {
        _layerMask = LayerMask.GetMask("Tile");
        _isBeingSelected = false;
        rb = GetComponent<Rigidbody>();
        GameObject obj_gameboard = GameObject.Find("Obj_GameBoard");
        _bhScriptRef = obj_gameboard.GetComponent<BoardHandler>();
        _collisionComponent = this.GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        UpdateCollider();
        UpdateSelectedAnimation();
    }

    // Removes the collider for all pieces whenever a piece is being held
    public void UpdateCollider()
    {
        if (_bhScriptRef.GetSelected())
        {
            _collisionComponent.enabled = false;
        }
        else
        {
            _collisionComponent.enabled = true;
        }
    }

    // Updates the movement of the piece that is being held
    public void UpdateSelectedAnimation()
    {
        if (_isBeingSelected)
        {
            //GetComponent<Rigidbody>().position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, _layerMask))
            {
                Vector3 test = new Vector3(hit.point.x, hit.point.y + 1.5f, hit.point.z);
                rb.position = test;
            }
        }
    }

    // Sets the position in the world, NOT the coordinates of the board
    public void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    public void SetPieceSelected(bool isSelected)
    {
        _isBeingSelected = isSelected;

        // Will only send piece inforation to the Board Controller if it is actually Selecteding a piece
        if (_isBeingSelected)
            _bhScriptRef.SetSelected(this.GetComponent<PieceController>());
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
        // if the boardhandler is not currently Selecteding any other piece
        if (!_bhScriptRef.GetSelected())
        {
            // Tells the board handler which piece it is Selecteding along with updating itself to that fact
            SetPieceSelected(true);

            // Calculates the move that the piece selected can make
            _bhScriptRef.CalculateMoves(this);
        }
            
    }

    public void SetPiece(CPS.ChessPiece newPiece)
    {
        _piece = newPiece;
    }

    public ref CPS.ChessPiece GetPiece()
    {
        return ref _piece;
    }

    public int[] GetPieceCoordinates()
    {
        return _piece.GetCoordinates();
    }
}