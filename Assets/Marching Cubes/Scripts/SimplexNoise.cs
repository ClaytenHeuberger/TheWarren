// Ported from Keijiro's shader (based on Ashima's GLSL)
// https://github.com/keijiro/NoiseShader
// C# translation for runtime use

using UnityEngine;

public static class SimplexNoise
{
    private static float Mod289(float x)
    {
        return x - Mathf.Floor(x / 289f) * 289f;
    }

    private static Vector3 Mod289(Vector3 x)
    {
        return x - new Vector3(Mathf.Floor(x.x / 289f), Mathf.Floor(x.y / 289f), Mathf.Floor(x.z / 289f)) * 289f;
    }

    private static Vector4 Mod289(Vector4 x)
    {
        return x - new Vector4(Mathf.Floor(x.x / 289f), Mathf.Floor(x.y / 289f), Mathf.Floor(x.z / 289f), Mathf.Floor(x.w / 289f)) * 289f;
    }

    private static Vector4 Permute(Vector4 x)
    {
        return Mod289(Vector4.Scale((x * 34f + Vector4.one), x));
    }

    private static Vector4 TaylorInvSqrt(Vector4 r)
    {
        return new Vector4(
            1.79284291400159f - 0.85373472095314f * r.x,
            1.79284291400159f - 0.85373472095314f * r.y,
            1.79284291400159f - 0.85373472095314f * r.z,
            1.79284291400159f - 0.85373472095314f * r.w
        );
    }

    public static float Noise(Vector3 v)
    {
        Vector2 C = new Vector2(1f / 6f, 1f / 3f);

        float iSum = v.x + v.y + v.z;
        Vector3 i = new Vector3(Mathf.Floor(v.x + iSum * C.y),
                                Mathf.Floor(v.y + iSum * C.y),
                                Mathf.Floor(v.z + iSum * C.y));

        Vector3 x0 = v - i + Vector3.one * (iSum * C.x);

        Vector3 g = new Vector3(x0.y >= x0.x ? 1 : 0,
                                x0.z >= x0.y ? 1 : 0,
                                x0.x >= x0.z ? 1 : 0);
        Vector3 l = Vector3.one - g;

        Vector3 i1 = Vector3.Min(g, new Vector3(l.z, l.x, l.y));
        Vector3 i2 = Vector3.Max(g, new Vector3(l.z, l.x, l.y));

        Vector3 x1 = x0 - i1 + Vector3.one * C.x;
        Vector3 x2 = x0 - i2 + Vector3.one * C.y;
        Vector3 x3 = x0 - Vector3.one * 0.5f;

        i = Mod289(i);

        Vector4 p = Permute(
                        Permute(
                            Permute(new Vector4(i.z, i.z + i1.z, i.z + i2.z, i.z + 1.0f)) +
                            new Vector4(i.y, i.y + i1.y, i.y + i2.y, i.y + 1.0f)) +
                        new Vector4(i.x, i.x + i1.x, i.x + i2.x, i.x + 1.0f));

        Vector4 j = p - Vector4.Scale(new Vector4(49f, 49f, 49f, 49f), new Vector4(Mathf.Floor(p.x / 49f), Mathf.Floor(p.y / 49f), Mathf.Floor(p.z / 49f), Mathf.Floor(p.w / 49f)));

        Vector4 x_ = new Vector4(Mathf.Floor(j.x / 7f), Mathf.Floor(j.y / 7f), Mathf.Floor(j.z / 7f), Mathf.Floor(j.w / 7f));
        Vector4 y_ = new Vector4(j.x - 7f * x_.x, j.y - 7f * x_.y, j.z - 7f * x_.z, j.w - 7f * x_.w);

        Vector4 x = (x_ * 2f + new Vector4(0.5f, 0.5f, 0.5f, 0.5f)) / 7f - Vector4.one;
        Vector4 y = (y_ * 2f + new Vector4(0.5f, 0.5f, 0.5f, 0.5f)) / 7f - Vector4.one;

        Vector4 h = Vector4.one - new Vector4(Mathf.Abs(x.x) + Mathf.Abs(y.x), Mathf.Abs(x.y) + Mathf.Abs(y.y), Mathf.Abs(x.z) + Mathf.Abs(y.z), Mathf.Abs(x.w) + Mathf.Abs(y.w));

        Vector4 b0 = new Vector4(x.x, x.y, y.x, y.y);
        Vector4 b1 = new Vector4(x.z, x.w, y.z, y.w);

        Vector4 s0 = new Vector4(Mathf.Floor(b0.x) * 2f + 1f, Mathf.Floor(b0.y) * 2f + 1f, Mathf.Floor(b0.z) * 2f + 1f, Mathf.Floor(b0.w) * 2f + 1f);
        Vector4 s1 = new Vector4(Mathf.Floor(b1.x) * 2f + 1f, Mathf.Floor(b1.y) * 2f + 1f, Mathf.Floor(b1.z) * 2f + 1f, Mathf.Floor(b1.w) * 2f + 1f);

        Vector4 sh = new Vector4(-Step(h.x, 0f), -Step(h.y, 0f), -Step(h.z, 0f), -Step(h.w, 0f));

        Vector4 a0 = new Vector4(b0.x + s0.x * sh.x, b0.z + s0.z * sh.y, b0.y + s0.y * sh.x, b0.w + s0.w * sh.y);
        Vector4 a1 = new Vector4(b1.x + s1.x * sh.z, b1.z + s1.z * sh.w, b1.y + s1.y * sh.z, b1.w + s1.w * sh.w);

        Vector3 g0 = new Vector3(a0.x, a0.y, h.x);
        Vector3 g1 = new Vector3(a0.z, a0.w, h.y);
        Vector3 g2 = new Vector3(a1.x, a1.y, h.z);
        Vector3 g3 = new Vector3(a1.z, a1.w, h.w);

        Vector4 norm = TaylorInvSqrt(new Vector4(Vector3.Dot(g0, g0), Vector3.Dot(g1, g1), Vector3.Dot(g2, g2), Vector3.Dot(g3, g3)));
        g0 *= norm.x;
        g1 *= norm.y;
        g2 *= norm.z;
        g3 *= norm.w;

        Vector4 m = new Vector4(
            Mathf.Max(0.6f - Vector3.Dot(x0, x0), 0f),
            Mathf.Max(0.6f - Vector3.Dot(x1, x1), 0f),
            Mathf.Max(0.6f - Vector3.Dot(x2, x2), 0f),
            Mathf.Max(0.6f - Vector3.Dot(x3, x3), 0f));

        m = new Vector4(m.x * m.x, m.y * m.y, m.z * m.z, m.w * m.w);
        m = new Vector4(m.x * m.x, m.y * m.y, m.z * m.z, m.w * m.w);

        Vector4 px = new Vector4(Vector3.Dot(x0, g0), Vector3.Dot(x1, g1), Vector3.Dot(x2, g2), Vector3.Dot(x3, g3));

        return 42f * Vector4.Dot(m, px);
    }

    private static float Step(float edge, float x) => x < edge ? 0f : 1f;
}
