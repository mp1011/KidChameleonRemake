using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public interface IHasHitboxes
    {
        RGRectangleI PrimaryHitbox { get; set; }
        RGRectangleI SecondaryHitbox { get; set; }
    }

    public static class HitboxUtil
    {
        public static void CopyHitboxesFrom(this IHasHitboxes dest, IHasHitboxes src)
        {
            dest.PrimaryHitbox = src.PrimaryHitbox;
            dest.SecondaryHitbox = src.SecondaryHitbox;
        }

        public static void SetHitbox(this IHasHitboxes h, HitboxType t, RGRectangleI rec)
        {
            if (t == HitboxType.Primary)
                h.PrimaryHitbox = rec;
            else if (t == HitboxType.Secondary)
                h.SecondaryHitbox = rec;
        }

        public static RGRectangleI GetHitbox(this IHasHitboxes h, HitboxType t)
        {
            if (t == HitboxType.Primary)
                return h.PrimaryHitbox;
            else if (t == HitboxType.Secondary)
                return h.SecondaryHitbox;
            else
                return RGRectangleI.Empty;
        }
    }

    public enum HitboxType
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Both = 3
    }
}
