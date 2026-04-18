import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { Toaster } from 'react-hot-toast'
import { useAuthStore } from './stores/authStore'
import AppLayout from './components/layout/AppLayout'
import LoginPage from './features/auth/LoginPage'
import RegisterPage from './features/auth/RegisterPage'
import GardenListPage from './features/garden/GardenListPage'
import GardenDetailPage from './features/garden/GardenDetailPage'
import BedDesignerPage from './features/bed-designer/BedDesignerPage'
import CatalogPage from './features/catalog/CatalogPage'
import CalendarPage from './features/calendar/CalendarPage'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { staleTime: 5 * 60_000, retry: 1 },
  },
})

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore(s => s.isAuthenticated)
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={<PrivateRoute><AppLayout /></PrivateRoute>}>
            <Route index element={<Navigate to="/gardens" replace />} />
            <Route path="gardens" element={<GardenListPage />} />
            <Route path="gardens/:gardenId" element={<GardenDetailPage />} />
            <Route path="gardens/:gardenId/beds/:bedId" element={<BedDesignerPage />} />
            <Route path="catalog" element={<CatalogPage />} />
            <Route path="calendar" element={<CalendarPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
      <Toaster
        position="bottom-right"
        toastOptions={{
          style: {
            fontFamily: 'Instrument Sans, system-ui, sans-serif',
            borderRadius: '0.75rem',
          },
        }}
      />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}
