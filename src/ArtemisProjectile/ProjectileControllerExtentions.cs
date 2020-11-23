using System.Collections.Generic;

namespace ArtemisProjectile
{
    internal static class ProjectileControllerExtentions
    {
        internal static void RenderLines(List<DebugLine> lines)
        {
            foreach (var line in lines)
                line.Render();
        }
        internal static void RenderLines(List<DebugLine> lines, float duration)
        {
            foreach (var line in lines)
                line.Render(duration);
        }
    }
}
