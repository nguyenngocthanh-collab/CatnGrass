using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================
//  PhaseData.cs
// ============================================================
[CreateAssetMenu(menuName = "WaveSystem/PhaseData", fileName = "Phase_01")]
public class PhaseData : ScriptableObject
{
    [Header("━━ TIMING ━━")]
    [Tooltip("Phase bắt đầu sau bao nhiêu giây")]
    public float startTime = 0f;

    [Tooltip("0 = vô hạn")]
    public float duration = 10f;

    [Header("━━ HAZARDS ━━")]
    public List<HazardEntry> hazards = new();
}

// ============================================================

[Serializable]
public class HazardEntry
{
    [Header("Type")]
    public HazardType type = HazardType.Shark;
    public bool showGizmo = true;
    // =====================================================
    // SPAWN SIDE
    // =====================================================
    [Header("━━ SPAWN SIDE ━━")]
    public SpawnSide spawnSide = SpawnSide.Left;

    [Tooltip("Random spawn side từ danh sách bên dưới")]
    public bool randomSpawnSide = false;
    public SpawnSide[] randomSidePool =
    {
        SpawnSide.Left, SpawnSide.Right,
        SpawnSide.Top,  SpawnSide.Bottom
    };

    // =====================================================
    // POSITION
    // =====================================================
    [Header("━━ POSITION ━━")]
    [Range(-1f, 1f)]
    public float sideOffset = 0f;
    public bool randomSideOffset = false;

    // =====================================================
    // AIM
    // =====================================================
    [Header("━━ AIM ━━")]
    [Range(-180f, 180f)]
    public float aimOffset = 0f;
    public bool randomAimOffset = false;
    public float randomAimMin = -30f;
    public float randomAimMax = 30f;

    // =====================================================
    // SPEED
    // =====================================================
    [Header("━━ SPEED ━━")]
    public float speed = 5f;
    public bool randomSpeed = false;
    public float randomSpeedMin = 4f;
    public float randomSpeedMax = 8f;

    // =====================================================
    // SCALE
    // =====================================================
    [Header("━━ SCALE ━━")]
    public float minScale = 1f;
    public float maxScale = 1f;
    public bool randomScale = false;

    // =====================================================
    // COLLIDER
    // =====================================================
    [Header("━━ COLLIDER ━━")]
    [Tooltip("Override collider size. (0,0) = giữ nguyên prefab")]
    public Vector2 colliderSize = Vector2.zero;
    public Vector2 colliderOffset = Vector2.zero;

    // =====================================================
    // TIMING
    // =====================================================
    [Header("━━ TIMING ━━")]
    [Tooltip("Giây sau khi phase bắt đầu thì spawn con cá này")]
    public float spawnDelay = 0f;

    [Tooltip("Thêm random delay (0 = không random)")]
    public float randomDelayRange = 0f;

    [Tooltip("Đứng im tại spawn point để chiếu warning (giây)")]
    public float holdDuration = 1f;

    [Tooltip("Warning fade out khi bắt đầu lao (giây)")]
    public float warningFadeOut = 0.35f;

    // =====================================================
    // WARNING
    // =====================================================
    [Header("━━ WARNING ━━")]
    public bool showWarningLine = true;
    public float warningLineLength = 14f;

    // =====================================================
    // HOOK
    // =====================================================
    public HookBehaviourData hookBehaviour;
}

// ============================================================

[Serializable]
public class HookBehaviourData
{
    public HookAxis axis = HookAxis.Vertical;
    public float castDistance = 6f;
    public float castSpeed = 3f;
    public float retractSpeed = 4f;
    public float holdDuration = 1.5f;
}

// ============================================================

public enum HazardType { Shark, Hook }
public enum HookAxis { Horizontal, Vertical }
public enum SpawnSide { Top, Bottom, Left, Right }