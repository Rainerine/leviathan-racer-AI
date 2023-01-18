using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class AIControl : MonoBehaviour
    {

        public List<Transform> waypoints;
        public Transform targetWaypoint;
        public float maxSteeringAngle = 60f;
        public float acceleration = 0.6f;
        public float braking = 0f;

        public Sensor sensorFrontLeft;
        public Sensor sensorFrontRight;
        public Sensor sensorFrontLeftAngled;
        public Sensor sensorFrontRightAngled;
        public Sensor sensorSideLeft;
        public Sensor sensorSideRight;

        public bool imminentCollision;
        private bool isReversing;

        private CarController carController; //reference to the car controller
        private Vector3 carToTarget;
        private Vector3 targetDirection;
        private float targetDistance;
        private float steeringAngle;
        private int targetWaypointIndex;

        private List<Transform> tempWaypoints;

        private void Awake()
        {
            //Get Car Controller script reference
            carController = GetComponent<CarController>();

            // Get Parent to all waypoints and run through them adding them to the list
            Transform waypointRoot = GameObject.Find("Waypoints").transform;

            foreach (Transform waypoint in waypointRoot)
            {
                waypoints.Add(waypoint);
            }

            targetWaypoint = waypoints[0];
        }

        private void FixedUpdate()
        {         
            carToTarget = new Vector3(targetWaypoint.position.x - transform.position.x, transform.position.y, targetWaypoint.position.z - transform.position.z);

            //Get targets distance and direction
            targetDistance = carToTarget.magnitude;
            targetDirection = carToTarget / targetDistance;

            //Get car direction right or left
            float targetDirectionSign = Mathf.Sign(Vector3.Dot(transform.right, targetDirection));

            //Get Steering Angle
            steeringAngle = targetDirectionSign * Vector3.Angle(transform.forward, targetDirection);
            braking = 0;
            acceleration = 0.6f;
            float maxSteeringAngle = 180f;

            // If there is an osbtacle sensed by only the left or right angled sensors override the steering angle
            if (sensorFrontRightAngled.hit && targetDirectionSign == 1f)
            {
                steeringAngle = 0;
            }
            if (sensorFrontLeftAngled.hit && targetDirectionSign == -1f)
            {
                steeringAngle = 0;
            }

            //If there is an obstacle on one side that is close steer slightly away form it
            if (sensorFrontRightAngled.hit)
            {
                steeringAngle += -maxSteeringAngle * sensorFrontRightAngled.hitPercentage * sensorFrontRightAngled.sensitivity;
            }
            if (sensorFrontLeftAngled.hit)
            {
                steeringAngle += maxSteeringAngle * sensorFrontLeftAngled.hitPercentage * sensorFrontLeftAngled.sensitivity;
            }
            //If there is an ostacle that approaches from the sides only
            if (sensorSideLeft.hit)
            {
                steeringAngle += maxSteeringAngle * sensorSideLeft.hitPercentage * sensorSideLeft.sensitivity;
            }
            if (sensorSideRight.hit)
            {
                steeringAngle += -maxSteeringAngle * sensorSideRight.hitPercentage * sensorSideRight.sensitivity;
            }
            //If only the front sensors are hit, turn away
            if (sensorFrontRight.hit)
            {
                steeringAngle += -maxSteeringAngle * sensorFrontRight.hitPercentage * sensorFrontRight.sensitivity;
            }
            if (sensorFrontLeft.hit)
            {
                steeringAngle += maxSteeringAngle * sensorFrontLeft.hitPercentage * sensorFrontLeft.sensitivity;
            }
            // if imminent collision ahead 
            if (sensorFrontRight.hit && sensorFrontLeft.hit)
            {

                // brake
                if (carController.CurrentSpeed > 60f)
                {
                    braking = -1f;
                }
                else
                {
                    acceleration = 0.5f;
                }

                // if both sensors are already way too close to wall reverse the car
                if (sensorFrontRight.hitPercentage > 0.8f && sensorFrontLeft.hitPercentage > 0.8f)
                {
                    isReversing = true;
                }

                // steer left or right
                if (sensorFrontRight.hitDistance < sensorFrontLeft.hitDistance)
                {
                    // turn left
                    steeringAngle = -maxSteeringAngle * sensorFrontRight.hitPercentage * sensorFrontRight.sensitivity;
                }
                else
                {
                    // turn right
                    steeringAngle = maxSteeringAngle * sensorFrontLeft.hitPercentage * sensorFrontLeft.sensitivity;
                }
            }

            //Reverses the car until the front sensors are clear of any obstacles
            if (isReversing)
            {
                braking = -1f;
                if(!sensorFrontRight.hit && !sensorFrontLeft.hit)
                {
                    isReversing = false;
                }
            }
            //Send information to car move function
            carController.Move(steeringAngle / 180f, acceleration, braking, braking);

        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Waypoint")
            {
                if(other.transform == targetWaypoint)
                {
                    if(tempWaypoints != null && tempWaypoints.Count > 0)
                    {
                        SetNextWaypoint(tempWaypoints);
                    }
                    else
                    {
                        SetNextWaypoint(waypoints);
                    }
                }

            }
        }

        void SetNextWaypoint(List<Transform> waypointList)
        {
                //Sets the next waypoint by the index number
                targetWaypointIndex = (targetWaypointIndex + 1) % waypointList.Count;
                targetWaypoint = waypointList[targetWaypointIndex];
        }

        void SwithToShortcut(List<Transform> waypointList)
        {
                //Resets index and sets the temporary waypoint value to the shortcut waypoint list
                targetWaypointIndex = 0;
                tempWaypoints = waypointList;
                //Sets initial waypoint
                SetNextWaypoint(tempWaypoints);
        }
        void SwitchBackToMainPath(Transform returnWaypoint)
        {
            for(int i = 0; i < waypoints.Count; i++)
            {
                if(waypoints[i].position == returnWaypoint.position && returnWaypoint != null)
                {
                    //Switch back to main pathway as designated by final shortcut waypoint
                    targetWaypoint = waypoints[i];
                    targetWaypointIndex = i - 1;

                }
            }
            tempWaypoints = null;
        }
    }
}
