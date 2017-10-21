using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : MonoBehaviour
{
    public enum ButtonType
    {
        CollectHoneyButton,
        GoHomeButton,
        DiscoverButton,
        MoveWorkQueueButton,
        BuildHiveTileButton,
        BuildWorkQueueButton
    }

    public ButtonType buttonType;

    public void onClicked()
    {
        switch (buttonType)
        {
            case ButtonType.CollectHoneyButton:
                break;
            case ButtonType.DiscoverButton:
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
                int workUnitIndex = Mathf.RoundToInt(this.GetComponent<RectTransform>().anchoredPosition.x / 58.0f);
                WorkUnit workUnit = GameController.getInstance().selectedBee.workQueue[workUnitIndex];


                { 
                    workUnit.stop();
                    GameController.getInstance().selectedBee.workQueue.Remove(workUnit);
                }


                GameController.getInstance().workQueueChangedFlag = true;
                break;
        }
    }
}
