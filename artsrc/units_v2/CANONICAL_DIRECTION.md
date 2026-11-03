# Hướng sửa theo yêu cầu mới

Nguồn chuyển động: 20model/*/*/character.json và PNG được manifest trỏ tới. Có20 nhân vật,350 pose. Mỗi con đủidle/walk/attack/dead;5 Legendary cóskill. Không phải bộ rig3D. Không giả định sheet là strip đều hay ô vuông.

Không dùng chuyển động tự sinh của bộ *_handfix.png làm chuẩn nữa; giữ file để đối chiếu, chưa nhậpgame. Chỉ đơn giản hóa art của pose gốc, không đổi tay, vũkhí, sốpose, thứtự, duration, anchors, normalization. Đi từng con, duyệt trước khi sang con khác.

dead_common:4pose mỗi clip. Walk thật dùng walk-poses-1-4.png frames1/4 và walk-corrections-2-3.png frames2/3, KHÔNG dùng rowwalk của base-sheet làmcanonicalwalk. Unity currently uses8 smoothframes perclip; không thay runtimeasset hay timing trong lượt này.

Bản thử dead_common_canonical_simple_candidate.png chỉ kiểm hướng surface simplification của base-sheet; chưa phải bộ đầy đủ final và chưa đo pose/anchor tương đương. Phải kiểm sai lệch so original trước khi import. Không thay UI màn hình.

Prompt imagegen tích hợp:

STYLE-TRANSFER ONLY of this exact sprite sheet, NOT new animation. Preserve EXACTLY all16 original poses in same4rows4columns, same locations, outer proportions, facing, sword positions, shield positions, hand grips, arm bends, leg positions, silhouette and death direction. Row1 idle,row2walk,row3attack,row4dead. Do NOT add scarf or redesign helmet. Do NOT invent poses or replace stab with slash. Simplify ONLY surface rendering: remove metallic scratches,rivets,wood grain,tiny teeth/toe bones,fine rib marks and gradients. Solid five main colors bonewhite,warmgrey,mediumbrown,darkteal,dullred(optional omit red if absent original); dark same-tone uniform chunky outlines. ONE hard-edge flat shadow patch maximum per major shape, lightupperleft. Keep skeleton recognizable50px. Every original weapon tip and limb complete; no square cell clipping. Transparent antialiasedalpha no magenta,text,labels,groundshadow. Trace the original pose geometry, not a generic skeleton mascot. Output same aspectratio as input. Preserve empty gaps.
