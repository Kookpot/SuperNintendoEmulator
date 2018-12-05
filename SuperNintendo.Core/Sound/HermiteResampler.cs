namespace SuperNintendo.Core.Sound
{
    public class HermiteResampler
    {
        private float r_step;
        private float r_frac;
        private int[] r_left = new int[4];
        private int[] r_right = new int[4];

        private static float hermite(float mu1, float a, float b, float c, float d)
        {
            float mu2, mu3, m0, m1, a0, a1, a2, a3;

            mu2 = mu1 * mu1;
            mu3 = mu2 * mu1;

            m0 = (float)((c - a) * 0.5);
            m1 = (float)((d - b) * 0.5);

            a0 = +2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + mu1;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;

            return (a0 * b) + (a1 * m0) + (a2 * m1) + (a3 * c);
        }
    }
}