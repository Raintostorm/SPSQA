#!/bin/bash
# Bien dich kiem script C# MA KHONG CAN TAT UNITY.
#
# Unity giu Temp/UnityLockfile nen khong chay duoc `Unity -batchmode`.
# Script nay goi thang Roslyn (trinh bien dich C#) di kem Unity, nen chay
# luc nao cung duoc. No CHI bao loi cu phap va loi kieu — khong thay the
# viec bam vao Unity de no reimport that su.
#
#   ./tools/csharp_check.sh
#
set -u
cd "$(dirname "$0")/.."

VER=$(grep -o 'm_EditorVersion: .*' ProjectSettings/ProjectVersion.txt | cut -d' ' -f2)
U="/Applications/Unity/Hub/Editor/$VER/Unity.app/Contents"
[ -d "$U" ] || { echo "Khong tim thay Unity $VER"; exit 1; }

DOTNET="$U/Resources/Scripting/NetCoreRuntime/dotnet"
CSC="$U/Resources/Scripting/DotNetSdkRoslyn/csc.dll"
MAN="$U/Resources/Scripting/Managed/UnityEngine"
API="$U/Resources/Scripting/UnityReferenceAssemblies/unity-4.8-api"

RSP=$(mktemp /tmp/spinsquad-csc.XXXXXX.rsp)
OUT=$(mktemp -d /tmp/spinsquad-csc.XXXXXX)
trap 'rm -rf "$RSP" "$OUT"' EXIT

{
  echo "-nologo"
  echo "-target:library"
  echo "-out:$OUT/check.dll"
  for d in "$API"/*.dll "$API"/Facades/*.dll "$MAN"/*.dll; do
    [ -f "$d" ] && echo "-r:$d"
  done
  # DLL cua package (InputSystem, UnityEngine.UI...) — co thi dung.
  #
  # Glob phai la *.dll chu khong phai Unity.*.dll: "Unity.*" doi dau cham
  # ngay sau chu Unity nen no TRUOT UnityEngine.UI.dll, va moi thu dung
  # uGUI bao loi "namespace UI does not exist" du Unity that van bien dich
  # duoc. Bo Assembly-CSharp* ra vi do la ban bien dich cua chinh cac file
  # dang kiem — nap vao la bao trung dinh nghia kieu.
  for d in Library/ScriptAssemblies/*.dll; do
    case "$(basename "$d")" in Assembly-CSharp*) continue;; esac
    [ -f "$d" ] && echo "-r:$PWD/$d"
  done
  # SpineUnitVisual can DLL cua Spine runtime, bo ra cho gon
  find Assets/_Project/Scripts -name '*.cs' ! -name 'SpineUnitVisual.cs' \
    | sed "s|^|$PWD/|"
} > "$RSP"

echo "Nguon: $(grep -c '\.cs$' "$RSP") file"
echo

ERR=$("$DOTNET" "$CSC" "@$RSP" 2>&1 | grep ': error ')
if [ -n "$ERR" ]; then
  echo "$ERR"
  echo
  echo "=> CO LOI"
  exit 1
fi
echo "=> Bien dich sach, khong loi."
