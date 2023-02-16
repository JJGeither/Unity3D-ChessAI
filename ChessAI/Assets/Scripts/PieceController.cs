using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

// This script deals with the pieces
// Whenever an interaction with a piece happens, such as selecting one, this script will handle it and pass the information to the BoardHandler script
public class PieceController : MonoBehaviour
{
    private int _layerMask;
    private bool _isBeingHeld;
    private Rigidbody rb;
    private CPS.ChessPiece _piece;
    private BoardHandler bhScriptRef;

    // Start is called before the first frame update
    private void Start()
    {
        _layerMask = LayerMask.GetMask("Tile");
        _isBeingHeld = false;
        rb = GetComponent<Rigidbody>();
        GameObject obj_gameboard = GameObject.Find("Obj_GameBoard");
        bhScriptRef = obj_gameboard.GetComponent<BoardHandler>();
    }

    private void Update()
    {
        if (_isBeingHeld)
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

    public void SetHold(bool isHolding)
    {
        _isBeingHeld = isHolding;

        // Will only send piece inforation to the Board Controller if it is actually holding a piece
        if (_isBeingHeld)
            bhScriptRef.SetHold(_piece);
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
        SetHold(true);
    }

    public void SetPiece(CPS.ChessPiece newPiece)
    {
        _piece = newPiece;
    }
}