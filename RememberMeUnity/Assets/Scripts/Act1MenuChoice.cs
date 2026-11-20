using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RememberMe
{
    public sealed class Act1MenuChoice : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {
        static readonly Color SelectedFill = new(.96f, .82f, .54f);
        static readonly Color SelectedText = new(.14f, .09f, .045f);
        Button button;
        Image background;
        Image rail;
        Text label;
        Text arrow;
        Color normalFill;
        Color normalText;

        public bool IsFocused => arrow.gameObject.activeSelf;

        public void Initialize(Button choice, Image surface, Image accent, Text caption, Text marker, Color fill, Color text)
        {
            button = choice;
            background = surface;
            rail = accent;
            label = caption;
            arrow = marker;
            normalFill = fill;
            normalText = text;
            Draw(false);
        }

        void Draw(bool focused)
        {
            background.color = !button.interactable ? new Color(.68f, .66f, .59f) : focused ? SelectedFill : normalFill;
            label.color = !button.interactable ? new Color(.27f, .26f, .22f) : focused ? SelectedText : normalText;
            label.fontStyle = focused ? FontStyle.Bold : FontStyle.Normal;
            arrow.gameObject.SetActive(focused);
            rail.enabled = focused;
        }

        public void OnSelect(BaseEventData eventData) => Draw(button.interactable);
        public void OnDeselect(BaseEventData eventData) => Draw(false);

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable) EventSystem.current.SetSelectedGameObject(gameObject, eventData);
        }
    }
}
