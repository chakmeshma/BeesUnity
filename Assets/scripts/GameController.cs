using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public long lastUIEventTimeStamp;
    public Stopwatch uiEventStopWatch;
    private static GameController instance;
    public UnityEngine.Object beePrefab;
    public UnityEngine.Object cHivePrefab;
    public UnityEngine.Object hiveTilePrefab;
    public enum GameState
    {
        NAVIGATION,
        HIVE_SELECTED,
        BEE_SELECTED,
        FlOWER_SELECTED,
        HIVE_BUILD
    }
    public enum BeeCommand
    {
        GoHome,
        CollectHoney,
        Discover
    }
    public enum HiveCommand
    {
        AddHiveTile
    }
    private GameState _state = GameState.NAVIGATION;
    public GameState state
    {
        get
        {
            return _state;
        }

        set
        {
            _state = value;

            updateUI();
        }
    }
    public Bee selectedBee;
    public HiveTileController selectedHiveTile;
    public FlowerTileController selectedFlowerTile;
    public Dictionary<System.Action, WorkUnit> beesActions;
    public readonly object beesActionLock = new object();
    public Hive hive;
    private Vector2 lastMouseDownPosition = new Vector2(-1.0f, 1.0f);
    private Vector2 lastMouseUpPosition = new Vector2(-1.0f, 1.0f);
    public float beeNormalHeight;
    public bool workQueueChangedFlag = false;
    public bool addBee = false;

    private void initHive()
    {
        this.hive = new Hive("Chakmeshma", Color.red, 100000, 50000);
    }

    void Awake()
    {
        instance = this;

        uiEventStopWatch = Stopwatch.StartNew();

        initHive();

        beesActions = new Dictionary<System.Action, WorkUnit>();

        UnityThread.initUnityThread();
    }

    public static GameController getInstance()
    {
        return instance;
    }

    public void resetState()
    {
        setBeeOutline(selectedBee, false);
        setHiveOutline(selectedHiveTile, false);
        setFlowerOutline(selectedFlowerTile, false);
        setHiveOutline(selectedHiveTile, false);

        this.selectedBee = null;
        this.selectedHiveTile = null;
        this.selectedFlowerTile = null;

        switch (this.state)
        {
            case GameState.HIVE_BUILD:
                foreach (GameObject grassTile in lastDisabledGrassTiles)
                {
                    grassTile.SetActive(true);
                    int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                    int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                    HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                }

                foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
                {
                    Destroy(cHiveController.gameObject);
                }
                break;
        }

        this.state = GameState.NAVIGATION;
    }

    private Stopwatch stopWatch;
    private long lastMouseDownTime = 0;
    private long lastMouseUpTime = 0;

    void Start()
    {
        stopWatch = Stopwatch.StartNew();
    }

    private void LateUpdate()
    {
        if (workQueueChangedFlag)
        {
            if (selectedBee != null)
                selectedBee.workQueueChanged = true;
            workQueueChangedFlag = false;
        }
    }

    private bool IsPointerOverUIObject()
    {
        if (UIController.getInstance().controlPanelGameobject != null)
        {
            Vector2 pointerPosition = Input.mousePosition;

            float canvasWdith = UIController.getInstance().controlPanelGameobject.transform.parent.GetComponent<RectTransform>().rect.width;
            float canvasHeight = UIController.getInstance().controlPanelGameobject.transform.parent.GetComponent<RectTransform>().rect.height;

            Vector2 controlPanelPosition = UIController.getInstance().controlPanelGameobject.GetComponent<RectTransform>().anchoredPosition;
            Vector2 controlPanelDimensions = new Vector2(UIController.getInstance().controlPanelGameobject.GetComponent<RectTransform>().rect.width, UIController.getInstance().controlPanelGameobject.GetComponent<RectTransform>().rect.height);

            if (pointerPosition.x >= controlPanelPosition.x && pointerPosition.x <= controlPanelPosition.x + controlPanelDimensions.x &&
                pointerPosition.y <= (controlPanelPosition.y + canvasHeight) && pointerPosition.y >= (controlPanelPosition.y + canvasHeight) - controlPanelDimensions.y)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    List<long> timeStamps = new List<long>();

    void Update()
    {
        for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
        {
            for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
            {
                GameObject tile = null;

                int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                Vector3 tilePosition = new Vector3(HexController.getInstance().tileAdjustentDistance * (indexI - HexController.getInstance().numberOfVerticalTiles) + (HexController.getInstance().tileAdjustentDistance / 2.0f * (indexJ % 2)), 0.0f, 1.5f * (indexJ - HexController.getInstance().numberOfHorizontalTiles));
            }
        }

        List<System.Action> toDelete = new List<System.Action>();

        lock (beesActionLock)
        {
            if (beesActions != null && beesActions.Count > 0)
            {
                foreach (KeyValuePair<System.Action, WorkUnit> entry in beesActions)
                {
                    UnityThread.executeInUpdate(entry.Key);

                    toDelete.Add(entry.Key);
                }

                foreach (KeyValuePair<System.Action, WorkUnit> entry in beesActions)
                {
                    if (entry.Value.finished)
                    {
                        beesActions[entry.Key].bee.workQueue.Remove(beesActions[entry.Key]);
                        try
                        {
                            selectedBee.workQueueChanged = true;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
        }


        hive.updateBeesCurrentAction();


        lock (beesActionLock)
        {
            foreach (System.Action action in toDelete)
            {
                beesActions.Remove(action);
            }
        }


        if (addBee)
        {

            GameObject newBee = Instantiate(beePrefab, new Vector3(UnityEngine.Random.Range(-30.0f, 30.0f), beeNormalHeight, UnityEngine.Random.Range(-30.0f, 30.0f)), Quaternion.identity) as GameObject;

            newBee.AddComponent<Bee>();
            newBee.GetComponent<Bee>().init(Bee.BeeType.Worker, string.Format("Bee #{0}", UnityEngine.Random.Range(1001, 9999)), Color.red);

            hive.addBee(newBee.GetComponent<Bee>());

            addBee = false;
        }


        int touchId = -1;

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMouseDownTime = stopWatch.ElapsedMilliseconds;

                lastMouseDownPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                lastMouseUpTime = stopWatch.ElapsedMilliseconds;

                lastMouseUpPosition = Input.mousePosition;
            }
        }
        else
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchId = Input.GetTouch(0).fingerId;

                lastMouseDownTime = stopWatch.ElapsedMilliseconds;

                lastMouseDownPosition = Input.GetTouch(0).position;
            }

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchId = Input.GetTouch(0).fingerId;

                lastMouseUpTime = stopWatch.ElapsedMilliseconds;

                lastMouseUpPosition = Input.GetTouch(0).position;
            }
        }


        if (lastMouseUpTime - lastMouseDownTime < 500L && lastMouseUpTime - lastMouseDownTime > 0 && (lastMouseDownPosition - lastMouseUpPosition).magnitude < 20.0f)
        {
            lastMouseDownTime = 0L;

            if (uiEventStopWatch.ElapsedMilliseconds - lastUIEventTimeStamp <= 100.0f)
            {

                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            int layerMask = LayerMask.GetMask("Minimap", "Bee", "Default", "Outlined", "Flower");


            if (!IsPointerOverUIObject())
            {

                if (Physics.Raycast(ray, out hitInfo, 200.0f, layerMask))
                {
                    TileController tileController = null;
                    try
                    {
                        tileController = hitInfo.transform.GetComponent<TileController>();
                    }
                    catch (System.Exception e)
                    {

                    }

                    Bee bee = null;

                    try
                    {
                        bee = hitInfo.transform.GetComponent<Bee>();
                    }
                    catch (System.Exception e)
                    {

                    }

                    if (tileController != null)
                    {
                        if (tileController.actAsGrassTile)
                        {
                            onGrassTileClicked(tileController);
                        }
                        else
                        {
                            if (tileController is GrassTileController)
                            {
                                onGrassTileClicked((GrassTileController)tileController);
                            }
                            else if (tileController is FlowerTileController)
                            {
                                onFlowerTileClicked((FlowerTileController)tileController);
                            }
                            else if (tileController is HiveConstructionTileController)
                            {
                                onConstructionHiveTileClicked((HiveConstructionTileController)tileController);
                            }
                            else if (tileController is HiveBuildTileController)
                            {
                                onBuildHiveTileClicked((HiveBuildTileController)tileController);
                            }
                            else if (tileController is HiveTileController)
                            {
                                onHiveTileClicked((HiveTileController)tileController);
                            }
                        }
                    }
                    else if (bee != null)
                    {
                        onBeeClicked(bee);
                    }
                }
            }
        }


        accordMapVisibilities();

        calculateShowFPS();
    }

    private void calculateShowFPS()
    {
        if (timeStamps.Count > 8)
        {
            float totalElapsedTime = timeStamps[9] - timeStamps[0];
            float totalFPS = 1000.0f / totalElapsedTime;

            float biggestDiffTime = 0.0f;
            int biggestDiffTime1Index = -1;

            for (int i = 0; i < 8; i++)
            {
                float diffTime = timeStamps[i + 1] - timeStamps[i];

                if (biggestDiffTime == 0.0f)
                {
                    biggestDiffTime = diffTime;
                    biggestDiffTime1Index = i;
                }

                if (diffTime > biggestDiffTime)
                {
                    biggestDiffTime = diffTime;
                    biggestDiffTime1Index = i;
                }
            }

            DebugController.getInstance().DisplayDebugText(String.Format("Update Time: {0}ms ({1} FPS)", totalElapsedTime, totalFPS));

            timeStamps.Clear();
        }
    }

    private Bee lastSelectedBee;

    private bool firstVisibility = true;

    private void accordMapVisibilities()
    {
        int indexI = -1;
        int indexJ = -1;

        if (visibilitiesChanged)
        {
            if (firstVisibility)
            {
                for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
                {
                    for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                    {
                        GameObject tile = null;

                        indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                        indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                        tile = HexController.getInstance().tiles[indexI][indexJ].gameObject;

                        if (!(HexController.getInstance().tiles[indexI][indexJ] is HiveTileController))
                            tile.GetComponent<TileController>().setState(TileController.TileState.Invisible);
                    }
                }

                firstVisibility = false;
            }
            else
            {

                switch (state)
                {
                    case GameState.BEE_SELECTED:

                        if (lastSelectedBee == null)
                        {
                            lastSelectedBee = selectedBee;
                        }

                        if (lastSelectedBee != selectedBee)
                        {
                            for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
                            {
                                for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                                {
                                    GameObject tile = null;

                                    indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                                    indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                                    tile = HexController.getInstance().tiles[indexI][indexJ].gameObject;

                                    if (!(HexController.getInstance().tiles[indexI][indexJ] is HiveTileController))
                                        tile.GetComponent<TileController>().setState(TileController.TileState.Invisible);
                                }
                            }
                        }

                        lastSelectedBee = selectedBee;


                        List<Tuple<int, int>> relativeIndicesList = new List<Tuple<int, int>>();


                        #region relative indices
                        relativeIndicesList.Add(new Tuple<int, int>(-6, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-6, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(-5, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(-4, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(-3, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -6));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(-2, 6));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -6));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(-1, 6));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -6));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(0, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(0, 6));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -6));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(1, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(1, 6));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -6));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(2, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(2, 6));
                        relativeIndicesList.Add(new Tuple<int, int>(3, -5));
                        relativeIndicesList.Add(new Tuple<int, int>(3, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(3, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(3, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(3, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(3, 5));
                        relativeIndicesList.Add(new Tuple<int, int>(4, -4));
                        relativeIndicesList.Add(new Tuple<int, int>(4, -3));
                        relativeIndicesList.Add(new Tuple<int, int>(4, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(4, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(4, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(4, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(4, 2));
                        relativeIndicesList.Add(new Tuple<int, int>(4, 3));
                        relativeIndicesList.Add(new Tuple<int, int>(4, 4));
                        relativeIndicesList.Add(new Tuple<int, int>(5, -2));
                        relativeIndicesList.Add(new Tuple<int, int>(5, -1));
                        relativeIndicesList.Add(new Tuple<int, int>(5, 0));
                        relativeIndicesList.Add(new Tuple<int, int>(5, 1));
                        relativeIndicesList.Add(new Tuple<int, int>(5, 2));
                        #endregion


                        RaycastHit[] raycastHits;
                        Ray ray = new Ray(new Vector3(selectedBee.transform.position.x, 15.0f, selectedBee.transform.position.z), Vector3.down);

                        //selectedBee.GetComponent<Collider>().enabled = false;

                        raycastHits = Physics.RaycastAll(ray, 20.0f);

                        foreach (RaycastHit raycastHit in raycastHits)
                        {
                            if (raycastHit.transform.gameObject.GetComponent<TileController>() != null)
                            {
                                indexI = raycastHit.transform.gameObject.GetComponent<TileController>().indexI;
                                indexJ = raycastHit.transform.gameObject.GetComponent<TileController>().indexJ;
                            }
                        }

                        //selectedBee.GetComponent<Collider>().enabled = true;


                        foreach (Tuple<int, int> entry in relativeIndicesList)
                        {
                            try
                            {
                                TileController tileController = HexController.getInstance().tiles[indexI + entry.First][indexJ + entry.Second];
                                tileController.setState(TileController.TileState.Visible);
                                selectedBee.visitedTiles.Add(tileController);
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        foreach (TileController tile in selectedBee.visitedTiles)
                        {
                            try
                            {
                                if (!relativeIndicesList.Contains(new Tuple<int, int>(tile.indexI - indexI, tile.indexJ - indexJ)) && tile.getState() == TileController.TileState.Visible && !(tile is HiveTileController))
                                    tile.setState(TileController.TileState.Memorized);
                            }
                            catch (Exception e)
                            {

                            }
                        }


                        //for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
                        //{
                        //    for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                        //    {
                        //        GameObject tile = null;

                        //        indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                        //        indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                        //        tile = HexController.getInstance().tiles[indexI][indexJ].gameObject;
                        //        TileController tileController = HexController.getInstance().tiles[indexI][indexJ];

                        //        Vector3 tilePosition = new Vector3(HexController.getInstance().tileAdjustentDistance * (indexI - HexController.getInstance().numberOfVerticalTiles) + (HexController.getInstance().tileAdjustentDistance / 2.0f * (indexJ % 2)), 0.0f, 1.5f * (indexJ - HexController.getInstance().numberOfHorizontalTiles));

                        //        if ((new Vector2(selectedBee.transform.position.x - tilePosition.x, selectedBee.transform.position.z - tilePosition.z).magnitude < 10.0f && tileController.getState() != TileController.TileState.Visible) || tileController is HiveTileController)
                        //        {

                        //        }
                        //        else if ((new Vector2(selectedBee.transform.position.x - tilePosition.x, selectedBee.transform.position.z - tilePosition.z).magnitude >= 10.0f) && (tileController.getState() == TileController.TileState.Visible || selectedBee.visitedTiles.Contains(tileController)))
                        //        {
                        //            tileController.setState(TileController.TileState.Memorized);
                        //        }
                        //        else if ((new Vector2(selectedBee.transform.position.x - tilePosition.x, selectedBee.transform.position.z - tilePosition.z).magnitude >= 10.0f) && tileController.getState() == TileController.TileState.Visible || tile.GetComponent<TileController>().getState() == TileController.TileState.Unknown)
                        //        {
                        //            tileController.setState(TileController.TileState.Invisible);
                        //        }
                        //    }
                        //}
                        break;
                    case GameState.NAVIGATION:
                        //for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
                        //{
                        //    for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                        //    {
                        //        GameObject tile = null;

                        //        int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                        //        int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                        //        tile = HexController.getInstance().tiles[indexI][indexJ].gameObject;

                        //        if (!(HexController.getInstance().tiles[indexI][indexJ] is HiveTileController))
                        //            tile.GetComponent<TileController>().setState(TileController.TileState.Invisible);
                        //    }
                        //}
                        break;
                }
            }

            visibilitiesChanged = false;
        }
    }

    public void onTileClicked(TileController tile)
    {

    }

    private void setBeeOutline(Bee bee, bool outlined)
    {
        if (bee == null)
            return;

        if (outlined)
        {
            setBeeLayer(bee.gameObject, LayerMask.NameToLayer("Outlined"));
            bee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = true;
        }
        else
        {
            setBeeLayer(bee.gameObject, LayerMask.NameToLayer("Bee"));
            bee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = false;
        }
    }


    private void setHiveOutline(HiveTileController hiveTileController, bool outlined)
    {
        if (hiveTileController == null)
            return;

        if (outlined)
        {
            setHiveLayer(GameObject.FindObjectsOfType<HiveTileController>(), LayerMask.NameToLayer("Outlined"));
        }
        else
        {
            setHiveLayer(GameObject.FindObjectsOfType<HiveTileController>(), LayerMask.NameToLayer("Hive"));
        }
    }

    private void setHiveLayer(HiveTileController[] hiveTileControllers, int layerMask)
    {
        foreach (HiveTileController hiveTileController in hiveTileControllers)
        {
            if (!hiveTileController.gameObject.name.Contains("Hive"))
                hiveTileController.gameObject.layer = layerMask;

            foreach (Transform child in hiveTileController.gameObject.transform)
            {
                setHiveGameObjectLayer(child.gameObject, layerMask);
            }
        }
    }

    private void setHiveGameObjectLayer(GameObject gameObject, int layerMask)
    {
        if (!gameObject.name.Contains("Hive"))
            gameObject.layer = layerMask;

        foreach (Transform child in gameObject.transform)
        {
            setHiveGameObjectLayer(child.gameObject, layerMask);
        }
    }

    private void setFlowerOutline(FlowerTileController flower, bool outlined)
    {
        if (flower == null)
            return;

        if (outlined)
        {
            setFlowerLayer(flower.gameObject, LayerMask.NameToLayer("Outlined"));
            flower.gameObject.transform.Find("Plane (4)").GetComponent<Renderer>().enabled = true;
        }
        else
        {
            setFlowerLayer(flower.gameObject, LayerMask.NameToLayer("Default"));
            flower.gameObject.transform.Find("Plane (4)").GetComponent<Renderer>().enabled = false;
        }
    }

    public void onBeeClicked(Bee bee)
    {
        setBeeOutline(selectedBee, false);
        setHiveOutline(selectedHiveTile, false);
        setFlowerOutline(selectedFlowerTile, false);
        setHiveOutline(selectedHiveTile, false);
        setBeeOutline(bee, true);


        this.selectedBee = bee;
        this.selectedHiveTile = null;
        this.selectedFlowerTile = null;

        selectedBee.workQueueChanged = true;

        switch (this.state)
        {
            case GameState.HIVE_BUILD:
                foreach (GameObject grassTile in lastDisabledGrassTiles)
                {
                    grassTile.SetActive(true);
                    int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                    int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                    HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                }

                foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
                {
                    Destroy(cHiveController.gameObject);
                }
                break;
        }

        this.state = GameState.BEE_SELECTED;

        visibilitiesChanged = true;
    }

    public void onHiveTileClicked(HiveTileController hiveTileController)
    {
        if (GameState.BEE_SELECTED == state)
        {
            if (selectedBee.honey > 0)
            {
                selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(hiveTileController.transform.position.x, beeStoreHeight, hiveTileController.transform.position.z), selectedBee.workQueue.Count <= 0));
                selectedBee.workQueue.Add(new StoreHoneyWorkUnit(selectedBee, hiveTileController.GetComponent<TileController>().indexI, hiveTileController.GetComponent<TileController>().indexJ, selectedBee.workQueue.Count <= 0));
            }
            else
            {
                selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(hiveTileController.transform.position.x, beeNormalHeight, hiveTileController.transform.position.z), selectedBee.workQueue.Count <= 0));
            }

            selectedBee.workQueueChanged = true;
        }
        else
        {

            setBeeOutline(selectedBee, false);
            setHiveOutline(selectedHiveTile, false);
            setFlowerOutline(selectedFlowerTile, false);
            setHiveOutline(selectedHiveTile, false);
            setHiveOutline(hiveTileController, true);

            this.selectedBee = null;
            this.selectedHiveTile = hiveTileController;
            this.selectedFlowerTile = null;

            switch (this.state)
            {
                case GameState.HIVE_BUILD:
                    foreach (GameObject grassTile in lastDisabledGrassTiles)
                    {
                        grassTile.SetActive(true);
                        int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                        int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                        HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                    }

                    foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
                    {
                        Destroy(cHiveController.gameObject);
                    }
                    break;
            }

            this.state = GameState.HIVE_SELECTED;
        }

    }

    public void onBuildHiveTileClicked(HiveBuildTileController hiveBuildTile)
    {
        if (state == GameState.BEE_SELECTED)
        {
            selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(hiveBuildTile.transform.position.x, beeBuildHeight, hiveBuildTile.transform.position.z), selectedBee.workQueue.Count <= 0));
            selectedBee.workQueue.Add(new BuildWorkUnit(selectedBee, hiveBuildTile.GetComponent<HiveBuildTileController>().indexI, hiveBuildTile.GetComponent<HiveBuildTileController>().indexJ, selectedBee.workQueue.Count <= 0));

            selectedBee.workQueueChanged = true;
        }
    }

    public void onConstructionHiveTileClicked(HiveConstructionTileController hiveConstructionTile)
    {
        if (state == GameState.HIVE_BUILD)
        {
            Vector3 bHivePosition = hiveConstructionTile.transform.position;

            GameObject bHiveTile = Instantiate(bHivePrefab, bHivePosition, Quaternion.identity) as GameObject;

            GameObject toDeleteFromList = null;

            foreach (GameObject grassTile in lastDisabledGrassTiles)
            {
                if (grassTile.transform.position != hiveConstructionTile.transform.position)
                {
                    grassTile.SetActive(true);
                    int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                    int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                    HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                }
                else
                {
                    toDeleteFromList = grassTile;

                    grassTile.SetActive(false);
                    int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                    int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                    bHiveTile.GetComponent<HiveBuildTileController>().init(indexI, indexJ, hive);

                    HexController.getInstance().tiles[indexI][indexJ] = bHiveTile.GetComponent<HiveBuildTileController>();
                }

            }

            lastDisabledGrassTiles.Remove(toDeleteFromList);

            foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
            {
                Destroy(cHiveController.gameObject);
            }


            setBeeOutline(selectedBee, false);
            setHiveOutline(selectedHiveTile, false);
            setFlowerOutline(selectedFlowerTile, false);
            setHiveOutline(selectedHiveTile, false);

            this.selectedBee = null;
            this.selectedHiveTile = null;
            this.selectedFlowerTile = null;

            state = GameState.NAVIGATION;
        }
    }

    public void onGrassTileClicked(TileController grassTileController)
    {
        if (state == GameState.BEE_SELECTED)
        {
            selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(grassTileController.transform.position.x, beeNormalHeight, grassTileController.transform.position.z), selectedBee.workQueue.Count <= 0));

            selectedBee.workQueueChanged = true;
        }
        else
        {
            setBeeOutline(selectedBee, false);
            setHiveOutline(selectedHiveTile, false);
            setFlowerOutline(selectedFlowerTile, false);
            setHiveOutline(selectedHiveTile, false);

            this.selectedBee = null;
            this.selectedHiveTile = null;
            this.selectedFlowerTile = null;

            switch (this.state)
            {
                case GameState.HIVE_BUILD:
                    foreach (GameObject grassTile in lastDisabledGrassTiles)
                    {
                        grassTile.SetActive(true);
                        int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                        int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                        HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                    }

                    foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
                    {
                        Destroy(cHiveController.gameObject);
                    }
                    break;
            }

            this.state = GameState.NAVIGATION;
        }
    }

    public void onFlowerTileClicked(FlowerTileController flowerTileController)
    {
        if (state != GameState.BEE_SELECTED)
        {
            setBeeOutline(selectedBee, false);
            setHiveOutline(selectedHiveTile, false);
            setFlowerOutline(selectedFlowerTile, false);
            setHiveOutline(selectedHiveTile, false);
            setFlowerOutline(flowerTileController, true);
        }



        switch (this.state)
        {
            case GameState.HIVE_BUILD:
                foreach (GameObject grassTile in lastDisabledGrassTiles)
                {
                    grassTile.SetActive(true);
                    int indexI = grassTile.GetComponent<GrassTileController>().indexI;
                    int indexJ = grassTile.GetComponent<GrassTileController>().indexJ;

                    HexController.getInstance().tiles[indexI][indexJ] = grassTile.GetComponent<GrassTileController>();
                }

                foreach (HiveConstructionTileController cHiveController in FindObjectsOfType<HiveConstructionTileController>())
                {
                    Destroy(cHiveController.gameObject);
                }
                break;
            case GameState.BEE_SELECTED:
                selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(flowerTileController.transform.position.x, beeFlowerCollectHeight, flowerTileController.transform.position.z), selectedBee.workQueue.Count <= 0));
                selectedBee.workQueue.Add(new CollectHoneyWorkUnit(selectedBee, flowerTileController.GetComponent<TileController>().indexI, flowerTileController.GetComponent<TileController>().indexJ, selectedBee.workQueue.Count <= 0));

                selectedBee.workQueueChanged = true;

                return;
                break;
        }

        if (state != GameState.BEE_SELECTED)
        {
            this.selectedBee = null;
            this.selectedHiveTile = null;
            this.selectedFlowerTile = flowerTileController;

            this.state = GameState.FlOWER_SELECTED;
        }
    }

    public void onBeeCommandIssued(BeeCommand beeCommand)
    {
        switch (beeCommand)
        {
            case BeeCommand.GoHome:
                float leastDistance = -1.0f;

                HiveTileController hiveTileController = null;

                for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
                {
                    for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
                    {
                        int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                        int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                        Vector3 tilePosition = new Vector3(HexController.getInstance().tileAdjustentDistance * (indexI - HexController.getInstance().numberOfVerticalTiles) + (HexController.getInstance().tileAdjustentDistance / 2.0f * (indexJ % 2)), 0.0f, 1.5f * (indexJ - HexController.getInstance().numberOfHorizontalTiles));

                        if (((tilePosition - selectedBee.transform.position).magnitude < leastDistance || leastDistance == -1.0f) && HexController.getInstance().tiles[indexI][indexJ] is HiveTileController)
                        {
                            leastDistance = (tilePosition - selectedBee.transform.position).magnitude;
                            hiveTileController = (HiveTileController)HexController.getInstance().tiles[indexI][indexJ];
                        }
                    }
                }

                if (selectedBee.honey > 0)
                {
                    selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(hiveTileController.transform.position.x, beeStoreHeight, hiveTileController.transform.position.z), selectedBee.workQueue.Count <= 0));
                    selectedBee.workQueue.Add(new StoreHoneyWorkUnit(selectedBee, hiveTileController.GetComponent<TileController>().indexI, hiveTileController.GetComponent<TileController>().indexJ, selectedBee.workQueue.Count <= 0));
                }
                else
                {
                    selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(hiveTileController.transform.position.x, beeNormalHeight, hiveTileController.transform.position.z), selectedBee.workQueue.Count <= 0));
                }
                break;
            case BeeCommand.Discover:
                selectedBee.workQueue.Add(new DiscoverCollectHoneyWorkUnit(selectedBee, selectedBee.workQueue.Count <= 0));
                break;

        }

        selectedBee.workQueueChanged = true;
    }

    public void updateUI()
    {
        UIController.getInstance().closeAllCP();
        switch (state)
        {
            case GameState.BEE_SELECTED:
                UIController.getInstance().showBeeCP(selectedBee);
                break;
            case GameState.FlOWER_SELECTED:
                UIController.getInstance().showFlowerCP(selectedFlowerTile);
                break;
            case GameState.HIVE_SELECTED:
                UIController.getInstance().showHiveCP(selectedHiveTile);
                break;
        }
    }

    private void setBeeLayer(GameObject gameObject, int layer)
    {
        if (gameObject.name == "Plane" || gameObject.name == "Plane (1)")
            return;

        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            setBeeLayer(child.gameObject, layer);
        }
    }

    private void setFlowerLayer(GameObject gameObject, int layer)
    {
        if (gameObject.name == "Plane (3)" || gameObject.name == "Plane (4)")
            return;

        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            setFlowerLayer(child.gameObject, layer);
        }
    }

    public void onHiveCommandIssued(HiveCommand hiveCommand)
    {
        switch (hiveCommand)
        {
            case HiveCommand.AddHiveTile:
                turnHiveCircumferenceBuildable();
                state = GameState.HIVE_BUILD;
                break;
        }
    }

    private List<GameObject> lastDisabledGrassTiles;
    public UnityEngine.Object bHivePrefab;
    public float beeBuildHeight;
    public bool visibilitiesChanged = true;
    public float beeFlowerCollectHeight;
    public float beeStoreHeight;

    private void turnHiveCircumferenceBuildable()
    {

        lastDisabledGrassTiles = new List<GameObject>();

        for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
        {
            for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
            {
                int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
                int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

                Vector3 tilePosition = new Vector3(HexController.getInstance().tileAdjustentDistance * indexI + (HexController.getInstance().tileAdjustentDistance / 2.0f * (indexJ % 2)), 0.0f, 1.5f * indexJ);

                TileController tileController = HexController.getInstance().tiles[indexI][indexJ];

                if (tileController is HiveTileController && !(tileController is HiveConstructionTileController) && !(tileController is HiveBuildTileController))
                {
                    TileController upLeftTile = null;
                    TileController upRightTile = null;
                    TileController downLeftTile = null;
                    TileController downRightTile = null;
                    TileController leftTile = null;
                    TileController rightTile = null;

                    int upLeftTileIndexI = indexI + (((indexJ % 2) != 0) ? (1) : (0));
                    int upLeftTileIndexJ = indexJ - 1;

                    int upRightTileIndexI = indexI + (((indexJ % 2) != 0) ? (0) : (-1));
                    int upRightTileIndexJ = indexJ - 1;

                    int downLeftTileIndexI = indexI + (((indexJ % 2) != 0) ? (1) : (0));
                    int downLeftTileIndexJ = indexJ + 1;

                    int downRightTileIndexI = indexI + (((indexJ % 2) != 0) ? (0) : (-1));
                    int downRightTileIndexJ = indexJ + 1;

                    int leftTileIndexI = indexI + 1;
                    int leftTileIndexJ = indexJ;

                    int rightTileIndexI = indexI - 1;
                    int rightTileIndexJ = indexJ;

                    try
                    {
                        upLeftTile = HexController.getInstance().tiles[upLeftTileIndexI][upLeftTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        upRightTile = HexController.getInstance().tiles[upRightTileIndexI][upRightTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        downLeftTile = HexController.getInstance().tiles[downLeftTileIndexI][downLeftTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        downRightTile = HexController.getInstance().tiles[downRightTileIndexI][downRightTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        leftTile = HexController.getInstance().tiles[leftTileIndexI][leftTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        rightTile = HexController.getInstance().tiles[rightTileIndexI][rightTileIndexJ];
                    }
                    catch (Exception e)
                    {

                    }

                    if (upLeftTile != null && upLeftTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(upLeftTile.gameObject);
                        upLeftTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, upLeftTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(upLeftTileIndexI, upLeftTileIndexJ, hive);
                        HexController.getInstance().tiles[upLeftTileIndexI][upLeftTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }

                    if (upRightTile != null && upRightTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(upRightTile.gameObject);

                        upRightTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, upRightTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(upRightTileIndexI, upRightTileIndexJ, hive);
                        HexController.getInstance().tiles[upRightTileIndexI][upRightTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }

                    if (downLeftTile != null && downLeftTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(downLeftTile.gameObject);

                        downLeftTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, downLeftTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(downLeftTileIndexI, downLeftTileIndexJ, hive);
                        HexController.getInstance().tiles[downLeftTileIndexI][downLeftTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }

                    if (downRightTile != null && downRightTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(downRightTile.gameObject);

                        downRightTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, downRightTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(downRightTileIndexI, downRightTileIndexJ, hive);
                        HexController.getInstance().tiles[downRightTileIndexI][downRightTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }

                    if (leftTile != null && leftTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(leftTile.gameObject);

                        leftTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, leftTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(leftTileIndexI, leftTileIndexJ, hive);
                        HexController.getInstance().tiles[leftTileIndexI][leftTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }

                    if (rightTile != null && rightTile is GrassTileController)
                    {
                        lastDisabledGrassTiles.Add(rightTile.gameObject);


                        rightTile.gameObject.SetActive(false);

                        GameObject cHive = Instantiate(cHivePrefab, rightTile.transform.position, Quaternion.identity) as GameObject;
                        cHive.GetComponent<HiveConstructionTileController>().init(rightTileIndexI, rightTileIndexJ, hive);
                        HexController.getInstance().tiles[rightTileIndexI][rightTileIndexJ] = cHive.GetComponent<HiveConstructionTileController>();
                    }
                }
            }
        }

        //for (int i = -HexController.getInstance().numberOfVerticalTiles + 1; i < HexController.getInstance().numberOfVerticalTiles; i++)
        //{
        //    for (int j = -HexController.getInstance().numberOfHorizontalTiles + 1; j < HexController.getInstance().numberOfHorizontalTiles; j++)
        //    {
        //        int indexI = i + HexController.getInstance().numberOfVerticalTiles - 1;
        //        int indexJ = j + HexController.getInstance().numberOfHorizontalTiles - 1;

        //        if(indexI == 62)
        //        {
        //            HexController.getInstance().tiles[indexI][indexJ].gameObject.SetActive(false);
        //        }
        //    }
        //}
    }
}
