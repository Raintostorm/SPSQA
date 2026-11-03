// Demo rules only; Unity battle is not modified.
export class Revival {
 constructor(){this.converted=new Set();this.used=new Set();this.processed=new Set()}
 die(unit,undeadCount=1){
  if(unit.hp>0)return 'alive';
  const event=unit.id+':'+unit.deaths;if(this.processed.has(event))return 'duplicate';this.processed.add(event);
  if(undeadCount<=0)return 'final';
  if(unit.team==='enemy'){
   if(unit.tier==='boss'||!['common','elite'].includes(unit.tier)||this.converted.has(unit.id))return 'final';
   this.converted.add(unit.id);unit.team='ally';unit.hp=unit.maxHp;return 'converted';
  }
  if(this.used.has(unit.id))return 'final';
  this.used.add(unit.id);unit.skull=true;unit.hp=unit.maxHp;return 'revived';
 }
}
export class Aura {
 constructor(atk=100,mana=100){this.atk=atk;this.mana=mana;this.active=mana>0;this.clock=0;this.damage=0}
 gainMana(amount){if(!this.active)this.mana=Math.min(100,this.mana+amount)}
 step(dt,targets,self){if(!this.active)return;const used=Math.min(dt,this.mana/15);this.mana=Math.max(0,this.mana-15*used);this.clock+=used;while(this.clock>=1-1e-8){this.clock-=1;let damage=0;for(const t of targets)if(t.inRange&&t.hp>0){const d=Math.min(t.hp,this.atk*.2*(1-(t.reduction||0)));t.hp-=d;damage+=d}self.hp=Math.min(self.maxHp,self.hp+damage*.5);this.damage+=damage}if(this.mana<1e-8){this.mana=0;this.active=false}}
}
export function chain(primary,enemies,maxJumps=2,radius=120){if(!primary||primary.hp<=0)return [];const hits=[primary];let last=primary;for(let i=0;i<maxJumps;i++){const next=enemies.filter(e=>e.hp>0&&!hits.includes(e)&&Math.hypot(e.x-last.x,e.y-last.y)<=radius).sort((a,b)=>Math.hypot(a.x-last.x,a.y-last.y)-Math.hypot(b.x-last.x,b.y-last.y))[0];if(!next)break;hits.push(next);last=next}return hits}
export function elfStats(base,bonusAtk,bonusSpeed){return {atk:base.atk*(1+bonusAtk),speed:base.speed*(1+bonusSpeed)}}
export function shield(unit,ratio=.25){unit.shield=Math.max(unit.shield||0,unit.maxHp*ratio)}
