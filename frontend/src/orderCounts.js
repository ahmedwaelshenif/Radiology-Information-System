const pageRows=result=>result?.items||result?.Items||(Array.isArray(result)?result:[]);
const totalRows=result=>Number(result?.totalCount??result?.TotalCount??result?.totalItems??result?.TotalItems??pageRows(result).length)||0;
const localDateKey=date=>`${date.getFullYear()}-${String(date.getMonth()+1).padStart(2,'0')}-${String(date.getDate()).padStart(2,'0')}`;
const localDateTime=(date,endOfDay=false)=>`${localDateKey(date)}T${endOfDay?'23:59:59.999':'00:00:00'}`;

export async function fetchOrderCountsForDate(request,dateKey){
 const [year,month,day]=dateKey.split('-').map(Number);
 const start=new Date(year,month-1,day);
 const pageSize=100;
 const query=new URLSearchParams({ScheduledFrom:localDateTime(start),ScheduledTo:localDateTime(start,true),PageNumber:'1',PageSize:String(pageSize)});
 const first=await request('/RadiologyOrder/search?'+query.toString());
 const rows=pageRows(first);
 const pageCount=Math.ceil(totalRows(first)/pageSize);
 const rest=pageCount>1?await Promise.all(Array.from({length:pageCount-1},(_,index)=>{
  const pageQuery=new URLSearchParams({ScheduledFrom:localDateTime(start),ScheduledTo:localDateTime(start,true),PageNumber:String(index+2),PageSize:String(pageSize)});
  return request('/RadiologyOrder/search?'+pageQuery.toString()).then(pageRows);
 })):[];
 const counts={Scheduled:0,Completed:0,Cancelled:0};
 for(const row of rows.concat(...rest)){
  const raw=row.status??row.Status;
  const status=typeof raw==='number'||/^\d+$/.test(String(raw))?['Scheduled','Completed','Cancelled'][Number(raw)]:String(raw??'').toLowerCase().replace(/^./,letter=>letter.toUpperCase());
  if(Object.prototype.hasOwnProperty.call(counts,status))counts[status]++;
 }
 return counts;
}
