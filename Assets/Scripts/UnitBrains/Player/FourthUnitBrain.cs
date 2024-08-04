using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;
using View;

namespace UnitBrains.Player
{
    public class FourthUnitBrain : DefaultPlayerUnitBrain
    {
        private VFXView _vfxView = ServiceLocator.Get<VFXView>();
        private BuffController _buffController = ServiceLocator.Get<BuffController>();

        public override string TargetUnitName => "Buffy Knight";

        public override void Update(float deltaTime, float time)
        {
            base.Update(deltaTime, time);
            // TODO: buff closest ally if it does not have a buff
            // TODO: pause by 0.5 seconds after buff "attack"
        }

        protected override List<Vector2Int> SelectTargets()
        {
            // TODO: choose allied units as targets
            var targets = base.SelectTargets();
            return targets;
        }

        public override Vector2Int GetNextStep()
        {
            var nextStep = base.GetNextStep();
            // TODO
            return nextStep;
        }
    }
}