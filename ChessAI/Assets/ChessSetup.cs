using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessSetup : MonoBehaviour
{
    [SerializeField]
    private GameObject _tile;
    [SerializeField]
    private Material[] _tileMaterials;



    // Start is called before the first frame update
    void Start()
    {
        drawTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void drawTiles()
    {
        int k = 1;
        for (int i = 0; i < 8; i++)
        {
            k++;
            for (int j = 0; j < 8; j++)
            {
                k++;
                GameObject myNewTile = Instantiate(_tile, new Vector3(j * 1.0f, 0, i * 1.0f), Quaternion.identity);
                myNewTile.transform.parent = gameObject.transform;
                myNewTile.GetComponent<Renderer>().material = _tileMaterials[k % 2];
            }
        }
    }
}
