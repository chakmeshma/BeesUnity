using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveTileController : TileController {
    public Hive hive;

    public HiveTileController(int indexI, int indexJ, Hive hive) : base(indexI, indexJ)
    {
        this.hive = hive;
    }

    public void init(int indexI, int indexJ, Hive hive)
    {
        this.indexI = indexI;
        this.indexJ = indexJ;

        this.hive = hive;
    }
}
