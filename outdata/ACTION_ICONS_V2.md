# Approved action icon import

Source: `artsrc/ui/action_icons_v2.png`, edited with built-in image generation.
Edit prompt: Preserve twelve approved icon designs and their 4x3 order; remove background and exterior halos; true transparent alpha with antialiased contours; no text or frames.

Extracted to `Assets/_Project/Resources/SharedUI`: settings, enemy, upgrade, relics, home, restart, battle, roll, resume, sound, vibration, close.
Previous replaced PNGs are preserved in `outdata/action_icons_v2_backup`.
Crop uses actual row gutters at 366 and 700 on the 1536x1024 sheet. Alpha is preserved without thresholding or colour keying.

Bindings: Home Start/Upgrade/Relics, Choose Level Play/Back, Battle Roll/Start/enemy preview, Settings and sound/vibration/resume/restart/home/close/cancel.
Upgrade/Relics/enemy preview use icon-only controls. Primary actions and settings toggles retain short labels/state.
Existing button callbacks and gameplay logic are unchanged.
