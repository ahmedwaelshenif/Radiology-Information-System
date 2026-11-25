import React,{useEffect,useMemo,useState} from 'react';
import {ArrowLeft,ArrowRight,CalendarDays,CheckCircle2,ChevronRight,Clock3,ShieldCheck,Users,Wallet,XCircle} from 'lucide-react';
import {endpoints} from './config/apiCatalog.js';

const orderStatuses=[
 {status:'Scheduled',label:'Scheduled',icon:Clock3,tone:'scheduled'},
 {status:'Completed',label:'Completed',icon:CheckCircle2,tone:'completed'},
 {status:'Cancelled',label:'Cancelled',icon:XCircle,tone:'cancelled'}
];
const localDateKey=date=>`${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;
const totalFrom=result=>Number(result?.totalCount??result?.TotalCount??result?.totalItems??result?.TotalItems??(Array.isArray(result)?result.length:0))||0;

export default function AdminDashboard({user,onNavigate,onAction,available=[],request}){
 const [stats,setStats]=useState([]),[todayCounts,setTodayCounts]=useState([]),[weeklyRevenue,setWeeklyRevenue]=useState([]),[weekOffset,setWeekOffset]=useState(0);
 const [loading,setLoading]=useState(true),[todayLoading,setTodayLoading]=useState(true),[revenueLoading,setRevenueLoading]=useState(true);
 const firstName=(user?.fullName||'there').split(' ')[0];
 const weekDays=useMemo(()=>{
  const today=new Date();
  const monday=new Date(today.getFullYear(),today.getMonth(),today.getDate());
  monday.setDate(monday.getDate()-((monday.getDay()+6)%7)+weekOffset*7);
  return Array.from({length:7},(_,index)=>{const date=new Date(monday);date.setDate(monday.getDate()+index);return {date,key:localDateKey(date),label:new Intl.DateTimeFormat(undefined,{weekday:'short'}).format(date)}});
 },[weekOffset]);
 const weekLabel=weekDays.length?`${weekDays[0].date.toLocaleDateString(undefined,{month:'short',day:'numeric'})} – ${weekDays[6].date.toLocaleDateString(undefined,{month:'short',day:'numeric',year:'numeric'})}`:'';

 useEffect(()=>{
  let alive=true;
  const metrics=[['count-patient','Patients'],['count-rad','Radiologists'],['count-rec','Receptionists']];
  Promise.all(metrics.map(async([key,label])=>{try{return {label,value:await request(endpoints.find(e=>e.key===key).path)}}catch{return {label,value:'—'}}})).then(values=>{if(alive){setStats(values);setLoading(false)}});
  const today=new Date();const start=new Date(today.getFullYear(),today.getMonth(),today.getDate());const end=new Date(start);end.setDate(end.getDate()+1);end.setMilliseconds(-1);
  Promise.all(orderStatuses.map(async item=>{
   const query=new URLSearchParams({Status:item.status,ScheduledFrom:start.toISOString(),ScheduledTo:end.toISOString(),PageNumber:'1',PageSize:'1'});
   try{return {...item,value:totalFrom(await request('/RadiologyOrder/search?'+query.toString()))}}catch{return {...item,value:'—'}}
  })).then(values=>{if(alive){setTodayCounts(values);setTodayLoading(false)}});
  return()=>{alive=false};
 },[request]);

 useEffect(()=>{
  let alive=true;setRevenueLoading(true);
  Promise.all(weekDays.map(async day=>{
   const start=new Date(day.date);const end=new Date(start);end.setDate(end.getDate()+1);end.setMilliseconds(-1);
   const query=new URLSearchParams({start:start.toISOString(),end:end.toISOString()});
   try{return {...day,amount:Number(await request('/Invoice/sum-amount?'+query.toString()))||0}}catch{return {...day,amount:0}}
  })).then(values=>{if(alive){setWeeklyRevenue(values);setRevenueLoading(false)}});
  return()=>{alive=false};
 },[weekDays,request]);

 const targets=[['Patients',undefined],['Team','Radiologists'],['Team','Receptionists']];
 return <div className="role-dashboard dashboard-admin">
  <section className="dashboard-hero"><div className="dashboard-hero-copy"><span className="eyebrow">ADMINISTRATION WORKSPACE</span><h1>Good day, {firstName}</h1><p>Monitor registered accounts and manage access across the organization.</p></div><div className="dashboard-hero-mark"><ShieldCheck size={28}/></div></section>
  <section className="dashboard-metrics">{stats.map((item,index)=>{const Glyph=Users;return <button key={item.label} type="button" className="dashboard-metric is-clickable" onClick={()=>onNavigate(targets[index][0],targets[index][1])}><span className={`metric-icon metric-${index}`}><Glyph size={18}/></span><span className="metric-label">{item.label}</span><strong>{loading?'—':item.value??'—'}</strong><small>Registered accounts</small><ChevronRight size={16} className="metric-arrow"/></button>})}</section>
  <section className="dashboard-panel price-management-panel"><div className="dashboard-panel-heading"><div><span className="eyebrow">PRICING</span><h2>Modality prices</h2><p>Keep imaging prices up to date for each modality.</p></div><button className="secondary-btn" type="button" onClick={()=>{const endpoint=available.find(item=>item.key==='price-update');if(endpoint)onAction(endpoint)}}><Wallet size={16}/>Update a price</button></div></section>
  <section className="order-count-section"><div className="order-count-heading"><div><span className="eyebrow">TODAY</span><h2>Orders scheduled today</h2><p>Today’s order totals by status.</p></div><button className="text-btn" onClick={()=>onNavigate('Orders')}>View orders <ArrowRight size={14}/></button></div><div className="order-count-cards">{todayCounts.map(item=>{const Glyph=item.icon;return <article className={`order-count-card order-count-${item.tone}`} key={item.status}><span className="order-count-icon"><Glyph size={19}/></span><span className="order-count-label">{item.label}</span><strong>{todayLoading?'—':item.value??'—'}</strong><small>Today</small></article>})}</div></section>
  <WeeklyRevenueGraph days={weeklyRevenue} loading={revenueLoading} weekLabel={weekLabel} canGoForward={weekOffset<0} onPrevious={()=>setWeekOffset(offset=>offset-1)} onNext={()=>setWeekOffset(offset=>Math.min(0,offset+1))}/>
 </div>;
}

function WeeklyRevenueGraph({days,loading,weekLabel,canGoForward,onPrevious,onNext}){
 const total=days.reduce((sum,day)=>sum+day.amount,0),max=Math.max(1,...days.map(day=>day.amount));
 const money=value=>Number(value||0).toLocaleString(undefined,{maximumFractionDigits:0});
 return <section className="dashboard-panel revenue-panel">
  <div className="dashboard-panel-heading"><div><span className="eyebrow">REVENUE</span><h2>Weekly revenue</h2><p>Total invoice amount per day for the selected week.</p></div><div className="revenue-summary"><div className="revenue-total"><strong>{money(total)}</strong><small>Week total</small></div><div className="revenue-week-controls"><button className="secondary-btn" type="button" onClick={onPrevious} aria-label="Previous week" title="Previous week"><ArrowLeft size={15}/></button><span><CalendarDays size={14}/>{weekLabel}</span><button className="secondary-btn" type="button" onClick={onNext} disabled={!canGoForward} aria-label="Next week" title="Next week"><ArrowRight size={15}/></button></div></div></div>
  {loading?<div className="revenue-loading">Loading daily revenue…</div>:<div className="revenue-chart"><svg viewBox="0 0 700 230" role="img" aria-label={`Invoice revenue per day for ${weekLabel}`}><line x1="38" y1="35" x2="674" y2="35"/><line x1="38" y1="105" x2="674" y2="105"/><line x1="38" y1="175" x2="674" y2="175"/>{[0,1,2].map(index=><text key={'grid-'+index} x="32" y={39+index*70} textAnchor="end">{money(max*(2-index)/2)}</text>)}{days.map((day,index)=>{const height=day.amount?Math.max(4,day.amount/max*130):0,x=53+index*91,y=175-height;return <g key={day.key}><title>{day.key}: {money(day.amount)}</title><rect className="revenue-bar" x={x} y={y} width="42" height={height} rx="8"/><text className="revenue-value" x={x+21} y={Math.max(20,y-8)} textAnchor="middle">{money(day.amount)}</text><text className="revenue-day" x={x+21} y="202" textAnchor="middle">{day.label}</text></g>})}</svg></div>}
 </section>;
}
