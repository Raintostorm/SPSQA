# Mythic v6 — frame crop correction

Keeps approved character designs unchanged. No generation or pixel edits. Runtime crops corrected only.

Cause: previous shared column boundaries incorrectly captured beginning of frame4 inside Idle/Walk frame3 and clipped left side of frame4. E.g. Idle3 old rect [576,23,309,244] extended to x885 although character ended x809 and next character began x865.

Fix: measure connected significant-alpha components from original sheets; one complete character per crop, with one-pixel margin. 20 source crops audited: all pixels alpha>100 belong to exactly one intended character; 0 significant neighboring pixels. Original image bytes preserved. crop-audit.json holds counts; audit-crops.py reproduces measurement from project root.

Base 12 poses use v4/base.png; Skill/Dead use v4 sheets. These sources remain unchanged. New bounds.json replaces earlier guessed grid regions. Runtime draw clears stage each frame.

Frame stepping now forces selected animation rather than allowing skill's walk/attack behavior to override the selected pose. Added gallery of all 20 individual frames clickable for inspection.

User selected the four other Mythic designs from v5/other-mythics.png for future use; preserved unchanged and displayed below gallery. No skill assignments or Unity gameplay installation in this pass.

URL http://127.0.0.1:8793/v6/
