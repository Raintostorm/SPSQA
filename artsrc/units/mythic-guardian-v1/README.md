# Mythic guardian — preview only

Generated with built-in imagegen, using human_legendary idle as style reference.
Prompt direction: simplify to faceless ivory/teal chibi knight, plain gold shoulder pads and boots, short cape, thin halo, small sword; no shield/filigree/particles; 4 idle and 4 cast/sustain frames on transparent sheet.

animations.png is original generated output. bounds.json contains read-only alpha bounds; browser crops at render time and aligns feet. index.html animates the generated poses and a separate code-drawn ground aura. No Unity gameplay integration.

Skill interpretation: radius follows caster and matches Elf Legendary default radius 1.2 world units (1.6 * .75). Each target receives .2 ATK per second; self-heal is .5 actual total damage, capped by max HP. Preview uses training targets without armor and a 5-second duration for demonstration only. Duration, mana cost and activation rules remain unconfirmed.

Run: python3 -m http.server 8793 --bind 127.0.0.1 --directory artsrc/units/mythic-guardian-v1
