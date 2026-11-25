using UnityEngine;

namespace RememberMe
{
    // One action per frame across menus, dialogue and gameplay. Mouse buttons use the
    // same action handlers as keyboard input. Never poll Return separately in a menu.
    public static class StoryInput
    {
        static int consumedFrame = -1;
        public static bool ConfirmPressed => Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);
        public static bool Consume(bool requested)
        {
            if (!requested || consumedFrame == Time.frameCount) return false;
            consumedFrame = Time.frameCount;
            return true;
        }
    }
}
