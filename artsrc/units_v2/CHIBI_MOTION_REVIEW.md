# dead_common chibi — thử walk/attack/dead

Mẫu duyệt: chibi_proportions_approved.png. Tạo bằng imagegen tích hợp. Image1 dùng style/proportions, Image2 dùng pose runtime; lượt sửa walk/dead đảo thứ tự reference để ưu tiên motion. Không thay Assets, clip,GUID,timing trong game.

Prompt chung:
Image1 is APPROVED DESIGN/PROPORTION reference only. Image2 is AUTHORITATIVE EIGHT runtime animation poses, in exact order. Redraw Image2 with Image1's exact cute simple chibi skeleton design. Keep EACH frame's arm angles, hand grip, leg action, body lean, sword direction, shield position, facing and phase. Do NOT invent a generic animation or copy idle across all frames. Exactly8 complete figures in1horizontalrow, very wide sheet, generous empty gaps,margins; no square cropping,no labels. Same standing head/body sizes across all poses. Approved chibi head+plain grey helmet half standingheight, short chunky limbs, cream simple ribcage atmost2 broad lines, round plainbrownshield, shortgreysword. TWO arms; sword remains anatomicalRIGHT hand with CLOSED grip on handle BEHIND guard, shieldLEFT throughout. No changing hands,no third arm,no floating sword. Uniform thick darktoned outlines, flatcream/grey/brown/darkeye fill, no texture, scratches,rivets,individual ribs,toes,teeth,nosehole,scarf or gradients. Lightupperleft if flatshadow used. True antialiasedtransparentalpha. Allweapon tips/head/feet intact. 

Walk:
Preserve original8walk phases and weight shifts, loop8to1. Sword always follows wrist and forearm, never fusedwithshield.

Attack:
Preserve original attack as ONE-HAND windingup then forward thrust/strike then recover, NOT invent broad overheadslash. Frame4 reference has missing blade: reconstruct the SAME sword rigidly from visible guard extending forwardRIGHT at wristangle, not empty handle. Keep original other7pose directions.

Dead:
Preserve original DEATH direction: initially recoil then hunch/fall FORWARD RIGHT, finally lying on RIGHT side headRIGHT feetLEFT. Do NOT reverse to falling backwardLEFT. Frames1/2,3/4,5/6,7/8 may be near-identical as original; do NOT force new phases. Final frame remains lying dead, not standing.

Lượt sửa walk nhấn mạnh bent/crossing legs ở frame2/7, steppingfoot4, raisedbackfoot6. Dead bỏ secondshield và khôi phục swordlow1/2. Tất cả có8cụm chính. Thửchuyểnđộng, chưa chứng nhận copy hình học từngpose chínhxác. Walk shield vẫn ít chuyển động hơnnguồn. Palette/viền chưađo đạt toànbộ.

GIF dùng duration của runtimeasset, một scale chung, côlập từngcụm với alphaAA gốc, không cắt ôvuông hay rescale từngframe. Baseline được đặt để xem chuyển động; lệchbaseline raw: idle0px,walk11px,attack0px,dead13px. Cần kiểm pivot/anchor trước khiimport. Dead giữframecuối2giây rồi resetdemo, không phải xác tự đứng dậy tronggame.
