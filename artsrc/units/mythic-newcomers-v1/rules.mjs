// Independent demo mechanics; no Unity changes.
export function timeModifiers(unit,field,now){const active=now>=field.start&&now<field.end&&unit.team==='enemy'&&unit.hp>0&&Math.hypot(unit.x-field.x,unit.y-field.y)<=field.radius;return {move:active?0:1,speed:active?0:1}}
export function devour(target,caster,atk,mult=3,healRatio=.5){if(!target||target.team===caster.team||target.hp<=0)return {damage:0,heal:0,pull:false};const damage=Math.min(target.hp,atk*mult*(1-(target.reduction||0)));target.hp-=damage;const heal=Math.min(caster.maxHp-caster.hp,damage*healRatio);caster.hp+=heal;return {damage,heal,pull:target.tier==='common'||target.tier==='elite'}}
export class Dash {constructor(){this.hit=new Set()}strike(enemies,from,to,width,atk){const result=[];for(const u of enemies){if(u.hp<=0||u.team!=='enemy'||this.hit.has(u.id))continue;const vx=to.x-from.x,vy=to.y-from.y,len=vx*vx+vy*vy,t=len?Math.max(0,Math.min(1,((u.x-from.x)*vx+(u.y-from.y)*vy)/len)):0,d=Math.hypot(u.x-from.x-vx*t,u.y-from.y-vy*t);if(d>width)continue;this.hit.add(u.id);const damage=Math.min(u.hp,atk*(1-(u.reduction||0)));u.hp-=damage;result.push({id:u.id,damage})}return result}}

export class MimicCoins {
 constructor(){this.wave=1;this.coins=0;this.pending=0;this.casts=new Set()}
 cast(id){if(this.casts.has(id))return false;this.casts.add(id);this.pending+=5;return true}
 nextWave(){const grant=this.pending;this.pending=0;this.wave++;this.coins+=grant;return grant}
}
