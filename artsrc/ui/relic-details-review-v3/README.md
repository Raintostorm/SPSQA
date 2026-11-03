# Relic Details v3 — approval preview
Built-in image_gen. Runtime unchanged.
Reference is actual saved game capture ../audit-proposals-v1/relic-detail-in-game.png, not fresh visual QA.
Proposal: navy modal, fixed identity/progress/footer, scroll stat body only when it overflows, simple dividers, scope/buff line then current value and next-level delta, gold price only. Preview shows Oak Staff and multi-stat Golden Armor; proposed price is illustrative. No mechanics changes.
Prompt: two complete iPhone X screenshots of single/multi-stat relic modal, current cartoon vault behind dim, navy/blue/gold controls, Oak Staff +3.9% attack next +0.3%; Golden Armor damage reduction +15.4 next +1.4 and HP +17.6 next +1.6; pinned header/material/footer and scroll body. Full generation prompt in conversation tool call.

Implemented after user approval: fixed navy frame with safe-area fit, independent stat ScrollRect and overflow scrollbar, fixed resource/price/status controls. Compile and relic economy regression pass. Runtime visual QA pending.
