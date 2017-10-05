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
    public Object moveWorkUnitIcon;
	private GameObject controlPanelGameobject = null;
	private Bee controlPanelBee = null;

	public void onGoHomePressed() {
		GameController.getInstance ().onBeeCommandIssued (GameController.BeeCommand.GoHome);
	}

	void Update() {
		if (controlPanelGameobject != null && controlPanelBee != null)
		{
			Bee bee = controlPanelBee;

			controlPanelGameobject.transform.Find("Bee Name").GetComponent<Text>().text = bee.beeName;

			GameObject hpGameObject = controlPanelGameobject.transform.Find("HP Container").Find("HP").gameObject;
			GameObject honeyGameObject = controlPanelGameobject.transform.Find("Honey Container").Find("Honey").gameObject;

			hpGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.HP) / bee.maxHP));
			honeyGameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0.0f, 208.432f, ((float)bee.honey) / bee.maxHoney));

			controlPanelGameobject.transform.Find("HP Text").GetComponent<Text>().text = string.Format("{0:0.0%}", bee.HP / bee.maxHP);

			controlPanelGameobject.transform.Find("Honey Text").GetComponent<Text>().text = string.Format("{0:0.0%}", bee.honey / bee.maxHoney);

			


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
	}

    void Awake()
    {
        UIController.instance = this;
    }

	public void closeBeeCP() {
		workerBeeCP.SetActive(false);
		maleBeeCP.SetActive(false);

		this.controlPanelGameobject = null;
		this.controlPanelBee = null;
	}

    public void showBeeCP(Bee bee)
    {
        workerBeeCP.SetActive(false);
        maleBeeCP.SetActive(false);


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
