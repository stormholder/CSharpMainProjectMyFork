using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private Vector2Int[] successors = {
            Vector2Int.down,
            //new Vector2Int(-1, -1), //down-left
            Vector2Int.left,
            //new Vector2Int(-1, 1), //up-left
            Vector2Int.up,
            //new Vector2Int(1, 1), //up-right
            Vector2Int.right,
            //new Vector2Int(1, -1), //down-right
        };
        //private const int MaxLength = 100;
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

        //private int CalculateEstimate(Vector2Int fromPos, Vector2Int toPos) => Math.Abs(fromPos.x - toPos.x) + Math.Abs(fromPos.y - toPos.y);
        private int CalculateEstimate(Vector2Int fromPos, Vector2Int toPos) => (int)Math.Sqrt(
                                    Math.Pow(toPos.x - fromPos.x, 2) +
                                    Math.Pow(toPos.y - fromPos.y, 2)
                                );

        private List<Vector2Int> getPathFromNode(Node node)
        {
            List<Vector2Int> _path = new();
            while (node != null)
            {
                _path.Add(node.Pos);
                node = node.Parent;
            }
            return _path;
        }

        private List<Vector2Int> CalculateAStar(Vector2Int fromPos, Vector2Int toPos)
        {
            Node startNode = new(fromPos, CalculateEstimate(fromPos, toPos));
            List<Node> openList = new() { startNode };
            HashSet<Vector2Int> closedList = new();
            bool routeFound = false;

            while(openList.Count > 0)
            {
                Node currentNode = openList[0];
                foreach (var node in openList)
                {
                    if (node.Value < currentNode.Value)
                    {
                        currentNode = node;
                    }
                }
                openList.Remove(currentNode);
                if (closedList.Contains(currentNode.Pos))
                    continue;

                if (routeFound)
                {
                    return getPathFromNode(currentNode);
                }

                closedList.Add(currentNode.Pos);

                foreach (Vector2Int n in successors)
                {
                    Vector2Int newPoint = currentNode.Pos + n;

                    if (closedList.Contains(newPoint))
                        continue;

                    if (newPoint == endPoint)
                    {
                        routeFound = true;
                        break;
                    }

                    if (!runtimeModel.IsTileWalkable(newPoint))
                        continue;

                    CalculateNeigborWeights(openList, currentNode, newPoint);
                }

            }
            // TODO: handle border cases (when route is not found)
            //return new() { fromPos };
            return null;
        }

        private void CalculateNeigborWeights(List<Node> openList, Node currentNode, Vector2Int newPoint)
        {
            var new_g = currentNode.G + 1;

            int nfo_i = -1; //if neighbor is in OpenList, set nfo_i to its index
            for (var i = 0; i < openList.Count; i++)
            {
                if (openList[i].Pos == newPoint)
                {
                    nfo_i = i;
                    break;
                }
            }
            if (nfo_i >= 0) //if neighbor found
            {
                if (new_g < openList[nfo_i].G)
                { //update and recalculate weights
                    openList[nfo_i].G = new_g;
                    openList[nfo_i].H = CalculateEstimate(openList[nfo_i].Pos, endPoint);
                    openList[nfo_i].Parent = currentNode;
                }
            }
            else //if neighbor is not in openList, just add it
            {
                openList.Add(new Node(
                    newPoint,
                    CalculateEstimate(newPoint, endPoint),
                    new_g,
                    currentNode));
            }
        }
    }
}