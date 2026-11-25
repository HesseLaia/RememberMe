using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RememberMe.Editor
{
    public static class Act1Verification
    {
        public static void Verify()
        {
            if (EditorSettings.defaultBehaviorMode != EditorBehaviorMode.Mode2D)
                throw new InvalidOperationException("Project must default to 2D.");
            var pipeline = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            if (!pipeline || !pipeline.GetType().Name.Contains("UniversalRenderPipeline"))
                throw new InvalidOperationException("The Unity 2D URP pipeline is not assigned.");
            var controller = Type.GetType("RememberMe.Act1Game, Assembly-CSharp");
            if (controller == null)
                throw new InvalidOperationException("The Act I playable controller is not implemented.");
            Debug.Log("Validated 2D defaults, URP and Act I controller.");
        }

        internal static void VerifyMemory(Act1Room[] rooms)
        {
            var formerReunion = JsonUtility.FromJson<Act1Story>("{\"Stage\":5,\"Room\":3}");
            var completedReunion = JsonUtility.FromJson<Act1Story>("{\"Stage\":6,\"Room\":3,\"SharedKeepsake\":true}");
            if (formerReunion.Stage != StoryStage.ExploreMemory || formerReunion.Room != RoomId.Study ||
                completedReunion.Stage != StoryStage.Complete || !completedReunion.SharedKeepsake)
                throw new InvalidOperationException("Past Act I room and stage values must still load.");

            foreach (var room in rooms.Where(room => room.Id == RoomId.Hall || room.Id == RoomId.Study))
            {
                if (room.GetComponentInChildren<ActorView>(true) || room.Targets.Any(target => target.Speaker))
                    throw new InvalidOperationException(room.Id + ": Grace must explore the memory alone.");
                var artwork = room.transform.Find(room.Id + "Rain");
                if (!artwork || !artwork.GetComponent<SpriteRenderer>().sprite)
                    throw new InvalidOperationException(room.Id + ": the rainy room artwork is missing.");
            }

            var study = rooms.Single(room => room.Id == RoomId.Study);
            if (study.Targets.Count(target => target.Id == TargetId.LeaveMemory) != 1)
                throw new InvalidOperationException("The back room must have a route out of the memory.");
            var flashback = rooms.Single(room => room.Id == RoomId.Flashback);
            if (!flashback.Dad || !flashback.Grandma)
                throw new InvalidOperationException("The earlier flashback must retain Dad and Grandma.");

            Debug.Log("REMEMBER_EMPTY_MEMORY_PASS: rainy front and back rooms, one exit and the intact Dad flashback.");
        }

        internal static void VerifyCemetery(Act1Room room)
        {
            var family = new[] { TargetId.Dad, TargetId.Mom, TargetId.Grandfather, TargetId.Uncle, TargetId.Aunt };
            var speakers = room.Targets.Where(target => target.Speaker).ToArray();
            if (!speakers.Select(target => target.Id).OrderBy(id => id).SequenceEqual(family.OrderBy(id => id)) ||
                room.GetComponentsInChildren<ActorView>(true).Length != family.Length ||
                room.GetComponentInChildren<Act1Patrol>(true) ||
                room.transform.Cast<Transform>().Any(child => child.name.StartsWith("Mourner ", StringComparison.Ordinal)))
                throw new InvalidOperationException("Only Grace’s family may appear in the cemetery.");

            var specialGraves = new[] { TargetId.WiltedGrave, TargetId.MossGrave, TargetId.SlateGrave, TargetId.PebbleGrave };
            var graveSprites = specialGraves.Select(id => room.Targets.Single(target => target.Id == id).FocusArt.sprite).ToArray();
            var cemeterySprites = room.GetComponentsInChildren<SpriteRenderer>(true);
            if (graveSprites.Distinct().Count() != specialGraves.Length ||
                graveSprites.Any(sprite => !sprite || cemeterySprites.Count(renderer => renderer.sprite == sprite) != 1))
                throw new InvalidOperationException("Each special grave must have a unique sprite used once in the cemetery.");

            Debug.Log("REMEMBER_CEMETERY_PASS: five family NPCs and four unique optional grave sprites.");
        }
    }
}
