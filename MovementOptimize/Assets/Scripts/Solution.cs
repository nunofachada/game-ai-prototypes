using LibGameAI.Optimizers;

public struct Solution : ISolution
{
    // Maximum acceleration
    public float MaxAccel { get; }

   // Maximum speed
    public float MaxSpeed { get; }

    // Maximum angular acceleration
    public float MaxAngularAccel { get; }

    // Maximum rotation (angular velocity)
    public float MaxRotation { get; }

    public Solution(
        float maxAccel, float maxSpeed,
        float maxAngularAccel, float maxRotation)
    {
        MaxAccel = maxAccel;
        MaxSpeed = maxSpeed;
        MaxAngularAccel = maxAngularAccel;
        MaxRotation = maxRotation;
    }

    public override string ToString() =>
        $"<maxAccel={MaxAccel}, maxSpeed={MaxSpeed}, " +
        $"maxAngularAccel={MaxAngularAccel}, maxRotation={MaxRotation}>";
}
