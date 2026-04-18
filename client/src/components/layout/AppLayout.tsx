import { Outlet, NavLink, useNavigate } from 'react-router-dom'
import { useAuthStore } from '../../stores/authStore'
import clsx from 'clsx'

const navItems = [
  { to: '/gardens', label: 'My Gardens', icon: '🌱' },
  { to: '/catalog', label: 'Plant Catalog', icon: '📚' },
  { to: '/calendar', label: 'Calendar', icon: '📅' },
]

export default function AppLayout() {
  const clearAuth = useAuthStore(s => s.clearAuth)
  const displayName = useAuthStore(s => s.displayName)
  const navigate = useNavigate()

  const handleLogout = () => {
    clearAuth()
    navigate('/login')
  }

  return (
    <div className="min-h-screen bg-paper-50 flex">
      {/* Sidebar */}
      <aside className="w-56 bg-white border-r border-paper-200 flex flex-col shrink-0">
        <div className="p-5 border-b border-paper-200">
          <h1 className="font-display text-2xl font-bold text-leaf-700">GreenPlot</h1>
          <p className="text-xs text-gray-500 mt-0.5">Garden Planning</p>
        </div>

        <nav className="flex-1 p-3 space-y-1">
          {navItems.map(({ to, label, icon }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) => clsx(
                'flex items-center gap-2.5 px-3 py-2 rounded-xl text-sm font-medium transition-colors',
                isActive
                  ? 'bg-leaf-50 text-leaf-700'
                  : 'text-gray-600 hover:bg-paper-100 hover:text-gray-900'
              )}
            >
              <span>{icon}</span>
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="p-3 border-t border-paper-200">
          <div className="px-3 py-2 mb-1">
            <p className="text-xs font-medium text-gray-900 truncate">{displayName}</p>
          </div>
          <button
            onClick={handleLogout}
            className="w-full flex items-center gap-2 px-3 py-2 rounded-xl text-sm text-gray-500 hover:bg-paper-100 transition-colors"
          >
            Sign out
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main className="flex-1 overflow-auto">
        <Outlet />
      </main>
    </div>
  )
}
