# Guardian four-frame attack proposal

Built-in imagegen, referenced the prior eight-frame Guardian. Exactly four new whole-body poses: wind-up, forward swing, contact with gold slash trail, recovery. Four .13s frames retain .52s attack duration and .7s cooldown. Runtime contact still aligns with the end of third frame at .39s.

Art is preview-only; runtime catalog unchanged. Existing idle/walk/skill/death unchanged, and other allies' VFX untouched. Source poses were cropped individually (the lower-left VFX extends beyond an equal-grid boundary). Final frame canvases are860x600 with common foot pivot430,540; crop-audit.json records source rectangles. No neighboring pose included. Character/effect quality remains subject to user approval.

Demo: index.html compares old four runtime poses to the four generated poses, with pause, slow playback and scrub. During cooldown, the old side displays actual idle and new side displays recovery.
