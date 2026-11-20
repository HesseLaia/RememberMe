using UnityEngine;

namespace RememberMe
{
    public sealed class Act1Target : MonoBehaviour
    {
        public TargetId Id;
        public string Label;
        public float Radius = 0.66f;
        public Vector2 Approach;
        public SpriteRenderer ConditionalArt;
        public SpriteRenderer FocusArt;
        public ActorView Speaker;
        public Collider2D Footprint;

        public Vector2 ReachPoint(Vector2 feet) => Footprint ? Footprint.ClosestPoint(feet) : Approach;

        public bool CanReach(Vector2 feet, out float distance)
        {
            Vector2 point = ReachPoint(feet);
            distance = Vector2.Distance(feet, point);
            if (distance > Radius) return false;
            if (distance <= .02f) return true;
            var obstruction = Physics2D.Linecast(feet, point, 1 << 8).collider;
            return !obstruction || obstruction == Footprint;
        }
    }
}
