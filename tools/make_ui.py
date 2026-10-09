#!/usr/bin/env python3
"""[DA NGHI HUU] Ve sprite tam cho HUD — panel, button, button_down.

!!! KHONG CHAY NUA. Ba file do gio la art AI, nhap bang
    tools/import_ui_art.py. Chay lai cai nay la GHI DE MAT.

Giu lai de doc lai cach tinh vien 9 manh, va de co ban ve tam neu can lam
lai tu dau. Muon chay that thi them --force.

Ve sprite cho HUD tran dau, kem file .meta nhap san.

Vi sao khong nho AI sinh: khung va nut la hinh chu nhat bo goc co vien —
thu ma code ve dung tung pixel. Anh AI tra ve thi sai kich thuoc, hay dinh
san chu ben trong, va nen khong trong suot. Nho AI chi dang cong voi ICON
(dong tien, thanh kiem, giap) va KHUNG CHAM TRO.

Ba anh deu ve kieu 9 MANH (9-slice): Unity giu nguyen bon goc, chi keo gian
phan giua. Nho vay mot anh 128x128 dung duoc cho ca nut nho lan khung dai,
vien khong bao gio bi beo ra.

    spriteBorder {x,y,z,w} = trai, duoi, phai, tren  (don vi pixel)

Hai rang buoc ve so:

  RADIUS < BORDER        khong thi goc bo bi cat lam doi khi keo gian
  be nho nhat >= 2*BORDER  khong thi bon goc chong len nhau

Rang buoc thu hai de quen. Voi BORDER 36 thi khong khung nao duoc thap hon
72px — da vap: o dem quan dat cao 64px, bon goc dam vao nhau. Ha ve 26 nen
gio nguong la 52px, con tho de dat.

Chay:  python3 tools/make_ui.py
"""

import hashlib
from pathlib import Path

from PIL import Image, ImageDraw

OUT = Path("Assets/_Project/Art/UI")
SIZE = 128
BORDER = 26          # be rong vung "goc" khong bi keo gian
RADIUS = 20          # < BORDER, xem ghi chu o tren
INK_W = 5
SS = 4               # ve gap 4 roi thu nho -> goc bo min

# (ten, mau nen RGBA, mau vien RGBA)
SPRITES = {
    "panel":       ((26, 30, 38, 214), (12, 14, 20, 255)),
    "button":      ((214, 158, 68, 255), (48, 32, 14, 255)),
    "button_down": ((158, 112, 44, 255), (48, 32, 14, 255)),
}


def draw(fill, ink):
    im = Image.new("RGBA", (SIZE * SS, SIZE * SS), (0, 0, 0, 0))
    d = ImageDraw.Draw(im)
    d.rounded_rectangle([0, 0, SIZE * SS - 1, SIZE * SS - 1],
                        radius=RADIUS * SS, fill=ink)
    k = INK_W * SS
    d.rounded_rectangle([k, k, SIZE * SS - 1 - k, SIZE * SS - 1 - k],
                        radius=(RADIUS - INK_W) * SS, fill=fill)
    return im.resize((SIZE, SIZE), Image.LANCZOS)


def meta(name):
    """GUID suy tu TEN, khong sinh ngau nhien.

    Ly do la loi A2 trong Handoff/11: GUID nam o .meta, mat no la moi tham
    chieu trong scene va prefab chet im lang. Suy tu ten thi chay lai script
    bao nhieu lan GUID van the."""
    guid = hashlib.md5(f"spinsquad/ui/{name}".encode()).hexdigest()
    return f"""fileFormatVersion: 2
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
  maxTextureSize: 2048
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
  spritePixelsToUnits: 100
  spriteBorder: {{x: {BORDER}, y: {BORDER}, z: {BORDER}, w: {BORDER}}}
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
    maxTextureSize: 2048
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
    import sys
    if "--force" not in sys.argv:
        print(__doc__.strip().split("\n\n")[1])
        return 1
    OUT.mkdir(parents=True, exist_ok=True)
    for name, (fill, ink) in SPRITES.items():
        draw(fill, ink).save(OUT / f"{name}.png")
        m = OUT / f"{name}.png.meta"
        # Ghi de .meta cung duoc vi GUID suy tu ten — van dung GUID cu.
        m.write_text(meta(name))
        print(f"  {name}.png  {SIZE}x{SIZE}  vien 9-manh {BORDER}px")
    print(f"=> khung nho nhat ve duoc: {2 * BORDER}px moi chieu")


if __name__ == "__main__":
    raise SystemExit(main() or 0)
