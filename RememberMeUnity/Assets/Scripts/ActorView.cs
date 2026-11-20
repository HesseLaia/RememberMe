using UnityEngine;

namespace RememberMe
{
    public sealed class ActorView : MonoBehaviour
    {
        public SpriteRenderer Renderer;
        public Sprite[] Idle;
        public Sprite[] Walk;
        public Sprite[] Run;
        public Sprite[] Crouch;
        public Sprite[] Cry;
        public bool Crying { get; private set; }
        float phase;
        int facing;
        int previousRow = -1;
        public int Facing => facing;
        public Vector2 Forward => facing switch { 1 => Vector2.left, 2 => Vector2.right, 3 => Vector2.up, _ => Vector2.down };

        public void Face(Vector2 direction) => facing = Direction(direction, facing);
        public void Look(Vector2 direction)
        {
            Face(direction);
            Pose(Vector2.zero, false, false, 0);
        }

        public void SetCrying(bool crying)
        {
            Crying = crying;
            Pose(Vector2.zero, false, false, 0);
        }

        public static int Direction(Vector2 input, int fallback)
        {
            if (input.sqrMagnitude < 0.0001f) return fallback;
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) return input.x < 0 ? 1 : 2;
            return input.y > 0 ? 3 : 0;
        }

        public void Pose(Vector2 movement, bool fast, bool low, float delta, Vector2? facingIntent = null)
        {
            bool moving = movement.sqrMagnitude > 0.000004f;
            Face(facingIntent ?? movement);
            int row = low ? 3 : moving ? (fast ? 2 : 1) : 0;
            bool sharedStride = (row == 1 || row == 2) && (previousRow == 1 || previousRow == 2);
            if (row != previousRow && !sharedStride) phase = 1;
            phase = Mathf.Repeat(phase + delta * (fast ? 11 : low ? 7 : 8), 4);
            previousRow = row;
            Sprite[] pose = Crying && !moving && Cry != null && Cry.Length == 4 ? Cry :
                row == 3 ? Crouch : row == 2 ? Run : row == 1 ? Walk : Idle;
            if (pose == null || pose.Length < 4) pose = Idle;
            if (pose != null && pose.Length >= 4)
            {
                int frame = pose.Length >= 12 ? (moving && (int)phase != 3 ? (int)phase : 1) : 0;
                Renderer.sprite = pose[facing * (pose.Length >= 12 ? 3 : 1) + frame];
            }
            Renderer.sortingOrder = Depth(transform.position.y);
        }

        public static int Depth(float groundY) => 2000 - Mathf.RoundToInt(groundY * 100);
    }
}
