using UnityEngine;

namespace RememberMe
{
    public sealed class Act1Patrol : MonoBehaviour
    {
        public GraceMotor Grace;
        public ActorView View;
        public Rigidbody2D Body;
        public Vector2[] Route;
        public float Speed = .84f;
        public float Wait = 1.25f;
        int next = 1;
        float remaining;
        Vector2 step;

        void FixedUpdate()
        {
            step = Vector2.zero;
            if (!Grace.CanMove) return;
            if (remaining > 0)
            {
                remaining -= Time.fixedDeltaTime;
                return;
            }

            Vector2 feet = Body.position;
            Vector2 destination = Route[next];
            if (Vector2.Distance(feet, destination) < .012f)
            {
                next = (next + 1) % Route.Length;
                remaining = Wait;
                return;
            }

            Vector2 proposed = Vector2.MoveTowards(feet, destination, Speed * Time.fixedDeltaTime);
            Vector2 player = Grace.Body.position;
            if ((proposed - player).sqrMagnitude < .30f * .30f &&
                (proposed - player).sqrMagnitude < (feet - player).sqrMagnitude) return;
            step = proposed - feet;
            Body.MovePosition(proposed);
        }

        void LateUpdate() => View.Pose(step, false, false, Time.deltaTime);
    }
}
