# human_rare revision 2 — pending review

Pose 2/4 corrected by built-in imagegen: spear shaft in front of torso/forearm, gripping mitten in front of shaft. Right hand remains grip, left empty. Strong thrust ribbon and two tip echoes on same axis.

Six whole poses + authored transforms. No square sprite cells. Original soft alpha preserved; variable bounding boxes and ownership masks prevent adjacent-pose contamination. Weapon VFX remains separate and limited to the strike window; walk dust unchanged. V1 preserved for comparison. Not imported into Unity.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/human_rare_trial_v2 --profile spear --effects --expressive --strong-fx

Full idle/walk/attack/dead demos beside this document. For unchanged source prompts, see sibling human_rare_trial_v1/PROMPT.md; human/elf targeted imagegen edit prompts saved in this folder.
