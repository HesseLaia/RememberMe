using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RememberMe
{
    public sealed class Act1HUD : MonoBehaviour
    {
        public Font BrowserTypeface;
        static readonly Color Ink = new(0.055f, 0.082f, 0.074f);
        static readonly Color Paper = new(0.98f, 0.95f, 0.88f);
        static readonly Color Gold = new(0.88f, 0.72f, 0.45f);
        static readonly Color Night = new(0.065f, 0.12f, 0.11f, 0.96f);
        static readonly Color Edge = new(0.48f, 0.39f, 0.25f, .63f);
        Font serif;
        RectTransform canvas;
        RectTransform hud;
        RectTransform modal;
        RectTransform dialogue;
        RectTransform chapterCard;
        RectTransform chapterCenter;
        Image illustration;
        Image illustrationBacking;
        Image splash;
        Image portrait;
        RectTransform portraitFrame;
        Text objective;
        Text cue;
        Text speaker;
        Text paragraph;
        Text chapterNumber;
        Text chapterTitle;
        RectTransform controls;
        RectTransform locator;
        RectTransform locatorRing;
        RectTransform finder;
        Text finderArrow;
        Canvas stageCanvas;
        CanvasGroup presentation;
        public float PresentationOpacity { get => presentation.alpha; set => presentation.alpha = value; }
        public bool IsModal => modal.gameObject.activeSelf;
        public bool IsBracketing => locatorRing.gameObject.activeInHierarchy;
        public bool HasPortrait => portrait.enabled && dialogue.gameObject.activeInHierarchy;

        public void Initialize()
        {
            var fallback = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
#if UNITY_WEBGL && !UNITY_EDITOR
            serif = BrowserTypeface ? BrowserTypeface : fallback;
#else
            serif = Font.CreateDynamicFontFromOSFont("Times New Roman", 24) ?? fallback;
#endif
            var stage = new GameObject("Storybook Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var component = stage.GetComponent<Canvas>();
            stageCanvas = component;
            component.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = stage.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 800);
            canvas = (RectTransform)stage.transform;
            _ = new GameObject("Event System", typeof(EventSystem), typeof(StandaloneInputModule));
            var art = Rect(canvas, "Presentation", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            presentation = art.gameObject.AddComponent<CanvasGroup>();
            presentation.blocksRaycasts = false;
            presentation.interactable = false;
            illustrationBacking = Surface(art, "Illustration backing", new Color(Night.r, Night.g, Night.b, 1), Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero);
            illustrationBacking.raycastTarget = false;
            illustrationBacking.enabled = false;
            illustration = Surface(art, "Illustration", Color.white, Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero);
            illustration.preserveAspect = true;
            illustration.raycastTarget = false;
            illustration.enabled = false;
            chapterCard = Surface(art, "Chapter card", new Color(Night.r, Night.g, Night.b, .81f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).rectTransform;
            chapterCenter = Rect(chapterCard, "Chapter lettering", new(.5f,.5f), new(.5f,.5f), new(-440,-95), new(440,95));
            chapterNumber = Copy(chapterCenter, "", 19, Gold, TextAnchor.MiddleCenter, new(0, 117), new(880, 150));
            Stroke(chapterCenter, Edge, new(356, 107), new(524, 108));
            chapterTitle = Copy(chapterCenter, "", 41, Paper, TextAnchor.MiddleCenter,
                new(0, 25), new(880, 99));
            chapterCard.gameObject.SetActive(false);
            hud = Rect(canvas, "HUD", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            locator = Rect(hud, "World objective", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            locatorRing = Rect(locator, "Four corners", Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            for (int i = 0; i < 4; i++)
            {
                Vector2 corner = new(i % 2, i / 2);
                Vector2 inward = Vector2.one - 2 * corner;
                var point = Rect(locatorRing, "Corner", corner, corner, Vector2.zero, Vector2.zero);
                Stroke(point, Night, Vector2.Min(new(-2, -2), inward * new Vector2(19, 5)), Vector2.Max(new(2, 2), inward * new Vector2(19, 5)));
                Stroke(point, Night, Vector2.Min(new(-2, -2), inward * new Vector2(5, 19)), Vector2.Max(new(2, 2), inward * new Vector2(5, 19)));
                Stroke(point, Gold, Vector2.Min(Vector2.zero, inward * new Vector2(17, 3)), Vector2.Max(Vector2.zero, inward * new Vector2(17, 3)));
                Stroke(point, Gold, Vector2.Min(Vector2.zero, inward * new Vector2(3, 17)), Vector2.Max(Vector2.zero, inward * new Vector2(3, 17)));
            }
            finder = Surface(locator, "Direction", Night, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero).rectTransform;
            Frame(finder, 4, Edge);
            finderArrow = Copy(finder, "▲", 19, Gold, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(38, 38));
            finderArrow.font = fallback;
            locator.gameObject.SetActive(false);
            var card = Surface(hud, "Objective card", Night, Vector2.up, Vector2.up,
                new Vector2(28, -108), new Vector2(476, -28)).rectTransform;
            Frame(card, 6, Edge);
            Stroke(card, Gold, new Vector2(0, 13), new Vector2(3, 67));
            Copy(card, "Current Objective", 16, Gold, TextAnchor.MiddleLeft,
                new Vector2(20, 51), new Vector2(430, 70));
            objective = Copy(card, "", 24, Paper, TextAnchor.MiddleLeft, new Vector2(20, 11), new Vector2(429, 50));
            objective.fontStyle = FontStyle.Bold;
            var hint = Surface(hud, "Interaction", Night, new Vector2(.5f, 0), new Vector2(.5f, 0),
                new Vector2(-267, 26), new Vector2(267, 76)).rectTransform;
            Frame(hint, 5, Edge);
            cue = Copy(hint, "", 22, Paper, TextAnchor.MiddleCenter, new Vector2(9, 0), new Vector2(525, 50));
            cue.fontStyle = FontStyle.Bold;
            controls = Surface(hud, "Controls", Night, Vector2.right, Vector2.right, new Vector2(-395, 25), new Vector2(-27, 62)).rectTransform;
            Frame(controls, 4, Edge);
            Copy(controls, "WASD  Move     Shift  Run     C  Crouch     Esc  Menu", 16, Paper, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(368, 37));
            dialogue = Surface(canvas, "Dialogue panel", Paper, new Vector2(.5f, 0), new Vector2(.5f, 0),
                new Vector2(-479, 26), new Vector2(479, 226)).rectTransform;
            Frame(dialogue, 8, Edge);
            portraitFrame = Surface(dialogue, "Portrait mount", new Color(.84f,.78f,.66f), Vector2.zero, Vector2.zero,
                new Vector2(27, 25), new Vector2(173, 175)).rectTransform;
            Frame(portraitFrame, 2, Edge);
            portrait = Surface(portraitFrame, "Speaker portrait", Color.white, Vector2.zero, Vector2.one,
                new Vector2(4, 4), new Vector2(-4, -4));
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
            Stroke(dialogue, Edge, new(198, 137), new(927, 138));
            speaker = Copy(dialogue, "", 28, Ink, TextAnchor.MiddleLeft,
                new Vector2(198, 143), new Vector2(925, 182));
            speaker.fontStyle = FontStyle.Bold;
            paragraph = Copy(dialogue, "", 28, Ink, TextAnchor.UpperLeft, new Vector2(198, 46), new Vector2(925, 128));
            Copy(dialogue, "E / Enter  ·  Continue", 17, Ink, TextAnchor.MiddleRight,
                new Vector2(196, 15), new Vector2(928, 39));
            dialogue.gameObject.SetActive(false);
            modal = Surface(canvas, "Menu", new Color(.03f, .05f, .05f, .74f), Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero).rectTransform;
            modal.gameObject.SetActive(false);
            splash = Surface(canvas, "Fade", Color.black, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            splash.raycastTarget = false;
            Fade(0);
        }

        public void Guide(string goal, string interaction)
        {
            if (objective.text != goal) objective.text = goal;
            if (cue.text != interaction) cue.text = interaction;
            cue.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(interaction));
            controls.gameObject.SetActive(string.IsNullOrEmpty(interaction));
        }

        public void Locate(Camera camera, Vector2? goal, Bounds? art)
        {
            locator.gameObject.SetActive(goal.HasValue);
            if (!goal.HasValue) return;
            Vector2 size = canvas.rect.size;
            Vector2 ScreenPoint(Vector3 p) => Vector2.Scale(camera.WorldToViewportPoint(p), size);
            Vector2 center = ScreenPoint(art?.center ?? (Vector3)goal.Value);
            bool onScreen = center.x > 33 && center.x < size.x - 33 && center.y > 52 && center.y < size.y - 30;
            bool bracket = onScreen && art.HasValue;
            locatorRing.gameObject.SetActive(bracket);
            finder.gameObject.SetActive(!bracket);
            if (bracket)
            {
                var bounds = art.Value;
                locatorRing.offsetMin = ScreenPoint(bounds.min) - new Vector2(9, 7);
                locatorRing.offsetMax = ScreenPoint(bounds.max) + new Vector2(9, 8);
            }
            else
            {
                var middle = size / 2;
                var direction = center - middle;
                Vector2 edgePoint = onScreen ? center + new Vector2(0, 36) : middle + direction /
                    Mathf.Max(Mathf.Abs(direction.x) / (middle.x - 44), Mathf.Abs(direction.y) / (middle.y - 87));
                finder.offsetMin = edgePoint - Vector2.one * 19;
                finder.offsetMax = edgePoint + Vector2.one * 19;
                finderArrow.text = onScreen ? "◆" : "▲";
                finderArrow.rectTransform.localEulerAngles = new Vector3(0, 0, onScreen ? 0 : Vector2.SignedAngle(Vector2.up, direction));
            }
        }

        public void ShowHUD(bool shown) => hud.gameObject.SetActive(shown);
        public void ChapterCard(bool visible, bool funeral = false)
        {
            if (visible)
            {
                chapterCenter.anchoredPosition = new Vector2(0, funeral ? -238 : 0);
                chapterNumber.text = funeral ? "A C T   I" : "A C T   I I";
                chapterTitle.text = funeral ? "The Funeral" : "The Things Grandma Left Behind";
            }
            chapterCard.gameObject.SetActive(visible);
        }
        public void Dialogue(string name, string line, Sprite face)
        {
            dialogue.gameObject.SetActive(true);
            speaker.text = name;
            paragraph.text = line;
            portrait.sprite = face;
            portrait.enabled = face != null;
            portraitFrame.gameObject.SetActive(face != null);
            var textInset = face ? 198 : 40;
            speaker.rectTransform.offsetMin = new(textInset, 143);
            paragraph.rectTransform.offsetMin = new(textInset, 46);
        }
        public void HideDialogue() => dialogue.gameObject.SetActive(false);
        public void Picture(Sprite image)
        {
            illustration.sprite = image;
            illustration.enabled = image != null;
            illustrationBacking.enabled = image != null;
        }

        public void PrepareCapture(Camera camera)
        {
            stageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            stageCanvas.worldCamera = camera;
            stageCanvas.planeDistance = 1;
            stageCanvas.sortingOrder = 32700;
            Canvas.ForceUpdateCanvases();
        }

        public void FinishCapture()
        {
            stageCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            stageCanvas.worldCamera = null;
        }
        public void Fade(float amount)
        {
            splash.enabled = amount > 0.001f;
            splash.color = new Color(0.06f, 0.08f, 0.075f, amount);
        }

        public void Menu(string title, string description, string[] choices, Action<int> onChoice,
            Func<int, bool> enabled = null, bool coverMenu = false, bool photoMenu = false)
        {
            HideMenu();
            modal.gameObject.SetActive(true);
            modal.GetComponent<Image>().color = coverMenu || photoMenu ? Color.clear : new Color(.03f, .05f, .05f, .74f);
            float listExtra = Mathf.Max(0, description.Split('\n').Length - 3) * 25;
            float height = coverMenu ? 74 : photoMenu ? 102 : 208 + choices.Length * 53 + listExtra;
            float width = coverMenu ? 585 : photoMenu ? 750 : 570;
            var anchor = new Vector2(.5f, coverMenu || photoMenu ? 0 : .5f);
            float minimum = coverMenu || photoMenu ? 22 : -height / 2;
            var box = Surface(modal, "Menu sheet", coverMenu || photoMenu ? Night : Paper, anchor, anchor,
                new Vector2(-width / 2, minimum), new Vector2(width / 2, minimum + height)).rectTransform;
            Frame(box, 7, coverMenu || photoMenu ? Gold : Edge);
            if (!coverMenu)
            {
                if (photoMenu) Copy(box, description, 19, Paper, TextAnchor.MiddleCenter,
                    new Vector2(12, 55), new Vector2(width - 12, 91));
                else
                {
                    Copy(box, title, 38, Ink, TextAnchor.MiddleCenter,
                        new Vector2(22, height - 86), new Vector2(width - 22, height - 24));
                    Stroke(box, Edge, new Vector2(44, height - 89), new Vector2(width - 44, height - 88));
                    Copy(box, description, 19, Ink, TextAnchor.UpperCenter,
                        new Vector2(32, height - 183 - listExtra), new Vector2(width - 32, height - 108));
                }
            }
            bool horizontal = coverMenu || photoMenu;
            var available = new List<Button>(choices.Length);
            for (int index = 0; index < choices.Length; index++)
            {
                int choice = index;
                float bottom = horizontal ? 14 : height - 225 - listExtra - index * 53;
                float buttonWidth = horizontal ? (width - 28 - (choices.Length - 1) * 9) / choices.Length : 410;
                float left = horizontal ? 14 + index * (buttonWidth + 9) : (width - buttonWidth) / 2;
                float buttonHeight = coverMenu ? 46 : photoMenu ? 38 : 43;
                Color normalFill = horizontal ? new Color(.81f, .80f, .73f) : new Color(.18f, .25f, .22f);
                Color normalText = horizontal ? Ink : Paper;
                var image = Surface(box, choices[index], normalFill, Vector2.zero, Vector2.zero,
                    new Vector2(left, bottom), new Vector2(left + buttonWidth, bottom + buttonHeight));
                var button = image.gameObject.AddComponent<Button>();
                button.targetGraphic = image;
                button.transition = Selectable.Transition.None;
                button.interactable = enabled == null || enabled(index);
                button.onClick.AddListener(() => onChoice(choice));
                var label = Copy(image.rectTransform, choices[index], 20, normalText, TextAnchor.MiddleCenter,
                    new Vector2(24, 0), new Vector2(buttonWidth - 12, buttonHeight));
                var marker = Copy(image.rectTransform, "›", 29, Ink, TextAnchor.MiddleCenter,
                    new Vector2(7, 0), new Vector2(28, buttonHeight));
                var accent = Surface(image.rectTransform, "Focus rule", new Color(.44f, .29f, .12f),
                    Vector2.zero, Vector2.up, new Vector2(0, 3), new Vector2(4, -3));
                accent.raycastTarget = false;
                image.gameObject.AddComponent<Act1MenuChoice>().Initialize(button, image, accent, label, marker, normalFill, normalText);
                if (button.interactable) available.Add(button);
            }
            for (int i = 0; i < available.Count; i++)
            {
                var navigation = new Navigation { mode = Navigation.Mode.Explicit };
                if (available.Count > 1)
                {
                    var previous = available[(i + available.Count - 1) % available.Count];
                    var next = available[(i + 1) % available.Count];
                    if (horizontal) { navigation.selectOnLeft = previous; navigation.selectOnRight = next; }
                    else { navigation.selectOnUp = previous; navigation.selectOnDown = next; }
                }
                available[i].navigation = navigation;
            }
            if (available.Count > 0) available[0].Select();
        }

        public void ChooseForVerification(int index)
        {
            var buttons = modal.GetComponentsInChildren<Button>();
            if (!buttons[index].interactable) throw new InvalidOperationException("Verification choice is disabled.");
            buttons[index].onClick.Invoke();
        }

        public void VerifyFocus(int index)
        {
            var choices = modal.GetComponentsInChildren<Act1MenuChoice>();
            for (int i = 0; i < choices.Length; i++)
                if (choices[i].IsFocused != (i == index)) throw new InvalidOperationException("Menu focus and highlight differ.");
            if (EventSystem.current.currentSelectedGameObject != choices[index].gameObject)
                throw new InvalidOperationException("Menu selected the wrong choice.");
        }

        public void MoveFocusForVerification(MoveDirection direction)
        {
            var events = EventSystem.current;
            var input = new AxisEventData(events) { moveDir = direction };
            ExecuteEvents.Execute(events.currentSelectedGameObject, input, ExecuteEvents.moveHandler);
        }

        public void SubmitForVerification()
        {
            var events = EventSystem.current;
            ExecuteEvents.Execute(events.currentSelectedGameObject, new BaseEventData(events), ExecuteEvents.submitHandler);
        }

        public void HoverForVerification(int index)
        {
            var choices = modal.GetComponentsInChildren<Act1MenuChoice>();
            ExecuteEvents.Execute(choices[index].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }

        public void HideMenu()
        {
            var events = EventSystem.current;
            var selection = events ? events.currentSelectedGameObject : null;
            if (selection && selection.transform.IsChildOf(modal)) events.SetSelectedGameObject(null);
            for (int i = modal.childCount - 1; i >= 0; i--)
            {
                GameObject item = modal.GetChild(i).gameObject;
                item.SetActive(false);
                Destroy(item);
            }
            modal.gameObject.SetActive(false);
        }

        static RectTransform Rect(Transform parent, string name, Vector2 min, Vector2 max, Vector2 lo, Vector2 hi)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = lo;
            rect.offsetMax = hi;
            return rect;
        }

        static Image Surface(Transform parent, string name, Color color, Vector2 min, Vector2 max, Vector2 lo, Vector2 hi)
        {
            var image = Rect(parent, name, min, max, lo, hi).gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        static void Stroke(Transform parent, Color color, Vector2 lo, Vector2 hi) =>
            Surface(parent, "Rule", color, Vector2.zero, Vector2.zero, lo, hi).raycastTarget = false;

        static void Frame(Transform parent, float inset, Color color)
        {
            var offset = Vector2.one * inset;
            Surface(parent, "Lower edge", color, Vector2.zero, Vector2.right, offset, new(-inset, inset + 1)).raycastTarget = false;
            Surface(parent, "Upper edge", color, Vector2.up, Vector2.one, new(inset, -inset - 1), -offset).raycastTarget = false;
            Surface(parent, "Left edge", color, Vector2.zero, Vector2.up, offset, new(inset + 1, -inset)).raycastTarget = false;
            Surface(parent, "Right edge", color, Vector2.right, Vector2.one, new(-inset - 1, inset), -offset).raycastTarget = false;
        }

        Text Copy(Transform parent, string value, int size, Color color, TextAnchor alignment, Vector2 lo, Vector2 hi)
        {
            var text = Rect(parent, "Text", Vector2.zero, Vector2.zero, lo, hi).gameObject.AddComponent<Text>();
            text.font = serif;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            return text;
        }
    }
}
