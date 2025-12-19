using System.Collections.Generic;
using UnityEngine;

public class ThrustersSyncController : MonoBehaviour
{
    [Header("Thruster Groups")]
    public ParticleSystem[] backThrusters;   // pour avancer
    public ParticleSystem[] frontThrusters;  // pour reculer
    public ParticleSystem[] leftThrusters;   // pour aller à droite
    public ParticleSystem[] rightThrusters;  // pour aller à gauche

    [Header("Emission")]
    public float idleRate = 0f;
    public float maxRate = 50f;

    private Dictionary<ParticleSystem, float> thrusterPowers =
        new Dictionary<ParticleSystem, float>();

    void Start()
    {
        InitGroup(backThrusters);
        InitGroup(frontThrusters);
        InitGroup(leftThrusters);
        InitGroup(rightThrusters);

        // Start all once
        foreach (var ps in thrusterPowers.Keys)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }

    void InitGroup(ParticleSystem[] group)
    {
        foreach (var ps in group)
        {
            if (!thrusterPowers.ContainsKey(ps))
                thrusterPowers.Add(ps, 0f);
        }
    }

    void SetThrusterPower(ParticleSystem ps, float power)
    {
        thrusterPowers[ps] = Mathf.Max(thrusterPowers[ps], power);
    }

    void ApplyPowers()
    {
        var keys = new List<ParticleSystem>(thrusterPowers.Keys);

        foreach (var ps in keys)
        {
            float power = thrusterPowers[ps];

            var emission = ps.emission;
            emission.rateOverTime = Mathf.Lerp(idleRate, maxRate, power);

            // reset for next frame
            thrusterPowers[ps] = 0f;
        }
    }

    public void UpdateThrusters(Vector2 moveInput)
    {
        float forward = Mathf.Clamp01(moveInput.y);
        float backward = Mathf.Clamp01(-moveInput.y);
        float right = Mathf.Clamp01(moveInput.x);
        float left = Mathf.Clamp01(-moveInput.x);

        // Avancer → thrusters arrière
        foreach (var ps in backThrusters)
            SetThrusterPower(ps, forward);

        // Reculer → thrusters avant
        foreach (var ps in frontThrusters)
            SetThrusterPower(ps, backward);

        // Aller à droite → thrusters gauche
        foreach (var ps in leftThrusters)
            SetThrusterPower(ps, right);

        // Aller à gauche → thrusters droite
        foreach (var ps in rightThrusters)
            SetThrusterPower(ps, left);

        ApplyPowers();
    }
}
