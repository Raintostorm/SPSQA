#!/usr/bin/env python3
"""Nhap tam art UI do AI tra ve: tach nen magenta, cat 3 mieng, ghi .meta.

AI tra ve mot tam duy nhat, nen MAGENTA dac chu khong trong suot. Tach nen
khong chi la "xoa diem mau magenta": ria da duoc khu rang cua, tuc moi diem
ria la MOT PHEP TRON giua mau that va magenta. Xoa thang thi con lai vien
hong — dung thu toi vua chan doan nham o anh chup man hinh.

Phai GIAI NGUOC phep tron:
    quan sat = that*alpha + key*(1-alpha)
    => that  = (quan sat - key*(1-alpha)) / alpha

Chay:  python3 tools/import_ui_art.py            # chi do va in
       python3 tools/import_ui_art.py --write
"""

import hashlib
import sys
from pathlib import Path

import numpy as np
from PIL import Image, ImageFilter

SRC = Path("artsrc/ui/ui_sheet.png")
OUT = Path("Assets/_Project/Art/UI")

# Chieu cao MUC TIEU tren man, don vi UI. Lay tu BattleHudBuilder.
TARGET_H = {"panel": 76.0, "button": 112.0, "button_down": 112.0}
REF_PPU = 100.0          # CanvasScaler.referencePixelsPerUnit, mac dinh cua Unity


def key_out(rgb, key, soft=150.0, hard=90.0):
    """Tach nen magenta. Ba buoc, moi buoc chua mot kieu sot khac nhau.

    1. KHOANG CACH toi mau key. Nguong rong (90..150) chu khong hep: anh AI
       tra ve qua WebP, nhieu nen o canh vien den bi day lech hang chuc don
       vi. Nguong hep la nhung diem do song sot nguyen ven — magenta DUC,
       alpha 255, rai khap anh. Da vap dung the.
       An toan vi mau that gan magenta nhat (do cua trai tim) van cach 206.

    2. SAC magenta. Lam, Do deu cao ma Luc thap la chi co nen. Bat nhung
       diem con sot ma buoc 1 khong voi toi.

    3. GIAI NGUOC phep tron o ria: moi diem ria la (that*a + key*(1-a)),
       xoa thang thi con vien hong.
    """
    d = np.linalg.norm(rgb - key, axis=2)
    a = np.clip((d - hard) / (soft - hard), 0.0, 1.0)

    r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    a[(g < r - 55) & (g < b - 55) & (r > 110) & (b > 110)] = 0.0

    safe = np.maximum(a, 0.25)[..., None]
    un = (rgb - key * (1.0 - safe)) / safe
    out = np.where(a[..., None] > 0.004, un, rgb)
    return defringe(np.clip(out, 0, 255), a), a


def defringe(colour, alpha, solid=0.92, radius=4):
    """Lay mau cua vung DAC lan ra vung ria.

    Giai nguoc o buoc 3 chia cho alpha, nen alpha cang nho ket qua cang
    nhieu. Thay han mau ria bang mau hang xom dac thi khong con phep chia
    nao — va do cung chinh la viec Unity lam khi bat alphaIsTransparency.
    """
    m = (alpha >= solid).astype(float)
    num = np.dstack([blur(colour[..., c] * m, radius) for c in range(3)])
    den = blur(m, radius)
    filled = num / np.maximum(den, 1e-4)[..., None]
    keep = m[..., None]
    return np.clip(colour * keep + filled * (1 - keep), 0, 255)


def blur(x, r):
    """Mo hop, chay hai lan cho muot. Tu viet bang numpy vi PIL khong nhan
    anh dang so thuc, va o day phai giu nguyen dai gia tri chu khong kep
    ve 0..255."""
    # Cong don PHAI them mot hang 0 o dau. Thieu no thi hieu hai lat cat
    # ra ngan hon dau vao dung mot pixel, va anh teo dan sau moi lan goi.
    k = 2 * r + 1
    for _ in range(2):
        for axis in (0, 1):
            pad = np.pad(x, [(r, r) if i == axis else (0, 0) for i in (0, 1)],
                         mode="edge")
            z = np.zeros([1 if i == axis else pad.shape[i] for i in (0, 1)])
            c = np.cumsum(np.concatenate([z, pad], axis=axis), axis=axis)
            lo = [slice(None)] * 2; hi = [slice(None)] * 2
            hi[axis] = slice(k, None); lo[axis] = slice(0, -k)
            x = (c[tuple(hi)] - c[tuple(lo)]) / k
    return x


def row_runs(mask, min_h=20):
    runs, s = [], None
    for i, v in enumerate(mask):
        if v and s is None:
            s = i
        elif not v and s is not None:
            if i - s > min_h:
                runs.append((s, i - 1))
            s = None
    if s is not None and len(mask) - s > min_h:
        runs.append((s, len(mask) - 1))
    return runs


