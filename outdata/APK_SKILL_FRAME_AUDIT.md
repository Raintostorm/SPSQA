# Skill frame and roll-motion audit — 2026-10-12

Read-only gameplay audit. Source: user-supplied Spin+Squad! 2.5.1 XAPK at /Users/vuhagiabao/spin/Spin+Squad!_2.5.1_APKPure.xapk, nested base_assets.apk. Scanned 5,621 Data entries and found 299 AnimationClip objects whose names contain "skill", including variants and effect clips. Not a gameplay recording or a measurement of device render FPS.

All 299 clips declare m_SampleRate = 60. Durations range from 0.183333 to 1.883333 seconds (11–113 sampling intervals). The most frequent duration is 1.016667 seconds (131 clips). Do not confuse sample rate, timeline samples, sprite image count, and actual rendered FPS. Dense curve arrays can include extra endpoint/guard samples.

Examples:
| APK clip | Duration | Sample rate | Sampling intervals | Dense frames | Non-null unique PPtr references |
|---|---:|---:|---:|---:|---:|
| 202045_archerking_skill_1 | 1.016667s | 60 | 61 | 63 | 3 |
| 202044_warriorking_skill_1 | 1.016667s | 60 | 61 | 63 | 6 |
| 202028_robotboss_skill_1 | 1.516667s | 60 | 91 | 93 | 7 |
| 202046_lionking_skill_1 | 1.016667s | 60 | 61 | 63 | 5 |

PPtr references are not a count of rendered frames or proof of independently drawn poses. The package uses sprite changes together with motion curves.

Current SpinsquadAgain Skill assets: all five Legendary clips have 42 sprite entries, each duration 0.03333333s, total approximately 1.4s, no loop or ping-pong. Pixel hashes reveal repeated images:
| Character | Timeline sprite entries | Distinct RGBA pixel images |
|---|---:|---:|
| Human | 42 | 14 |
| Elf | 42 | 16 |
| Machine | 42 | 11 |
| Dead | 42 | 23 |
| Monster | 42 | 23 |

These are baked from a small set of authored poses and procedural movement; not 42 independently authored poses. The new generated VFX candidates have six source frames each and are NOT imported into game. The latest chat preview uses six sampled character frames and blends between six VFX cells, so its smoothness is not representative of the full 42-entry Unity clip.

Roll in Unity: 0.55s straight-line constant-speed light trail, per-render-frame position update, then instantaneous recruit spawn and ring. Latest art preview: 0.9s straight path, cross-fading comet sheet frames, immediate character appearance on arrival. Shape/pivot differences between generated frames can create ghosting and jitter even at high render FPS.

Recommended, not implemented: one stable comet head with a continuous curved trail, Bézier trajectory updated every rendered frame, mild ease-in/out, a brief arrival flash, then 0.12–0.18s scale/opacity reveal at the final board position. Use separate appearance/travel/impact phases rather than cross-fading the whole six-frame sheet. Preserve exactly-once payouts and pause handling. More source frames alone will not solve pivot drift, linear motion or abrupt spawn.
