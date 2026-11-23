using System;
using System.Collections.Generic;
using UnityEngine;
using Expression = RememberMe.Act1Game.Expression;

namespace RememberMe
{
    public sealed partial class KitchenPrototype : MonoBehaviour
    {
        const float Width = 1152f;
        const float Height = 864f;
        const float Ppu = 134f;

        enum Room { Kitchen, Hall, Parlor, Bedroom }
        enum Chapter { Bedroom, Ingredients, Cards, Baking, Tray, FindDoor, ReturnForGrandma, Escort, Ending, Finished }

        [Serializable] sealed class RoomCatalog { public RoomArt[] rooms; }
        [Serializable] sealed class RoomArt { public string art; public float top; public Rect[] furniture; }

        sealed class Actor
        {
            public ActorView View;
            public SpriteRenderer Renderer => View.Renderer;
            public Vector2 Feet;

        }

        sealed class Target
        {
            public readonly Vector2 Feet;
            public readonly string Label;
            public readonly Action Use;
            public Target(float x, float y, string label, Action use)
            {
                Feet = new Vector2(x, y);
                Label = label;
                Use = use;
            }
        }

        RoomCatalog catalog;
        Room currentRoom;
        Chapter chapter;
        Camera view;
        SpriteRenderer backdrop;
        Material roomMaterial;
        [SerializeField] Sprite[] graceIdle, graceWalk, graceDiagonal, graceRun, graceCrouch;
        [SerializeField] Sprite[] grandmaIdle, grandmaWalk;
        GraceMotor motor;
        GameObject obstacles;
        Collider2D grandmaFeet;
        AudioSource footsteps;
        AudioClip[] woodSteps;
        int lastStep = -1;
        Actor grace;
        Actor grandma;
        AudioSource music;
        AudioSource sound;
        Sprite[] graceFaces, grandmaFaces;
        Expression mood;
        Texture2D photo, parlorPreview;
        readonly Texture2D[] cardArt = new Texture2D[3];
        int visibleCard = -1;
        bool throughDoor, waterSpace;
        [SerializeField] Font typeface;
        [SerializeField] Sprite recipeTable, shadow;
        Act1HUD hud;
        Act1Patrol grandmaRoutine;
        Rigidbody2D grandmaBody;
        SpriteRenderer table, tableShadow;
        Target grandmaTarget;
        int choiceFrame;
        Vector2 cameraVelocity;
        Vector2 facing = Vector2.up;
        readonly Queue<Vector2> trail = new Queue<Vector2>();
        Vector2 lastTrailPoint;

        Target[] targets;
        Target nearby;
        string speaker;
        string[] lines;
        Action afterDialogue;
        int line;
        int anchors;
        bool looped;
        bool showPhoto;
        float decay;
        float endingTime, transition;
        Texture2D parcel, quilt;
        int wrongDoors;

