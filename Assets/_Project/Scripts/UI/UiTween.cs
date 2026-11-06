using System;
using System.Collections;
using UnityEngine;

namespace Project.UI
{
    // ANIM-001: bo chuyen dong dung chung cho UI.
    // Luon dung unscaledDeltaTime: nut toc do x1/x2/x3 doi Time.timeScale, va khi pause
    // timeScale = 0 — hoat anh UI khong duoc bi nhanh/dung theo.
    public static class UiTween
    {
        // Bang thoi luong chuan — moi hoat anh lay tu day de nhip dong deu.
        public const float Fast = .15f, Normal = .3f, Slow = .6f;

        public static float Linear(float t) => t;
        public static float OutCubic(float t) { t = 1 - t; return 1 - t * t * t; }
        public static float InCubic(float t) => t * t * t;
        // Vuot qua dich mot chut roi ve — hop cho "bat ra".
        public static float OutBack(float t) { const float c = 1.70158f; t -= 1; return 1 + (c + 1) * t * t * t + c * t * t; }

        // Chay step(0..1) trong duration. skip() tra true -> nhay thang toi 1 va ket thuc.
        // step(1) LUON duoc goi dung mot lan o cuoi, ke ca khi bo qua, de trang thai cuoi luon dung.
        public static IEnumerator Run(float duration, Action<float> step, Func<float, float> ease = null, Func<bool> skip = null)
        {
            ease ??= OutCubic;
            float t = 0;
            while (t < duration && (skip == null || !skip()))
            {
                step(ease(Mathf.Clamp01(t / duration)));
                yield return null;
                t += Time.unscaledDeltaTime;
            }
            step(1);
        }

        public static IEnumerator Wait(float seconds, Func<bool> skip = null)
        {
            float t = 0;
            while (t < seconds && (skip == null || !skip())) { yield return null; t += Time.unscaledDeltaTime; }
        }
    }

    // Quay deu theo thoi gian thuc (tia sang sau the hiem).
    public sealed class UiSpin : MonoBehaviour
    {
        public float DegreesPerSecond = 30;
        void Update() => transform.Rotate(0, 0, -DegreesPerSecond * Time.unscaledDeltaTime);
    }
}
