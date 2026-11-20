using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace RememberMe.Editor
{
    public static class Act1Builder
    {
        public const string ScenePath = "Assets/Scenes/RememberMeAct1.unity";
        const string Art = "Assets/Art/";
        const float Pixels = 96;
        static Material material;
        static List<Act1Target> targets;
        static float layoutHeight;

        [MenuItem("Remember Me/Assemble Act I")]
        public static void Assemble()
        {
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (!shader) shader = Shader.Find("Sprites/Default");
            material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/RememberSprited.mat");
            if (!material)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, "Assets/Settings/RememberSprited.mat");
            }

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(6, 4.5f, -10);
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true; camera.orthographicSize = 4.15f;
            camera.backgroundColor = new Color(.09f, .12f, .12f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            var world = new GameObject("Act I");
            var game = world.AddComponent<Act1Game>();
            game.WorldCamera = camera;
            game.HUD = world.AddComponent<Act1HUD>();
            game.HUD.BrowserTypeface = AssetDatabase.LoadAssetAtPath<Font>(Art + "Fonts/Tinos-Regular.ttf");
            game.Rooms = new[] { Cemetery(world.transform), Interior(world.transform, RoomId.Flashback),
                Interior(world.transform, RoomId.Hall), Interior(world.transform, RoomId.Study) };
            var actor = Actor(world.transform, "Grace", "GraceIdlePolished", P(576, 640), .88f);
            actor.Walk = Frames("GraceWalkPolished");
            actor.WalkDiagonal = Frames("GraceWalkDiagonal");
            actor.Run = Frames("GraceRunPolished");
            actor.Crouch = Frames("GraceCrouchPolished");
            game.Grace = actor.gameObject.AddComponent<GraceMotor>();
            game.Grace.View = actor;
            game.Grace.Body = actor.GetComponent<Rigidbody2D>();
            game.Grace.Body.bodyType = RigidbodyType2D.Kinematic;
            game.Grace.Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            game.Grace.Body.gravityScale = 0;
            game.Grace.Feet = actor.GetComponent<CircleCollider2D>();
            game.Grace.Feet.radius = .115f / actor.transform.localScale.x;
            game.Grace.Feet.offset = Vector2.zero;
            foreach (var patrol in game.Rooms[0].GetComponentsInChildren<Act1Patrol>()) patrol.Grace = game.Grace;
            game.Cover = Image("Story/Cover");
            game.Gravestone = Image("Story/Gravestone");
            game.ClosedParcel = Image("Story/ClosedParcel");
            game.OpenParcel = Image("Story/OpenParcel");
            game.Mechanism = Image("Story/Mechanism");
            game.Reunion = Image("Story/Reunion");
            game.WindowView = Image("Story/WindowView");
            game.GracePortrait = Image("Portraits/Grace");
            game.MomPortrait = Image("Portraits/Mom");
            game.DadPortrait = Image("Portraits/Dad");
            game.DadSadPortrait = Image("Portraits/DadSad");
            game.GrandmaPortrait = Image("Portraits/Grandma");
            game.GrandpaPortrait = Image("Portraits/Grandpa");
            game.UnclePortrait = Image("Portraits/Uncle");
            game.AuntPortrait = Image("Portraits/Aunt");
            game.TitleMusic = Audio("RM_StoryTitle"); game.BoxMusic = Audio("RM_StoryKeepsake");
            game.Hinge = Audio("RM_DoorCreak"); game.Latch = Audio("RM_DoorPalm");
            game.Gravel = Audio("RM_StoryStone"); game.Board = Audio("RM_StoryWood"); game.Cloth = Audio("RM_Cloth");
            foreach (var room in game.Rooms) room.gameObject.SetActive(room.Id == RoomId.Hall);
            game.Grace.Place(game.Rooms[2].Entry);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorBuildSettings.RemoveConfigObject("com.unity.input.settings.actions");
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 800;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, new[] { GraphicsDeviceType.Direct3D11 });
            AssetDatabase.SaveAssets();
            VerifyScene(game);
            Debug.Log("REMEMBER_EDITOR_PASS: editable 2D scene, animation sets and room paths verified.");
        }

        public static void Build()
        {
            Act1Verification.Verify();
            Assemble();
            Directory.CreateDirectory("Build/Windows");
            var report = BuildPipeline.BuildPlayer(new[] { ScenePath }, "Build/Windows/RememberMe.exe",
                BuildTarget.StandaloneWindows64, BuildOptions.None);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Windows build: " + report.summary.result);
            Debug.Log("REMEMBER_BUILD_PASS: " + report.summary.totalSize + " bytes");
        }

        [MenuItem("Remember Me/Build for GitHub Pages")]
        public static void BuildForGitHubPages()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                throw new InvalidOperationException("Install Web Build Support for this Unity Editor.");
            const string fontLicense = Art + "Fonts/Tinos-LICENSE.txt";
            if (!AssetDatabase.LoadAssetAtPath<Font>(Art + "Fonts/Tinos-Regular.ttf") || !File.Exists(fontLicense))
                throw new InvalidOperationException("The browser typeface and its license are required.");
            Act1Verification.Verify();
            Assemble();
            const string output = "Build/GitHubPages/docs";
            Directory.CreateDirectory(output);
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.template = "PROJECT:RememberMePages";
            var report = BuildPipeline.BuildPlayer(new[] { ScenePath }, output, BuildTarget.WebGL, BuildOptions.None);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Web build: " + report.summary.result);
            File.WriteAllText(Path.Combine(output, ".nojekyll"), "");
            File.Copy(fontLicense, Path.Combine(output, "Tinos-LICENSE.txt"), true);
            if (Directory.EnumerateFiles(output, "*", SearchOption.AllDirectories).Any(file => new FileInfo(file).Length >= 100L * 1024 * 1024))
                throw new InvalidOperationException("The Web build contains a file at or above GitHub's 100 MiB Git limit.");
            Debug.Log("REMEMBER_WEB_BUILD_PASS: " + report.summary.totalSize + " bytes");
        }

        static void VerifyScene(Act1Game game)
        {
            if (!game.WorldCamera.orthographic ||
                UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include).Length > 0 ||
                game.Grace.Feet.offset != Vector2.zero || game.Grace.View.Run.Length != 12 ||
                game.Grace.View.Walk.Length != 12 || game.Grace.View.Crouch.Length != 12)
                throw new InvalidOperationException("Invalid 2D scene or Grace motion sheet.");
            foreach (var room in game.Rooms)
                room.gameObject.SetActive(false);
            foreach (var room in game.Rooms)
            {
                room.gameObject.SetActive(true);
                if (room.Id != RoomId.Flashback)
                {
                    Physics2D.SyncTransforms();
                    VerifyRoomPaths(room);
                }
                room.gameObject.SetActive(false);
            }
            game.Rooms.Single(room => room.Id == RoomId.Hall).gameObject.SetActive(true);
        }

        static void VerifyRoomPaths(Act1Room room)
        {
            const int samplesPerUnit = 10;
            int width = (int)room.Size.x * samplesPerUnit + 1;
            int height = (int)room.Size.y * samplesPerUnit + 1;
            const float footClearance = .131f;
            Vector2 Position(Vector2Int cell) => (Vector2)cell / samplesPerUnit;
            bool Blocked(Vector2 feet) => Physics2D.OverlapCircle(feet, footClearance, 1 << 8);
            var visited = new bool[width, height];
            var queue = new Queue<Vector2Int>();
            var start = Vector2Int.RoundToInt(room.Entry * samplesPerUnit);
            if (Blocked(room.Entry) || Blocked(Position(start)) || Blocked(room.ReturnEntry))
                throw new InvalidOperationException(room.Id + ": obstructed spawn.");
            queue.Enqueue(start);
            visited[start.x, start.y] = true;
            var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            var reachable = new List<Vector2>();
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                reachable.Add(Position(cell));
                foreach (var direction in directions)
                {
                    var next = cell + direction;
                    if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height || visited[next.x, next.y]) continue;
                    visited[next.x, next.y] = true;
                    if (!Blocked(Position(next))) queue.Enqueue(next);
                }
            }
            var unreachable = room.Targets.Where(target => Blocked(target.Approach) ||
                !reachable.Any(feet => Vector2.Distance(feet, target.Approach) < .16f && target.CanReach(feet, out _)))
                .Select(target => target.Label).ToArray();
            if (unreachable.Length > 0)
                throw new InvalidOperationException(room.Id + ": unreachable approaches: " + string.Join(", ", unreachable));
            Debug.Log($"REMEMBER_PATHS: {room.Id} · {room.Targets.Length} approachable targets from spawn.");
            if (room.Id == RoomId.Cemetery)
            {
                var visitors = Enumerable.Range(1, 8).Select(i => room.transform.Find("Mourner " + i).GetComponent<SpriteRenderer>()).ToArray();
                if (visitors.Select(visitor => visitor.sprite.texture).Distinct().Count() != visitors.Length)
                    throw new InvalidOperationException("Cemetery visitors must have distinct artwork.");
                for (int i = 0; i < 4; i++) _ = Image("Actors/MournerReserves/" + i);
                Debug.Log("REMEMBER_VISITORS: eight distinct cemetery sprites and four movement routes.");
            }
            foreach (var patrol in room.GetComponentsInChildren<Act1Patrol>())
            for (int leg = 0; leg < patrol.Route.Length; leg++)
            {
                Vector2 startLeg = patrol.Route[leg], endLeg = patrol.Route[(leg + 1) % patrol.Route.Length];
                int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(startLeg, endLeg) * 20));
                for (int i = 0; i <= steps; i++)
                    if (Blocked(Vector2.Lerp(startLeg, endLeg, i / (float)steps)))
                        throw new InvalidOperationException($"{patrol.name} crosses a solid footprint on leg {leg + 1}.");
            }
        }

        static Vector2 P(float x, float y) => new(x / Pixels, (layoutHeight - y) / Pixels);

        static Act1Room Room(Transform root, RoomId id)
        {
            targets = new List<Act1Target>();
            var room = ObjectUnder(root, id.ToString()).AddComponent<Act1Room>();
            room.Id = id;
            layoutHeight = room.Size.y * Pixels;
            float width = room.Size.x * Pixels;
            if (id == RoomId.Cemetery) BuildCemeteryLawn();
            var ground = ArtAt(room.transform, id == RoomId.Cemetery ? "Ground/CemeteryLawn" : "Ground/" + id,
                width / 2, layoutHeight, width, layoutHeight);
            ground.sortingOrder = -3000;
            room.Entry = P(576, 640); room.ReturnEntry = room.Entry;
            return room;
        }

        static Act1Room Cemetery(Transform root)
        {
            var room = Room(root, RoomId.Cemetery);
            var parent = room.transform;
            room.Entry = P(1188, 1260); room.ReturnEntry = P(1434, 737);
            room.Music = Audio("RM_StoryFarewell"); room.Ambience = Audio("RM_StoryRain");
            BuildCemeteryPaths();
            ArtAt(parent, "Ground/CemeteryPaving", 1056, 1440, 2112, 1440).sortingOrder = -2990;
            Boundary(parent, 1056, 127, 2040, 22);
            Boundary(parent, 38, 715, 22, 1234); Boundary(parent, 2074, 715, 22, 1234);
            Boundary(parent, 501, 1332, 930, 21); Boundary(parent, 1611, 1332, 930, 21);
            Boundary(parent, 1056, 1421, 190, 22);
            Boundary(parent, 966, 1380, 12, 97); Boundary(parent, 1146, 1380, 12, 97);
            for (int index = 0; index < 22; index++)
            {
                int x = 82 + 93 * index;
                ArtAt(parent, "Props/Fence", x, 137, 94, 55);
                if (index != 10 && index != 11) ArtAt(parent, "Props/Fence", x, 1342, 94, 55);
            }
            for (int index = 0; index < 14; index++)
            {
                ArtAt(parent, "Props/FenceSide", 38, 225 + 85 * index, 31, 96);
                ArtAt(parent, "Props/FenceSide", 2074, 225 + 85 * index, 31, 96).flipX = true;
            }
            foreach (var point in new[] { new Vector2(108, 285), new(110, 625), new(105, 977), new(113, 1297),
                new(1992, 287), new(2009, 620), new(1980, 968), new(1988, 1295), new(1055, 231) })
                Prop(parent, "Tree", point.x, point.y, 158, 164, 39, 26);

            var rows = new[] { 335, 465, 655, 775, 975, 1095 };
            var columns = new[] { 400, 545, 690, 835, 1277, 1422, 1567, 1712 };
            var shapes = new[] { "GraveSlate", "GraveMoss", "GraveStones", "GraveWilt" };
            for (int row = 0; row < rows.Length; row++)
            for (int col = 0; col < columns.Length; col++)
            {
                int x = columns[col], y = rows[row];
                switch ((row, col))
                {
                    case (2, 4): Grave(parent, "GraveMain", x, y, TargetId.Grave, "Read Grandma’s headstone", 99); break;
                    case (4, 3): Grave(parent, "GraveWilt", x, y, TargetId.WiltedGrave, "Examine the withered bouquet"); break;
                    case (1, 1): Grave(parent, "GraveMoss", x, y, TargetId.MossGrave, "Read the moss-covered stone"); break;
                    case (0, 6): Grave(parent, "GraveSlate", x, y, TargetId.SlateGrave, "Look at the slate and rosemary"); break;
                    case (3, 7): Grave(parent, "GraveStones", x, y, TargetId.PebbleGrave, "Look at the three pebbles"); break;
                    case (1, 2): Grave(parent, "GraveStones", x, y, TargetId.FamilyPlot, "Read two names side by side"); break;
                    case (5, 5): Grave(parent, "GraveSlate", x, y, TargetId.OldCross, "Read the old cross"); break;
                    case (0, 2): Grave(parent, "GraveMoss", x, y, TargetId.Memorial, "Read the weathered inscription"); break;
                    default:
                        int style = (row * 3 + col * 7 + col / 3) % shapes.Length;
                        int width = 74 + (row * 17 + col * 13) % 18;
                        var stone = Prop(parent, shapes[style], x, y, width, 70 + (col * 11 + row) % 19, width * .72f, 20);
                        float light = .79f + ((row * 11 + col) % 9) * .023f;
                        stone.color = new Color(light, light * 1.015f, light);
                        break;
                }
            }
            var flowers = ArtAt(parent, "Props/Daisies", 1333, 660, 57, 29);
            Target(parent, TargetId.WhiteFlowers, "Look at Grandma’s white flowers", 1333, 688).FocusArt = flowers;
            ArtAt(parent, "Props/Lilies", 523, 333, 45, 26);
            ArtAt(parent, "Props/Lilies", 1587, 1097, 43, 24);
            ArtAt(parent, "Props/Lilies", 722, 779, 46, 27);

            var parcelBench = Prop(parent, "Bench", 1370, 1228, 119, 69, 109, 22);
            var parcel = On(parent, "Parcel", 1347, 1228, 25, 22, 27);
            On(parent, "Bag", 1391, 1228, 27, 30, 27);
            Reach(Target(parent, TargetId.Parcel, "Inspect the parcel", 1370, 1256, parcel), parcelBench, parcel);
            var registerBench = Prop(parent, "Bench", 560, 570, 118, 69, 109, 22);
            var register = On(parent, "Register", 565, 570, 25, 15, 27);
            Reach(Target(parent, TargetId.Register, "Read the visitor register", 560, 598), registerBench, register);
            var noteBench = Prop(parent, "Bench", 1578, 262, 118, 69, 109, 22);
            var note = On(parent, "Letters", 1588, 262, 22, 12, 26);
            Reach(Target(parent, TargetId.Note, "Read the note tied with twine", 1578, 289), noteBench, note);
            var bath = Prop(parent, "Birdbath", 230, 752, 59, 68, 30, 20);
            Reach(Target(parent, TargetId.Birdbath, "Watch rain in the birdbath", 230, 779), bath);
            var puddle = ArtAt(parent, "Props/Puddle", 1145, 787, 95, 34);
            puddle.sortingOrder = -2900;
            Target(parent, TargetId.Puddle, "Watch the puddle", 1145, 801).FocusArt = puddle;
            var ribbon = ArtAt(parent, "Props/Ribbon", 470, 957, 28, 14);
            Target(parent, TargetId.Ribbon, "Pick up the ribbon", 470, 980, ribbon).FocusArt = ribbon;

            var dad = NPC(parent, "Dad", 1451, 706, TargetId.Dad, "Talk to Dad").Speaker;
            dad.Cry = Enumerable.Range(0, 4).Select(i => Image("Actors/DadCrying/" + i)).ToArray();
            room.Dad = dad;
            dad.Look(P(1277, 655) - (Vector2)dad.transform.position);
            NPC(parent, "Mom", 1235, 1250, TargetId.Mom, "Talk to Mom");
            NPC(parent, "Grandpa", 1925, 813, TargetId.Grandfather, "Talk to Grandpa");
            NPC(parent, "UncleHD", 783, 891, TargetId.Uncle, "Talk to Uncle James").Speaker.Look(Vector2.left);
            NPC(parent, "AuntHD", 371, 266, TargetId.Aunt, "Talk to Aunt").Speaker.Look(Vector2.right);
            Mourners(parent);
            var plaque = Prop(parent, "Plaque", 952, 1283, 44, 65, 24, 16);
            Reach(Target(parent, TargetId.Plaque, "Read the gate plaque", 927, 1307), plaque);
            Target(parent, TargetId.Gate, "Listen at the gate", 1056, 1372);
            Rain(parent, room.Size);
            room.Targets = targets.ToArray();
            return room;
        }

        static Act1Room Interior(Transform root, RoomId id)
        {
            var room = Room(root, id); var parent = room.transform;
            bool flash = id == RoomId.Flashback, study = id == RoomId.Study;
            room.Music = Audio(flash ? "RM_StoryFlashback" : "RM_StoryKeepsake");
            room.Ambience = Audio("RM_StoryRoom");
            room.Entry = P(576, study ? 777 : 621);
            room.ReturnEntry = P(576, 265);
            float north = study ? 306 : 224;
            Boundary(parent, 576, north - 14, 920, 25);
            Boundary(parent, 117, 453, 22, 455); Boundary(parent, 1034, 453, 23, 455);
            Boundary(parent, 325, 672, 416, 21); Boundary(parent, 829, 672, 410, 21);
            Boundary(parent, 525, 738, 22, 137); Boundary(parent, 626, 738, 22, 137);
            Boundary(parent, 576, 820, 115, 20);
            var rug = ArtAt(parent, "Props/Rug", 568, 580, 294, 177);
            rug.sortingOrder = -100;

            if (!study)
            {
                var sofa = Prop(parent, "Sofa", 253, 529, 116, 88, 108, 33);
                var sideboard = Prop(parent, "Sideboard", 260, 347, 118, 101, 109, 32);
                Prop(parent, "Cabinet", 911, 352, 83, 121, 78, 30);
                Prop(parent, "Chair", 870, 544, 57, 72, 53, 28);
                var teaTable = Prop(parent, "TeaTable", 944, 548, 45, 48, 34, 28);
                var cup = On(parent, "Cup", 944, 548, 17, 14, 34);
                room.Door = ArtAt(parent, flash ? "Props/DoorNew" : "Props/DoorOpenNew", 576, 224, flash ? 53 : 61, 96);
                if (flash)
                {
                    room.ClosedDoor = Image("Props/DoorNew");
                    room.OpenDoor = Image("Props/DoorOpenNew");
                    room.Dad = Actor(parent, "Dad at the doorway", "Dad", P(686, 337), 1.09f);
                    room.Grandma = Actor(parent, "Grandma at the doorway", "Grandma", P(540, 302), 1.03f);
                }
                else
                {
                    var console = Prop(parent, "ConsoleFront", 707, 423, 96, 64, 89, 32);
                    var book = On(parent, "Book", 712, 423, 26, 18, 52);
                    Reach(Target(parent, TargetId.Book, "Take the blue storybook", 708, 448, book), console, book);
                    var sewing = Prop(parent, "Sewing", 324, 492, 27, 23, 24, 18);
                    Reach(Target(parent, TargetId.Sewing, "Check the sewing basket", 353, 502), sewing);
                    Reach(Target(parent, TargetId.Photo, "Look at the photograph", 260, 371), sideboard);
                    Reach(Target(parent, TargetId.Blanket, "Touch the patchwork blanket", 253, 556), sofa);
                    Reach(Target(parent, TargetId.Tea, "Look at the teacup", 946, 571), teaTable, cup);
                    Target(parent, TargetId.Window, "Look out the window", 890, 250);
                    Target(parent, TargetId.ToStudy, "Follow the music", 576, 248).FocusArt = room.Door;
                }
            }
            else
            {
                Prop(parent, "Sideboard", 260, 407, 121, 104, 112, 32);
                var cabinet = Prop(parent, "Cabinet", 924, 366, 85, 125, 79, 28);
                var console = Prop(parent, "ConsoleFront", 657, 401, 94, 61, 86, 32);
                On(parent, "Letters", 640, 401, 26, 14, 49);
                On(parent, "Sewing", 676, 401, 27, 24, 52);
                Prop(parent, "Chair", 831, 461, 57, 70, 53, 27);
                var teaTable = Prop(parent, "TeaTable", 902, 482, 48, 48, 41, 28);
                On(parent, "Cup", 892, 482, 15, 12, 35);
                On(parent, "Cup", 910, 482, 12, 10, 34);
                var book = On(parent, "Book", 922, 365, 24, 21, 47);
                Reach(Target(parent, TargetId.Shelf, "Inspect the cabinet", 924, 390, book), cabinet);
                Target(parent, TargetId.Window, "Look through the bay window", 268, 323);
                Reach(Target(parent, TargetId.Letters, "Read the letters", 654, 426), console);
                Reach(Target(parent, TargetId.Tea, "Look at the two cups", 902, 507), teaTable);
                Target(parent, TargetId.ToHall, "Return to the front room", 576, 796);
                NPC(parent, "Grandma", 798, 367, TargetId.Grandma, "Talk to Grandma");
            }
            room.Targets = targets.ToArray();
            return room;
        }

        static void Grave(Transform parent, string name, int x, int y, TargetId id, string label, int width = 91)
        {
            var stone = Prop(parent, name, x, y, width, 85, width * .77f, 21);
            Reach(Target(parent, id, label, x, y + 29), stone);
        }

        static Act1Target NPC(Transform room, string name, int x, int y, TargetId id, string label)
        {
            var actor = Actor(room, label, name, P(x, y), name == "Grandma" ? 1.03f : 1.10f);
            var target = Target(room, id, label, x, y + 29);
            target.Footprint = Boundary(actor.transform, x, y - 4, 24, 19);
            target.Radius = .71f;
            target.Speaker = actor;
            target.FocusArt = actor.Renderer;
            return target;
        }

        static void Mourners(Transform room)
        {
            Vector2[][] routes = { new Vector2[] { new(250, 390), new(250, 633) },
                new Vector2[] { new(730, 241), new(890, 241), new(890, 273), new(730, 273) },
                new Vector2[] { new(1856, 685), new(1856, 430) },
                new Vector2[] { new(1610, 1204), new(1823, 1204) } };
            string[] walkingVisitors = { "VisitorA", "VisitorC", "VisitorD", "VisitorE" };
            for (int i = 0; i < routes.Length; i++)
            {
                var points = routes[i];
                string set = walkingVisitors[i];
                var actor = Actor(room, "Mourner " + (i + 1), set,
                    P(points[0].x, points[0].y), 1.04f + .025f * i);
                actor.Walk = Frames(set + "Walk");
                actor.Look(P(points[1].x, points[1].y) - (Vector2)actor.transform.position);
                actor.gameObject.layer = 9;
                var patrol = actor.gameObject.AddComponent<Act1Patrol>();
                patrol.View = actor;
                patrol.Speed = .72f + .095f * i;
                patrol.Wait = 1.35f + .17f * i;
                patrol.Route = points.Select(point => P(point.x, point.y)).ToArray();
                patrol.Body = actor.gameObject.AddComponent<Rigidbody2D>();
                patrol.Body.bodyType = RigidbodyType2D.Kinematic;
                patrol.Body.interpolation = RigidbodyInterpolation2D.Interpolate;
                var feet = actor.gameObject.AddComponent<CircleCollider2D>();
                feet.offset = Vector2.zero;
                feet.radius = .105f / actor.transform.localScale.x;
            }
            Vector2[] positions = { new(320, 1036), new(1805, 1036), new(406, 798), new(1409, 486) };
            for (int i = 0; i < positions.Length; i++)
            {
                var point = positions[i];
                float height = 96 + (i % 3) * 3;
                var art = ArtAt(room, "Actors/MournersPolished/" + i, point.x, point.y, height * 164f / 208, height);
                art.gameObject.name = "Mourner " + (i + 5);
                Boundary(art.transform, point.x, point.y - 5, 23, 17);
            }
        }

        static ActorView Actor(Transform parent, string name, string sprites, Vector2 pos, float height)
        {
            var obj = ObjectUnder(parent, name);
            obj.transform.position = pos;
            var view = obj.AddComponent<ActorView>();
            view.Renderer = obj.AddComponent<SpriteRenderer>(); view.Renderer.sharedMaterial = material;
            view.Idle = Enumerable.Range(0, 4).Select(index => Image("Actors/" + sprites + "/" + index)).ToArray();
            if (sprites == "Grandma") view.Walk = Frames("GrandmaWalk");
            view.Renderer.sprite = view.Idle[0];
            obj.transform.localScale *= height / view.Idle[0].bounds.size.y;
            view.Pose(Vector2.zero, false, false, 0);
            return view;
        }

        static Sprite[] Frames(string name) => Enumerable.Range(0, 12)
            .Select(index => Image("Actors/" + name + "/" + index)).ToArray();
        static SpriteRenderer Prop(Transform room, string name, float x, float y, float w, float h, float footW, float footH)
        {
            var image = ArtAt(room, "Props/" + name, x, y, w, h);
            Boundary(image.transform, x, y - footH / 2, footW, footH);
            return image;
        }
        static SpriteRenderer On(Transform room, string name, float x, float floor, float width, float height, float rise)
        {
            var sprite = ArtAt(room, "Props/" + name, x, floor - rise, width, height);
            sprite.sortingOrder = ActorView.Depth(P(x, floor).y) + 1;
            return sprite;
        }
        static SpriteRenderer ArtAt(Transform room, string key, float x, float y, float width, float height)
        {
            var obj = ObjectUnder(room, Path.GetFileName(key));
            obj.transform.position = P(x, y);
            var render = obj.AddComponent<SpriteRenderer>();
            render.sharedMaterial = material;
            render.sprite = Image(key);
            render.sortingOrder = ActorView.Depth(obj.transform.position.y);
            obj.transform.localScale = new Vector3(width / Pixels / render.sprite.bounds.size.x,
                height / Pixels / render.sprite.bounds.size.y, 1);
            return render;
        }
        static BoxCollider2D Boundary(Transform room, float x, float y, float w, float h)
        {
            var obj = ObjectUnder(room, "Footprint");
            obj.transform.SetParent(null);
            obj.transform.position = P(x, y);
            obj.transform.SetParent(room, true);
            obj.layer = 8;
            var box = obj.AddComponent<BoxCollider2D>();
            Vector3 scale = obj.transform.lossyScale;
            box.size = new Vector2(w / scale.x, h / scale.y) / Pixels;
            return box;
        }
        static Act1Target Target(Transform root, TargetId id, string label, float x, float y, SpriteRenderer detail = null)
        {
            var obj = ObjectUnder(root, label);
            obj.transform.position = P(x, y);
            var target = obj.AddComponent<Act1Target>();
            target.Id = id; target.Label = label; target.Approach = obj.transform.position;
            target.ConditionalArt = detail;
            targets.Add(target);
            return target;
        }
        static void Reach(Act1Target target, SpriteRenderer prop, SpriteRenderer focus = null)
        {
            target.Footprint = prop.GetComponentInChildren<Collider2D>();
            target.FocusArt = focus ? focus : prop;
        }
        static GameObject ObjectUnder(Transform root, string name)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(root, false);
            return obj;
        }
        static Sprite Image(string key)
        {
            string path = Art + key + ".png";
            if (AssetImporter.GetAtPath(path) is not TextureImporter importer)
                throw new FileNotFoundException("Missing art " + path);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite && Mathf.Abs(sprite.pivot.y) < .01f && sprite.pixelsPerUnit == Pixels &&
                importer.maxTextureSize == 4096 && importer.textureCompression == TextureImporterCompression.Uncompressed)
                return sprite;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = Pixels;
            importer.maxTextureSize = 4096;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(.5f, 0);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (!sprite || Mathf.Abs(sprite.pivot.y) > .01f || sprite.pixelsPerUnit != Pixels)
                throw new InvalidOperationException("Expected 96 PPU with a bottom pivot: " + path);
            return sprite;
        }
        static AudioClip Audio(string key)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + key + ".ogg");
            if (!clip) throw new FileNotFoundException("Missing audio " + key);
            return clip;
        }
        static void Rain(Transform parent, Vector2 size)
        {
            var obj = ObjectUnder(parent, "Rain");
            obj.transform.position = new Vector3(size.x / 2, size.y, 0);
            var system = obj.AddComponent<ParticleSystem>();
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = system.main;
            main.startLifetime = 1.25f; main.startSpeed = 0;
            main.startSize = .018f; main.startColor = new Color(.76f, .87f, .93f, .25f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            var emission = system.emission; emission.rateOverTime = 395;
            var shape = system.shape; shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(size.x, size.y + 2, .01f);
            shape.position = new Vector3(0, -size.y / 2 + 1, 0);
            var velocity = system.velocityOverLifetime; velocity.enabled = true;
            velocity.x = -.57f; velocity.y = -4.7f; velocity.z = 0f;
            var renderer = obj.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.renderMode = ParticleSystemRenderMode.Stretch; renderer.lengthScale = 2.0f;
            renderer.velocityScale = .048f; renderer.sortingOrder = 3500;
            system.Play();
        }

        static void BuildCemeteryPaths()
        {
            const string output = Art + "Ground/CemeteryPaving.png";
            if (File.Exists(output)) return;
            const int width = 2112, height = 1440;
            var stone = new Texture2D(2, 2);
            var layer = new Texture2D(width, height, TextureFormat.RGBA32, false);
            try
            {
                if (!ImageConversion.LoadImage(stone, File.ReadAllBytes(Art + "Ground/PathStone.png")))
                    throw new InvalidOperationException("Cannot decode cemetery paving texture.");
                var source = stone.GetPixels32();
                var result = new Color32[width * height];
                // Each rectangle shares alpha coverage with the adjacent paths so corners stay seamless.
                Vector4[] paths = { new(986, 209, 1126, 1440), new(205, 214, 303, 1236), new(1809, 214, 1907, 1236),
                    new(220, 207, 1892, 278), new(221, 511, 1891, 574), new(220, 832, 1892, 911),
                    new(221, 1166, 1891, 1248), new(1102, 756, 1198, 847), new(956, 1235, 1156, 1354) };
                for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float depth = -20;
                    foreach (var p in paths)
                        depth = Mathf.Max(depth, Mathf.Min(Mathf.Min(x - p.x, p.z - x), Mathf.Min(y - p.y, p.w - y)));
                    if (depth < -11) continue;
                    float fringe = Mathf.PerlinNoise(x * .072f, y * .072f) * 14 - 7;
                    int ix = Mirror(x * 3, stone.width), iy = Mirror(y * 3, stone.height);
                    var c = source[iy * stone.width + ix];
                    c.a = (byte)(255 * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(-10, 12, depth + fringe)));
                    result[(height - 1 - y) * width + x] = c;
                }
                layer.SetPixels32(result);
                layer.Apply(false);
                File.WriteAllBytes(output, layer.EncodeToPNG());
                AssetDatabase.ImportAsset(output, ImportAssetOptions.ForceSynchronousImport);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(stone);
                UnityEngine.Object.DestroyImmediate(layer);
            }
        }

        static void BuildCemeteryLawn()
        {
            const string output = Art + "Ground/CemeteryLawn.png";
            if (File.Exists(output)) return;
            const int width = 3168, height = 2160;
            var tile = new Texture2D(2, 2);
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                if (!ImageConversion.LoadImage(tile, File.ReadAllBytes(Art + "Ground/LawnTexture.png")))
                    throw new InvalidOperationException("Cannot decode the lawn texture.");
                Color32[] source = tile.GetPixels32(), result = new Color32[width * height];
                for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int i = Mirror(y + x / 6, tile.height) * tile.width + Mirror(x - y / 7 + tile.width, tile.width);
                    var c = source[i];
                    float shade = .82f + .22f * Mathf.PerlinNoise(x * .006f, y * .006f);
                    result[y * width + x] = new Color32((byte)(c.r * shade), (byte)(c.g * shade), (byte)(c.b * shade), 255);
                }
                texture.SetPixels32(result);
                texture.Apply(false);
                File.WriteAllBytes(output, texture.EncodeToPNG());
                AssetDatabase.ImportAsset(output, ImportAssetOptions.ForceSynchronousImport);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(tile);
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        static int Mirror(int pixel, int length)
        {
            int offset = pixel % (length * 2);
            return offset < length ? offset : length * 2 - offset - 1;
        }
    }
}
