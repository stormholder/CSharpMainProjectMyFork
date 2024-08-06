using Model.Config;
using Model.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitBrains.Buff
{
    public class AttackPowerBuff : BaseBuff
    {
        public AttackPowerBuff(Unit unit, float duration, float modifier) : base(unit, duration, modifier) {
            this.BuffType = BuffType.AttackPower;
        }

        public override void Activate()
        {
            base.Activate();
            unit.AttackModifier = modifier;
        }

        public override void Dispose()
        {
            base.Dispose();
            unit.AttackModifier = 1.0f;
        }
    }
}
