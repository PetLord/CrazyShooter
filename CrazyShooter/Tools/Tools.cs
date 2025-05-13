namespace CrazyShooter.Tools
{
    public static class MathUtils
    {
        public static float ToRadians(float degrees) => degrees * (MathF.PI / 180f);
        public static float ToDegrees(float radians) => radians * (180f / MathF.PI);
    }
}