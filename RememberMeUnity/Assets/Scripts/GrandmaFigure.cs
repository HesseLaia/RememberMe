using System;
using System.Collections.Generic;
using UnityEngine;

namespace RememberMe
{
    [DefaultExecutionOrder(100)]
    public sealed class GrandmaFigure : MonoBehaviour
    {
        const int Columns = 41, Rows = 53;
        readonly Dictionary<Sprite, Mesh> poses = new Dictionary<Sprite, Mesh>();
        SpriteRenderer source;
        MeshFilter filter;
        MeshRenderer surface;
        Material paint;
        Sprite active;

        public void Bind(ActorView actor)
        {
            source = actor.Renderer;
            filter = gameObject.AddComponent<MeshFilter>();
            surface = gameObject.AddComponent<MeshRenderer>();
            paint = new Material(Resources.Load<Shader>("KitchenChapter/Art/FigurePaint"));
            surface.sharedMaterial = paint;
            var depths = Resources.Load<TextAsset>("KitchenChapter/Data/GrandmaVolume").text.Split(
                new[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            int frame = 0;
            foreach (var clips in new[] {actor.Idle, actor.Walk})
                foreach (var sprite in clips) poses.Add(sprite, Build(sprite, Convert.FromBase64String(depths[frame++])));
            source.forceRenderingOff = true;
        }

        static Mesh Build(Sprite sprite, byte[] depth)
        {
            int count = Columns * Rows;
            var vertices = new Vector3[2*count];
            var uv = new Vector2[vertices.Length];
            var indices = new List<int>();
            Rect r = sprite.rect;
            var span = r.size / sprite.pixelsPerUnit;
            for (int i=0; i<count; i++)
            {
                var xy = new Vector2((i%Columns)/(float)(Columns-1),(i/Columns)/(float)(Rows-1));
                var pos = Vector2.Scale(xy,span) - sprite.pivot / sprite.pixelsPerUnit;
                float relief = depth[i]/255f * span.y;
                vertices[i] = new Vector3(pos.x,pos.y,-relief);
                vertices[i+count] = new Vector3(pos.x,pos.y,relief*.72f);
                uv[i] = uv[i+count] = new Vector2((r.x+xy.x*r.width)/sprite.texture.width,(r.y+xy.y*r.height)/sprite.texture.height);
            }
            void Quad(int a,int b,int c,int d) {indices.Add(a);indices.Add(b);indices.Add(c);indices.Add(a);indices.Add(c);indices.Add(d);}
            bool Filled(int x,int y)
            {
                if(x<0 || x>=Columns-1 || y<0 || y>=Rows-1) return false;
                int i=y*Columns+x;
                return depth[i]+depth[i+1]+depth[i+Columns]+depth[i+Columns+1] > 0;
            }
            for(int y=0;y<Rows-1;y++) for(int x=0;x<Columns-1;x++)
            {
                if(!Filled(x,y)) continue;
                int a=y*Columns+x,b=a+1,c=a+Columns+1,d=a+Columns;
                Quad(a,d,c,b); Quad(a+count,b+count,c+count,d+count);
                if(!Filled(x-1,y)) Quad(a,a+count,d+count,d);
                if(!Filled(x+1,y)) Quad(b,c,c+count,b+count);
                if(!Filled(x,y-1)) Quad(a,b,b+count,a+count);
                if(!Filled(x,y+1)) Quad(d,d+count,c+count,c);
            }
            var mesh = new Mesh {name="Grandma sculpted pose",vertices=vertices,uv=uv};
            mesh.SetTriangles(indices,0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return mesh;
        }

        void LateUpdate()
        {
            surface.enabled = source.enabled;
            if (!source.enabled) return;
            if (active != source.sprite && source.sprite != null)
            {
                active=source.sprite;
                filter.sharedMesh=poses[active];
                paint.mainTexture=active.texture;
            }
            surface.sortingOrder=source.sortingOrder;
            float side=source.GetComponent<ActorView>().Forward.x;
            var aim=Quaternion.Euler(4f,6f*side,0);
            transform.localRotation=Quaternion.Slerp(transform.localRotation,aim,1-Mathf.Exp(-12*Time.deltaTime));
        }

        void OnDestroy()
        {
            foreach(var mesh in poses.Values) Destroy(mesh);
            if(paint) Destroy(paint);
        }
    }
}