        void Awake()
        {
            catalog = JsonUtility.FromJson<RoomCatalog>(Resources.Load<TextAsset>("KitchenChapter/Data/Rooms").text);
            view = new GameObject("Story camera").AddComponent<Camera>();
            view.gameObject.AddComponent<AudioListener>();
            view.orthographic = true;
            view.orthographicSize = Height / Ppu / 2f;
            view.transform.position = new Vector3(Width / Ppu / 2f, -Height / Ppu / 2f, -10f);
            view.clearFlags = CameraClearFlags.SolidColor;
            view.backgroundColor = new Color(.041f, .045f, .058f);
            backdrop = new GameObject("Room illustration").AddComponent<SpriteRenderer>();
            roomMaterial = new Material(Resources.Load<Shader>("KitchenChapter/Art/MemoryWash"));
            backdrop.sharedMaterial = roomMaterial;
            backdrop.transform.position = new Vector3(0, -Height / Ppu, 0);
            hud = gameObject.AddComponent<Act1HUD>();
            hud.BrowserTypeface = typeface;
            hud.Initialize();
            table = Prop("Family recipe table", recipeTable, new Vector2(355,534), 193,121);
            tableShadow = Prop("Table contact shadow",shadow,new Vector2(355,545),206,34);
            tableShadow.sortingOrder = 1;
            graceFaces = Act1Game.LoadGraceExpressions();
            grandmaFaces = Act1Game.LoadGrandmaExpressions();
            grace = CreateActor("Grace", graceIdle, graceWalk, .88f);
            grace.View.WalkDiagonal = graceDiagonal;
            grace.View.Run = graceRun;
            grace.View.Crouch = graceCrouch;
            motor = grace.View.gameObject.AddComponent<GraceMotor>();
            motor.View = grace.View;
            motor.Body = motor.GetComponent<Rigidbody2D>();
            motor.Body.bodyType = RigidbodyType2D.Kinematic;
            motor.Body.interpolation = RigidbodyInterpolation2D.Interpolate;
            motor.Body.gravityScale = 0;
            motor.Feet = motor.GetComponent<CircleCollider2D>();
            motor.Feet.radius = .115f / motor.transform.localScale.x;
            motor.Feet.offset = Vector2.zero;
            grandma = CreateActor("Grandma", grandmaIdle, grandmaWalk, 1.03f);
            var figure = new GameObject("Grandma figure");
            figure.transform.SetParent(grandma.View.transform,false);
            figure.AddComponent<GrandmaFigure>().Bind(grandma.View);
            grandma.View.gameObject.layer = 9;
            var grandmaCollider = grandma.View.gameObject.AddComponent<CircleCollider2D>();
            grandmaCollider.radius = .12f / grandma.View.transform.localScale.x;
            grandmaCollider.offset = Vector2.zero;
            grandmaFeet = grandmaCollider;
            grandmaBody = grandma.View.gameObject.AddComponent<Rigidbody2D>();
            grandmaBody.bodyType = RigidbodyType2D.Kinematic;
            grandmaBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            grandmaRoutine = grandma.View.gameObject.AddComponent<Act1Patrol>();
            grandmaRoutine.Grace = motor;
            grandmaRoutine.View = grandma.View;
            grandmaRoutine.Body = grandmaBody;
            grandmaRoutine.Speed = .62f;
            grandmaRoutine.Wait = 3.2f;
            grandmaRoutine.Route = new[] {World(new Vector2(625,397)),World(new Vector2(695,373)),World(new Vector2(536,389))};
            var feetShade = Prop("Grandma contact shadow", shadow, Vector2.zero, 47,13);
            feetShade.transform.SetParent(grandma.View.transform, true);
            feetShade.transform.localPosition = new Vector3(0,-.015f,0);
            feetShade.sortingOrder = 2;
            grandmaShadow = feetShade;
            footsteps = gameObject.AddComponent<AudioSource>();
            footsteps.volume = .55f;
            woodSteps = Resources.LoadAll<AudioClip>("Footsteps/Wood");
            motor.Footfall += PlayWoodStep;
            music = gameObject.AddComponent<AudioSource>();
            music.loop = true;
            music.volume = .32f;
            sound = gameObject.AddComponent<AudioSource>();
            parcel = LoadTexture("Parcel");
            quilt = LoadTexture("Quilt");
            photo = LoadTexture("Photograph");
            for (int i = 0; i < cardArt.Length; i++) cardArt[i] = LoadTexture("Card" + i);
            parlorPreview = LoadTexture("Parlor");
            kitchenArt = LoadTexture("Kitchen");
            cookieTray = new GameObject("Grandma’s enamel tray").AddComponent<SpriteRenderer>();
            enamel = new Material(Resources.Load<Shader>("KitchenChapter/Art/EnamelTray"));
            var white = Texture2D.whiteTexture;
            trayQuad = Sprite.Create(white, new Rect(0,0,white.width,white.height),new Vector2(.5f,.5f),white.width,0,SpriteMeshType.FullRect);
            cookieTray.sprite = trayQuad;
            cookieTray.sharedMaterial = enamel;
            Restart();
            StartPlaythroughIfRequested();
        }

        static Texture2D LoadTexture(string name)
        {
            var texture = Resources.Load<Texture2D>("KitchenChapter/Art/" + name);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            return texture;
        }

        static Actor CreateActor(string name, Sprite[] idle, Sprite[] walk, float height)
        {
            var obj = new GameObject(name);
            var actor = obj.AddComponent<ActorView>();
            actor.Renderer = obj.AddComponent<SpriteRenderer>();
            actor.Idle = idle;
            actor.Walk = walk;
            actor.transform.localScale *= height / idle[0].bounds.size.y;
            actor.Look(Vector2.down);
            return new Actor {View = actor};
        }

        static Vector2 World(Vector2 feet) => new Vector2(feet.x, -feet.y) / Ppu;

        void PlayWoodStep()
        {
            if (woodSteps.Length == 0 || !motor.CanMove) return;
            int index = lastStep < 0 ? UnityEngine.Random.Range(0, woodSteps.Length) :
                (lastStep + 1 + UnityEngine.Random.Range(0, Mathf.Max(1, woodSteps.Length - 1))) % woodSteps.Length;
            lastStep = index;
            footsteps.pitch = UnityEngine.Random.Range(.94f, 1.06f);
            float level = motor.Crouched ? .23f : motor.Running ? .58f : .42f;
            footsteps.PlayOneShot(woodSteps[index], level * UnityEngine.Random.Range(.87f, 1.13f));
        }

        SpriteRenderer grandmaShadow;
        SpriteRenderer cookieTray;
        Material enamel;
        Sprite trayQuad;
        Texture2D kitchenArt;
        bool inspectJars, sortingCards;
        int[] remainingCardIds = Array.Empty<int>();
        int exitWaypoint;
        float musicFade;
        AudioClip queuedMusic;
        readonly Vector2[] coatRoute = {new Vector2(508,503), new Vector2(418,380), new Vector2(371,244), new Vector2(319,246)};

        void OnDestroy()
        {
            if (trayQuad) Destroy(trayQuad);
            if (enamel) Destroy(enamel);
            if (roomMaterial) Destroy(roomMaterial);
        }


