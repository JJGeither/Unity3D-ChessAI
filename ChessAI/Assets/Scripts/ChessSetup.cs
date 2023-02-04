using UnityEngine;
using CPS = ChessPieceScript;   //alias to script.

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

    //private ChessPieceScript _CPS;

    // Start is called before the first frame update
    private void Start()
    {
        //_CPS = GetComponent<ChessPieceScript>();
        DrawTiles();
        SetupBoard();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DrawTiles()
    {
        GameObject childObjectTile = new GameObject("ChildObjectTile"); //creates a child to store all the children for organization
        childObjectTile.transform.parent = gameObject.transform;
        int k = 1;
        for (int i = 0; i < 8; i++)
        {
            k++;
            for (int j = 0; j < 8; j++)
            {
                k++;
                GameObject myNewTile = Instantiate(_tile, new Vector3(j * 1.0f, 0, i * 1.0f), Quaternion.identity);
                myNewTile.transform.parent = childObjectTile.transform;
                myNewTile.GetComponent<Renderer>().material = _tileMaterials[k % 2];
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
                GameObject myNewPiece = Instantiate(piece.pieceObject, new Vector3(piece.X, .05f, piece.Y), Quaternion.identity);

                myNewPiece.transform.rotation = Quaternion.Euler(-90, 0, 0);
                myNewPiece.transform.localScale = new Vector3(1500, 1500, 1500);
                myNewPiece.transform.parent = childObjectPiece.transform;
            }

        }
    }
}