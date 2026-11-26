import React,{useEffect,useMemo,useState} from 'react';
import {ArrowLeft,ArrowRight,CalendarDays,CheckCircle2,ChevronRight,Clock3,RefreshCw,ShieldCheck,Users,Wallet,XCircle} from 'lucide-react';
import {endpoints} from './config/apiCatalog.js';
import {fetchOrderCountsForDate} from './orderCounts.js';

const orderStatuses=[
 {status:'Scheduled',label:'Scheduled',icon:Clock3,tone:'scheduled'},
 {status:'Completed',label:'Completed',icon:CheckCircle2,tone:'completed'},
 {status:'Cancelled',label:'Cancelled',icon:XCircle,tone:'cancelled'}
];
const localDateKey=date=>`${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;

export default function AdminDashboard({user,onNavigate,onAction,available=[],request}){
 const [stats,setStats]=useState([]),[todayCounts,setTodayCounts]=useState([]),[selectedOrderDate,setSelectedOrderDate]=useState(()=>localDateKey(new Date())),[weeklyRevenue,setWeeklyRevenue]=useState([]),[prices,setPrices]=useState([]),[priceReload,setPriceReload]=useState(0),[pricesLoading,setPricesLoading]=useState(true),[priceError,setPriceError]=useState(''),[weekOffset,setWeekOffset]=useState(0);
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
  return()=>{alive=false};
 },[request]);

 useEffect(()=>{
  let alive=true;setPricesLoading(true);setPriceError('');
  request(endpoints.find(item=>item.key==='price-list')?.path||'/Price/GetAll').then(result=>{
   if(!alive)return;const rows=result?.items||result?.Items||result?.prices||result?.Prices||(Array.isArray(result)?result:[]);setPrices(rows);
  }).catch(error=>{if(alive){setPrices([]);setPriceError(error.message||'Could not load modality prices.')}}).finally(()=>alive&&setPricesLoading(false));
  return()=>{alive=false};
 },[request,priceReload]);

 useEffect(()=>{
  let alive=true;setTodayLoading(true);
  fetchOrderCountsForDate(request,selectedOrderDate).then(counts=>{if(alive)setTodayCounts(orderStatuses.map(item=>({...item,value:counts[item.status]})))}).catch(()=>{if(alive)setTodayCounts(orderStatuses.map(item=>({...item,value:'—'})))}).finally(()=>alive&&setTodayLoading(false));
  return()=>{alive=false};
 },[selectedOrderDate,request]);

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
  <div className="price-overview-grid"><section className="dashboard-panel price-list-panel"><div className="dashboard-panel-heading"><div><span className="eyebrow">PRICING</span><h2>Modality prices</h2><p>Current price for each imaging modality.</p></div><button className="text-btn" type="button" onClick={()=>setPriceReload(value=>value+1)} aria-label="Refresh prices" title="Refresh prices"><RefreshCw size={15}/></button></div>{pricesLoading?<div className="price-list-state">Loading prices…</div>:priceError?<div className="price-list-state price-list-error">{priceError}</div>:prices.length===0?<div className="price-list-state">No modality prices are available.</div>:<div className="price-list">{prices.map((item,index)=>{const modality=item.modality??item.Modality??item.name??item.Name;const modalityNames=['MRI','CT','X-ray','Ultrasound','PET','Mammography','Fluoroscopy'];const label=typeof modality==='number'?modalityNames[modality]??`Modality ${modality}`:({XRay:'X-ray'}[modality]||String(modality??'Modality'));const amount=Number(item.amount??item.Amount);return <div className="price-list-row" key={item.id??item.Id??label??index}><span>{label}</span><strong>{Number.isFinite(amount)?amount.toLocaleString(undefined,{minimumFractionDigits:2,maximumFractionDigits:2}):'—'}</strong></div>})}</div>}</section><section className="dashboard-panel price-management-panel"><div className="price-manage-icon"><Wallet size={20}/></div><span className="eyebrow">PRICE MANAGEMENT</span><h2>Update prices</h2><p>Choose a modality and enter its new price.</p><button className="primary-btn" type="button" onClick={()=>{const endpoint=available.find(item=>item.key==='price-update');if(endpoint)onAction(endpoint)}}>Change prices <ArrowRight size={15}/></button></section></div>
  <section className="order-count-section"><div className="order-count-heading"><div><span className="eyebrow">ORDERS BY DATE</span><h2>Orders scheduled on {new Date(`${selectedOrderDate}T00:00:00`).toLocaleDateString(undefined,{dateStyle:'medium'})}</h2><p>Order totals by status for the selected date.</p></div><div className="order-count-controls"><label className="dashboard-date-filter"><CalendarDays size={16}/><span>Date</span><input type="date" value={selectedOrderDate} onChange={event=>event.target.value&&setSelectedOrderDate(event.target.value)}/></label><button className="text-btn" onClick={()=>onNavigate('Orders')}>View orders <ArrowRight size={14}/></button></div></div><div className="order-count-cards">{todayCounts.map(item=>{const Glyph=item.icon;return <article className={`order-count-card order-count-${item.tone}`} key={item.status}><span className="order-count-icon"><Glyph size={19}/></span><span className="order-count-label">{item.label}</span><strong>{todayLoading?'—':item.value??'—'}</strong><small>{selectedOrderDate===localDateKey(new Date())?'Today':new Date(`${selectedOrderDate}T00:00:00`).toLocaleDateString(undefined,{month:'short',day:'numeric'})}</small></article>})}</div></section>
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
