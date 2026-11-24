# Frontend

React + Vite interface for the controllers in the API project.

## Run locally

1. Start the ASP.NET API on `http://localhost:5230`.
2. From this folder run `npm install` once, then `npm run dev`.
3. Open the Vite URL printed in the terminal (normally `http://localhost:5173`).

The development server proxies `/api` requests to the API port above. Change the proxy target in `vite.config.js` if the API runs elsewhere.

The UI uses the role returned by login to show role appropriate workflows. Data sections load a paged list on entry and provide filters, record details, and common actions. Create and update workflows use labeled forms; report PDFs can be downloaded where available.
