namespace PhysicsEngine2D.Net.Basic
{
    public struct MassData
    {
        private float _mass;

        public float Mass
        {
            get => _mass;
            set
            {
                InverseMass = 1 / value;
                _mass = value;
            }
        }

        public float InverseMass { get; private set; }

        public float Inertia { get; set; }

        public float InverseInertia { get; set; }
    }
}