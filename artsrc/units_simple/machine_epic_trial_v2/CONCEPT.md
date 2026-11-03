# Furnace Crawler V2 — flamethrower

User correction: attack is FIRE BREATH / FLAMETHROWER, never a stab or mechanical ram. Fixed short nozzle attached to chassis; brace backwards, emit continuous attached flame jet, shut off, recover. No extending piston, no forward lunge, no detached fireball. Flame is a separate hard-edge red/orange/yellow effect anchored to actual nozzle silhouette using the same scale and pivot as the firing pose. Active 0.20–0.62s in a 1.15s cycle; taper in/out. Walk dust unchanged. Idle/walk/dead retained. Complete dynamic crops with original antialiased alpha. Demo only; Unity unchanged.

Reproduce: python3 tools/preview_simple_bandit.py --directory artsrc/units_simple/machine_epic_trial_v2 --profile flamethrower --effects --expressive --strong-fx
