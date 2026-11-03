#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_contents="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_contents/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-discovery-test.XXXXXX)
refs=()
for dll in "$runtime"/shared/Microsoft.NETCore.App/8.0.16/*.dll; do refs+=("-r:$dll"); done
"$runtime/dotnet" "$unity_contents/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/MonsterDiscoveryTests.dll" "${refs[@]}" tools/tests/MonsterDiscoveryTests.cs Assets/_Project/Scripts/Meta/MonsterDiscovery.cs Assets/_Project/Scripts/Meta/SaveData.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/MonsterDiscoveryTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/MonsterDiscoveryTests.dll"
