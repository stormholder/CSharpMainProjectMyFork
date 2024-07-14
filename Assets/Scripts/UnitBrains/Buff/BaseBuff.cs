using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnitBrains.Buff
{
    public class BaseBuff
    {
        public float Duration { get; set; }
        public float Modifier { get; private set; }

        public BaseBuff(float duration, float modifier) {
            Duration = duration;
            Modifier = modifier;
        }
    }
}
