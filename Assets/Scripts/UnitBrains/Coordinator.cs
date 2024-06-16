using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.UnitBrains
{

    internal struct UnitDistance
    {
        public double Distance;
        public IReadOnlyUnit Unit;

        public UnitDistance(double distance, IReadOnlyUnit unit)
        {
            this.Distance = distance;
            this.Unit = unit;
        }
    }

    public class Coordinator
    {
        private static Coordinator? instance;

        private Coordinator() {
            var playerBase = runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
            var botPlayerBase = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            var midpoint = new Vector2Int((playerBase.x + botPlayerBase.x) / 2, (playerBase.y + botPlayerBase.y) / 2);
            midpointDistance = Math.Sqrt(Math.Pow(playerBase.x - midpoint.x, 2) + Math.Pow(playerBase.y - midpoint.y, 2));
            //(midpoint - playerBase).sqrMagnitude;
            timeUtil.AddFixedUpdateAction(FixedUpdate);
        }

        private double midpointDistance = 0;

        public static Coordinator GetInstance()
        {
            if (instance == null)
            {
                instance = new Coordinator();
            }
            return instance;
        }

        public IReadOnlyUnit? recommendedUnitForPlayer;
        public Vector2Int? recommendedPosForPlayer;
        public IReadOnlyUnit? recommendedUnitForBotPlayer;
        public Vector2Int? recommendedPosForBotPlayer;

        protected IReadOnlyRuntimeModel runtimeModel => ServiceLocator.Get<IReadOnlyRuntimeModel>();
        protected TimeUtil timeUtil => ServiceLocator.Get<TimeUtil>();

        private void FixedUpdate(float timeDelta)
        {
            var playerBase = runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
            var botPlayerBase = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            var enemiesOnTerritory = runtimeModel.RoUnits
                .Select(unit => new UnitDistance(distanceToBase(unit), unit))
                .Where(unitDistance => unitDistance.Distance <= midpointDistance)
                .ToList();
            if (enemiesOnTerritory.Any())
            {
                //if enemy is on our territory, select closest one to our base
                recommendedUnitForPlayer = enemiesOnTerritory
                    .Where(unitDistance => !unitDistance.Unit.Config.IsPlayerUnit)
                    .OrderBy(unitDistance => unitDistance.Distance)
                    .Select(unitDistance => unitDistance.Unit)
                    .FirstOrDefault();
                recommendedUnitForBotPlayer = enemiesOnTerritory
                    .Where(unitDistance => unitDistance.Unit.Config.IsPlayerUnit)
                    .OrderBy(unitDistance => unitDistance.Distance)
                    .Select(unitDistance => unitDistance.Unit)
                    .FirstOrDefault();

                recommendedPosForPlayer = new Vector2Int(playerBase.x + 1, playerBase.y);
                recommendedPosForBotPlayer = new Vector2Int(botPlayerBase.x - 1, botPlayerBase.y);
            }
            else
            {
                //else select one with lowest health value
                recommendedUnitForPlayer = runtimeModel.RoUnits
                    .Where(unit => !unit.Config.IsPlayerUnit)
                    .OrderBy(unit => unit.Health)
                    .FirstOrDefault();
                recommendedUnitForBotPlayer = runtimeModel.RoUnits
                    .Where(unit => unit.Config.IsPlayerUnit)
                    .OrderBy(unit => unit.Health)
                    .FirstOrDefault();

                var closestPlayerUnit = runtimeModel.RoUnits
                    .Where(unit => unit.Config.IsPlayerUnit)
                    .OrderBy(unit => distanceToBase(unit))
                    .FirstOrDefault();
                
                var closestBotPlayerUnit = runtimeModel.RoUnits
                    .Where(unit => !unit.Config.IsPlayerUnit)
                    .OrderBy(unit => distanceToBase(unit))
                    .FirstOrDefault();

                recommendedPosForPlayer = closestBotPlayerUnit?.Pos; // new Vector2Int(playerBase.x + 1, playerBase.y);
                recommendedPosForBotPlayer = closestPlayerUnit?.Pos; // new Vector2Int(botPlayerBase.x - 1, botPlayerBase.y);
            }
        }

        public void Update()
        {
        }

        private double distanceToBase(IReadOnlyUnit unit)
        {
            var opponentBase = runtimeModel.RoMap.Bases[unit.Config.IsPlayerUnit ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
            //var distance = unit.Pos - enemyBase;
            var distance = Math.Sqrt(Math.Pow(opponentBase.x - unit.Pos.x, 2) + Math.Pow(opponentBase.y - unit.Pos.y, 2));
            return distance;
        }

        //private bool IsEnemyOnOwnHalf(IReadOnlyUnit unit)
        //{
        //    var ownBase = runtimeModel.RoMap.Bases[unit.Config.IsPlayerUnit ? RuntimeModel.PlayerId : RuntimeModel.BotPlayerId];
        //    var enemyBase = runtimeModel.RoMap.Bases[unit.Config.IsPlayerUnit ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
        //    var midpoint = new Vector2Int((ownBase.x + enemyBase.x) / 2, (ownBase.y + enemyBase.y) / 2);
        //    var midpointToBase = (midpoint - ownBase).sqrMagnitude;
        //    var distance = unit.Pos - ownBase;
        //    return distance.sqrMagnitude <= midpointToBase;
        //}

    }
}
