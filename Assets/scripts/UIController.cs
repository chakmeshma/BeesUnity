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


    void Awake()
    {
        UIController.instance = this;
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
    }
}
