using UnityEngine;

namespace RememberMe
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public sealed class GraceMotor : MonoBehaviour
    {
        public ActorView View;
        public Rigidbody2D Body;
        public CircleCollider2D Feet;
        public float WalkSpeed = 2.45f;
        public float RunSpeed = 4.15f;
        public float CrouchSpeed = 1.42f;
        public bool CanMove { get; set; }
        public bool Crouched { get; private set; }
        public Vector2 Motion { get; private set; }
        public bool Running { get; private set; }
        readonly RaycastHit2D[] hits = new RaycastHit2D[8];
        Vector2 intent;
        bool sprint;
        float currentSpeed;
        float turnUntil;

        public static Vector2 ClampInput(Vector2 direction) => Vector2.ClampMagnitude(direction, 1);

        public void SetInput(Vector2 direction, bool run, bool toggle)
        {
            Vector2 input = CanMove ? ClampInput(direction) : Vector2.zero;
            if (input != Vector2.zero)
            {
                int priorFacing = View.Facing;
                View.Face(input);
                if (intent == Vector2.zero && priorFacing != View.Facing) turnUntil = Time.time + .05f;
            }
            else turnUntil = 0;
            intent = input;
            sprint = run;
            if (CanMove && toggle) Crouched = !Crouched;
            if (CanMove && run) Crouched = false;
        }

        public void Place(Vector2 position)
        {
            Body.position = position;
            transform.position = position;
            Crouched = false;
            Motion = Vector2.zero;
            Running = false;
            intent = Vector2.zero;
            currentSpeed = 0;
            turnUntil = 0;
            Physics2D.SyncTransforms();
        }

        public void LookAt(Vector2 point)
        {
            Motion = Vector2.zero;
            View.Face(point - Body.position);
            View.Pose(Vector2.zero, false, Crouched, 0);
        }

        void FixedUpdate()
        {
            Vector2 direction = CanMove && Time.time >= turnUntil ? intent : Vector2.zero;
            Running = CanMove && sprint && !Crouched && direction.sqrMagnitude > 0;
            float speed = Crouched ? CrouchSpeed : Running ? RunSpeed : WalkSpeed;
            currentSpeed = direction == Vector2.zero ? 0 : Mathf.MoveTowards(currentSpeed, speed, 22 * Time.fixedDeltaTime);
            Vector2 delta = direction * (currentSpeed * Time.fixedDeltaTime);
            Vector2 before = Body.position;
            Motion = Resolve(delta);
            Body.MovePosition(before + Motion);
        }

        public Vector2 Resolve(Vector2 delta)
        {
            float wanted = delta.magnitude;
            if (wanted < 0.00001f) return Vector2.zero;
            Vector2 direction = delta / wanted;
            Vector2 normal = Vector2.zero;
            float distance = Distance(direction, wanted, ref normal);
            if (distance >= wanted || normal == Vector2.zero) return direction * distance;

            Vector2 along = direction - normal * Vector2.Dot(direction, normal);
            float remaining = (wanted - distance) * along.magnitude;
            if (remaining < 0.00001f) return direction * distance;
            along.Normalize();
            Vector2 unused = Vector2.zero;
            return direction * distance + along * Distance(along, remaining, ref unused);
        }

        float Distance(Vector2 direction, float length, ref Vector2 normal)
        {
            const float skin = 0.016f;
            var filter = new ContactFilter2D { useTriggers = false, useLayerMask = true };
            filter.SetLayerMask((1 << 8) | (1 << 9));
            int count = Feet.Cast(direction, filter, hits, length + skin);
            float distance = length;
            for (int index = 0; index < count; index++)
            {
                if (Vector2.Dot(hits[index].normal, direction) >= -0.01f) continue;
                float candidate = Mathf.Max(0, hits[index].distance - skin);
                if (candidate >= distance) continue;
                distance = candidate;
                normal = hits[index].normal;
            }
            return distance;
        }

        void LateUpdate() => View.Pose(Motion, Running, Crouched, Time.deltaTime, CanMove ? intent : Vector2.zero);
    }
}
