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
        static readonly Color HintBackground = new(.045f, .073f, .063f, .79f);
        static readonly Color KeyBackground = new(.94f, .84f, .64f, .18f);
        Font serif;
        RectTransform canvas;
        RectTransform hud;
        RectTransform modal;
        RectTransform dialogue;
        RectTransform chapterCard;
        RectTransform chapterCenter;
        Image illustration;
        Image illustrationBacking;
        RectTransform windingFrame;
        Image windingHand;
        Image memoryButterfly;
        Image splash;
        Image portrait;
        RectTransform portraitFrame;
        Text objective;
        Text cue;
        Text cueKey;
        Text speaker;
        Text paragraph;
        Text chapterNumber;
        Text chapterTitle;
        RectTransform controls;
        RectTransform interaction;
        RectTransform objectiveArrow;
        Canvas stageCanvas;
        CanvasGroup presentation;
        public float PresentationOpacity { get => presentation.alpha; set => presentation.alpha = value; }
        public bool IsModal => modal.gameObject.activeSelf;
        public bool HasObjectiveArrow => objectiveArrow.gameObject.activeInHierarchy;
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
            windingFrame = Rect(illustration.rectTransform, "Music box close-up bounds", Vector2.one * .5f,
                Vector2.one * .5f, Vector2.zero, Vector2.zero);
            windingFrame.gameObject.AddComponent<RectMask2D>();
            windingHand = Surface(windingFrame, "Grace winds the key", Color.white,
                Vector2.one * .5f, Vector2.one * .5f, Vector2.zero, Vector2.zero);
            windingHand.sprite = Resources.Load<Sprite>("MusicBox/Hand");
            windingHand.raycastTarget = false;
            windingHand.rectTransform.pivot = new Vector2(.96f, .51f);
            windingHand.enabled = false;
            chapterCard = Surface(art, "Chapter card", new Color(Night.r, Night.g, Night.b, .81f),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).rectTransform;
            chapterCenter = Rect(chapterCard, "Chapter lettering", new(.5f,.5f), new(.5f,.5f), new(-440,-95), new(440,95));
            chapterNumber = Copy(chapterCenter, "", 19, Gold, TextAnchor.MiddleCenter, new(0, 117), new(880, 150));
            Stroke(chapterCenter, Edge, new(356, 107), new(524, 108));
            chapterTitle = Copy(chapterCenter, "", 41, Paper, TextAnchor.MiddleCenter,
                new(0, 25), new(880, 99));
            chapterCard.gameObject.SetActive(false);
            hud = Rect(canvas, "HUD", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            objectiveArrow = Rect(hud, "Objective arrow", Vector2.zero, Vector2.zero, new(-14, -14), new(14, 14));
            ArrowWing(objectiveArrow, -1, 6, HintBackground);
            ArrowWing(objectiveArrow, 1, 6, HintBackground);
            ArrowWing(objectiveArrow, -1, 3, Gold);
            ArrowWing(objectiveArrow, 1, 3, Gold);
            objectiveArrow.gameObject.SetActive(false);
            var card = Surface(hud, "Objective card", HintBackground, Vector2.up, Vector2.up,
                new Vector2(24, -85), new Vector2(434, -24)).rectTransform;
            card.GetComponent<Image>().raycastTarget = false;
            Copy(card, "Objective", 15, Gold, TextAnchor.MiddleLeft, new(14, 35), new(396, 54));
            objective = Copy(card, "", 21, Paper, TextAnchor.MiddleLeft, new(14, 7), new(396, 36));
            objective.fontStyle = FontStyle.Bold;
            interaction = HintBar(hud, "Interaction");
            cueKey = HintKey(interaction, "", 84);
            cue = HintText(interaction, "");
            interaction.gameObject.SetActive(false);
            controls = HintBar(hud, "Controls");
            HintControl(controls, "WASD", "Move", 52);
            HintControl(controls, "Shift", "Run", 46);
            HintControl(controls, "C", "Crouch", 24);
            HintControl(controls, "Esc", "Menu", 32);
            memoryButterfly = Surface(canvas, "Memory butterfly", Color.white, Vector2.zero, Vector2.zero,
                new(-18, -13), new(18, 13));
            memoryButterfly.sprite = Resources.Load<Sprite>("Story/GuideButterfly");
            memoryButterfly.preserveAspect = true;
            memoryButterfly.raycastTarget = false;
            memoryButterfly.enabled = false;
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

        public void Guide(string goal, string key, string action)
        {
            if (objective.text != goal) objective.text = goal;
            if (cueKey.text != key) cueKey.text = key;
            if (cue.text != action) cue.text = action;
            bool hasAction = !string.IsNullOrEmpty(action);
            interaction.gameObject.SetActive(hasAction);
            controls.gameObject.SetActive(!hasAction);
        }

        public void Locate(Camera camera, Vector2? goal, Bounds? art)
        {
            objectiveArrow.gameObject.SetActive(goal.HasValue);
            if (!goal.HasValue) return;
            Vector2 size = canvas.rect.size;
            Vector2 ScreenPoint(Vector3 p) => Vector2.Scale(camera.WorldToViewportPoint(p), size);
            Vector2 center = ScreenPoint(art?.center ?? (Vector3)goal.Value);
            var min = new Vector2(24, 78);
            var max = size - new Vector2(24, 25);
            bool onScreen = center.x >= min.x && center.x <= max.x && center.y >= min.y && center.y <= max.y;
            Vector2 position;
            Vector2 direction;
            if (onScreen)
            {
                float above = (art.HasValue ? ScreenPoint(art.Value.max).y : center.y) + 22;
                if (above > max.y)
                {
                    float below = (art.HasValue ? ScreenPoint(art.Value.min).y : center.y) - 18;
                    position = new Vector2(center.x, Mathf.Max(min.y, below));
                    direction = Vector2.up;
                }
                else
                {
                    position = new Vector2(center.x, above);
                    direction = Vector2.down;
                }
            }
            else
            {
                var middle = (min + max) / 2;
                var extent = (max - min) / 2;
                direction = center - middle;
                position = middle + direction /
                    Mathf.Max(Mathf.Abs(direction.x) / extent.x, Mathf.Abs(direction.y) / extent.y);
            }
            objectiveArrow.anchoredPosition = position;
            objectiveArrow.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, direction));
        }

        static void ArrowWing(RectTransform arrow, int side, float thickness, Color color)
        {
            var wing = Surface(arrow, "Chevron wing", color, Vector2.one * .5f, Vector2.one * .5f,
                new Vector2(side * 4.5f - 7, -thickness * .5f), new Vector2(side * 4.5f + 7, thickness * .5f));
            wing.raycastTarget = false;
            wing.rectTransform.localEulerAngles = new Vector3(0, 0, -side * 42);
        }

        static RectTransform HintBar(RectTransform parent, string name)
        {
            var anchor = new Vector2(.5f, 0);
            var background = Surface(parent, name, HintBackground, anchor, anchor, new(0, 20), new(0, 60));
            background.raycastTarget = false;
            var layout = background.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 0, 0);
            layout.spacing = 9;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            background.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            return background.rectTransform;
        }

        Text HintKey(RectTransform parent, string key, float width)
        {
            var badge = Surface(parent, "Key", KeyBackground, Vector2.zero, Vector2.zero, Vector2.zero, new(width, 25));
            badge.raycastTarget = false;
            var size = badge.gameObject.AddComponent<LayoutElement>();
            size.preferredWidth = width;
            size.preferredHeight = 25;
            var text = Copy(badge.transform, key, 15, Paper, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero);
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            return text;
        }

        Text HintText(RectTransform parent, string label) =>
            Copy(parent, label, 19, Paper, TextAnchor.MiddleCenter, Vector2.zero, Vector2.zero);

        void HintControl(RectTransform parent, string key, string label, float width)
        {
            HintKey(parent, key, width);
            HintText(parent, label);
        }

        public void ShowHUD(bool shown) => hud.gameObject.SetActive(shown);

        public bool ShowButterfly(Camera camera, Vector2? point, float opacity = 1)
        {
            if (!point.HasValue || !memoryButterfly.sprite || opacity <= 0)
            {
                memoryButterfly.enabled = false;
                return false;
            }
            Vector2 screen = Vector2.Scale(camera.WorldToViewportPoint(point.Value), canvas.rect.size);
            Vector2 size = canvas.rect.size;
            if (screen.x < 20 || screen.x > size.x - 20 || screen.y < 57 || screen.y > size.y - 21)
            {
                memoryButterfly.enabled = false;
                return false;
            }
            float flutter = Time.unscaledTime * 18;
            var rect = memoryButterfly.rectTransform;
            rect.anchoredPosition = screen;
            rect.localScale = new Vector3(.54f + .46f * Mathf.Abs(Mathf.Sin(flutter)), 1, 1);
            rect.localRotation = Quaternion.Euler(0, 0, 12 * Mathf.Sin(flutter * .17f));
            memoryButterfly.color = new Color(1, 1, 1, opacity);
            memoryButterfly.enabled = true;
            return true;
        }
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
            windingHand.enabled = false;
            illustration.sprite = image;
            illustration.enabled = image != null;
            illustrationBacking.enabled = image != null;
        }

        public void WindMusicBox(float progress)
        {
            windingHand.enabled = windingHand.sprite && illustration.sprite && progress < 1;
            if (!windingHand.enabled) return;
            Vector2 panel = illustration.rectTransform.rect.size;
            Vector2 image = illustration.sprite.rect.size;
            Vector2 fitted = image * Mathf.Min(panel.x / image.x, panel.y / image.y);
            windingFrame.sizeDelta = fitted;
            // The grip point is the center of the brass loop in Mechanism.png.
            Vector2 grip = Vector2.Scale(new Vector2(.198f, .77f) - Vector2.one * .5f, fitted);
            float reach = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0, .25f, progress))
                * (1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.80f, 1, progress)));
            float turn = Mathf.InverseLerp(.25f, .80f, progress);
            var hand = windingHand.rectTransform;
            hand.anchoredPosition = grip - new Vector2((1 - reach) * fitted.x * .13f, (1 - reach) * fitted.y * .08f);
            float width = fitted.x * .26f;
            hand.sizeDelta = new Vector2(width, width * windingHand.sprite.rect.height / windingHand.sprite.rect.width);
            hand.localEulerAngles = new Vector3(0, 0, 19 * Mathf.Sin(turn * Mathf.PI * 4));
            windingHand.color = new Color(1, 1, 1, reach);
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
