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
    private AIController aiScriptRef;

    //private ChessPieceScript _CPS;

    // Start is called before the first frame update
    private void Start()
    {
        bhScriptRef = this.GetComponent<BoardHandler>();
        aiScriptRef = this.GetComponent<AIController>();
        DrawTiles();
        SetupPieces();
    }

    public void DrawTiles()
    {
        GameObject childObjectTile = new GameObject("ChildObjectTile"); //creates a child to store all the children for organization
        childObjectTile.transform.parent = gameObject.transform;
        int k = 0;
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
                myNewTile.AddComponent<BoxCollider>();
                Rigidbody rb = myNewTile.AddComponent<Rigidbody>();
                rb.isKinematic = true;

                TileController myNewTileController = myNewTile.AddComponent<TileController>();
                myNewTileController.SetTileCoordinate(y, x);
            }
        }
    }

    public void SetupPieces()
    {
        CPS.ChessBoard chessBoard = new CPS.ChessBoard(_FENString);
        GameObject childObjectPiece = new GameObject("ChildObjectPiece");   //creates a child to store all the children for organization
        childObjectPiece.transform.parent = gameObject.transform;
        foreach (var piece in chessBoard.GetBoard())
        {
            if (piece.GetName() != "empty")
            {
                // Deals with object transformations
                int[] pieceCoordinates = piece.GetCoordinates();
                bhScriptRef.SetupPiece(piece, pieceCoordinates);
            }
        }
        bhScriptRef.SetBoard(chessBoard);
        aiScriptRef.SetBoard(chessBoard);
    }
}