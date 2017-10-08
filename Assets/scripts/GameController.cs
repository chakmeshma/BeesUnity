using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    private static GameController instance;
	public UnityEngine.Object beePrefab;
    public enum GameState
    {
        NAVIGATION,
        HIVE_SELECTED,
        BEE_SELECTED,
        FlOWER_SELECTED
    }
	public enum BeeCommand {
		GoHome,
		CollectHoney,
		Discover
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

    private void initHive()
    {
        this.hive = new Hive("Chakmeshma", Color.red, 1000, 500);
    }

    void Awake()
    {
        instance = this;

        initHive();

        beesActions = new Dictionary<System.Action, WorkUnit>();

        UnityThread.initUnityThread();
    }

    public static GameController getInstance()
    {
        return instance;
    }

    private Stopwatch stopWatch;
    private long lastMouseDownTime = 0;
    private long lastMouseUpTime = 0;

	void Start () {
        stopWatch = Stopwatch.StartNew();
	}
	
	void Update () {
        List<System.Action> toDelete = new List<System.Action>();

        lock (beesActionLock)
        {
            foreach (KeyValuePair<System.Action, WorkUnit> entry in beesActions)
            {
                UnityThread.executeInUpdate(entry.Key);

                toDelete.Add(entry.Key);
            }

            foreach (KeyValuePair<System.Action, WorkUnit> entry in beesActions)
		    {
			    if (entry.Value.finished) {
				    beesActions [entry.Key].bee.workQueue.Remove (beesActions [entry.Key]);
                    selectedBee.workQueueChanged = true;
                }
		    }
        }

        hive.updateBeesCurrentAction();

		lock(beesActionLock)
		{
			foreach(System.Action action in toDelete)
			{
				beesActions.Remove(action);
			}
		}

		if (Input.GetKeyDown (KeyCode.N)) {

			GameObject newBee = Instantiate (beePrefab, new Vector3 (UnityEngine.Random.Range (-30.0f, 30.0f), beeNormalHeight, UnityEngine.Random.Range (-30.0f, 30.0f)), Quaternion.identity) as GameObject;

			newBee.AddComponent<Bee> ();
			newBee.GetComponent<Bee> ().init (Bee.BeeType.Worker, string.Format ("Bee #{0}", UnityEngine.Random.Range (1001, 9999)), Color.red);

				hive.addBee(newBee.GetComponent<Bee>());
		}

        if(Input.GetMouseButtonDown(0))
        {
            lastMouseDownTime = stopWatch.ElapsedMilliseconds;

            lastMouseDownPosition = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            lastMouseUpTime = stopWatch.ElapsedMilliseconds;

            lastMouseUpPosition = Input.mousePosition;
        }

        if(lastMouseUpTime - lastMouseDownTime < 500L && lastMouseUpTime - lastMouseDownTime > 0 && (lastMouseDownPosition - lastMouseUpPosition).magnitude < 20.0f)
        {
            lastMouseDownTime = 0L;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            int layerMask = LayerMask.GetMask("Minimap", "Bee", "Default", "Outlined", "Flower");

            if (!EventSystem.current.IsPointerOverGameObject())
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
                        if (tileController is GrassTileController)
                        {
                            onGrassTileClicked((GrassTileController)tileController);
                        }
                        else if (tileController is HiveTileController)
                        {
                            onHiveTileClicked((HiveTileController)tileController);
                        }
                        else if (tileController is FlowerTileController)
                        {
                            onFlowerTileClicked((FlowerTileController)tileController);
                        }

                    }
                    else if (bee != null)
                    {
                        onBeeClicked(bee);
                    }
                }
            }
        }
    }

    public void onTileClicked(TileController tile)
    {

    }

    private void setBeeOutline(Bee bee, bool outlined)
    {
        if (bee == null)
            return;

        if(outlined)
        {
            setBeeLayer(bee.gameObject, LayerMask.NameToLayer("Outlined"));
            bee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = true;
        } else
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
            setHiveLayer(GameObject.FindObjectsOfType<HiveTileController>(), LayerMask.NameToLayer("Outlined"));        }
        else
        {
            setHiveLayer(GameObject.FindObjectsOfType<HiveTileController>(), LayerMask.NameToLayer("Hive"));
        }
    }

    private void setHiveLayer(HiveTileController[] hiveTileControllers, int layerMask)
    {
        foreach (HiveTileController hiveTileController in hiveTileControllers)
        {
            if (!hiveTileController.gameObject.name.Contains("Hive Tile"))
                hiveTileController.gameObject.layer = layerMask;

            foreach (Transform child in hiveTileController.gameObject.transform)
            {
                setHiveGameObjectLayer(child.gameObject, layerMask);
            }
        }
    }

    private void setHiveGameObjectLayer(GameObject gameObject, int layerMask)
    {
        if (!gameObject.name.Contains("Hive Tile"))
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

        this.state = GameState.BEE_SELECTED;
    }

    public void onHiveTileClicked(HiveTileController hiveTileController)
    {
        setBeeOutline(selectedBee, false);
        setHiveOutline(selectedHiveTile, false);
        setFlowerOutline(selectedFlowerTile, false);
        setHiveOutline(selectedHiveTile, false);
        setHiveOutline(hiveTileController, true);

        this.selectedBee = null;
        this.selectedHiveTile = hiveTileController;
        this.selectedFlowerTile = null ;

        this.state = GameState.HIVE_SELECTED;
    }

    public void onGrassTileClicked(GrassTileController grassTileController)
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

            this.state = GameState.NAVIGATION;
        }
    }

    public void onFlowerTileClicked(FlowerTileController flowerTileController)
    {
        setBeeOutline(selectedBee, false);
        setHiveOutline(selectedHiveTile, false);
        setFlowerOutline(selectedFlowerTile, false);
        setHiveOutline(selectedHiveTile, false);
        setFlowerOutline(flowerTileController, true);

        this.selectedBee = null;
        this.selectedHiveTile = null;
        this.selectedFlowerTile = flowerTileController;

        this.state = GameState.FlOWER_SELECTED;

    }

    public void onBeeCommandIssued(BeeCommand beeCommand)
    {
        switch (beeCommand)
        {
            case BeeCommand.GoHome:
                selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(0.0f, 6.51f, 0.0f), selectedBee.workQueue.Count <= 0));
                break;
            case BeeCommand.Discover:
                selectedBee.workQueue.Add(new DiscoverWorkUnit(selectedBee, selectedBee.workQueue.Count <= 0));
                break;

        }

        selectedBee.workQueueChanged = true;
    }

    public void updateUI() {
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

        foreach(Transform child in gameObject.transform)
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
}
