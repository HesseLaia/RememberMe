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
        float pace;
        bool glanced;
        Vector2 step;

        void FixedUpdate()
        {
            step = Vector2.zero;
            if (!Grace.CanMove) { pace = 0; return; }
            Vector2 feet = Body.position;
            Vector2 player = Grace.Body.position;
            if (remaining > 0)
            {
                remaining -= Time.fixedDeltaTime;
                Vector2 towardGrace = player - feet;
                if (!glanced && remaining < Wait * .65f &&
                    towardGrace.sqrMagnitude > .09f && towardGrace.sqrMagnitude < 1.15f * 1.15f)
                {
                    View.Face(towardGrace);
                    glanced = true;
                }
                return;
            }

            Vector2 destination = Route[next];
            float distance = Vector2.Distance(feet, destination);
            if (distance < .012f)
            {
                next = (next + 1) % Route.Length;
                remaining = Wait;
                pace = 0;
                glanced = false;
                return;
            }

            float targetPace = Mathf.Min(Speed, Mathf.Max(.26f, distance * 3.8f));
            pace = Mathf.MoveTowards(pace, targetPace, 4.4f * Time.fixedDeltaTime);
            Vector2 proposed = Vector2.MoveTowards(feet, destination, pace * Time.fixedDeltaTime);
            if ((proposed - player).sqrMagnitude < .30f * .30f &&
                (proposed - player).sqrMagnitude < (feet - player).sqrMagnitude)
            {
                pace = 0;
                View.Face(player - feet);
                return;
            }
            step = proposed - feet;
            Body.MovePosition(proposed);
        }

        void LateUpdate() => View.Pose(step, false, false, Time.deltaTime);
    }
}
