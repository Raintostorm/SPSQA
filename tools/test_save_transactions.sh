#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_contents="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_contents/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-save-test.XXXXXX)
refs=()
for dll in "$runtime"/shared/Microsoft.NETCore.App/8.0.16/*.dll; do refs+=("-r:$dll"); done
"$runtime/dotnet" "$unity_contents/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/SaveTests.dll" "${refs[@]}" tools/tests/SaveTransactionTests.cs Assets/_Project/Scripts/Meta/SaveSystem.cs Assets/_Project/Scripts/Meta/SaveData.cs Assets/_Project/Scripts/Meta/GameProgress.cs Assets/_Project/Scripts/Meta/TreasureRoll.cs Assets/_Project/Scripts/Data/TreasureItem.cs Assets/_Project/Scripts/Data/Rank.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/SaveTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/SaveTests.dll"
