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
        rewrite:path=>path.replace(/^\/api/,'')
      }
    }
  }
});
