using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {

    public int indexI = -1;
    public int indexJ = -1;

    public TileController(int indexI, int indexJ)
    {
        this.indexI = indexI;
        this.indexJ = indexJ;
    }
}
