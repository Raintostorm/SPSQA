# Runtime integration, 2026-10-30

- tools/build_mythic_manifest.py now installs the four approved padded frames to guardian-attack-v3-0..3.bytes and the Guardian attack entries8..11. Idle/walk/skill/dead and attack timing(.13 each, .52 total, contact step2, cooldown .7) remain as before. The generator retains this override on subsequent imports.
- UnitBrain commits to a living target during chase, checks attack-in-progress before targeting/facing, and captures an independent attackTarget. A dead victim cannot redirect contact damage. It acquires again only after the attack finishes. Applies to ordinary allies and enemies too.
- DamagePopup uses the bundled Lilita One font (Google Fonts, OFL license included), larger type, dark four-corner outline, thousands separators, staggered origin and .9s real-time lifetime (paused with gameplay). A bounded pool of64 reuses objects, cached font and shared materials.
- Tests: tools/test_target_lock.sh compiles the real UnitBrain with engine doubles, passes stable-chase/dead-victim/no-redirection/next-target cases. C# compilation clean. Frame pivots, alpha bounds and font files validated. Unity reimport completed; full visual combat verification on device is still pending.
- Font source: https://github.com/google/fonts/tree/main/ofl/lilitaone
