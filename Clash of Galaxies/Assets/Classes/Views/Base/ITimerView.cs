﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Views
{
    interface ITimerView
    {
        Action<int> CheckStateMakeMoveCallback { get; set; }
        void StartTimer();
        void StartTimer(bool isPlayerUserMove);
        void StopTimer();
    }
}
