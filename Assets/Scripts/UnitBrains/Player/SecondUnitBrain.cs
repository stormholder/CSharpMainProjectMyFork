using System.Collections.Generic;
using System.Linq;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;

        private List<Vector2Int> unreachableTargets = new();
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            
            var currentTemperature = GetTemperature();
            if (currentTemperature >= overheatTemperature) return;
            
            for (int i = 0; i <= currentTemperature; i++) {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            IncreaseTemperature();
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            if (!unreachableTargets.Any() || unreachableTargets.Any(t => IsTargetInRange(t))) {
                return unit.Pos;
            }
            return unit.Pos.CalcNextStepTowards(unreachableTargets.First());
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            
            List<Vector2Int> targets = new();
            List<Vector2Int> reachableTargets = new();
            unreachableTargets.Clear();
            var allTargets = GetAllTargets();
            var closestValue = float.MaxValue;

            if (!allTargets.Any()) {
                targets.Add(
                    runtimeModel.RoMap.Bases[
                        IsPlayerUnitBrain 
                            ? Model.RuntimeModel.BotPlayerId 
                            : Model.RuntimeModel.PlayerId
                        ]
                );
                return targets;
            }
            
            Vector2Int closestTarget = new Vector2Int(int.MinValue, int.MinValue);
            foreach (var target in targets) {
                var targetDistance = DistanceToOwnBase(target);
                if (targetDistance < closestValue) {
                    closestValue = targetDistance;
                    closestTarget = target;
                }
            }
            if (closestValue != float.MaxValue) {
                targets.Clear();
                targets.Add(closestTarget);
            }

            SortByDistanceToOwnBase(targets);

            unreachableTargets.Add(targets[0]);

            if (IsTargetInRange(targets[0])) {
                reachableTargets.Add(targets[0]);
            }

            return reachableTargets;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}