// Preview only: reads production Idle clips without changing game assets or saves.
import fs from 'node:fs';
import path from 'node:path';
const root = process.cwd();
const version = process.argv[2] || 'v2';
if (!/^v\d+$/.test(version)) throw Error('Expected a version such as v2 or v3');
const dest = path.join(root, `artsrc/ui/ally-stage-demo-${version}`);
fs.mkdirSync(dest, {recursive:true});
const races = ['human','elf','dead','monster','machine'];
const files = fs.readdirSync(path.join(root,'Assets/_Project/Art'), {recursive:true}).filter(x=>x.endsWith('.png.meta'));
const guids = new Map(files.map(x=>{
  const p = path.join(root,'Assets/_Project/Art',x);
  return [fs.readFileSync(p,'utf8').match(/^guid: (\w+)/m)?.[1],p.slice(0,-5)];
}));
const data = races.map(race=>{
  const unit=fs.readFileSync(`Assets/_Project/Data/Units/Unit_${race}_common.asset`,'utf8');
  const clip=fs.readFileSync(`Assets/_Project/Data/Units/Anim_${race}_common.asset`,'utf8').split('  Idle:')[1].split('  Walk:')[0];
  const frames=[...clip.split('    Durations:')[0].matchAll(/guid: (\w+)/g)].map(m=>{
    const p=guids.get(m[1]); if(!p)throw Error(`Missing frame ${m[1]}`);
    const meta=fs.readFileSync(p+'.meta','utf8');
    const pivot=meta.match(/spritePivot: \{x: ([\d.-]+), y: ([\d.-]+)/);
    return {src:path.relative(dest,p),pivot:[+pivot[1],+pivot[2]],ppu:+meta.match(/spritePixelsToUnits: ([\d.]+)/)[1]};
  });
  const durations=[...clip.split('    Durations:')[1].split('    Sequence:')[0].matchAll(/- ([\d.]+)/g)].map(m=>+m[1]);
  const get=k=>unit.match(new RegExp(`^  ${k}: (.+)$`,'m'))?.[1].replaceAll('"','');
  return {race,name:get('DisplayName'),hp:+get('BaseHp'),atk:+get('BaseAttack'),range:+get('AttackRange'),cooldown:+get('AttackCooldown'),ranged:get('IsRanged')==='1',frames,durations};
});
const template=fs.readFileSync('tools/ally_stage_demo.template.html','utf8');
fs.writeFileSync(path.join(dest,'index.html'), template.replace('/*UNIT_DATA*/',JSON.stringify(data)));
console.log(`Demo: ${dest}/index.html`);
console.log(data.map(x=>`${x.race}: ${x.frames.length} original Idle frames, ${x.durations.reduce((a,b)=>a+b,0).toFixed(2)}s`).join('\n'));
