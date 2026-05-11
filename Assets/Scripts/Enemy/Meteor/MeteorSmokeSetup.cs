using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MeteorSmokeSetup : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        main.startSpeed = 0f;

        // Particle nhỏ
        main.startSize = 0.18f;

        // Trail dài
        main.startLifetime = 5f;

        // Không biến mất sớm
        main.maxParticles = 5000;

        // =========================
        // EMISSION
        // =========================

        var emission = ps.emission;
        emission.rateOverTime = 280f;

        // =========================
        // SHAPE
        // =========================

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;

        // Ít tỏa
        shape.angle = 1f;
        shape.radius = 0.01f;

        // =========================
        // SIZE OVER LIFETIME
        // =========================

        var sizeLife = ps.sizeOverLifetime;
        sizeLife.enabled = true;

        AnimationCurve sizeCurve = new AnimationCurve();

        // Nở cực nhanh
        sizeCurve.AddKey(0f, 0.1f);

        // Đạt kích thước gần max rất sớm
        sizeCurve.AddKey(0.15f, 1f);

        // Giữ gần nguyên kích thước
        sizeCurve.AddKey(1f, 1.15f);

        sizeLife.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // =========================
        // COLOR OVER LIFETIME
        // =========================

        var colorLife = ps.colorOverLifetime;
        colorLife.enabled = true;

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(
                    new Color(0.7f, 0.7f, 0.7f),
                    0f
                ),

                new GradientColorKey(
                    new Color(0.7f, 0.7f, 0.7f),
                    1f
                )
            },

            new GradientAlphaKey[]
            {
                // Giữ alpha lâu
                new GradientAlphaKey(0.65f, 0f),

                new GradientAlphaKey(0.65f, 0.85f),

                // Fade cuối
                new GradientAlphaKey(0f, 1f)
            }
        );

        colorLife.color = gradient;

        // =========================
        // NOISE
        // =========================

        var noise = ps.noise;
        noise.enabled = true;

        // Rất nhẹ để không bị tan
        noise.strength = 0.08f;
        noise.frequency = 0.3f;
    }
}