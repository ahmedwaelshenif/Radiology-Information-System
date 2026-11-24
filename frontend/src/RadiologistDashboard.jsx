import React,{useEffect,useState} from 'react';
import {ArrowRight,CheckCircle2,ChevronRight,ClipboardList,Clock3,FileText,Microscope,Users,XCircle} from 'lucide-react';
import {icons} from './config/apiCatalog.js';

const actions=[['study-search','Review studies'],['medical-create','Add clinical note'],['plan-create','Create treatment plan']];
const orderStatuses=[{status:0,label:'Scheduled',icon:Clock3,tone:'scheduled'},{status:1,label:'Completed',icon:CheckCircle2,tone:'completed'},{status:2,label:'Cancelled',icon:XCircle,tone:'cancelled'}];
const rowsFrom=result=>result?.items||result?.Items||(Array.isArray(result)?result:[]);
const localDateKey=date=>`${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;

export default function RadiologistDashboard({user,onNavigate,onAction,available,request}){
 const [stats,setStats]=useState([]),[counts,setCounts]=useState([]),[attention,setAttention]=useState([]),[week,setWeek]=useState([]);
 const [loading,setLoading]=useState(true),[attentionLoading,setAttentionLoading]=useState(true),[weekLoading,setWeekLoading]=useState(true);
 const firstName=(user?.fullName||'there').split(' ')[0];

 useEffect(()=>{
  let alive=true;
  const metrics=[
   ['study-count','Pending studies','/RadiologyStudy/Count-By-Status?status=0'],
   ['study-count-awaiting','Awaiting reports','/RadiologyStudy/Count-By-Status?status=1'],
   ['medical-search','Clinical records','/MedicalRecord/search?PageNumber=1&PageSize=1']
  ];
  Promise.all(metrics.map(async([key,label,path])=>{
   try{const value=await request(path);return {key,label,value:typeof value==='number'?value:value?.totalCount??value?.TotalCount??value?.items?.length??value?.Items?.length??'—'}}
   catch{return {key,label,value:'—'}}
  })).then(values=>{if(alive){setStats(values);setLoading(false)}});
  Promise.all(orderStatuses.map(async item=>{
   try{return {...item,value:Number(await request(`/RadiologyOrder/Count-By-Status?status=${item.status}`))||0}}
   catch{return {...item,value:'—'}}
  })).then(values=>alive&&setCounts(values));
  Promise.all([0,1].map(async status=>{
   try{const result=await request(`/RadiologyStudy/search?Status=${status}&PageNumber=1&PageSize=4`);return rowsFrom(result).map(row=>({...row,queueStatus:status}))}
   catch{return []}
  })).then(lists=>{if(alive){setAttention(lists.flat().sort((a,b)=>b.queueStatus-a.queueStatus).slice(0,5));setAttentionLoading(false)}});
  const today=new Date();
  // Use a two-week window so recently seeded and real completed studies appear.
  const days=Array.from({length:14},(_,index)=>{const date=new Date(today);date.setDate(today.getDate()-13+index);return {date,key:localDateKey(date),label:new Intl.DateTimeFormat('en',{weekday:'short',day:'numeric'}).format(date),count:0}});
  const from=days[0].key,to=days[6].key;
  request(`/RadiologyStudy/search?Status=2&StudyDateFrom=${from}&StudyDateTo=${to}&PageNumber=1&PageSize=100`).then(result=>{
   if(!alive)return;const countByDate=new Map();rowsFrom(result).forEach(row=>{const key=String(row.studyDate??row.StudyDate??'').slice(0,10);if(key)countByDate.set(key,(countByDate.get(key)||0)+1)});setWeek(days.map(day=>({...day,count:countByDate.get(day.key)||0})));setWeekLoading(false)
  }).catch(()=>{if(alive){setWeek(days);setWeekLoading(false)}});
  return()=>{alive=false};
 },[request]);

 const destinations=['Studies','Studies','Medical records'];
 const actionButton=([key,label])=>{
  const endpoint=available.find(item=>item.key===key);
  if(!endpoint)return null;
  const Glyph=icons[endpoint.group]||ArrowRight;
  return <button key={key} className="role-action" onClick={()=>key==='study-search'?onNavigate('Studies'):onAction(endpoint)}><span className="role-action-icon"><Glyph size={18}/></span><span>{label}</span><ArrowRight size={15}/></button>;
 };
 const maxWeekCount=Math.max(1,...week.map(day=>day.count));
 const totalWeek=week.reduce((sum,day)=>sum+day.count,0);

 return <div className="role-dashboard dashboard-radiologist">
  <section className="dashboard-hero"><div className="dashboard-hero-copy"><span className="eyebrow">RADIOLOGY WORKSPACE</span><h1>Good day, {firstName}</h1><p>Review imaging studies, complete diagnostic reports, and coordinate patient care.</p></div><div className="dashboard-hero-mark"><Microscope size={28}/></div></section>
  <section className="dashboard-metrics">{stats.map((item,i)=>{const Glyph=[ClipboardList,FileText,Users][i];return <button key={item.key} type="button" className="dashboard-metric is-clickable" onClick={()=>onNavigate(destinations[i])}><span className={`metric-icon metric-${i}`}><Glyph size={18}/></span><span className="metric-label">{item.label}</span><strong>{loading?'—':item.value??'—'}</strong><small>In your clinical workflow</small><ChevronRight size={16} className="metric-arrow"/></button>})}</section>
  <div className="dashboard-columns">
   <section className="dashboard-panel dashboard-actions"><div className="dashboard-panel-heading"><div><span className="eyebrow">CLINICAL WORKFLOW</span><h2>Clinical actions</h2><p>Move from study review to reporting and care coordination.</p></div></div><div className="role-action-list">{actions.map(actionButton)}</div></section>
   <section className="dashboard-panel dashboard-order-counts"><div className="dashboard-panel-heading"><div><span className="eyebrow">IMAGING WORKFLOW</span><h2>Orders by status</h2><p>Current order totals across the imaging service.</p></div><button className="text-btn" onClick={()=>onNavigate('Orders')}>View orders <ArrowRight size={14}/></button></div><div className="order-count-cards">{counts.map(item=>{const Glyph=item.icon;return <article className={`order-count-card order-count-${item.tone}`} key={item.status}><span className="order-count-icon"><Glyph size={19}/></span><span className="order-count-label">{item.label}</span><strong>{item.value??'—'}</strong><small>Orders</small></article>})}</div></section>
  </div>
  <div className="dashboard-columns dashboard-insights">
   <section className="dashboard-panel dashboard-attention"><div className="dashboard-panel-heading"><div><span className="eyebrow">PRIORITY QUEUE</span><h2>Needs attention</h2><p>Studies ready for review or reporting.</p></div><button className="text-btn" onClick={()=>onNavigate('Studies')}>Open studies <ArrowRight size={14}/></button></div>
    {attentionLoading?<div className="dashboard-insight-loading">Loading study queue…</div>:attention.length?<div className="attention-list">{attention.map((study,index)=>{const status=study.queueStatus===1?'Awaiting report':'Pending review';const icon=study.queueStatus===1?FileText:Clock3;const Glyph=icon;const id=study.id??study.Id??study.studyId;return <button className="attention-row" key={id??index} onClick={()=>onNavigate('Studies')}><span className={`attention-icon ${study.queueStatus===1?'awaiting':''}`}><Glyph size={17}/></span><span className="attention-copy"><b>{study.patientName??study.PatientName??`Patient #${study.patientId??study.PatientId??'—'}`}</b><small>Order #{study.orderId??study.OrderId??'—'} · {status}</small></span><span className="attention-date">{String(study.studyDate??study.StudyDate??'').slice(0,10)||'Date pending'}</span><ChevronRight size={16}/></button>})}</div>:<div className="dashboard-empty dashboard-queue-empty"><span className="attention-icon done"><CheckCircle2 size={18}/></span><b>All caught up</b><span>No studies currently need attention.</span></div>}
   </section>
   <section className="dashboard-panel dashboard-weekly"><div className="dashboard-panel-heading"><div><span className="eyebrow">LAST 14 DAYS</span><h2>Completed studies</h2><p>Daily completed study volume.</p></div><div className="weekly-total"><strong>{weekLoading?'—':totalWeek}</strong><small>last 14 days</small></div></div>
    <div className="weekly-chart" aria-label={`${totalWeek} completed studies in the last seven days`}>{week.map(day=><div className="weekly-day" key={day.key}><b>{weekLoading?'':day.count||''}</b><span className="weekly-bar-track"><span className="weekly-bar" style={{height:`${Math.max(day.count?12:4,day.count/maxWeekCount*100)}%`}}/></span><small>{day.label}</small></div>)}</div>
    <button className="weekly-link" onClick={()=>onNavigate('Studies')}>View studies <ArrowRight size={14}/></button>
   </section>
  </div>
 </div>;
}
