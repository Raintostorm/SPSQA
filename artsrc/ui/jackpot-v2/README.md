# Jackpot preview art v2

Generated with built-in imagegen, using actual human_legendary idle sprite as style-only reference. Real RGBA transparent source PNGs. Not imported into Unity; preview requires approval first.

Prompt set:
- Ribbon: one long empty gold ribbon, folded ends, cream highlight/orange lower rim, chunky dark outline, small coin piles and stars at ends, centre above left empty for original game actors. No text or new characters.
- Fireworks: 3x2 six-stage white/gold ignition, radial burst, starburst, gold/purple confetti, fading particles. Equal 512px cells, fixed centre, true alpha, flat cartoon outlines.

Demo stages: six slots settle -> white/blue/purple/gold build -> fireworks -> gold ribbon at reel with existing Human/Elf/Machine sprites bouncing -> JACKPOT title pops/settles -> ribbon retracts to result. x1/x2/x3 and pause. It is a motion proposal, not a captured Unity run or exact reproduction of the reference game. Original actor body sprites unchanged. Runtime gameplay and payout untouched.

Build: tools/build_jackpot_review.py. Editable animation fragment: tools/jackpot_review.template.html.
