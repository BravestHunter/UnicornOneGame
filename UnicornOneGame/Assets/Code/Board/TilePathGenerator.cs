using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnicornOne.Board
{
    public static class TilePathGenerator
    {
        public static HexCoordinates[] Directions = new HexCoordinates[]
        {
            new HexCoordinates(new Vector2Int(1, 0)),
            new HexCoordinates(new Vector2Int(0, 1)),
            new HexCoordinates(new Vector2Int(-1, 1)),
            new HexCoordinates(new Vector2Int(-1, 0)),
            new HexCoordinates(new Vector2Int(0, -1)),
            new HexCoordinates(new Vector2Int(1, -1))
        };

        public static TilePath Generate(Tile startTile, Tile finishTile, Tile roadTile, int length)
        {
            Debug.Assert(length >= 3);

            var pathTiles = GeneratePathTiles(length);
            CentralizeTiles(pathTiles, new HexCoordinates(0, 0));

            TilePath.TileEntry[] tiles = pathTiles.Select(c => new TilePath.TileEntry() { Position = c }).ToArray();
            tiles[0].Tile = startTile;
            tiles[tiles.Length - 1].Tile = finishTile;
            for (int i = 1; i < tiles.Length - 1; i++)
            {
                tiles[i].Tile = roadTile;
            }

            TilePath tilePath = new();
            tilePath.Tiles = tiles;

            return tilePath;
        }

        private static HexCoordinates[] GeneratePathTiles(int length)
        {
            HexCoordinates[] tiles = null;
            while (tiles == null)
            {
                tiles = TryGeneratePathTiles(length);
            }

            return tiles;
        }

        private static HexCoordinates[] TryGeneratePathTiles(int length)
        {
            List<HexCoordinates> tiles = new List<HexCoordinates>(length);

            HexCoordinates start = new HexCoordinates(0, 0);
            tiles.Add(start);

            HashSet<HexCoordinates> set = new HashSet<HexCoordinates>() { start };

            bool IsPossibleNextStep(HexCoordinates position)
            {
                return Directions.Select(d => position + d).Where(p => set.Contains(p)).Count() == 1;
            }

            int direction = Random.Range(0, 6);
            tiles.Add(start + Directions[direction]);
            set.Add(tiles[tiles.Count - 1]);

            List<HexCoordinates> nextStepsContainer = new List<HexCoordinates>(3);
            List<int> nextStepIndices = new List<int>(3);
            for (int i = 2; i < length; i++)
            {
                HexCoordinates lastStep = tiles[tiles.Count - 1];

                HexCoordinates next0 = (lastStep + Directions[(direction + 5) % 6]);
                if (IsPossibleNextStep(next0))
                {
                    nextStepsContainer.Add(next0);
                    nextStepIndices.Add((direction + 5) % 6);
                }
                HexCoordinates next1 = (lastStep + Directions[direction]);
                if (IsPossibleNextStep(next1))
                {
                    nextStepsContainer.Add(next1);
                    nextStepIndices.Add(direction);
                }
                HexCoordinates next2 = (lastStep + Directions[(direction + 1) % 6]);
                if (IsPossibleNextStep(next2))
                {
                    nextStepsContainer.Add(next2);
                    nextStepIndices.Add((direction + 1) % 6);
                }

                if (nextStepsContainer.Count == 0)
                {
                    return null;
                }

                int nextStepIndex = Random.Range(0, nextStepsContainer.Count);

                tiles.Add(nextStepsContainer[nextStepIndex]);
                set.Add(nextStepsContainer[nextStepIndex]);
                direction = nextStepIndices[nextStepIndex];

                nextStepsContainer.Clear();
                nextStepIndices.Clear();
            }

            Debug.Assert(tiles.Count == length);

            return tiles.ToArray();
        }

        private static void CentralizeTiles(HexCoordinates[] tiles, HexCoordinates center)
        {
            Debug.Assert(tiles.Length > 0);

            Vector2Int xRange = new Vector2Int(int.MaxValue, int.MinValue);
            Vector2Int yRange = new Vector2Int(int.MaxValue, int.MinValue);

            for (int i = 0; i < tiles.Length; i++)
            {
                if (xRange.x > tiles[i].X)
                {
                    xRange.x = tiles[i].X;
                }
                if (xRange.y < tiles[i].X)
                {
                    xRange.y = tiles[i].X;
                }

                if (yRange.x > tiles[i].Y)
                {
                    yRange.x = tiles[i].Y;
                }
                if (yRange.y < tiles[i].Y)
                {
                    yRange.y = tiles[i].Y;
                }
            }

            HexCoordinates diff = new HexCoordinates(
                center.X - (xRange.x + xRange.y) / 2,
                center.Y - (yRange.x + yRange.y) / 2
            );

            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] += diff;
            }
        }
    }
}
