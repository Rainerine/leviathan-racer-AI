using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Sensor : MonoBehaviour {

    public string layerToSense;
    public float magnitude;
    public bool hit;
    public bool rightSensor;
    public float sensitivity = 1f;
    public float hitDistance;
    public float hitPercentage;

    public Color safeColor;
    public Color warningColor;
    public Color dangerColor;

	// Update is called once per frame
	void Update () {


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        int layerMask = 1 << LayerMask.NameToLayer(layerToSense);

        hit = Physics.Raycast(ray, out hitInfo, magnitude, layerMask);

        if (hit)
        {
            hitDistance = hitInfo.distance; //Distance to hit
            hitPercentage = 1 - hitDistance / magnitude; //Percentage to hit destination
            Debug.DrawLine(ray.origin, hitInfo.point, Color.Lerp(warningColor, dangerColor, hitPercentage));
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + transform.forward * magnitude, safeColor);
            hitPercentage = 0;
        }
	}
}
