using UnityEngine;
using System.Collections;

//Declare difficulities
public enum SkillLevel {
    Easy, Medium, Hard
}

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]

    public class AIRanking : MonoBehaviour
    {
        public SkillLevel skill;
        public float targetDistance;
        public float maxTargetDistance;
        public RacePath racepath;
        public float maxFullTorque;
        public float minFullTorque;

        private CarController carController; //reference to the car controller
        private DistanceTracker aiDistance; //tracks distance of AI
        private DistanceTracker playerDistance; //tracks distance of player
        private GameObject player; //reference to player's car
        private LapTracker lapTracker; //reference to AI's lap tracker

        private float targetDistanceForward;
        private float targetDistanceBackward;
        private float distanceToTargetPoint;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //Get Car and AI Controller script references
            carController = GetComponent<CarController>();
            aiDistance = GetComponent<DistanceTracker>();
            playerDistance = player.GetComponent<DistanceTracker>();
            lapTracker = GetComponent<LapTracker>();
        }

        // Update is called once per frame
        void Update()
        {
            TargetDistancefromPlayer();

            //When AI Car starts the race do the following depending on difficulty
            if (lapTracker.lap == 1)
            {
                if(skill.ToString() == "Easy")
                {
                    targetDistance = playerDistance.distanceAlongTrack;         //Target is with player
                }
                if (skill.ToString() == "Medium")
                {
                    targetDistance = targetDistanceForward;                     //Target is ahead of player
                }
                if (skill.ToString() == "Hard")
                {
                    targetDistance = targetDistanceForward;                     //Target is ahead of player
                }
            }
            //When AI Car has reached the 2nd lap
            if (lapTracker.lap == 2)
            {
                if (skill.ToString() == "Easy")
                {
                    targetDistance = targetDistanceBackward;                    //Target is behind player
                }
                if (skill.ToString() == "Medium")
                {
                    targetDistance = playerDistance.distanceAlongTrack;         //Target is with player
                }
                if (skill.ToString() == "Hard")
                {
                    targetDistance = targetDistanceForward;                     //Target is ahead of player
                }
            }
            //If AI car has reached the third lap
            if (lapTracker.lap == 3)
            {
                if (skill.ToString() == "Easy")
                {
                    targetDistance = targetDistanceBackward;                    //Target is behind the player
                }
                if (skill.ToString() == "Medium")
                {
                    targetDistance = targetDistanceBackward;                    //Target is behind the player
                }
                if (skill.ToString() == "Hard")
                {
                    targetDistance = playerDistance.distanceAlongTrack;         //Target is with the player
                }
            }

            // Changes the torque of the AI vehicle by calculating the distance between the target and the AI car /100 clamped to a 1 to -1 ratio
       
            distanceToTargetPoint = targetDistance - aiDistance.distanceAlongTrack;

            //If distance is greater or less than -max target distance
            if(distanceToTargetPoint > maxTargetDistance || distanceToTargetPoint < maxTargetDistance)
            {
                //Speed up or slow down to get to target
                ChangeSpeed(distanceToTargetPoint / 100);
            }
            else 
            {
                //Adjust target to player when within max target distance
                ChangeSpeed(playerDistance.distanceAlongTrack - aiDistance.distanceAlongTrack / 100);
            }

        }
        //Get max values for target distance from players distance along track
        void TargetDistancefromPlayer()
        {
            //Distance forward from player
            targetDistanceForward = playerDistance.distanceAlongTrack + maxTargetDistance;
            //Distance behind player
            targetDistanceBackward = playerDistance.distanceAlongTrack - maxTargetDistance;
        }

        //Function to change the torque of the car by in a range
        void ChangeSpeed(float value)
        {
            //Chang torque inbetween min and max values as defined per diffiuclty or throughout race
                carController.m_FullTorqueOverAllWheels = Mathf.Clamp(carController.m_FullTorqueOverAllWheels + value, minFullTorque, maxFullTorque);
        }
    }
}
