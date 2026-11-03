# Prep UI v2

- New generated background: `artsrc/bg/bg_prep_ui_v2.png`, installed at the existing prep sprite path so scene GUID references remain intact. Battle road unchanged.
- Runtime navy HUD/dock and independently placed blue Roll / orange Start Battle. Sliced procedural sprites preserve rounded corners.
- Six clipped reels scroll vertically with cubic deceleration; first stops at 1.1s, subsequent stops 0.17s apart. Final symbols use the existing payout, not a second roll.
- Reel row placed below the board (bottom anchor 19%, height 205 UI units), above the action dock. Hidden grid and drag behavior unchanged.
- Pause freezes presentation, leaving Prep cancels presentation. Buttons lock during spin. Existing spend-coins-before-start rule retained.
- C# compilation passed (70 files). Native Unity verification was blocked: editor menu showed Play disabled and repeated input returned unchanged state. Do not treat this as visually verified in Play yet.
