using UnityEngine;

[RequireComponent(typeof(LapTracker))]

public class DistanceTracker : MonoBehaviour {
    public float distanceAlongTrack;
    public RacePath racePath;

    LapTracker lapTracker;

    void Awake() {
        lapTracker = GetComponent<LapTracker>();
    }

    // Update is called once per frame
    void Update() {
        // the distance along the track calculated using the lap #, lapt distance and distance along track
        distanceAlongTrack = (lapTracker.lap - 1) * racePath.lapDistance + racePath.GetDistanceAlongTrack(transform.position);
    }
}
