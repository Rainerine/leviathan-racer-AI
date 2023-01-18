using UnityEngine;
using System.Collections;

public class AIShortcutFinishPoint : MonoBehaviour {

    //Saves the return waypoint value for the shortcut
    public Transform returnWaypoint;
    
    void OnTriggerEnter(Collider other)
    {
        //Sends the message to the car control that we are at the end of the shortcut and need to return to the main pathway
        if (other.tag == "AI")
        {
            other.SendMessage("SwitchBackToMainPath", returnWaypoint);
        }

    }    
}
