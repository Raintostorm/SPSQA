# Elite / Boss attack refinement

Original APK inspected as motion reference only, not imported into runtime. Mini King attack: 1.5167s, OnAttackEvent at 0.8667s; skill: 1.0167s, OnSkillEvent at 0.5833s. Boss King shares these event timings and has 11 sprite pointers in Attack versus 9 for Mini King. Queen skill is 1s, event at 0.5833s. These events do not by themselves establish damage or skill mechanics.

Built-in ImageGen, transparent output. Generated assets retain their original alpha. Existing own teal Bramble and crimson stone-shell Overlord sheets serve as identity references; APK images are not generation inputs. Idle/Walk/Dead and combat balance unchanged. Attack keeps 6 frames, impact on snap frame 3, 0.85s total.

Prompt set: six poses in 3 columns / 2 rows, facing left; neutral, deep squash, rear up with wide empty black mouth, forward toothless snap, flattened recoil, recovery. Preserve pale angry eyes, back spikes/plates, thick outlines and simple two-tone cartoon style. No teeth, fangs, tongue, drool, particles, text, grid or glow. Full silhouette and transparent padding in every cell. Elite uses spring-loaded anticipation; Boss uses heavier squash and towering gape.

Boss correction prompt: preserve all six poses and red stone-shell identity; remove all red haze, aura, rim glow and soft shadows; truly transparent crisp edges.

Source assets: elite-bite-v3.png and boss-bite-v3.png. Runtime: ChapterOneSlimes/elite_bite.png and boss_bite.png, with measured layout JSON. Earlier v2 sources retained for recovery.
