using System.Collections.Generic;
using UnityEngine;

// Estimates how much ball size is needed to clear the path (simple simulation).
// Go along the path; every obstacle in the ball corridor needs a shot.
// The shot gets bigger to also burn next blockers, if it is cheaper than a new shot.
// After every shot the ball is smaller, so the corridor is narrower (like in the game).
public static class BalanceEstimator
{
    private struct Item
    {
        public float along;
        public float side;
        public Vector3 center;
        public float radius;
        public bool alive;
    }

    // obstacles: xyz = position, w = diameter
    public static float RequiredSize(GameConfig config, Vector3 start, Vector3 end, List<Vector4> obstacles)
    {
        start.y = end.y = 0f;
        Vector3 forward = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        float length = Vector3.Distance(start, end) - config.doorStopDistance;

        var items = new List<Item>();
        foreach (Vector4 o in obstacles)
        {
            var item = new Item { center = new Vector3(o.x, 0f, o.z), radius = o.w * 0.5f, alive = true };
            item.along = Vector3.Dot(item.center - start, forward);
            item.side = Vector3.Dot(item.center - start, right);
            if (item.along >= 0f && item.along <= length + item.radius) items.Add(item);
        }
        items.Sort((a, b) => a.along.CompareTo(b.along));

        float ballSize = config.startSize;
        float total = 0f;
        for (int i = 0; i < items.Count; i++)
        {
            if (!items[i].alive || !Blocks(items[i], ballSize)) continue;

            // shot hits the front side of this obstacle
            Vector3 hitPoint = items[i].center - forward * items[i].radius;
            float shot = config.minShotSize;
            for (int j = i + 1; j < items.Count; j++)
            {
                if (!items[j].alive || !Blocks(items[j], ballSize)) continue;
                float need = Reach(hitPoint, items[j]) / config.infectionRadius;
                if (need <= shot) continue;
                if (need - shot > config.minShotSize) break; // a new shot is cheaper
                shot = need;
            }

            // everything in the infection radius burns
            float radius = shot * config.infectionRadius;
            for (int j = i; j < items.Count; j++)
            {
                if (j == i || Reach(hitPoint, items[j]) <= radius)
                {
                    Item burned = items[j];
                    burned.alive = false;
                    items[j] = burned;
                }
            }

            total += shot;
            ballSize -= shot;
        }
        return total;
    }

    private static bool Blocks(Item item, float ballSize)
    {
        return Mathf.Abs(item.side) < ballSize * 0.5f + item.radius;
    }

    // distance from the hit point to the obstacle surface
    private static float Reach(Vector3 hitPoint, Item item)
    {
        return Mathf.Max(0f, Vector3.Distance(hitPoint, item.center) - item.radius);
    }
}
