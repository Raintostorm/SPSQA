#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_runtime="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_runtime/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-ally-stats.XXXXXX)
trap 'rm -rf "$test_output"' EXIT
refs=()
for reference in "$runtime"/shared/Microsoft.NETCore.App/8.*/*.dll; do refs+=("-r:$reference"); done
"$runtime/dotnet" "$unity_runtime/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/AllyStatPreviewTests.dll" "${refs[@]}" tools/tests/AllyStatPreviewTests.cs Assets/_Project/Scripts/Data/Rank.cs Assets/_Project/Scripts/Data/MythicKind.cs Assets/_Project/Scripts/Meta/AllyStatPreview.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/AllyStatPreviewTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/AllyStatPreviewTests.dll"
