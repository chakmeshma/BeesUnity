using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerTileController : TileController
{
    private int maxHoney;
    private int initialHoney;

    public FlowerTileController(int indexI, int indexJ, int maxHoney, int initialHoney) : base(indexI, indexJ)
    {
        this.maxHoney = maxHoney;
        this.initialHoney = initialHoney;
    }
}