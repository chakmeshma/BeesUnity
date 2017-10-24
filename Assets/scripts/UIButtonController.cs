using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : MonoBehaviour
{
    public enum ButtonType
    {
        AddBeeButton,
        CollectHoneyButton,
        GoHomeButton,
        DiscoverButton,
        MoveWorkQueueButton,
        BuildHiveTileButton,
        BuildWorkQueueButton,
        CPCloseButton,
        StoreWorkQueueButton
    }

    public ButtonType buttonType;

    public void onClicked()
    {
        switch (buttonType)
        {
            case ButtonType.CollectHoneyButton:
                break;
            case ButtonType.DiscoverButton:
                GameController.getInstance().onBeeCommandIssued(GameController.BeeCommand.Discover);
                break;
            case ButtonType.AddBeeButton:
                GameController.getInstance().addBee = true;
                break;
            case ButtonType.GoHomeButton:
                GameController.getInstance().onBeeCommandIssued(GameController.BeeCommand.GoHome);
                break;
            case ButtonType.BuildHiveTileButton:
                GameController.getInstance().lastUIEventTimeStamp = GameController.getInstance().uiEventStopWatch.ElapsedMilliseconds;
                GameController.getInstance().onHiveCommandIssued(GameController.HiveCommand.AddHiveTile);
                break;
            case ButtonType.MoveWorkQueueButton:
            case ButtonType.BuildWorkQueueButton:
            case ButtonType.StoreWorkQueueButton:
                int workUnitIndex = Mathf.RoundToInt(this.GetComponent<RectTransform>().anchoredPosition.x / 58.0f);

                WorkUnit workUnit = null;

                try { workUnit = GameController.getInstance().selectedBee.workQueue[workUnitIndex]; }
                catch (System.Exception e)
                {
                    return;
                }


                { 
                    workUnit.stop();
                    GameController.getInstance().selectedBee.workQueue.Remove(workUnit);
                }


                GameController.getInstance().workQueueChangedFlag = true;
                break;
            case ButtonType.CPCloseButton:
                GameController.getInstance().resetState();
                break;
        }
    }
}
