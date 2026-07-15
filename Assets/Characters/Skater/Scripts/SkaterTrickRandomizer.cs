using UnityEngine;

[AddComponentMenu("Ball Roller/Skater Trick Randomizer")]
[RequireComponent(typeof(Animator))]
public class SkaterTrickRandomizer : MonoBehaviour
{
    public float minInterval = 5f;
    public float maxInterval = 12f;

    Animator animator;
    float nextTrickTime;

    void Awake()
    {
        animator = GetComponent<Animator>();
        ScheduleNextTrick();
    }

    void Update()
    {
        if (Time.time >= nextTrickTime)
        {
            animator.SetTrigger("DoTrick");
            ScheduleNextTrick();
        }
    }

    void ScheduleNextTrick()
    {
        nextTrickTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
