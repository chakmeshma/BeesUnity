using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveTileController : TileController {
    private Hive hive;

    public HiveTileController(int indexI, int indexJ, Hive hive) : base(indexI, indexJ)
    {
        this.hive = hive;
    }
}
