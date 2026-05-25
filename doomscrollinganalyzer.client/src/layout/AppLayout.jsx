import { NavLink, Outlet } from 'react-router-dom'
import Navbar from './Navbar.jsx'
import Sidebar from './Sidebar.jsx'

const mobileLinks = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/analytics', label: 'Analytics' },
  { to: '/screentime', label: 'Entries' },
]

export default function AppLayout() {
  return (
    <div className="app-shell">
      <Navbar />
      <div className="flex min-h-[calc(100vh-4rem)]">
        <Sidebar />
        <main className="w-full px-4 py-6 sm:px-6 lg:px-8">
          <div className="mb-5 flex gap-2 overflow-x-auto lg:hidden">
            {mobileLinks.map((link) => (
              <NavLink
                key={link.to}
                className="rounded-full border border-slate-700/70 px-4 py-2 text-sm font-bold text-slate-300"
                to={link.to}
              >
                {link.label}
              </NavLink>
            ))}
          </div>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