        SpriteRenderer Prop(string name, Sprite sprite, Vector2 feet, float width, float height)
        {
            var prop = new GameObject(name).AddComponent<SpriteRenderer>();
            prop.sprite = sprite;
            prop.transform.position = World(feet);
            prop.transform.localScale = new Vector3(width / (Ppu * sprite.bounds.size.x), height / (Ppu * sprite.bounds.size.y),1);
            prop.sortingOrder = ActorView.Depth(prop.transform.position.y);
            return prop;
        }

        void Restart()
        {
            chapter = Chapter.Bedroom;
            looped = false;
            wrongDoors = anchors = ingredients = cards = baking = errors = 0;
            photoSeen = showPhoto = false;
            endingTime = decay = 0;
            exitWaypoint = 0;
            waterSpace = throughDoor = inspectJars = sortingCards = false;
            visibleCard = -1;
            choices = null;
            PlayMusic("Rain");
            Enter(Room.Bedroom);
            Say("Grace", Expression.Quiet, new[] { "The rain still hasn't stopped. The box is just where I left it, by my bed." });
        }

        void Enter(Room room)
        {
            currentRoom = room;
            transition = .45f;
            var art = LoadTexture(catalog.rooms[(int)room].art);
            if (backdrop.sprite != null) Destroy(backdrop.sprite);
            backdrop.sprite = Sprite.Create(art, new Rect(0, 0, art.width, art.height),
                Vector2.zero, Ppu, 0, SpriteMeshType.FullRect);
            backdrop.transform.localScale = new Vector3(Width / art.width, Height / art.height, 1f);
            grace.Feet = room == Room.Kitchen ? new Vector2(585, 475) : new Vector2(576, 627);
            grandma.Feet = room == Room.Kitchen ? new Vector2(625, 397) :
                room == Room.Hall ? new Vector2(576, 656) : new Vector2(542, 607);
            motor.Place(World(grace.Feet));
            motor.View.Look(Vector2.up);
            facing = Vector2.up;
            grandma.View.transform.position = World(grandma.Feet);
            grandma.View.Look(Vector2.down);
            grandmaFeet.enabled = room == Room.Kitchen && chapter != Chapter.Escort;
            grandmaBody.position = World(grandma.Feet);
            grandmaRoutine.enabled = room == Room.Kitchen && chapter < Chapter.ReturnForGrandma;
            table.enabled = tableShadow.enabled = room == Room.Kitchen;
            cameraVelocity = Vector2.zero;
            BuildObstacles();
            grandma.Renderer.enabled = room == Room.Parlor ||
                (room == Room.Kitchen) ||
                (room == Room.Hall && chapter == Chapter.Escort);
            grandmaShadow.enabled = grandma.Renderer.enabled;
            trail.Clear();
            lastTrailPoint = grace.Feet;
            targets = room == Room.Kitchen ? new[]
            {
                new Target(895, 339, "Blue serving tray", TakeTray),
                new Target(177, 339, "Flour cupboard", Flour),
                new Target(330, 339, "White jars", Jars),
                new Target(761, 339, "Refrigerator", Fridge),
                new Target(585, 339, "Recipe book", Recipe),
                new Target(677, 339, "Mixing bowl", Bowl),
                new Target(460, 339, "Baking counter", Counter),
                new Target(518, 339, "Family photograph", FamilyPhoto),
                new Target(975, 339, "Music box", MusicBox),
                grandmaTarget = new Target(625, 397, "Talk to Grandma", HelpGrandma),
                new Target(355, 538, "Read the family recipe", Recipe),
                new Target(576, 742, "Kitchen doorway", LeaveKitchen)
            } : room == Room.Hall ? new[]
            {
                new Target(317, 343, "Listen at the left door", ClockDoor),
                new Target(576, 343, "Look under the middle door", BlueDoor),
                new Target(836, 343, "Listen at the right door", SinkDoor),
                new Target(576, 742, "Back to the kitchen", () => Enter(Room.Kitchen))
            } : room == Room.Bedroom ? new[] {new Target(405, 485, "Open the keepsake box", OpenBox)} : Array.Empty<Target>();
            nearby = null;

        }

