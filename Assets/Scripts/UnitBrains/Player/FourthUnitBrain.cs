using System.Collections.Generic;
using System.Linq;
using Controller;
using UnitBrains.Buff;
using UnityEngine;
using Utilities;
using View;

namespace UnitBrains.Player
{
    public class FourthUnitBrain : DefaultPlayerUnitBrain
    {
        private VFXView _vfxView = ServiceLocator.Get<VFXView>();
        private BuffController _buffController = ServiceLocator.Get<BuffController>();
        private bool _isBuffReady = false;
        private float CooldownTimeSeconds = .5f;
        private float _cooldownTime = 0f;

        public override string TargetUnitName => "Buffy Knight";

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);
            if (!_isBuffReady)
            {
                _cooldownTime += Time.deltaTime;
                if (_cooldownTime >= CooldownTimeSeconds)
                {
                    _cooldownTime = 0;
                    _isBuffReady = true;
                }
            } else
            {
                var allies = SelectAllies();
                // buff first ally if it does not have a buff
                if (allies.Any())
                {
                    var ally = allies.First();
                    _buffController.AddUnitBuff(ally, new AttackPowerBuff(3.0f, 2.0f));
                    _buffController.AddUnitBuff(ally, new SpeedBuff(2.0f, 1.5f));
                    Debug.Log($"Buffed ally \"{ally.Config.Name}\"");
                    _vfxView.PlayVFX(ally.Pos, VFXView.VFXType.BuffApplied);
                    _isBuffReady = false;
                }
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            return new();
        }

        private List<Model.Runtime.Unit> SelectAllies()
        {
            // choose allied units as targets
            return runtimeModel.RoUnits
                .Where(u => u.Config.IsPlayerUnit == IsPlayerUnitBrain)
                .Where(u => IsTargetInRange(u.Pos))
                .Select(u => (Model.Runtime.Unit)u)
                .Where(u => !_buffController.GetUnitBuffs(u).Any())
                .ToList();
        }

        public override Vector2Int GetNextStep()
        {
            var nextStep = base.GetNextStep();
            // TODO
            return nextStep;
        }
    }
}