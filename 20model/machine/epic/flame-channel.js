(function(root){
  'use strict';
  class FlameChannel {
    constructor({dps=40,tickInterval=.25,startup=.35,recovery=.4}={}) {
      this.dps=dps;this.tickInterval=tickInterval;this.startup=startup;this.recovery=recovery;
      this.phase='idle';this.phaseTime=0;this.sustainTime=0;this.exposure=0;
      this.totalTicks=0;this.totalDamage=0;this.alive=true;
    }
    start(){if(!this.alive)return;this.phase='startup';this.phaseTime=0;this.sustainTime=0;this.exposure=0;}
    stop(){if(this.phase==='startup'||this.phase==='sustain'){this.phase='recovery';this.phaseTime=0;}this.exposure=0;}
    cancel(){this.phase='idle';this.phaseTime=0;this.exposure=0;}
    kill(){this.alive=false;this.cancel();}
    revive(){this.alive=true;this.cancel();}
    resetTotals(){this.totalTicks=0;this.totalDamage=0;}
    get emitting(){return this.alive&&this.phase==='sustain';}
    get frame(){return this.phase==='startup'?0:this.phase==='sustain'?1+Math.floor((this.sustainTime+1e-9)/.18)%2:this.phase==='recovery'?3:0;}
    advance(dt,inRange){
      if(!Number.isFinite(dt)||dt<0)throw new Error('dt must be finite and nonnegative');
      if(!this.alive)return 0;
      let remaining=dt,hits=0;
      if(this.phase==='startup'){
        const consumed=Math.min(remaining,Math.max(0,this.startup-this.phaseTime));
        this.phaseTime+=consumed;remaining-=consumed;
        if(this.phaseTime+1e-9>=this.startup){this.phase='sustain';this.phaseTime=0;}
      }
      if(this.phase==='sustain'){
        this.sustainTime+=remaining;
        if(inRange){
          this.exposure+=remaining;
          hits=Math.floor((this.exposure+1e-9)/this.tickInterval);
          this.exposure=Math.max(0,this.exposure-hits*this.tickInterval);
          this.totalTicks+=hits;this.totalDamage+=hits*this.dps*this.tickInterval;
        }else this.exposure=0;
      }else if(this.phase==='recovery'){
        this.phaseTime+=remaining;
        if(this.phaseTime+1e-9>=this.recovery){this.phase='idle';this.phaseTime=0;}
      }
      return hits;
    }
  }
  if(typeof module!=='undefined'&&module.exports)module.exports=FlameChannel;
  else root.FlameChannel=FlameChannel;
})(typeof globalThis!=='undefined'?globalThis:this);
