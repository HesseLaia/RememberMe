using System;
using System.IO;
using UnityEngine;

namespace RememberMe
{
    public enum StoryStage { Farewell, FindDad, FindMom, OpenParcel, FindStudy, Reunion, Complete }

    [Serializable]
    public sealed class Act1Story
    {
        public StoryStage Stage;
        public RoomId Room;
        public Vector2 Feet;
        public int Graves;
        public bool Ribbon;
        public bool Photo;
        public bool Book;
        public bool Shelved;
        public bool ParcelOpened;
        public bool MomCalled;
        public bool SharedKeepsake;
        public int CemeteryLayout;

        public bool VisitOldGrave(int index)
        {
            if (index < 0 || index > 3) return false;
            Graves |= 1 << index;
            if (Graves != 15 || MomCalled) return false;
            MomCalled = true;
            return true;
        }

        public void Save(string file)
        {
            string json = JsonUtility.ToJson(this);
#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(file, json);
            PlayerPrefs.Save();
#else
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            string temp = file + ".tmp";
            File.WriteAllText(temp, json);
            if (File.Exists(file)) File.Replace(temp, file, null);
            else File.Move(temp, file);
#endif
        }

        public static bool TryRead(string file, out Act1Story story)
        {
            story = null;
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                string json = PlayerPrefs.GetString(file, "");
#else
                if (!File.Exists(file)) return false;
                string json = File.ReadAllText(file);
#endif
                if (!json.Contains("\"Stage\"") || !json.Contains("\"Room\"") || !json.Contains("\"Feet\"")) return false;
                story = JsonUtility.FromJson<Act1Story>(json);
                return story != null && Enum.IsDefined(typeof(RoomId), story.Room) &&
                    Enum.IsDefined(typeof(StoryStage), story.Stage) && story.Room != RoomId.Flashback &&
                    float.IsFinite(story.Feet.x) && float.IsFinite(story.Feet.y) &&
                    story.Feet.x >= 0 && story.Feet.x <= (story.Room == RoomId.Cemetery ? Act1Room.CemeterySize.x : Act1Room.HouseSize.x) &&
                    story.Feet.y >= 0 && story.Feet.y <= (story.Room == RoomId.Cemetery ? Act1Room.CemeterySize.y : Act1Room.HouseSize.y);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                                           error is ArgumentException)
            {
                return false;
            }
        }
    }
}
