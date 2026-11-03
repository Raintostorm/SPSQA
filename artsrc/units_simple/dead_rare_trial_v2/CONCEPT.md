# dead_rare revision 2 — pending review

Source art and ranged effect unchanged from V1. No foot dust, fast straight cyan orb.

Six whole poses + authored transforms. No square sprite cells. Original soft alpha preserved; variable bounding boxes and ownership masks prevent adjacent-pose contamination. Weapon VFX remains separate and limited to the strike window; walk dust unchanged. V1 preserved for comparison. Not imported into Unity.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/dead_rare_trial_v2 --profile wisp --effects --expressive --strong-fx

Full idle/walk/attack/dead demos beside this document. For unchanged source prompts, see sibling dead_rare_trial_v1/PROMPT.md; human/elf targeted imagegen edit prompts saved in this folder.
