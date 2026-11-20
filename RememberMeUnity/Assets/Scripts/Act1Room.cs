using UnityEngine;

namespace RememberMe
{
    public enum RoomId { Cemetery, Flashback, Hall, Study }
    public enum TargetId
    {
        Grave, WiltedGrave, MossGrave, SlateGrave, PebbleGrave, Dad, Mom, Parcel, Ribbon,
        Birdbath, Grandfather, Gate, Photo, Blanket, Window, Clock, Sewing, Book, Shelf,
        Letters, Tea, ToStudy, ToHall, Grandma, WhiteFlowers, Uncle, Aunt, Puddle, Plaque,
        Register, Note, FamilyPlot, Memorial, OldCross
    }

    public sealed class Act1Room : MonoBehaviour
    {
        public RoomId Id;
        public static readonly Vector2 CemeterySize = new(22, 15);
        public static readonly Vector2 HouseSize = new(12, 9);
        public Vector2 Size => Id == RoomId.Cemetery ? CemeterySize : HouseSize;
        public Vector2 Entry;
        public Vector2 ReturnEntry;
        public AudioClip Music;
        public AudioClip Ambience;
        public SpriteRenderer Door;
        public Sprite OpenDoor;
        public Sprite ClosedDoor;
        public ActorView Dad;
        public ActorView Grandma;
        public Act1Target[] Targets;
    }
}