        void Update()
        {
            if (grandmaRoutine.enabled)
            {
                Vector2 pos = grandmaBody.position * Ppu;
                grandma.Feet = new Vector2(pos.x,-pos.y);
            }
            float dt = Mathf.Min(Time.deltaTime, .05f);
            FitView();
            UpdateMusic(dt);
            transition = Mathf.Max(0, transition - dt);
            if (Input.GetKeyDown(KeyCode.Escape) && lines == null && choices == null)
            {
                if (chapter == Chapter.Finished)
                    UnityEngine.SceneManagement.SceneManager.LoadScene("RememberMeAct1");
                else
                {
                    Offer("Paused",new[] {"Resume","Return to title"}, index => {
                        if (index == 1) UnityEngine.SceneManagement.SceneManager.LoadScene("RememberMeAct1");
                    });
                }
                return;
            }
            if (Input.GetKeyDown(KeyCode.R) && chapter == Chapter.Finished) Restart();

            bool confirm = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.Space);
            motor.CanMove = choices == null && lines == null && chapter < Chapter.Ending;
            if (!motor.CanMove) motor.SetInput(Vector2.zero, false, false);
            if (choices != null)
            {
                for (int n = 0; n < choices.Length; n++)
                    if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + n))) { Choose(n); break; }
                return;
            }
            if (lines != null)
            {
                nearby = null;
                if (confirm && choiceFrame != Time.frameCount) Advance();
                grandma.View.Pose(Vector2.zero, false, false, 0);
            }
            else if (chapter == Chapter.Ending && currentRoom == Room.Parlor) AdvanceEnding(dt);
            else if (chapter != Chapter.Finished)
            {
                MoveGrace();
                FindTarget();
                if (confirm) nearby?.Use();
                if (chapter == Chapter.Escort) FollowGrace(dt);
            }

            float goal = chapter == Chapter.Cards || chapter == Chapter.Baking || chapter == Chapter.Tray ? .36f : chapter == Chapter.FindDoor ? .94f :
                chapter == Chapter.ReturnForGrandma ? .90f - anchors * .22f :
                chapter == Chapter.Escort ? .28f : chapter == Chapter.Finished ? .45f : 0f;
            decay = Mathf.MoveTowards(decay, goal, dt * .48f);
            roomMaterial.SetFloat("_Decay", decay);
            roomMaterial.SetFloat("_Path", chapter == Chapter.Escort ? 1f : anchors * .24f);
        }

        void FitView()
        {
            float ratio = Screen.width / (float)Mathf.Max(1, Screen.height);
            bool kitchen = currentRoom == Room.Kitchen;
            float aspect = kitchen ? Mathf.Clamp(ratio, 1.25f, 1.72f) : Width / Height;
            float halfHeight = kitchen ? 273f : Height / 2;
            view.orthographicSize = halfHeight / Ppu;
            Vector2 center = new Vector2(Width / 2, Height / 2);
            if (kitchen)
            {
                float halfWidth = halfHeight * aspect;
                center = new Vector2(Mathf.Clamp(grace.Feet.x,92+halfWidth,1060-halfWidth),
                    Mathf.Clamp(grace.Feet.y - 105, 38+halfHeight, 827-halfHeight));
            }
            var destination = World(center);
            var cameraPoint = kitchen ? Vector2.SmoothDamp((Vector2)view.transform.position,destination,ref cameraVelocity,.14f) : destination;
            view.transform.position = new Vector3(cameraPoint.x,cameraPoint.y,-10);
            if (ratio >= aspect)
            {
                float w = aspect / ratio;
                view.rect = new Rect((1f - w) / 2f, 0, w, 1);
            }
            else
            {
                float h = ratio / aspect;
                view.rect = new Rect(0, (1f - h) / 2f, 1, h);
            }
        }

        static bool Held(KeyCode a, KeyCode b) => Input.GetKey(a) || Input.GetKey(b);

        void MoveGrace()
        {
            var input = new Vector2(
                (Held(KeyCode.D, KeyCode.RightArrow) ? 1 : 0) - (Held(KeyCode.A, KeyCode.LeftArrow) ? 1 : 0),
                (Held(KeyCode.W, KeyCode.UpArrow) ? 1 : 0) - (Held(KeyCode.S, KeyCode.DownArrow) ? 1 : 0));
            if (verifyingRoute) input = routeInput;
            bool escort = chapter == Chapter.Escort;
            motor.WalkSpeed = escort ? 1.15f : 2.45f;
            motor.SetInput(input, !escort && Held(KeyCode.LeftShift, KeyCode.RightShift), !escort && Input.GetKeyDown(KeyCode.C));
            if (input != Vector2.zero) facing = input.normalized;
            Vector2 position = motor.Body.position * Ppu;
            grace.Feet = new Vector2(position.x, -position.y);
        }

        void BuildObstacles()
        {
            if (obstacles != null) { obstacles.SetActive(false); Destroy(obstacles); }
            obstacles = new GameObject("Walkable room boundaries");
            var data = catalog.rooms[(int)currentRoom];
            AddWall(new Rect(0,0,Width,data.top));
            AddWall(new Rect(0,0,127,Height));
            AddWall(new Rect(1025,0,Width-1025,Height));
            AddWall(new Rect(0,675,523,Height-675));
            AddWall(new Rect(630,675,Width-630,Height-675));
            AddWall(new Rect(523,806,107,Height-806));
            foreach (Rect rect in data.furniture) AddWall(rect);
            if (currentRoom == Room.Kitchen) AddWall(new Rect(279,507,152,24));
            if (currentRoom == Room.Bedroom) AddWall(new Rect(202,420,163,64));
            Physics2D.SyncTransforms();
        }

        void AddWall(Rect rect)
        {
            var wall = new GameObject("Boundary");
            wall.layer = 8;
            wall.transform.SetParent(obstacles.transform);
            wall.transform.position = World(rect.center);
            wall.AddComponent<BoxCollider2D>().size = rect.size / Ppu;
        }

        void FollowGrace(float dt)
        {
            if (Vector2.Distance(lastTrailPoint, grace.Feet) >= 11f)
            {
                trail.Enqueue(grace.Feet);
                lastTrailPoint = grace.Feet;
            }
            if (Vector2.Distance(grandma.Feet, grace.Feet) < 34f) trail.Clear();
            if (trail.Count == 0)
            {
                grandma.View.Pose(Vector2.zero, false, false, 0);
                return;
            }
            Vector2 point = trail.Peek();
            Vector2 next = Vector2.MoveTowards(grandma.Feet, point, 210f * dt);
            MoveActor(grandma, next, dt);
            if (next == point) trail.Dequeue();
        }

        static void MoveActor(Actor actor, Vector2 next, float dt)
        {
            Vector2 delta = World(next - actor.Feet);
            actor.Feet = next;
            actor.View.transform.position = World(next);
            actor.View.Pose(delta, false, false, dt);
        }

        void FindTarget()
        {
            nearby = null;
            float best = currentRoom == Room.Kitchen ? 39f : 96f;
            foreach (var target in targets)
            {
                Vector2 delta = (target == grandmaTarget ? grandma.Feet : target.Feet) - grace.Feet;
                float distance = delta.magnitude;
                if (distance >= best) continue;
                if (distance > 36f && Vector2.Dot(facing, new Vector2(delta.x, -delta.y).normalized) < -.35f) continue;
                best = distance;
                nearby = target;
            }
        }


        int ingredients, cards, baking, errors;
        bool photoSeen;
        string[] choices;

        Action<int> choose;

        void Offer(string title, string[] options, Action<int> callback)
        {
            hud.HideDialogue();
            hud.ShowHUD(false);
            hud.Menu(title,title,options,Choose,photoMenu:true);
            choices = options;
            choose = callback;
            nearby = null;
        }

        void Choose(int index)
        {
            hud.HideMenu();
            inspectJars = sortingCards = false;
            choiceFrame = Time.frameCount;
            choices = null;
            var callback = choose;
            choose = null;
            callback(index);
        }

        void OpenBox()
        {
            Offer("Inside the keepsake box", new[] {"Lift the old photograph", "Wind the little music box"}, index => {
                if (index == 1) { Play("RM_Chime", .35f); Say("Grace", Expression.Quiet, new[] {"Just a few notes. Then it stops."}, OpenBox); return; }
                showPhoto = true;
                Say("Grace", Expression.Reflective, new[] {"That's me. And Grandma. I was so little.", "We used to make cookies."}, () => {
                    Play("RM_StoryTap", .35f);
                    showPhoto = false;
                    chapter = Chapter.Ingredients;
                    Enter(Room.Kitchen);
                    PlayMusic("RM_StoryKeepsake");
                    Say("Grandma", Expression.Warm, new[] {"Gracie? There you are. Come help me, sweetheart."},
                        () => Say("Grace", Expression.Surprise, new[] {"Grandma? You're here! Can you come home with me?"},
                        () => Say("Grandma", Expression.Warm, new[] {"After the cookies, dear. Your dad's waiting.", "Could you get the flour, butter, eggs, and sugar?"})));
                });
            });
        }

        void FamilyPhoto()
        {
            photoSeen = true;
            Say("Grace", Expression.Surprise, new[] {"That's Dad! He's so little."}, () =>
                Say("Grandma", Expression.Reflective, new[] {"By the living room window. See the blue curtains? He always waited there for cookies."}));
        }

        void MusicBox()
        {
            Play("RM_Chime", .34f);
            Say("Grandma", Expression.Warm, new[] {"Your dad liked that song. I used to play it at bedtime."});
        }

        void Flour()
        {
            ingredients |= 1;
            Say("Grace", new[] {"Flour. I'll set the bag on the counter."});
        }

        void Fridge()
        {
            if (chapter == Chapter.Cards) { Card(4, "Shape the cookies"); return; }
            ingredients |= 2;
            Say("Grace", new[] {"Butter and eggs, together on the shelf."});
        }

        void Jars()
        {
            inspectJars = true;
            Offer("Two white jars. The labels have worn away.", new[] {"Jar with the blue ribbon", "Plain jar"}, i => {
                if (i == 0) ingredients |= 4;
                Say("Grace", new[] {i == 0 ? "Sugar. I'll take the ribbon jar." : "Wait. This might be salt. I'll put it back."});
            });
        }

        void Card(int bit, string name)
        {
            cards |= bit;
            visibleCard = bit == 1 ? 0 : bit == 2 ? 1 : 2;
            Say("Grace", new[] {"Dad drew this one: " + name + "."}, () => visibleCard = -1);
        }

        void Bowl()
        {
            if (chapter == Chapter.Cards) Card(1, "Add eggs and flour");
            else if (chapter == Chapter.Baking) Bake();
            else Say("Grace", new[] {"Grandma's wooden spoon is resting in the bowl."});
        }

        void Counter()
        {
            if (chapter == Chapter.Cards) Card(2, "Mix into dough");
            else if (chapter == Chapter.Baking) Bake();
            else Recipe();
        }

        void Recipe()
        {
            if (chapter == Chapter.Ingredients)
            {
                Offer("The old recipe book", new[] {"Read Dad's note", "Set out the ingredients"}, i => {
                    if (i == 0) Say("Dad's note", new[] {"Mom, sugar has the blue ribbon. Salt has no ribbon. Take your time. Love, your son."});
                    else if (ingredients != 7) Say("Grace", new[] {"We're missing something. Flour from the cupboard, butter and eggs from the fridge, sugar from the jars."});
                    else {
                        chapter = Chapter.Cards;
                        Say("Grandma", Expression.Reflective, new[] {"Your dad used to steal the sugar. He thought I never noticed.", "Now, let's see... What comes next? This part is hard to read."},
                            () => Say("Grace", Expression.Concern, new[] {"The clock stopped. There are drawings by the bowl, the stove and the fridge."}));
                    }
                });
            }
            else if (chapter == Chapter.Cards)
            {
                if (cards != 7) Say("Grace", new[] {"Three steps are missing. I'll look by the bowl, the stove and the fridge."});
                else SortCards(new List<int>());
            }
            else if (chapter == Chapter.Baking) Bake();
            else Say("Grace", new[] {"Dad's pictures are still here."});
        }

        void SortCards(List<int> order)
        {
            string[] names = {"Add eggs and flour", "Mix into dough", "Shape the cookies"};
            var remaining = new List<int> {2,0,1};
            remaining.RemoveAll(order.Contains);
            remainingCardIds = remaining.ToArray();
            sortingCards = true;
            string sequence = "Cream butter and sugar";
            foreach (int step in order) sequence += " → " + names[step];
            Offer(sequence + "   ·   Choose step " + (order.Count+2), remaining.ConvertAll(i => names[i]).ToArray(), selection => {
                order.Add(remaining[selection]);
                if (order.Count < 3) { SortCards(order); return; }
                if (order[0] != 0 || order[1] != 1) {
                    errors++;
                    Say("Grace", new[] {errors > 1 ? "We add the ingredients before we mix. Shaping comes last." : "Hmm. That doesn't look right. Let's try again."});
                    return;
                }
                chapter = Chapter.Baking;
                Say("Grace", Expression.Warm, new[] {"Look, Grandma. Dad drew these."}, () =>
                    Say("Grandma", Expression.Warm, new[] {"Oh. These are nice. Shall we do it together?"}));
            });
        }

        void Bake()
        {
            string[] actions = {"Turn the spoon together", "Fold the dough over", "Place the first rounds on the tray", "Shape a bigger cookie for Dad"};
            Offer("At the mixing bowl", new[] {actions[baking]}, i => {
                Play("RM_StoryTap", .25f);
                baking++;
                if (baking == 3) Say("Grandma", Expression.Warm, new[] {"Wait. Let's make this one bigger. Your dad always likes the biggest one."});
                else if (baking == 4) {
                    chapter = Chapter.Tray;
                    Play("RM_Chime", .4f);
                    Say("Grace", Expression.Warm, new[] {"They smell good!"}, () => Say("Grandma", Expression.Warm, new[] {"We made them together. Let's bring these to your dad."}));
                }
                else Say("Grandma", new[] {baking == 1 ? "I'll hold the bowl, love. You turn the spoon." : "There. Feel how the dough holds together now?"});
            });
        }

        void TakeTray()
        {
            if (chapter < Chapter.Tray) { Say("Grace", new[] { "We'll need this when the cookies are ready." }); return; }
            if (chapter != Chapter.Tray)
            {
                Say("Grace", new[] { "I still have the tray. The cookies are warm." });
                return;
            }
            chapter = Chapter.FindDoor;
            Play("RM_Swell", .54f);
            Say("Grandma", Expression.Concern, new[] { "The living room is just… Where did the door go?" },
                () => Say("Grace", new[] { "Wait here. I’ll find it." }));
        }

        void LeaveKitchen()
        {
            if (chapter <= Chapter.Tray)
            {
                Say("Grace", new[] { "We promised Dad cookies. We still have work to do." });
                return;
            }
            if (!looped)
            {
                looped = true;
                Play("RM_DoorCreak", .52f);
                Enter(Room.Kitchen);
                grace.Feet = new Vector2(485, 641);
                motor.Place(World(grace.Feet));
                Say("Grace", Expression.Surprise, new[] { "I just went through that door. Why am I back in the kitchen?" });
                return;
            }
            Enter(Room.Hall);
        }

        void ClockDoor()
        {
            wrongDoors++;
            Play("RM_StoryTap", .52f);
            Say("Grace", new[] { "Tick. Tick. Our clock sounded like that before it stopped." }, () =>
            {
                if (chapter == Chapter.Escort) return;
                Play("RM_DoorCreak", .42f);
                Enter(Room.Kitchen);
            });
        }

        void SinkDoor()
        {
            Play("RM_StoryDrip", .62f);
            wrongDoors++;
            transition = .45f;
            waterSpace = true;
            Say("Grace", Expression.Tense, new[] { "Water dripping. The walls have disappeared.", wrongDoors >= 2 ? "Dad sat by the blue curtains. I should look for that light." : "There’s the hallway again." }, () => waterSpace = false);
        }

        void BlueDoor()
        {
            if (chapter == Chapter.Escort)
            {
                if (Vector2.Distance(grace.Feet, grandma.Feet) > 100f)
                {
                    Say("Grace", new[] { "One moment. We’ll go through together." });
                    return;
                }
                chapter = Chapter.Ending;
                Enter(Room.Parlor);
                endingTime = 0f;
                Say("Grandma", new[]
                {
                    "Leave the biggest one by the window, would you?",
                    "He’ll be home from school any minute."
                }, () => Say("Grace", Expression.Surprise, new[] { "From school?" },
                    () => Say("Grandma", new[] { "Oh, his coat. It gets cold when the sun goes down." })));
                return;
            }
            if (chapter == Chapter.ReturnForGrandma)
            {
                Say("Grace", new[] { "I’m going back for Grandma." });
                return;
            }
            chapter = Chapter.ReturnForGrandma;
            throughDoor = true;
            Play("RM_DoorCreak", .42f);
            Say("Grace", new[]
            {
                photoSeen ? "Like the blue curtains in Dad’s picture!" : "Blue curtains. There’s an empty chair by the window.",
                "Grandma? … She’s still in the kitchen."
            }, () => throughDoor = false);
        }

        void HelpGrandma()
        {
            if (chapter == Chapter.Ingredients) { Say("Grandma", new[] {"Could you get the flour, butter, eggs, and sugar?"}); return; }
            if (chapter == Chapter.Cards || chapter == Chapter.Baking) { Say("Grandma", new[] {"Let's look at the pictures on the counter together."}); return; }
            if (chapter != Chapter.ReturnForGrandma)
            {
                Say("Grandma", new[] { "Make sure the one in the middle doesn’t fall off." });
                return;
            }
            Say("Grandma", Expression.Concern, new[] {"Grace? Where did you go? I can't find the door."},
                () => Say("Grace", Expression.Warm, new[] {"I'm here. I found it. Come with me."},
                () => Say("Grandma", Expression.Quiet, new[] {"Will you stay with me?"},
                () => Say("Grace", Expression.Warm, new[] {"Yes. Take my hand."}, () => {
                    chapter = Chapter.Escort;
                    grandmaRoutine.enabled = false;
                    grandmaFeet.enabled = false;
                    trail.Clear();
                    lastTrailPoint = grace.Feet;
                    PlayMusic("RM_Remember");
                }))));
        }

        void AdvanceEnding(float dt)
        {
            float previous = endingTime;
            endingTime += dt;
            if (exitWaypoint < coatRoute.Length)
            {
                Vector2 next = Vector2.MoveTowards(grandma.Feet,coatRoute[exitWaypoint],119f * dt);
                MoveActor(grandma,next,dt);
                if (next == coatRoute[exitWaypoint]) exitWaypoint++;
            }
            else grandma.View.Look(Vector2.up);
            if (previous < 3.8f && endingTime >= 3.8f) Play("Key", .7f);
            if (endingTime < 4.9f || exitWaypoint < coatRoute.Length) return;
            chapter = Chapter.Finished;
            Play("RM_Chime", .47f);
            Say("Grace", Expression.Tense, new[] { "Grandma, wait—!" });
        }

        void Say(string name, string[] words, Action onFinish = null)
            => Say(name, Expression.Neutral, words, onFinish);
        
        void Say(string name, Expression expression, string[] words, Action onFinish = null)
        {
            mood = expression;
            speaker = name;
            lines = words;
            line = 0;
            afterDialogue = onFinish;
            if (name == "Grandma" && grandma.Renderer.enabled)
                grandma.View.Look(World(grace.Feet - grandma.Feet));
            PresentLine();
        }

        void PresentLine()
        {
            var faces = speaker == "Grandma" ? grandmaFaces : speaker == "Grace" ? graceFaces : null;
            hud.Dialogue(speaker,lines[line],faces == null ? null : faces[(int)mood]);
            hud.ShowHUD(false);
        }

        void LateUpdate()
        {
            PositionTray();
            bool exploring = lines == null && choices == null;
            hud.ShowHUD(exploring);
            if (exploring)
                hud.Guide(Objective(),"E",nearby == null ? "" :
                    nearby == grandmaTarget && chapter == Chapter.ReturnForGrandma ? "Offer Grandma your hand" : nearby.Label);
        }

        void Advance()
        {
            if (++line < lines.Length) { PresentLine(); return; }
            hud.HideDialogue();
            lines = null;
            var callback = afterDialogue;
            afterDialogue = null;
            callback?.Invoke();
        }

        void Play(string name, float volume) =>
            sound.PlayOneShot(Resources.Load<AudioClip>("KitchenChapter/Audio/" + name), volume);

        void PlayMusic(string name)
        {
            queuedMusic = Resources.Load<AudioClip>("KitchenChapter/Audio/" + name);
            if (music.clip == queuedMusic && music.isPlaying) { queuedMusic = null; return; }
            musicFade = music.isPlaying ? .38f : 0f;
        }

        void UpdateMusic(float dt)
        {
            if (queuedMusic != null)
            {
                musicFade = Mathf.Max(0,musicFade-dt);
                music.volume = Mathf.Min(music.volume,.32f*musicFade/.38f);
                if (musicFade > 0) return;
                music.clip = queuedMusic;
                queuedMusic = null;
                music.Play();
            }
            music.volume = Mathf.MoveTowards(music.volume,.32f,dt*.46f);
        }

        void PositionTray()
        {
            cookieTray.enabled = chapter >= Chapter.Tray;
            if (!cookieTray.enabled) return;
            bool carried = chapter > Chapter.Tray && chapter < Chapter.Ending;
            Vector2 center = carried ? grace.Feet + new Vector2(18,-36) :
                currentRoom == Room.Parlor ? new Vector2(778,290) : new Vector2(895,212);
            cookieTray.transform.position = World(center);
            cookieTray.transform.localScale = new Vector3(carried ? .27f : .41f,carried ? .13f : .19f,1);
            cookieTray.sortingOrder = carried ? grace.Renderer.sortingOrder + 1 : 3;
        }

        static int CountFinds(int bits) => (bits & 1) + ((bits >> 1) & 1) + ((bits >> 2) & 1);

        string Objective()
        {
            switch (chapter)
            {
                case Chapter.Bedroom: return "Grace’s bedroom · the night of the funeral";
                case Chapter.Ingredients:
                    if (ingredients == 7) return "Set the ingredients beside the recipe.";
                    return "Gather ingredients  ·  " + CountFinds(ingredients) + " / 3";
                case Chapter.Cards: return cards == 7 ? "Put Dad’s cards in order at the recipe book." :
                    "Find Dad’s recipe cards  ·  " + CountFinds(cards) + " / 3";
                case Chapter.Baking: return "Make the cookies together at the bowl.";
                case Chapter.Tray: return "Pick up the blue serving tray.";
                case Chapter.FindDoor: return "Find the room with the blue curtains.";
                case Chapter.ReturnForGrandma:
                    if (currentRoom == Room.Hall) return "Return to the kitchen for Grandma.";
                    return "Go to Grandma and offer her your hand.";
                case Chapter.Escort:
                    return currentRoom == Room.Kitchen ? "Lead Grandma back to the hallway." : "Walk with Grandma to the blue curtains.";
                case Chapter.Finished: return "Next · The Coat by the Door   /   R to replay · Esc for title";
                default: return "By the Blue Curtains";
            }
        }

        void OnGUI()
        {
            if (view == null) return;
            Rect viewport = view.pixelRect;
            var matrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(new Vector3(viewport.xMin, Screen.height - viewport.yMax),
                Quaternion.identity, new Vector3(viewport.width / Width, viewport.height / Height, 1f));

            if (currentRoom == Room.Bedroom)
            {
                GUI.DrawTexture(new Rect(192, 347, 190, 150), quilt, ScaleMode.ScaleToFit);
                GUI.DrawTexture(new Rect(382, 409, 50, 53), parcel, ScaleMode.ScaleToFit);
            }
            if (chapter == Chapter.Baking && currentRoom == Room.Kitchen)
                DrawBaking();
            if (inspectJars)
            {
                Panel(new Rect(329,170,494,345));
                GUI.DrawTextureWithTexCoords(new Rect(338,179,476,327), kitchenArt,
                    new Rect(104f/480,1-104f/360,60f/480,41f/360));
            }
            if (waterSpace) Tint(new Rect(0,0,Width,Height),new Color(.025f,.055f,.075f,.94f));
            if (visibleCard >= 0) GUI.DrawTexture(new Rect(405,245,342,212),cardArt[visibleCard]);
            if (sortingCards)
                for (int n = 0; n < remainingCardIds.Length; n++)
                {
                    float left = (Width - 171f) / 2f + (n - (remainingCardIds.Length - 1) / 2f) * 182f;
                    GUI.DrawTexture(new Rect(left,340,171,106),cardArt[remainingCardIds[n]]);
                }
            if (throughDoor)
            {
                Panel(new Rect(240,127,672,448));
                GUI.DrawTexture(new Rect(250,137,652,428),parlorPreview,ScaleMode.ScaleToFit);
            }
            if (showPhoto)
            {
                Panel(new Rect(240, 127, 672, 448));
                GUI.DrawTexture(new Rect(250, 137, 652, 428), photo, ScaleMode.ScaleToFit);
            }

            if (transition > 0)
                Tint(new Rect(0,0,Width,Height), new Color(.08f,.08f,.08f,transition/.45f));
            GUI.matrix = matrix;
        }

        static void Tint(Rect rect, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;
        }

        void DrawBaking()
        {
            Panel(new Rect(754,24,355,71));
            Label(new Rect(771,30,330,32), "Grandma’s recipe", 19, Color.white);
            Label(new Rect(771,57,330,29), new[] {"Cream the mixture", "Fold into dough", "Shape the cookies", "The biggest one for Dad"}[baking], 16, Color.white);
        }

        static void Panel(Rect bounds)
        {
            var previous = GUI.color;
            GUI.color = new Color(.073f, .093f, .092f, .95f);
            GUI.DrawTexture(bounds, Texture2D.whiteTexture);
            GUI.color = new Color(.76f, .60f, .33f);
            GUI.DrawTexture(new Rect(bounds.x, bounds.y, 2f, bounds.height), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        static void Label(Rect bounds, string value, int size, Color color)
        {
            var style = new GUIStyle(GUI.skin.label) {fontSize = size, wordWrap = true};
            style.normal.textColor = color;
            GUI.Label(bounds, value, style);
        }
    }
}
