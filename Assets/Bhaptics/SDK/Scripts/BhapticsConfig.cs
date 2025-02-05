﻿using Bhaptics.Tact.Unity;
using UnityEngine;

[System.Obsolete("No more updates. Use SDK2.")]
[CreateAssetMenu(fileName = "Data", menuName = "bHaptics/[Deprecated] Create Config", order = 1)]
public class BhapticsConfig : ScriptableObject
{
    [Header("Windows Settings")]
    public bool launchPlayerIfNotRunning = true;

    [Header("Android Settings")]
    public BhapticsAndroidManager AndroidManagerPrefab;


    [Tooltip("If you set it true, you don't need to define permissions and external bHaptics VR Player(beta) on SideQuest will be required.")]
    public bool UseOnlyBackgroundMode = false;
}