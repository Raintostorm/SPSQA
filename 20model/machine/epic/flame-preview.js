 const channel=new FlameChannel(),nozzles=[[336,692],[739,693],[1123,693],[1484,694]],smoke=[];
 let clock=0,smokeClock=0,hitFlash=0;
 const phaseNames={idle:'Sẵn sàng',startup:'Khởi động',sustain:'Phun liên tục',recovery:'Ngắt lửa / hồi'};
 function robotX(){return width*.3;}
 function nozzle(size,c=frame){const p=poses[2][c],sc=size/referenceHeight;return{x:robotX()+(nozzles[c][0]-p.anchor_x)*sc,y:195+(nozzles[c][1]-p.anchor_y)*sc};}
 function target(size){const o=nozzle(size,1);return{x:o.x+size*(el('target').checked?.62:1.5),y:o.y,radius:size*.075};}
 function inCone(size){const o=nozzle(size,1),p=target(size),dx=p.x-o.x;return dx>=0&&dx<=size*1.05&&Math.abs(p.y-o.y)<=size*.02+dx*.18;}
 function flame(g,o,size,time){
  g.save();const length=size*1.05,gradient=g.createLinearGradient(o.x,o.y,o.x+length,o.y);
  gradient.addColorStop(0,color('--yellow'));gradient.addColorStop(.35,color('--orange'));gradient.addColorStop(.82,color('--orange'));gradient.addColorStop(1,'transparent');
  g.fillStyle=gradient;g.globalAlpha=.8;g.beginPath();g.moveTo(o.x,o.y);
  for(let i=1;i<=20;i++){const u=i/20,w=size*(.012+Math.pow(Math.sin(Math.PI*u),.75)*.17)*(1+.14*Math.sin(u*32-time*26));g.lineTo(o.x+length*u,o.y-w);}
  for(let i=20;i>=1;i--){const u=i/20,w=size*(.012+Math.pow(Math.sin(Math.PI*u),.75)*.17)*(1+.14*Math.sin(u*27-time*29+2));g.lineTo(o.x+length*u,o.y+w);}
  g.closePath();g.fill();g.globalAlpha=.85;g.fillStyle=color('--yellow');g.beginPath();g.moveTo(o.x,o.y-size*.015);
  for(let i=1;i<=16;i++){const u=i/16;g.lineTo(o.x+length*.78*u,o.y-size*.07*Math.sin(Math.PI*u)*(1+.2*Math.sin(u*30-time*31)));}
  g.lineTo(o.x+length*.78,o.y);for(let i=16;i>=1;i--){const u=i/16;g.lineTo(o.x+length*.78*u,o.y+size*.07*Math.sin(Math.PI*u)*(1+.2*Math.sin(u*28-time*28)));}g.closePath();g.fill();
  for(let i=0;i<8;i++){const u=(time*2.3+i*.127)%1;g.globalAlpha=.45*(1-u);g.fillStyle=color(i%2?'--yellow':'--orange');g.beginPath();g.ellipse(o.x+length*u,o.y+Math.sin(i*2.1+time*3)*size*.1*u,size*(.025+.045*u),size*(.012+.014*u),Math.sin(i)*.3,0,Math.PI*2);g.fill();}g.fillStyle=color('--orange');g.globalAlpha=.7;for(let i=0;i<9;i++){const u=(time*2.2+i*.113)%1;g.beginPath();g.arc(o.x+length*u,o.y+Math.sin(i*2.4+time*4)*size*.18*u,size*.009,0,Math.PI*2);g.fill();}g.restore();
 }
 function drawTarget(size){const p=target(size);ctx.save();ctx.strokeStyle=color(hitFlash>0?'--orange':'--muted-foreground');ctx.fillStyle=color('--muted');ctx.lineWidth=2;ctx.beginPath();ctx.arc(p.x,p.y,p.radius,0,Math.PI*2);ctx.fill();ctx.stroke();ctx.beginPath();ctx.moveTo(p.x,p.y+p.radius);ctx.lineTo(p.x,195);ctx.moveTo(p.x-size*.07,195);ctx.lineTo(p.x+size*.07,195);ctx.stroke();if(hitFlash>0){ctx.fillStyle=color('--foreground');ctx.font='500 '+getComputedStyle(root).fontSize+' sans-serif';ctx.textAlign='center';ctx.fillText('−10',p.x,p.y-p.radius-size*(.08+(.16-hitFlash)*.4));}ctx.restore();}
 el('target').onchange=()=>{channel.exposure=0;hitFlash=0;save();};
 el('stop').onclick=()=>{if(row()===2&&(channel.phase==='startup'||channel.phase==='sustain')){channel.stop();playing=true;last=null;el('play').textContent='Tạm dừng';}else{channel.cancel();pause();frame=3;}save();};
 el('retry').onclick=()=>{channel.revive();channel.resetTotals();hitFlash=0;smoke.length=0;el('state').value='2';channel.start();frame=0;elapsed=0;playing=true;last=null;el('play').textContent='Tạm dừng';board();save();};
 function draw(t){
  const r=row(),size=Number(el('size').value),dt=last!==null&&playing?Math.min((t-last)/1000,.1)*Number(el('speed').value):0;last=t;
  clock+=dt;hitFlash=Math.max(0,hitFlash-dt);
  if(playing&&r===2){const hits=channel.advance(dt,inCone(size));if(hits)hitFlash=.16;frame=channel.frame;if(channel.phase==='idle'){frame=3;pause();}}
  else if(playing){elapsed+=dt;if(r===1)travel+=dt*size*.8/seconds[1];while(elapsed+1e-9>=durations[r][frame]){elapsed=Math.max(0,elapsed-durations[r][frame]);if(r===3&&frame===3){pause();elapsed=0;break;}frame=(frame+1)%4;}}
  let x=robotX();if(r===1&&!el('still').checked)x=70+travel%Math.max(width-180,80);
  for(const p of smoke){p.age+=dt;p.x+=p.vx*dt;p.y+=p.vy*dt;}for(let i=smoke.length-1;i>=0;i--)if(smoke[i].age>.9)smoke.splice(i,1);
  if(r!==3&&dt>0){smokeClock+=dt;while(smokeClock>=.09){smokeClock-=.09;const p=poses[r][frame],sc=size/referenceHeight;smoke.push({x:x+(124+384*frame-p.anchor_x)*sc,y:195+(16+256*r-p.anchor_y)*sc,age:0,size,vx:-size*.09,vy:-size*.3});}}
  ctx.clearRect(0,0,width,250);ctx.fillStyle=color('--muted');ctx.fillRect(0,195,width,55);ctx.strokeStyle=color('--border');ctx.beginPath();ctx.moveTo(0,195);ctx.lineTo(width,195);ctx.stroke();
  for(const p of smoke){ctx.save();ctx.fillStyle=color('--muted-foreground');ctx.globalAlpha=.28*(1-p.age/.9);ctx.beginPath();ctx.arc(p.x,p.y,p.size*(.015+p.age*.045),0,Math.PI*2);ctx.fill();ctx.restore();}
  if(r===2){const emitting=channel.emitting||(!playing&&channel.phase==='idle'&&(frame===1||frame===2));if(emitting)flame(ctx,nozzle(size),size,clock);drawTarget(size);}
  if(image.complete&&image.naturalWidth)drawPose(ctx,r,frame,x,195,size/referenceHeight);
  const status=names[r]+' · frame '+(frame+1)+'/4'+(r===2?' · '+(playing||channel.phase!=='idle'?phaseNames[channel.phase]:'Tư thế xem thử'):'');
  if(status!==lastStatus){el('status').textContent=status;lastStatus=status;}
  el('combat-controls').hidden=r!==2;el('damage').hidden=r!==2;el('damage').textContent='40 DPS · 10 / 0,25 giây · Tick: '+channel.totalTicks+' · Tổng damage: '+channel.totalDamage+(channel.emitting&&inCone(size)?' · Tick tiếp: '+Math.max(0,.25-channel.exposure).toFixed(2)+' giây':'');
  board();requestAnimationFrame(draw);
 }
