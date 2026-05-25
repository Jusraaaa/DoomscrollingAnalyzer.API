import { NavLink } from 'react-router-dom'

const links = [
  { to: '/dashboard', label: 'Dashboard', marker: '01' },
  { to: '/analytics', label: 'Analytics', marker: '02' },
  { to: '/screentime', label: 'Screen Time', marker: '03' },
]

export default function Sidebar() {
  return (
    <aside className="hidden w-72 shrink-0 border-r border-slate-700/40 bg-slate-950/35 p-5 backdrop-blur-xl lg:block">
      <div className="mb-8 rounded-2xl border border-cyan-300/20 bg-cyan-300/8 p-4">
        <p className="text-sm font-bold text-slate-100">Digital Wellness OS</p>
        <p className="mt-1 text-xs leading-5 text-slate-400">
          Monitor late-night loops, social spikes, and weekly usage patterns.
        </p>
      </div>
      <nav className="space-y-2">
        {links.map((link) => (
          <NavLink key={link.to} className="nav-link" to={link.to}>
            <span className="text-xs font-black text-cyan-300/80">{link.marker}</span>
            <span className="font-semibold">{link.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}
