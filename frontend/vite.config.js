import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import mkcert from 'vite-plugin-mkcert'

export default defineConfig({
  plugins: [
    react(),
    mkcert() 
  ],
  server: {
    host: 'localhost',
    port: 5173,
    open: true,
    proxy: {
      '/iot-api': {
        target: 'http://45.117.177.157:82',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/iot-api/, '/api'),
      }
    }
  }
})