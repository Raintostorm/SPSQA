#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_contents="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_contents/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-contribution-test.XXXXXX)
trap 'rm -rf "$test_output"' EXIT
refs=()
for dll in "$runtime"/shared/Microsoft.NETCore.App/8.0.16/*.dll; do refs+=("-r:$dll"); done
"$runtime/dotnet" "$unity_contents/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/ContributionTests.dll" "${refs[@]}" tools/tests/ContributionTests.cs Assets/_Project/Scripts/Battle/BattleContribution.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/ContributionTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/ContributionTests.dll"
