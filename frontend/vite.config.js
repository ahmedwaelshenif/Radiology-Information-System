import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
const apiTarget = process.env.API_PROXY_TARGET || 'https://localhost:7219';
export default defineConfig({
  plugins:[react()],
  server:{
    port:5173,
    proxy:{
      '/api':{
        target:apiTarget,
        changeOrigin:true,
        secure:false,
        // Most controllers use routes like /RadiologyOrder, so remove the frontend proxy prefix.
        // BookOrderController itself is explicitly routed under /api/BookOrder; preserve that prefix.
        rewrite:path=>path.startsWith('/api/BookOrder')?path:path.replace(/^\/api/,'')
      }
    }
  }
});
