namespace Dreamteck.Forever
{
    using UnityEngine;

    [AddComponentMenu("Ball Roller/Ball Lane Input")]
    [RequireComponent(typeof(LaneRunner))]
    public class BallLaneInput : MonoBehaviour
    {
        LaneRunner runner;

        void Awake()
        {
            runner = GetComponent<LaneRunner>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) runner.lane--;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) runner.lane++;
        }
    }
}
