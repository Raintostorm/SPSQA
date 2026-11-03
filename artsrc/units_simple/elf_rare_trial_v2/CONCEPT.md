# elf_rare revision 2 — pending review

Pose 2/6 corrected by built-in imagegen: weapon returned to same near-side right hand used in idle/recovery; far-side left hand empty. Strong tapered diagonal slash plus pale displaced tip path.

Six whole poses + authored transforms. No square sprite cells. Original soft alpha preserved; variable bounding boxes and ownership masks prevent adjacent-pose contamination. Weapon VFX remains separate and limited to the strike window; walk dust unchanged. V1 preserved for comparison. Not imported into Unity.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/elf_rare_trial_v2 --profile skeleton --effects --expressive --strong-fx

Full idle/walk/attack/dead demos beside this document. For unchanged source prompts, see sibling elf_rare_trial_v1/PROMPT.md; human/elf targeted imagegen edit prompts saved in this folder.
