using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RememberMe
{
    public sealed partial class KitchenPrototype
    {
        bool verifyingRoute;
        Vector2 routeInput;

        void StartPlaythroughIfRequested()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out var page) &&
                (page.Query == "?remember-smoke=1" || page.Query == "?kitchen-check=1"))
                StartCoroutine(VerifyChapter());
#endif
        }

        IEnumerator VerifyChapter()
        {
            verifyingRoute = true;
            yield return null;
            IEnumerator Read()
            {
                while (lines != null) { yield return null; Advance(); }
            }
            IEnumerator Select(int index)
            {
                if (choices == null) throw new InvalidOperationException("Expected a decision in " + chapter);
                yield return null;
                hud.ChooseForVerification(index);
                yield return Read();
            }
            IEnumerator Visit(string label, Vector2 destination)
            {
                yield return WalkTo(destination);
                var target = Array.Find(targets, item => item.Label == label);
                if (target == null) throw new InvalidOperationException("Absent target " + label);
                Vector2 point = target == grandmaTarget ? grandma.Feet : target.Feet;
                facing = World(point - grace.Feet).normalized;
                motor.View.Look(facing);
                FindTarget();
                if (nearby != target) throw new InvalidOperationException("Cannot reach " + label + " from " + grace.Feet + ", found " + nearby?.Label);
                nearby.Use();
                yield return Read();
            }
            IEnumerator CounterVisit(string label,int x) => Visit(label,new Vector2(x,364));
            void Expect(Chapter expected)
            {
                if (chapter != expected) throw new InvalidOperationException("Expected "+expected+", got "+chapter);
            }
            yield return Read();
            yield return Visit("Open the keepsake box",new Vector2(411,503));
            yield return Select(1); yield return Select(0); Expect(Chapter.Ingredients);
            yield return CounterVisit("Family photograph",518);
            yield return CounterVisit("Music box",975);
            yield return CounterVisit("Flour cupboard",177);
            yield return CounterVisit("White jars",330); yield return Select(1);
            yield return CounterVisit("Recipe book",585); yield return Select(1); Expect(Chapter.Ingredients);
            yield return CounterVisit("Recipe book",585); yield return Select(0);
            yield return CounterVisit("White jars",330); yield return Select(0);
            yield return CounterVisit("Refrigerator",761);
            yield return Visit("Read the family recipe",new Vector2(355,554));yield return Select(1); Expect(Chapter.Cards);
            yield return CounterVisit("Mixing bowl",677); yield return CounterVisit("Baking counter",460);
            yield return CounterVisit("Refrigerator",761);
            yield return CounterVisit("Recipe book",585);
            yield return Select(0); yield return Select(0);yield return Select(0); Expect(Chapter.Cards);
            yield return CounterVisit("Recipe book",585);
            yield return Select(1); yield return Select(1);yield return Select(0); Expect(Chapter.Baking);
            for(int n=0;n<4;n++){yield return CounterVisit("Mixing bowl",677);yield return Select(0);}
            Expect(Chapter.Tray);
            yield return CounterVisit("Blue serving tray",895);
            yield return Visit("Kitchen doorway",new Vector2(576,723));
            yield return Visit("Kitchen doorway",new Vector2(576,723));
            yield return Visit("Listen at the left door",new Vector2(317,369));
            yield return Visit("Kitchen doorway",new Vector2(576,723));
            yield return Visit("Listen at the right door",new Vector2(836,369));
            yield return Visit("Look under the middle door",new Vector2(576,369)); Expect(Chapter.ReturnForGrandma);
            yield return Visit("Back to the kitchen",new Vector2(576,723));
            yield return Visit("Talk to Grandma",grandma.Feet+new Vector2(0,34));Expect(Chapter.Escort);
            yield return Visit("Kitchen doorway",new Vector2(576,723));
            yield return WalkTo(new Vector2(576,369));
            for (int i=0;i<180 && Vector2.Distance(grace.Feet,grandma.Feet)>90;i++) yield return null;
            yield return Visit("Look under the middle door",new Vector2(576,369));Expect(Chapter.Ending);
            float deadline=Time.realtimeSinceStartup+15;
            while(chapter != Chapter.Finished && Time.realtimeSinceStartup<deadline)yield return null;
            yield return Read(); Expect(Chapter.Finished);
            verifyingRoute = false;
            Debug.Log("KITCHEN_ROUTE_PASS: walked every puzzle, recovered from wrong choices and doors, escorted Grandma and reached the coat.");
        }

        IEnumerator WalkTo(Vector2 goal)
        {
            const float limit = 12f;
            float deadline = Time.realtimeSinceStartup+limit;
            while (Vector2.Distance(grace.Feet,goal)>5f)
            {
                if (Time.realtimeSinceStartup>deadline)throw new InvalidOperationException("Route blocked in "+chapter+" at "+grace.Feet+" going to "+goal);
                Vector2 waypoint = goal;
                if (!ClearSegment(grace.Feet,goal)) waypoint=FirstWaypoint(grace.Feet,goal);
                float end=Time.time+.12f;
                do {
                    var delta=waypoint-grace.Feet;
                    routeInput=new Vector2(delta.x,-delta.y).normalized*Mathf.Clamp01(delta.magnitude/16f);
                    yield return null;
                } while (Time.time<end && Vector2.Distance(grace.Feet,waypoint)>5f);
            }
            routeInput=Vector2.zero;
            yield return new WaitForFixedUpdate();yield return null;
        }

        static bool ClearSegment(Vector2 from,Vector2 to)
        {
            var delta=World(to-from);
            return !Physics2D.CircleCast(World(from),.118f,delta.normalized,delta.magnitude,(1<<8)|(1<<9));
        }

        static Vector2 FirstWaypoint(Vector2 start,Vector2 goal)
        {
            const int step=20;
            var queue=new Queue<Vector2Int>(); var visited=new HashSet<Vector2Int>();
            var first=new Dictionary<Vector2Int,Vector2>();
            var origin=Vector2Int.RoundToInt(start/step);
            queue.Enqueue(origin);visited.Add(origin);
            while(queue.Count>0)
            {
                var node=queue.Dequeue();Vector2 point=node==origin?start:(Vector2)node*step;
                if(node!=origin && ClearSegment(point,goal))return first[node];
                for(int x=-1;x<=1;x++)for(int y=-1;y<=1;y++)
                {
                    var next=node+new Vector2Int(x,y);Vector2 at=(Vector2)next*step;
                    if(at.x<110||at.x>1040||at.y<180||at.y>800||visited.Contains(next)||!ClearSegment(point,at))continue;
                    visited.Add(next);queue.Enqueue(next);
                    first[next]=node==origin?at:first[node];
                }
            }
            return start;
        }
    }
}
