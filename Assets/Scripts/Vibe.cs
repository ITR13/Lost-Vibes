using System;

namespace LostVibes
{
    [Serializable]
    public struct Vibe
    {
        public string Name;
        public float Velocity;
        public float Bias;
        public float Cooldown;
        public float Resistance;
    }
}