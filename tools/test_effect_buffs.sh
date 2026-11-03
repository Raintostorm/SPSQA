#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")/.."
unity_contents="/Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents"
runtime="$unity_contents/Resources/Scripting/NetCoreRuntime"
test_output=$(mktemp -d /tmp/spinsquad-effect-buff-test.XXXXXX)
refs=()
for dll in "$runtime"/shared/Microsoft.NETCore.App/8.0.16/*.dll; do refs+=("-r:$dll"); done
"$runtime/dotnet" "$unity_contents/Resources/Scripting/DotNetSdkRoslyn/csc.dll" -nologo -nostdlib -target:exe "-out:$test_output/EffectBuffRulesTests.dll" "${refs[@]}" tools/tests/EffectBuffRulesTests.cs Assets/_Project/Scripts/Battle/BurnClock.cs Assets/_Project/Scripts/Battle/HarvestGrowth.cs Assets/_Project/Scripts/Battle/EffectBuffCatalog.cs Assets/_Project/Scripts/Battle/EffectBuffMath.cs Assets/_Project/Scripts/Battle/EffectBuffScaling.cs Assets/_Project/Scripts/Battle/EffectBuffPlacement.cs Assets/_Project/Scripts/Battle/GroundBuffCells.cs
cp tools/tests/SummonTests.runtimeconfig.json "$test_output/EffectBuffRulesTests.runtimeconfig.json"
"$runtime/dotnet" "$test_output/EffectBuffRulesTests.dll"
