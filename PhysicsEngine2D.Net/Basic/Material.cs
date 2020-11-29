namespace PhysicsEngine2D.Net.Basic
{
    /// <summary>
    /// <remarks>
    /// Ref to: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-core-engine--gamedev-7493
    /// Rock        Density : 0.6  Restitution : 0.1
    /// Wood        Density : 0.3  Restitution : 0.2
    /// Metal       Density : 1.2  Restitution : 0.05
    /// BouncyBall  Density : 0.3  Restitution : 0.8
    /// SuperBall   Density : 0.3  Restitution : 0.95
    /// Pillow      Density : 0.1  Restitution : 0.2
    /// Static      Density : 0.0  Restitution : 0.4
    /// </remarks>
    /// </summary>
    public struct Material
    {
        public float Density { get; set; }

        public float Restitution { get; set; }
    }
}