using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

// This script sets up the game so that the other scripts can handel the actual game
// It creates the tiles and pieces using the help of the "ChessPieceScript" script
public class ChessSetup : MonoBehaviour
{
    [SerializeField]
    private string _FENString;

    [SerializeField]
    private GameObject _tile;

    [SerializeField]
    private Material[] _pieceMaterials = new Material[2];

    [SerializeField]
    private Material[] _tileMaterials = new Material[2];

    private BoardHandler bhScriptRef;

    //private ChessPieceScript _CPS;

    // Start is called before the first frame update
    private void Start()
    {
        bhScriptRef = this.GetComponent<BoardHandler>();
        DrawTiles();
        SetupBoard();
    }

    public void DrawTiles()
    {
        GameObject childObjectTile = new GameObject("ChildObjectTile"); //creates a child to store all the children for organization
        childObjectTile.transform.parent = gameObject.transform;
        int k = 1;
        for (int x = 0; x < 8; x++)
        {
            k++;
            for (int y = 0; y < 8; y++)
            {
                k++;
                GameObject myNewTile = Instantiate(_tile, new Vector3(y * 1.0f, 0, x * 1.0f), Quaternion.identity);
                myNewTile.transform.parent = childObjectTile.transform;
                myNewTile.GetComponent<Renderer>().material = _tileMaterials[k % 2];

                // Added for onmousedown detection by the tile controller
                myNewTile.AddComponent<CapsuleCollider>();
                Rigidbody rb = myNewTile.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                TileController myNewTileController = myNewTile.AddComponent<TileController>();
                myNewTileController.SetTileCoordinate(y, x);    //FIX THIS
            }
        }
    }

    public void SetupBoard()
    {
        CPS.ChessBoard chessBoard = new CPS.ChessBoard(_FENString);
        GameObject childObjectPiece = new GameObject("ChildObjectPiece");   //creates a child to store all the children for organization
        childObjectPiece.transform.parent = gameObject.transform;
        foreach (var piece in chessBoard.GetBoard())
        {
            if (piece != null)
            {
                // Deals with object transformations
                int[] pieceCoordinates = piece.GetCoordinates();
                GameObject myNewPiece = Instantiate(piece.GetGameObject(), new Vector3(pieceCoordinates[0], .05f, pieceCoordinates[1]), Quaternion.identity);
                piece.SetGameObject(ref myNewPiece);
                myNewPiece.transform.rotation = Quaternion.Euler(-90, 0, 0);
                myNewPiece.transform.localScale = new Vector3(1500, 1500, 1500);

                // Sets the parent of the piece to be a new chess parent object
                myNewPiece.transform.parent = childObjectPiece.transform;

                // Added for onmousedown detection by the piece controller
                myNewPiece.AddComponent<CapsuleCollider>();
                Rigidbody rb = myNewPiece.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                // Passes the information of the piece into the piece controller
                PieceController myNewPieceController = myNewPiece.AddComponent<PieceController>();
                myNewPieceController.SetPiece(piece);
            }
        }
        bhScriptRef.SetBoard(chessBoard);
    }
}