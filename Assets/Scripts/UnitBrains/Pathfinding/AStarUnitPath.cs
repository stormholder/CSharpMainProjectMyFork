using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using Utilities;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private Vector2Int[] dxy = {
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up
        };
        private const int MaxLength = 100;
        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            var route = CalculateAStar(startPoint, endPoint);
            if (route != null)
            {
                route.Reverse();
                path = route.ToArray();
            }
        }

        private int CalculateEstimate(Vector2Int fromPos, Vector2Int toPos) => Math.Abs(fromPos.x - toPos.x) + Math.Abs(fromPos.y - toPos.y);

        private List<Vector2Int> CalculateAStar(Vector2Int fromPos, Vector2Int toPos)
        {
            var counter = 0;
            var routeFound = false;
            Node startNode = new(fromPos, CalculateEstimate(fromPos, toPos));
            List<Node> openList = new() { startNode };
            HashSet<Vector2Int> closedList = new();
            Node currentNode = openList[0];
            while (openList.Count > 0 && counter++ < MaxLength) {
                foreach (var node in openList) {
                    if (node.Value < currentNode.Value) {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.CoordinatePoint);

                if (routeFound) {
                    List<Vector2Int> _path = new();
                    while(currentNode != null) {
                        _path.Add(currentNode.CoordinatePoint);
                        currentNode = currentNode.Parent;
                    }
                    return _path;
                }

                for (int i = 0; i < dxy.Length; i++) {
                    Vector2Int newPoint = currentNode.CoordinatePoint + dxy[i];
                    if (closedList.Contains(newPoint))
                        continue;

                    if (newPoint == toPos)
                        routeFound = true;

                    if (runtimeModel.IsTileWalkable(newPoint) || routeFound) {
                        Node neighbor = new(newPoint, CalculateEstimate(newPoint, toPos), startNode.Cost, currentNode);
                        bool hasOpenNode = false;
                        foreach (var node in openList) {
                            if (node.CoordinatePoint == newPoint) {
                                hasOpenNode = true;
                                break;
                            }
                        }
                        if (!hasOpenNode)
                            openList.Add(neighbor);
                    }
                }
            }
            // TODO: handle border cases (when route is not found)
            return new() { fromPos };
        }
    }
}