using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    private static GameController instance;
	public Object beePrefab;
    public enum GameState
    {
        NAVIGATION,
        HIVE_SELECTED,
        BEE_SELECTED
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
	public Dictionary<System.Action, WorkUnit> beesActions;
    public readonly object beesActionLock = new object();
    public Hive hive;

    private void initHive()
    {
        this.hive = new Hive(1000, 500);
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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
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
			GameObject newBee = Instantiate (beePrefab, new Vector3 (Random.Range (-30.0f, 30.0f), 6.51f, Random.Range (-30.0f, 30.0f)), Quaternion.identity) as GameObject;

			newBee.AddComponent<Bee> ();
			newBee.GetComponent<Bee> ().init (Bee.BeeType.Worker, string.Format ("Bee #{0}", Random.Range (1001, 9999)));

				hive.addBee(newBee.GetComponent<Bee>());
		}

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            int layerMask = LayerMask.GetMask("Minimap", "Bee", "Default", "Outlined");

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if(Physics.Raycast(ray, out hitInfo, 200.0f, layerMask))
                {
                    TileController tileController = null;
                    try
                    {
                        tileController = hitInfo.transform.GetComponent<TileController>();
                    } catch(System.Exception e)
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
                        } else if (tileController is HiveTileController)
                        {
                            Debug.Log("Hive clicked");
                        }

                    } else if(bee != null)
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

    public void onBeeClicked(Bee bee)
    {
        if (this.selectedBee != null)
        {
            setBeeLayer(this.selectedBee.gameObject, LayerMask.NameToLayer("Bee"));
            this.selectedBee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = false;
        }

        this.selectedBee = bee;
        this.selectedHiveTile = null;

        setBeeLayer(this.selectedBee.gameObject, LayerMask.NameToLayer("Outlined"));
        this.selectedBee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = true;
        selectedBee.workQueueChanged = true;

        this.state = GameState.BEE_SELECTED;
    }

    public void onHiveTileClicked(HiveTileController hiveTileController)
    {
    }

    public void onGrassTileClicked(GrassTileController grassTileController)
    {
        if (this.selectedBee != null)
        {
            setBeeLayer(this.selectedBee.gameObject, LayerMask.NameToLayer("Bee"));
            this.selectedBee.gameObject.transform.Find("Plane (1)").GetComponent<Renderer>().enabled = false;
        }

        this.selectedBee = null;
        this.selectedHiveTile = null;

        this.state = GameState.NAVIGATION;
    }

	public void onBeeCommandIssued(BeeCommand beeCommand) {
		switch (beeCommand) {
		case BeeCommand.GoHome:

                if (selectedBee.workQueue.Count > 0)
                {
                    selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(0.0f, 6.51f, 0.0f), false));
                    selectedBee.workQueueChanged = true;
                }
                else
                {
                    selectedBee.workQueue.Add(new MoveWorkUnit(selectedBee, new Vector3(0.0f, 6.51f, 0.0f), true));
                    selectedBee.workQueueChanged = true;
                }
                break;
		}
	}

    public void updateUI() {
        UIController.getInstance().closeBeeCP();

        switch(state)
        {
            case GameState.BEE_SELECTED:
                UIController.getInstance().showBeeCP(selectedBee);
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
}
