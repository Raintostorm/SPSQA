# Chapter 1 slime demo v1

Original generated art; built-in ImageGen, transparent_background=true. Not copied from APK. Not installed into Unity.

- Moss and Iron: melee.
- Spore and Ember: ranged.
- Each PNG: 4 columns × 4 rows. Rows: Idle / Walk / Attack / Dead. Four poses per state, 16 per character. Original generated alpha preserved; no image post-processing.
- Browser preview uses equal-grid sprite playback; these are rough four-pose animations, not final smoothed runtime clips. Projectiles are embedded in attack poses, not separate combat projectile assets yet.
- UI: standalone Enemy Codex mockup plus synchronized animation gallery. No invented combat statistics.
- Before Unity integration: validate registration/pivots, refine frame timing and extract separate projectile sprites, then wire enemy definitions. No balance changes made.

## Prompt set

### moss

Use case: stylized-concept. Asset type: 2D game animation sprite sheet for SpinSquadAgain, ORIGINAL enemy art, not copied from another game.
Subject: Moss Slime, MELEE: small grass-green jelly, simple two angry dark eyes, tiny leaf tuft, no arms or legs. Attack is a forward body slam.
Style: extremely simple flat cartoon mobile game sprite, thick uniform black outlines, 2 tone cel shading, bold clean silhouette readable at 64 pixels. Same simplicity as a tiny flat cartoon knight. No painterly details, no realism, no texture.
Layout: exactly FOUR columns and FOUR rows equal-size cells, square sheet. 16 separate frames of ONE same character. No margins between cells, frame centered in each cell, feet baseline at 80% cell height. Transparent background throughout, no grid lines, no text, no labels, no shadows. Character occupies about 60% cell width and 55% cell height, consistent design/scale/registration, facing LEFT in every frame.
Row 1 IDLE: neutral, slight squash, neutral, slight stretch breathing loop.
Row 2 WALK: squash, upward small hop, descending stretch, land squash; locomotion bounce, no displacement across cell.
Row 3 ATTACK: windup squash, anticipation stretch backward, impact leaning LEFT, recovery.
Row 4 DEAD: struck recoil, collapsing low, flattened puddle, small flattened motionless puddle. Same accessories remain and fall with body. Not a loop.
Avoid: extra characters, weapons, backgrounds, typography, cinematic lighting, fancy glowing rendering. Preserve exact 4x4 grid.

### iron

Use case: stylized-concept. Asset type: 2D game animation sprite sheet for SpinSquadAgain, ORIGINAL enemy art, not copied from another game.
Subject: Iron Slime, MELEE: squat blue jelly with a simple gray iron helmet and a single broad metal brow plate, no arms or legs. Attack is a heavy forward headbutt.
Style: extremely simple flat cartoon mobile game sprite, thick uniform black outlines, 2 tone cel shading, bold clean silhouette readable at 64 pixels. Same simplicity as a tiny flat cartoon knight. No painterly details, no realism, no texture.
Layout: exactly FOUR columns and FOUR rows equal-size cells, square sheet. 16 separate frames of ONE same character. No margins between cells, frame centered in each cell, feet baseline at 80% cell height. Transparent background throughout, no grid lines, no text, no labels, no shadows. Character occupies about 60% cell width and 55% cell height, consistent design/scale/registration, facing LEFT in every frame.
Row 1 IDLE: neutral, slight squash, neutral, slight stretch breathing loop.
Row 2 WALK: squash, upward small hop, descending stretch, land squash; locomotion bounce, no displacement across cell.
Row 3 ATTACK: windup squash, anticipation stretch backward, impact leaning LEFT, recovery.
Row 4 DEAD: struck recoil, collapsing low, flattened puddle, small flattened motionless puddle. Same accessories remain and fall with body. Not a loop.
Avoid: extra characters, weapons, backgrounds, typography, cinematic lighting, fancy glowing rendering. Preserve exact 4x4 grid.

### spore

Use case: stylized-concept. Asset type: 2D game animation sprite sheet for SpinSquadAgain, ORIGINAL enemy art, not copied from another game.
Subject: Spore Slime, RANGED: purple jelly with a small cream mushroom cap with three purple spots, simple dark eyes, no arms or legs. Attack inflates cheeks then spits one small round purple spore to the LEFT.
Style: extremely simple flat cartoon mobile game sprite, thick uniform black outlines, 2 tone cel shading, bold clean silhouette readable at 64 pixels. Same simplicity as a tiny flat cartoon knight. No painterly details, no realism, no texture.
Layout: exactly FOUR columns and FOUR rows equal-size cells, square sheet. 16 separate frames of ONE same character. No margins between cells, frame centered in each cell, feet baseline at 80% cell height. Transparent background throughout, no grid lines, no text, no labels, no shadows. Character occupies about 60% cell width and 55% cell height, consistent design/scale/registration, facing LEFT in every frame.
Row 1 IDLE: neutral, slight squash, neutral, slight stretch breathing loop.
Row 2 WALK: squash, upward small hop, descending stretch, land squash; locomotion bounce, no displacement across cell.
Row 3 ATTACK: windup squash, anticipation stretch backward, impact leaning LEFT, projectile visible near left edge only in frame 3, recovery.
Row 4 DEAD: struck recoil, collapsing low, flattened puddle, small flattened motionless puddle. Same accessories remain and fall with body. Not a loop.
Avoid: extra characters, weapons, backgrounds, typography, cinematic lighting, fancy glowing rendering. Preserve exact 4x4 grid.

### ember

Use case: stylized-concept. Asset type: 2D game animation sprite sheet for SpinSquadAgain, ORIGINAL enemy art, not copied from another game.
Subject: Ember Slime, RANGED: coral-orange jelly with a small yellow flame-shaped tuft enclosed by black outline, simple dark eyes, no arms or legs. Attack inflates then spits one small orange fireball to the LEFT.
Style: extremely simple flat cartoon mobile game sprite, thick uniform black outlines, 2 tone cel shading, bold clean silhouette readable at 64 pixels. Same simplicity as a tiny flat cartoon knight. No painterly details, no realism, no texture.
Layout: exactly FOUR columns and FOUR rows equal-size cells, square sheet. 16 separate frames of ONE same character. No margins between cells, frame centered in each cell, feet baseline at 80% cell height. Transparent background throughout, no grid lines, no text, no labels, no shadows. Character occupies about 60% cell width and 55% cell height, consistent design/scale/registration, facing LEFT in every frame.
Row 1 IDLE: neutral, slight squash, neutral, slight stretch breathing loop.
Row 2 WALK: squash, upward small hop, descending stretch, land squash; locomotion bounce, no displacement across cell.
Row 3 ATTACK: windup squash, anticipation stretch backward, impact leaning LEFT, projectile visible near left edge only in frame 3, recovery.
Row 4 DEAD: struck recoil, collapsing low, flattened puddle, small flattened motionless puddle. Same accessories remain and fall with body. Not a loop.
Avoid: extra characters, weapons, backgrounds, typography, cinematic lighting, fancy glowing rendering. Preserve exact 4x4 grid.

