using UnityEngine;

namespace ArtemisProjectile
{
    internal sealed class DebugLine
    {
        public Vector3 StartPoint { get; }
        public Vector3 EndPoint { get; }
        public Color Color { get; }

        public DebugLine(Vector3 startPoint, Vector3 endPoint, Color color)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Color = color;
        }

        public void Render() => Debug.DrawLine(StartPoint, EndPoint, Color);
        public void Render(float duration) => Debug.DrawLine(StartPoint, EndPoint, Color, duration);
    }
}
