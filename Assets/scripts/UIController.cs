using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    private static UIController instance;
    public static UIController getInstance()
    {
        return instance;
    }
    public GameObject workerBeeCP;
    public GameObject maleBeeCP;
    public GameObject flowerCP;
    public GameObject hiveCP;
    public Object moveWorkUnitIcon;
	private GameObject controlPanelGameobject = null;
	private Bee controlPanelBee = null;
    private FlowerTileController controlPanelFlower = null;
    private Hive hive = null;


    public void onGoHomePressed() {
		GameController.getInstance ().onBeeCommandIssued (GameController.BeeCommand.GoHome);
	}

	void Update() {
        if (controlPanelGameobject != null)
        {
            if (controlPanelGameobject != null && controlPanelBee != null)
            {
                Bee bee = controlPanelBee;

                controlPanelGameobject.transform.Find("Color").GetComponent<UnityEngine.UI.RawImage>().color = bee.color;
                controlPanelGameobject.transform.Find("Bee Name").GetComponent<Text>().text = bee.beeName;

                GameObject hpGameObject = controlPanelGameobject.transform.Find("HP Container").Find("HP").gameObject;
                GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

                hpGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.HP) / bee.maxHP));
                honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.honey) / bee.maxHoney));

                controlPanelGameobject.transform.Find("HP Text").GetComponent<Text>().text = string.Format("{0:0.0%}", (float)bee.HP / bee.maxHP);

                controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", (float)bee.honey / bee.maxHoney);

                if (bee.workQueueChanged)
                {
                    Transform workQueue = controlPanelGameobject.transform.Find("Work Queue");


                    List<Transform> children = new List<Transform>();
                    foreach (Transform child in workQueue)
                        children.Add(child);

                    for (int i = 0; i < children.Count; i++)
                    {
                        //Destroy(children[i].gameObject.GetComponent<RawImage>());
                        DestroyImmediate(children[i].gameObject);
                    }

                    for (int i = 0; i < bee.workQueue.Count; i++)
                    {
                        if (bee.workQueue[i] is MoveWorkUnit)
                        {
                            GameObject workUnitIcon = Instantiate(moveWorkUnitIcon, workQueue) as GameObject;
                            workUnitIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(58 * i, 0.0f);
                        }
                        //else if(bee.workQueue[i] is GoHomeWorkUnit)
                        //{

                        //} else if(bee.workQueue[i] is StoreHoneyWorkUnit)
                        //{

                        //} else if(bee.workQueue[i] is CollectHoneyWorkUnit)
                        //{

                        //}
                    }

                    bee.workQueueChanged = false;
                }
            }
            else if (hive != null)
            {
                controlPanelGameobject.transform.Find("Color").GetComponent<UnityEngine.UI.RawImage>().color = hive.color;
                controlPanelGameobject.transform.Find("Hive Name").GetComponent<Text>().text = hive.hiveName;

                GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

                honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)hive.honey) / hive.maxHoney));

                controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", (float)hive.honey / hive.maxHoney);
            }
        }
	}

    void Awake()
    {
        UIController.instance = this;
    }

    public void closeAllCP()
    {
        workerBeeCP.SetActive(false);
        maleBeeCP.SetActive(false);
        flowerCP.SetActive(false);
        hiveCP.SetActive(false);

        this.controlPanelGameobject = null;
        this.controlPanelBee = null;
        this.controlPanelFlower = null;
        this.hive = null;
    }

    public void showFlowerCP(FlowerTileController flower)
    {
        GameObject controlPanelGameobject = null;

        controlPanelGameobject = flowerCP;

        controlPanelGameobject.SetActive(true);

        GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

        honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)flower.honey) / flower.maxHoney));

        controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", flower.honey / flower.maxHoney);

        this.controlPanelGameobject = controlPanelGameobject;
        this.controlPanelFlower = flower;
    }

    public void showHiveCP(HiveTileController hiveTile)
    {
        GameObject controlPanelGameobject = null;

        controlPanelGameobject = hiveCP;

        controlPanelGameobject.SetActive(true);

        GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

        honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)hiveTile.hive.honey) / hiveTile.hive.maxHoney));

        controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", hiveTile.hive.honey / hiveTile.hive.maxHoney);

        this.controlPanelGameobject = controlPanelGameobject;
        this.hive = hiveTile.hive;
    }

    public void showBeeCP(Bee bee)
    {
        GameObject controlPanelGameobject = null;

        switch(bee.type)
        {
            case Bee.BeeType.Male:
                controlPanelGameobject = maleBeeCP;
                break;
            case Bee.BeeType.Worker:
                controlPanelGameobject = workerBeeCP;
                break;
        }

        controlPanelGameobject.SetActive(true);

        controlPanelGameobject.transform.Find("Bee Name").GetComponent<Text>().text = bee.beeName;

        GameObject hpGameObject = controlPanelGameobject.transform.Find("HP Container").Find("HP").gameObject;
        GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

        hpGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.HP) / bee.maxHP));
        honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.honey) / bee.maxHoney));

        controlPanelGameobject.transform.Find("HP Text").GetComponent<Text>().text = string.Format("{0:0.0%}", bee.HP / bee.maxHP);

        controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", bee.honey / bee.maxHoney);

        Transform workQueue = controlPanelGameobject.transform.Find("Work Queue");


        List<Transform> children = new List<Transform>();
        foreach(Transform child in workQueue)
            children.Add(child);

        for (int i = 0; i < children.Count; i++)
        {
            //Destroy(children[i].gameObject.GetComponent<RawImage>());
            DestroyImmediate(children[i].gameObject);
        }

        for(int i = 0; i < bee.workQueue.Count; i++)
        {
            if (bee.workQueue[i] is MoveWorkUnit)
            {
                GameObject workUnitIcon = Instantiate(moveWorkUnitIcon, workQueue) as GameObject;
            }
            //else if(bee.workQueue[i] is GoHomeWorkUnit)
            //{

            //} else if(bee.workQueue[i] is StoreHoneyWorkUnit)
            //{

            //} else if(bee.workQueue[i] is CollectHoneyWorkUnit)
            //{

            //}
        }

		this.controlPanelGameobject = controlPanelGameobject;
		this.controlPanelBee = bee;
    }
}
