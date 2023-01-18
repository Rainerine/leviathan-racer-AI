using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIShortcut : MonoBehaviour {

    public Transform shortcutRoot;
    public List<Transform> shortcutWaypoints;
    public RaceManager raceManager;

    // Use this for initialization
    void Awake () {

        // Get Parent to all shortcut waypoints and run through them adding them to the list
        foreach (Transform waypoint in shortcutRoot)
        {
            shortcutWaypoints.Add(waypoint);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //If the AI car passes by this trigger generate a ranom number between 0 and 100 which will determine its percentage of taking the shortcut
        if(other.tag == "AI")
        {
            int takePercentage = Random.Range(0, 100);
            if (other.name == raceManager.racers.Last().name)
            {
                if (takePercentage > 50)
                {
                    Debug.Log("Take Shortcut 50%");
                    other.SendMessage("SwithToShortcut", shortcutWaypoints);
                }
                else
                {
                    Debug.Log("Go Normal way 50%");
                }
            }
            else
            {
                if (takePercentage > 80)
                {
                    Debug.Log("Take Shortcut 20%");
                    other.SendMessage("SwithToShortcut", shortcutWaypoints);
                }
                else
                {
                    Debug.Log("Go Normal way 80%");
                }
            }
        }
    }
}
