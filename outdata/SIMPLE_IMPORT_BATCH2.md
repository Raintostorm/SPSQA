# Progressive replacement — batch 2 / 2026-10-11

User requested continuation of replacement. Installed latest trial versions for five Rare, human/elf/dead/monster Epic, and five Legendary. Machine Epic explicitly deferred; its Unity animation set was not modified. Combined with Common batch 1, 19/20 units now use new art.

Importer: tools/import_simple_common.py --remaining. Renderer: approved trial profiles, expressive motion and local effects, variable alpha rectangles; no square clipping. Original animation asset GUIDs and UnitDefinition stats/scales preserved. PPU matches each unit's pre-import measured idle world height. Sprite poses are sampled/baked transforms of authored art, not newly drawn frame counts. Ranged projectiles excluded from baked frame effects because gameplay owns them.

All five Legendary Skill clips imported, including Human vertical sword plant and Machine crossed-arm shield. Runtime does NOT currently call Skill: this is explicitly recorded as a gameplay gap, not silently reported as functional skill support. No new skill mechanics/cooldowns were invented in this pass.

Validation: 2,091 complete transparent frames total across 19 units; valid anchors and animation sprite references. C# compilation clean. Full combat visual validation remains pending. Backups: outdata/simple_import_backup/Anim_<unit>.asset; previous sprite files remain untouched.

Screen/roll audit: outdata/SCREEN_AND_ROLL_GAPS.md. Source inventory from supplied APK: outdata/reference/spinsquad_ui_inventory.json. Static asset evidence is distinguished from live gameplay verification.
