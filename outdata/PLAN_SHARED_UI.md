# Shared UI implementation plan

Scope: shared controls first; individual screen redesigns deferred. SpinSquad screenshots and local APK inventory are references for hierarchy/readability, not assets to copy. Static APK inspection does not establish exact live layout or animation timing.

## 1. Establish shared controls

- Inventory existing sprites and reuse suitable ones before generating replacements.
- Orange: primary action. Blue: secondary action. Dark: Back/Cancel. Grey: disabled.
- Standardize font, margins, touch areas, rounded corners and normal/pressed/disabled states.
- Use correctly configured 9-slice panels; preserve square icons and avoid stretched corners.
- Check at actual phone scale, on light and dark backgrounds.

Gate: show one control preview containing buttons, popup and toggles; approve before applying everywhere.

## 2. Supply missing art, one coordinated style

Candidate missing assets, subject to inventory verification:

1. Utility icon sheet: Settings gear, Close, Back, Pause, Resume, Restart, Home, speaker, music note and vibration. Empty controls receive text from code, never baked English labels.
2. Toggle sheet: ON/OFF with matching geometry; separate slider track/thumb only if volume sliders are implemented.
3. Shared popup panel: quiet centre, thick readable outline, restrained flat colour bands, generous text area; safe 9-slice border.
4. Dark/disabled button variants only where existing art cannot serve them.

Generate only approved gaps. Match our orange/blue buttons and chibi roster; use SpinSquad for compact hierarchy and strong readability, not identical artwork. Transparent antialiased alpha, no magenta, no checkerboard, no cropped parts, no forced square cells. Normal/pressed pairs must have matching dimensions and pivots. Validate generated assets before import; exact pair geometry may require code-native controls rather than two independently generated images.

Do not generate more backgrounds, relics, characters, shop art or whole-screen mockups in this pass.

## 3. Settings

- Shared overlay accessible from Home and battle; English labels.
- Music, Sound Effects, Vibration and Close.
- Verify actual audio/haptic infrastructure first; implement supported behaviour or explicitly mark unsupported options, never a fake toggle.
- Persist preferences backward-compatibly without changing progress/currencies.
- Modal input blocking, predictable close/back handling, safe-area placement.

## 4. Pause and confirmations

- In battle: Resume, Restart and Home.
- Pause gameplay when opened; remember and restore the selected speed, including x2/x3.
- Confirm Restart/Home before discarding an ongoing attempt.
- Coordinate unscaled UI animations and any ongoing roll/reward presentation: no lost, duplicated or newly rerolled rewards.
- Always restore time scale on scene exit and prevent multiple transitions.

## 5. Feedback and shared messages

- Subtle press/release motion, click sound and supported optional haptic.
- Prevent double activation of spending and scene changes.
- Shared English popup for insufficient Gold/Keys and confirmation/error messages.
- Keep existing reward logic; roll redesign is a separate subsequent pass.

## 6. Verify and hand off

- Compile and check actual Unity behaviour, not just static preview.
- Home/battle open-close, pause at each speed, restart/exit, rapid taps and input behind overlays.
- Settings persist after restart; existing saves still load.
- Phone aspect ratios/safe areas; readable labels and undistorted corners.
- Show in-game screenshots and list changes/remaining gaps.

Execution order: inventory/control preview → generate missing art → import shared controls → Settings → Pause/confirmations → feedback → verification. User reviews the first style preview before expansion.
