using System.Collections.Generic;
using System.Linq;

namespace UnicornOne.Core.Utils
{
    public class HexPathFinder
    {
        public List<HexCoords> FindPath(HexCoords from, HexCoords to)
        {
            PriorityQueue<HexCoords, int> queue = new();
            queue.Enqueue(from, 0);

            Dictionary<HexCoords, int> costMap = new()
            { { from, 0 } };
            Dictionary<HexCoords, HexCoords> cameFromMap = new();

            while (queue.Count > 0)
            {
                HexCoords lastPosition = queue.Dequeue();

                if (lastPosition == to)
                {
                    break;
                }

                int lastCost = costMap[lastPosition];
                foreach (HexCoords newPosition in HexUtils.GetNeighbors(lastPosition))
                {
                    int newCost = lastCost + 1;

                    int setCost;
                    if (!costMap.TryGetValue(newPosition, out setCost))
                    {
                        setCost = int.MaxValue;
                    }

                    if (newCost < setCost)
                    {
                        int priority = newCost + newPosition.DistanceTo(to);
                        queue.Enqueue(newPosition, priority);

                        costMap[newPosition] = newCost;
                        cameFromMap[newPosition] = lastPosition;
                    }
                }
            }

            int pathLength = costMap[to];
            List<HexCoords> path = new(pathLength + 1) { to, cameFromMap[to] };
            for (int i = 1; i < pathLength; i++)
            {
                path.Add(cameFromMap[path.Last()]);
            }

            return path;
        }
    }
}