def cap_width(alpha, thr=0.5):
    """Be rong phan TRANG TRI o hai dau, do bang bong cua hinh.

    Hai dau co nap go va dai kim loai phinh ra, CAO hon thanh ngang. Nen
    chieu cao cua cot la tin hieu hinh hoc sach — khong phu thuoc mau sac
    hay anh sang, khac voi cach do bang do lech mau."""
    h = (alpha > thr).sum(axis=0)
    w = len(h)
    body = np.median(h[w // 3: 2 * w // 3])
    tol = body * 1.02 + 1

    # Quet TU GIUA RA, khong phai tu mep vao. Mep ngoai cung bi bo tron nen
    # chieu cao o do NHO hon than — quet tu mep vao se dung ngay o cot dau
    # tien va bao "khong co nap", dung loi toi vua vap.
    left = next((x for x in range(w // 2, -1, -1) if h[x] > tol), -1) + 1
    right = next((x for x in range(w // 2, w) if h[x] > tol), w)
    return left, w - 1 - right + 1


META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 1
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMasterTextureLimit: 0
  ignorePngGamma: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 4096
  textureSettings:
    serializedVersion: 2
    filterMode: 2
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 100
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 9
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: {ppu}
  spriteBorder: {{x: {bx}, y: {by}, z: {bz}, w: {bw}}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignoreMipmapLimit: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 4096
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 0
    compressionQuality: 100
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    customData:
    physicsShape: []
    bones: []
    spriteID: 5e97eb03825dee720800000000000000
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    spriteCustomMetadata:
      entries: []
    nameFileIdTable: {{}}
  mipmapLimitGroupName:
  pSDRemoveMatte: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"""


def main():
    write = "--write" in sys.argv
    if not SRC.exists():
        print(f"Khong thay {SRC}")
        return 1

    rgb = np.asarray(Image.open(SRC).convert("RGB")).astype(float)
    corner = rgb[:8, :8].reshape(-1, 3)
    key = np.median(corner, axis=0)
    print(f"mau nen do duoc: {key.astype(int)}")

    colour, alpha = key_out(rgb, key)
    runs = row_runs((alpha > 0.5).any(axis=1))
    names = ["panel", "button", "button_down"]
    if len(runs) != len(names):
        print(f"Mong doi {len(names)} mieng, tim thay {len(runs)} — dung lai.")
        return 1

    # Hai nut dung CHUNG mot Image, chi doi sprite khi nhan. Khac chieu cao
    # hay khac spriteBorder la lúc nhan nut nhay co — nen ep ve cung khuon.
    cut = []
    for (y0, y1), name in zip(runs, names):
        A = alpha[y0:y1 + 1]
        cols = np.where((A > 0.5).any(axis=0))[0]
        x0, x1 = cols[0], cols[-1]
        A = A[:, x0:x1 + 1]
        C = colour[y0:y1 + 1, x0:x1 + 1]
        h, w = A.shape

        capL, capR = cap_width(A)
        cut.append([name, C, A, max(capL, capR)])
        print(f"{name}: {w}x{h}px  nap trai {capL} phai {capR}")

    # Gom hai nut ve cung chieu cao (dem o giua) va cung spriteBorder.
    btn = [c for c in cut if c[0].startswith("button")]
    bh = max(c[2].shape[0] for c in btn)
    bcap = max(c[3] for c in btn)
    for c in btn:
        pad = bh - c[2].shape[0]
        if pad:
            t, b = pad // 2, pad - pad // 2
            c[1] = np.pad(c[1], ((t, b), (0, 0), (0, 0)))
            c[2] = np.pad(c[2], ((t, b), (0, 0)))
        c[3] = bcap

    print()
    for name, C, A, cap in cut:
        h, w = A.shape
        ppu = REF_PPU * h / TARGET_H[name]
        unit = cap * REF_PPU / ppu
        print(f"{name:12s} {w}x{h}  PPU {ppu:7.2f}  cao {h * REF_PPU / ppu:5.1f} UI  "
              f"vien {cap} -> {unit:5.1f} UI moi ben  (be rong NHO NHAT dung duoc "
              f"{2 * unit:.0f})")
        if not write:
            continue
        OUT.mkdir(parents=True, exist_ok=True)
        Image.fromarray(np.dstack([C, A * 255]).astype(np.uint8), "RGBA").save(
            OUT / f"{name}.png")
        guid = hashlib.md5(f"spinsquad/ui/{name}".encode()).hexdigest()
        (OUT / f"{name}.png.meta").write_text(META.format(
            guid=guid, ppu=f"{ppu:.2f}", bx=cap, by=0, bz=cap, bw=0))

    print("\n" + ("da ghi" if write else "chua ghi — chay lai voi --write"))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
