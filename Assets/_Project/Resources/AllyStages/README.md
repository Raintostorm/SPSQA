# Approved ally preview stages

Runtime copies of the five approved v3 PNGs in `artsrc/ui/ally-stage-demo-v3/`.
Generation prompts and source provenance are recorded in that folder's README.

Loaded as textures by `AllyUpgradeView` via `AllyStages/{human,elf,dead,monster,machine}`.
Race enum order is Human, Elf, Dead, Monster, Machine. These images contain
backgrounds only; ally sprites and all UI text remain separate runtime controls.

The Upgrade scene keeps its serialized roster and balances. UpgradeScreen installs
the new runtime view, falling back to the legacy screen if stage imports are missing.
