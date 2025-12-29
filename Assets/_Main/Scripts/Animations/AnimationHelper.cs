using UnityEngine;

namespace MeteorMadness.Animations
{
    public static class AnimationHelper
    {
        public class PanelPosition
        {
            public PanelPosition(RectTransform panel, Direction position)
            {
                StartPos = panel.anchoredPosition;
                OffScreenPos = GetOffscreenPos(panel, position);
            }
            
            public PanelPosition(RectTransform panel, Direction position, Vector2 offset)
            {
                StartPos = panel.anchoredPosition;
                OffScreenPos = GetOffscreenPos(panel, position) + offset;
            }

            public readonly Vector2 StartPos;
            public readonly Vector2 OffScreenPos;
        }
        
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
                Direction.Left       => new Vector2(-size.x, 0),
                Direction.Right      => new Vector2(size.x, 0),
                Direction.Up         => new Vector2(0, size.y),
                Direction.Down       => new Vector2(0, -size.y),
                Direction.UpLeft     => new Vector2(-size.x, size.y),
                Direction.UpRight    => new Vector2(size.x, size.y),
                Direction.DownLeft   => new Vector2(-size.x, -size.y),
                Direction.DownRight  => new Vector2(size.x, -size.y),
                _                    => Vector2.zero
            };
        }
            
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }
    }
}