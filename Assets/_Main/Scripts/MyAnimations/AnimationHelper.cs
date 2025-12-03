using UnityEngine;

namespace _Main.Scripts.MyAnimations
{
    public static class AnimationHelper
    {
        public static float GetCanvasWidth(RectTransform target)
        {
            return target.rect.width;
        }
            
        public static float GetCanvasHeight(RectTransform target)
        {
            return target.rect.height;
        }

        public static Vector2 GetOffscreenPos(RectTransform rect, Direction direction)
        {
            Vector2 size = rect.rect.size;
            return rect.anchoredPosition + GetDirection(direction,size);
        }

        private static Vector2 GetDirection(Direction dir, Vector2 size)
        {
            return dir switch
            {
                Direction.Left  => new Vector2(-size.x, 0),
                Direction.Right => new Vector2(size.x, 0),
                Direction.Up    => new Vector2(0, size.y),
                Direction.Down  => new Vector2(0, -size.y),
                _ => Vector2.zero
            };
        }
            
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}