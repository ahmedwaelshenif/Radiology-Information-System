import React,{useEffect,useState} from 'react';
import {ArrowRight,CalendarDays,CheckCircle2,ChevronRight,ClipboardList,Clock3,FileText,Microscope,Users,XCircle} from 'lucide-react';
import {icons} from './config/apiCatalog.js';
import {fetchOrderCountsForDate} from './orderCounts.js';

const actions=[['study-search','Review studies'],['medical-create','Add clinical note'],['plan-create','Create treatment plan']];
const orderStatuses=[{status:0,label:'Scheduled',icon:Clock3,tone:'scheduled'},{status:1,label:'Completed',icon:CheckCircle2,tone:'completed'},{status:2,label:'Cancelled',icon:XCircle,tone:'cancelled'}];
const rowsFrom=result=>result?.items||result?.Items||(Array.isArray(result)?result:[]);
const totalFrom=result=>Number(result?.totalCount??result?.TotalCount??result?.totalItems??result?.TotalItems??rowsFrom(result).length)||0;
const localDateKey=date=>`${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;

export default function RadiologistDashboard({user,onNavigate,onAction,available,request}){
 const [stats,setStats]=useState([]),[counts,setCounts]=useState([]),[selectedOrderDate,setSelectedOrderDate]=useState(()=>localDateKey(new Date())),[attention,setAttention]=useState([]),[week,setWeek]=useState([]);
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
  Promise.all([0,1].map(async status=>{
   try{const result=await request(`/RadiologyStudy/search?Status=${status}&PageNumber=1&PageSize=4`);return rowsFrom(result).map(row=>({...row,queueStatus:status}))}
   catch{return []}
  })).then(lists=>{if(alive){setAttention(lists.flat().sort((a,b)=>b.queueStatus-a.queueStatus).slice(0,5));setAttentionLoading(false)}});
  const today=new Date();
  // Use a two-week window so recently seeded and real completed studies appear.
  const days=Array.from({length:14},(_,index)=>{const date=new Date(today);date.setDate(today.getDate()-13+index);return {date,key:localDateKey(date),label:new Intl.DateTimeFormat('en',{weekday:'short',day:'numeric'}).format(date),count:0}});
  const from=days[0].key,to=days[days.length-1].key;
  const loadCompletedStudies=async()=>{
   const pageSize=100;const firstQuery=new URLSearchParams({Status:'2',StudyDateFrom:from,StudyDateTo:to,PageNumber:'1',PageSize:String(pageSize)});
   const first=await request('/RadiologyStudy/search?'+firstQuery.toString());const records=rowsFrom(first);const pageCount=Math.ceil(totalFrom(first)/pageSize);
   if(pageCount<=1)return records;
   const remaining=await Promise.all(Array.from({length:pageCount-1},(_,index)=>{const query=new URLSearchParams({Status:'2',StudyDateFrom:from,StudyDateTo:to,PageNumber:String(index+2),PageSize:String(pageSize)});return request('/RadiologyStudy/search?'+query.toString()).then(rowsFrom)}));
   return records.concat(...remaining);
  };
  loadCompletedStudies().then(records=>{
   if(!alive)return;const countByDate=new Map();records.forEach(row=>{const key=String(row.studyDate??row.StudyDate??'').slice(0,10);if(key)countByDate.set(key,(countByDate.get(key)||0)+1)});setWeek(days.map(day=>({...day,count:countByDate.get(day.key)||0})));setWeekLoading(false)
  }).catch(()=>{if(alive){setWeek(days);setWeekLoading(false)}});
  return()=>{alive=false};
 },[request]);

 useEffect(()=>{
  let alive=true;setCounts([]);
  fetchOrderCountsForDate(request,selectedOrderDate).then(values=>{if(alive)setCounts(orderStatuses.map(item=>({...item,value:values[item.label]})))}).catch(()=>{if(alive)setCounts(orderStatuses.map(item=>({...item,value:'—'})))});
  return()=>{alive=false};
 },[selectedOrderDate,request]);

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
   <section className="dashboard-panel dashboard-order-counts"><div className="dashboard-panel-heading"><div><span className="eyebrow">ORDERS BY DATE</span><h2>Orders scheduled on {new Date(`${selectedOrderDate}T00:00:00`).toLocaleDateString(undefined,{dateStyle:'medium'})}</h2><p>Order totals by status for the selected date.</p></div><div className="order-count-controls"><label className="dashboard-date-filter"><CalendarDays size={16}/><span>Date</span><input type="date" value={selectedOrderDate} onChange={event=>event.target.value&&setSelectedOrderDate(event.target.value)}/></label><button className="text-btn" onClick={()=>onNavigate('Orders')}>View orders <ArrowRight size={14}/></button></div></div><div className="order-count-cards">{counts.map(item=>{const Glyph=item.icon;return <article className={`order-count-card order-count-${item.tone}`} key={item.status}><span className="order-count-icon"><Glyph size={19}/></span><span className="order-count-label">{item.label}</span><strong>{item.value??'—'}</strong><small>{selectedOrderDate===localDateKey(new Date())?'Today':new Date(`${selectedOrderDate}T00:00:00`).toLocaleDateString(undefined,{month:'short',day:'numeric'})}</small></article>})}</div></section>
  </div>
  <div className="dashboard-columns dashboard-insights">
   <section className="dashboard-panel dashboard-attention"><div className="dashboard-panel-heading"><div><span className="eyebrow">PRIORITY QUEUE</span><h2>Needs attention</h2><p>Studies ready for review or reporting.</p></div><button className="text-btn" onClick={()=>onNavigate('Studies')}>Open studies <ArrowRight size={14}/></button></div>
    {attentionLoading?<div className="dashboard-insight-loading">Loading study queue…</div>:attention.length?<div className="attention-list">{attention.map((study,index)=>{const status=study.queueStatus===1?'Awaiting report':'Pending review';const icon=study.queueStatus===1?FileText:Clock3;const Glyph=icon;const id=study.id??study.Id??study.studyId;return <button className="attention-row" key={id??index} onClick={()=>onNavigate('Studies')}><span className={`attention-icon ${study.queueStatus===1?'awaiting':''}`}><Glyph size={17}/></span><span className="attention-copy"><b>{study.patientName??study.PatientName??`Patient #${study.patientId??study.PatientId??'—'}`}</b><small>Order #{study.orderId??study.OrderId??'—'} · {status}</small></span><span className="attention-date">{String(study.studyDate??study.StudyDate??'').slice(0,10)||'Date pending'}</span><ChevronRight size={16}/></button>})}</div>:<div className="dashboard-empty dashboard-queue-empty"><span className="attention-icon done"><CheckCircle2 size={18}/></span><b>All caught up</b><span>No studies currently need attention.</span></div>}
   </section>
   <section className="dashboard-panel dashboard-weekly"><div className="dashboard-panel-heading"><div><span className="eyebrow">LAST 14 DAYS</span><h2>Completed studies</h2><p>Daily completed study volume.</p></div><div className="weekly-total"><strong>{weekLoading?'—':totalWeek}</strong><small>last 14 days</small></div></div>
    <div className="weekly-chart" aria-label={`${totalWeek} completed studies in the last 14 days`}>{week.map(day=><div className="weekly-day" key={day.key}><b>{weekLoading?'':day.count||''}</b><span className="weekly-bar-track"><span className="weekly-bar" style={{height:`${Math.max(day.count?12:4,day.count/maxWeekCount*100)}%`}}/></span><small>{day.label}</small></div>)}</div>
    <button className="weekly-link" onClick={()=>onNavigate('Studies')}>View studies <ArrowRight size={14}/></button>
   </section>
  </div>
 </div>;
}
