# Progressive replacement — batch 1

Installed: human_common (Bandit V5), elf_common V2, dead_common V1, monster_common V1, machine_common V1. Other rarities and deferred Machine Epic untouched.

Each has 99 baked transparent frames across idle/walk/attack/dead. These are sampled transforms of approved authored poses, not 99 newly drawn poses. Dynamic non-square crop, preserved alpha, root pivot included. Same measured world idle height as previous art; gameplay definitions and animation asset GUIDs retained. BakedMotion disables duplicate runtime bob/lunge. Walk/attack effects are baked; ranged projectiles remain gameplay-owned; existing final-frame death VFX retained.

Recovery: previous animation assets in outdata/simple_import_backup; original source sprites untouched. Restore the individual Anim asset from this backup to undo a unit. New files live under Assets/_Project/Art/UnitsSimple.

Buttons: btn_primary, btn_primary_down, btn_blue, btn_blue_down, btn_small replaced using buttons_v5.png. Original GUIDs retained. Normal/pressed pairs share dimensions and slice borders. Old sprites/meta in outdata/buttons_v5_backup. Removed Home runtime grey-card override so scene buttons remain blue. Prompt: artsrc/ui/BUTTONS_V5_PROMPT.md; built-in imagegen.

Background review: Home remains unchanged. Its detailed painted landscape, fine foliage and soft sky differ from the simplified thick-outline characters. The quiet dirt area still supports silhouettes; future background pass should simplify foliage/material detail while retaining existing UI and character placement. Do not change camera/grid coordinates just to accommodate art.

Validation: 495 transparent frames have complete padded alpha bounds and valid pivots; C# compiles clean. Unity imported new assets and Play-mode Home showed the new human/elf/skeleton. Full combat review of all five is still required; no claim that every battle animation has been visually approved in-engine.
