# Motion study — not approved game assets

Supplied XAPK: extracted human `1005_bandit`; COMMON rarity not confirmed. Reference only, never imported into Assets.

Preview decodes local sprite/scale/Euler curves at30Hz. Does not reproduce prefab hierarchy, original pivots, world motion or slash VFX; approximate foot-centred presentation, NOT an exact game recording. Durations idle .5167s, walk1.0167s, attack1.5167s, dead .8333s. Skeleton uses generated original art and these motion rhythms. Death uses upright death pose plus decoded clockwise rotation; unused generated corpse pose has wrong fall direction. No square source cuts. Shared scale; true source alpha retained. Style not certified five flatcolors/55%black.

Tool: built-in imagegen.

Base prompt:
Create an original dead_common skeleton game animation, NOT the red-haired character in image1. Image1 is ONLY MOTION POSE rhythm reference. Image2 is our skeleton IDENTITY reference: plain pot helmet, cream skull/bones, purplewrap, brown roundshield, greyshortsword. Keep that design, simplify flat geometric shapes, heavy pureblack6% outlines, ONLYflat cream#EAD0B6 purple#6B3FA0 grey#8A8A8A brown#7A5A3A black. No gradients texture shadows nose teeth fingers details. Face RIGHT identical side3/4angle always. Sword SAME right anatomicalhand and shield SAME leftarm always, never swap or mirror. Use short compact movements rather than exaggerated long strides. Consistent body/headsize acrossframes. Transparent alpha AA, wide row with80px gaps60px margins, full weapon/limbextents NOsquarecells NOclipping NOtext.

Clip specs:
{
  "idle": "Exactly THREE standing idle poses in one row: ready, small compression, recovery. Very subtle breathing, plantedfeet, weapongripunchanged.",
  "walk": "Exactly TWO poses in one row per actual reference: ready standing stance; compact bentknees stepping/hop stance with one foot lifted and sword raised slightly. NOT long strides. This alternates with idle and transform bob adds motion. Samebaseline groundedfoot.",
  "attack": "Exactly THREE poses in one row: windup sword lifted behind ABOVE RIGHT shoulder in SAME sword hand (shield stays leftfront), transition sword arm moves forward, strike lowforward sword sweep right with slight torsolean. Match motion stages only, retain skeleton identity. Samegroundfeet, continuousgrip.",
  "dead": "Exactly TWO poses in one row: standing stagger leaning back a little; SAME skeleton lying on its BACK, headLEFT feetRIGHT, rotatedabout90degrees counterclockwise from upright because original death curve z=-90. Sword stays in same anatomicalhand and shield samearm. No reanimation, no torsoflipping, completecorpse."
}

Attack correction:
Precise edit: change ONLY FIRST figure in this3figure skeleton attack strip. Keep figures2and3 unchanged. Firstfigure hand ownership WRONG: redraw FIRST as windup version of SECOND: brownshield remains on LEFT of torso same arm asfigure2, sword grip on RIGHT of torso SAMEfrontarm asfigure2, swordhand raised on RIGHT of skull at shoulderheight with bladepointing UP RIGHT. Never swordleft ofskull, never shieldright oftorso. Trace same swordshoulder tohand assecondfigure. Keep exact skull/helmet/body/feet scale, sameflatcolorsandheavyoutlines, transparentalpha, margins. No otherchanges.

