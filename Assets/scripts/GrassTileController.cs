using UnityEngine;

public class GrassTileController : TileController
{
    public GrassTileController(int indexI, int indexJ) : base(indexI, indexJ)
    {
    }

    public void init(int indexI, int indexJ)
    {
        this.indexI = indexI;
        this.indexJ = indexJ;
    }
}