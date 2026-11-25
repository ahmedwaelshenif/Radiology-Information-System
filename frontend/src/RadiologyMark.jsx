import React from 'react';
import skeletonUrl from './radiology-skeleton.svg?url';

// Health Icons outline skeleton. The source icon is CC0/public domain.
export default function RadiologyMark({size=28,className=''}){
 return <span className={`radiology-mark ${className}`.trim()} style={{width:size,height:size,maskImage:`url(${skeletonUrl})`,WebkitMaskImage:`url(${skeletonUrl})`}} aria-hidden="true"/>;
}
