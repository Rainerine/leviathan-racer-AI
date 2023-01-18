using UnityEngine;
using System.Collections.Generic;

public class RacePath : MonoBehaviour {

    public Transform markerParent;
    public List<Segment> racePathSegments;
    public List<Transform> racePathMarkers;
    public Transform segmentParent;
    public GameObject segmentPrefab;
    public float distanceBetweenSegments;
    public float lapDistance;
    public Color debugSegmentColor;
    float markerDistance;

    void Awake() {
        // add all race path markers to a list
        foreach (Transform child in markerParent) {
            racePathMarkers.Add(child);
        }
    }

    void Start() {
        // if distance between segments hasn't been set in the inspector, exit early to avoid division by zero
        if (distanceBetweenSegments == 0) return;
        CreateRacePathSegments();
    }

    /// <summary>
    /// Creates the race path segments using the race path markers
    /// </summary>
    void CreateRacePathSegments() {
        // loop through all markers
        for (int i = 0; i < racePathMarkers.Count; i++) {
            // get a reference to the current marker
            Transform currentMarker = racePathMarkers[i];
            // get the vector to the next marker
            Vector3 currentToNextMarker = racePathMarkers[(i + 1) % racePathMarkers.Count].position - currentMarker.position;
            // calculate the distance to the next marker
            float distanceToNextMarker = currentToNextMarker.magnitude;
            // get the normalized direction to the next marker
            Vector3 directionToNextMarker = currentToNextMarker / distanceToNextMarker;

            // calculate how many steps to the next marker
            int steps = Mathf.CeilToInt(distanceToNextMarker / distanceBetweenSegments);

            // set the initial distance along the track from this marker;
            float distanceAlongTrack = markerDistance;
            
            // step along the direction toward the next marker, laying down segments as we go
            for (int j = 0; j < steps; j++) {
                // create a new segment and place it along the way to the next marker
                Segment segment = (Instantiate(segmentPrefab, currentMarker.position + directionToNextMarker * j * distanceBetweenSegments, Quaternion.identity) as GameObject).GetComponent<Segment>();
                // make the segment a child of the segments parent
                segment.transform.parent = segmentParent;
                // add the segment to the segments list
                racePathSegments.Add(segment);
                // set the distance along the track to the distance along the track
                segment.distanceAlongTrack = distanceAlongTrack;
                // calculate the next distance along the track
                distanceAlongTrack += distanceBetweenSegments;
            }
            // update the total marker distance
            markerDistance += distanceToNextMarker;
            // update the lapdistance by adding up all of the distance markers
            lapDistance += distanceToNextMarker;
        }
    }



    /// <summary>
    /// Returns the closest segment to a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Segment GetClosestSegment(Vector3 point) {

        // set the closest segment to far away
        float closestDistance = Mathf.Infinity;
        Segment closestSegment = null;

        // loop through each segment to determine which is closest - (slow and inefficient, but simple)
        for (int i = 0; i < racePathSegments.Count; i++) {
            // get a reference to the current segment
            Segment segment = racePathSegments[i];
            // get the distance to the segment
            float segmentDistance = Vector3.Distance(point, segment.transform.position);
            // if distance is smaller than the closest distance
            if (segmentDistance < closestDistance) {
                // update closest distance and segment
                closestDistance = segmentDistance;
                closestSegment = segment;
            }
        }

        Debug.DrawLine(point, closestSegment.transform.position, debugSegmentColor);
        return closestSegment;
    }


    /// <summary>
    /// Returns the distance along the track queried from the closest segment from a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public float GetDistanceAlongTrack(Vector3 point) {
        Segment closestSegment = GetClosestSegment(point);
        return closestSegment.distanceAlongTrack;
    }
}
