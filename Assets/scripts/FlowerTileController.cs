using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTileController : TileController
{
    public int maxHoney;
    public int honey;

    public FlowerTileController(int indexI, int indexJ, int maxHoney, int initialHoney) : base(indexI, indexJ)
    {
        this.maxHoney = maxHoney;
        this.honey = initialHoney;
    }

    public void init(int indexI, int indexJ, int maxHoney, int initialHoney)
    {
        this.indexI = indexI;
        this.indexJ = indexJ;

        this.maxHoney = maxHoney;
        this.honey = initialHoney;
    }
}