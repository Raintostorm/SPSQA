#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_contents="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_contents/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-stage-buff-test.XXXXXX)
refs=()
for dll in "$runtime"/shared/Microsoft.NETCore.App/8.0.16/*.dll; do refs+=("-r:$dll"); done
"$runtime/dotnet" "$unity_contents/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/StageBuffRollTests.dll" "${refs[@]}" tools/tests/StageBuffRollTests.cs Assets/_Project/Scripts/Gacha/StageRollPreview.cs Assets/_Project/Scripts/Gacha/RollCellKind.cs Assets/_Project/Scripts/Gacha/RollPayout.cs Assets/_Project/Scripts/Gacha/RollResolver.cs Assets/_Project/Scripts/Gacha/BuffStatKind.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/StageBuffRollTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/StageBuffRollTests.dll"
