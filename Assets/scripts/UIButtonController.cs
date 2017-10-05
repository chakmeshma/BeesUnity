using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonController : MonoBehaviour {
    public enum ButtonType
    {
        CollectHoneyButton,
        GoHomeButton,
        DiscoverButton,
        GoHomeWorkQueueButton
    }

    public ButtonType buttonType;

	public void onClicked() {
		switch (buttonType) {
		case ButtonType.CollectHoneyButton:
			break;
		case ButtonType.DiscoverButton:
			break;
		case ButtonType.GoHomeButton:
			GameController.getInstance ().onBeeCommandIssued (GameController.BeeCommand.GoHome);
			break;
        case ButtonType.GoHomeWorkQueueButton:
            List<WorkUnit> toDelete = new List<WorkUnit>();

            lock (GameController.getInstance().beesActionLock) { 
            
                foreach(KeyValuePair<System.Action, WorkUnit> entry in GameController.getInstance().beesActions)
                {
                    if(entry.Value.bee == GameController.getInstance().selectedBee && entry.Value is MoveWorkUnit)
                    {
                            entry.Value.finished = true;
                    }
                }


            }
            break;
		}
	}
}
