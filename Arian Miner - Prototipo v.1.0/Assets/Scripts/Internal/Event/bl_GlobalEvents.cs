using UnityEngine;

namespace Arian
{
    public static class bl_GlobalEvents
    {

        public class OnStartPlay
        {
        }

        public class OnFailGame
        {
            public OnFailGame()
            {
                bl_Event.Global.DispatchEvent(this);
            }
        }

        public class OnPoint
        {
            public int Point;

            public OnPoint(int _p)
            {
                Point = _p;
                bl_Event.Global.DispatchEvent(this);
            }
        }

        public class OnChangeSide
        {
            public LevelSides Side;
            public bool byFlip;

            public OnChangeSide(LevelSides s, bool isFlip)
            {
                Side = s;
                byFlip = isFlip;
                bl_Event.Global.DispatchEvent(this);
            }
        }

        public class OnSlowMotion
        {
            public float Duration;
            public OnSlowMotion(float _duration)
            {
                Duration = _duration;
                bl_Event.Global.DispatchEvent(this);
            }
        }

        public class OnFinishMining
        {
            public MiningStates miningState;

            public OnFinishMining(MiningStates m)
            {
                miningState = m;
                bl_Event.Global.DispatchEvent(this);
            }
        }

        public class OnPause
        {
            public bool Pause;
            public OnPause(bool pause)
            {
                Pause = pause;
                bl_Event.Global.DispatchEvent(this);
            }
        }

    }
}