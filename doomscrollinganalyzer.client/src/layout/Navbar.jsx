import { useAuth } from '../auth/useAuth.js'

export default function Navbar() {
  const { logout, user } = useAuth()

  return (
    <header className="sticky top-0 z-30 border-b border-slate-700/40 bg-slate-950/60 backdrop-blur-xl">
      <div className="flex h-16 items-center justify-between px-4 sm:px-6 lg:px-8">
        <div>
          <p className="text-xs font-bold uppercase tracking-[0.24em] text-cyan-300">
            Doomscrolling Analyzer
          </p>
          <p className="hidden text-sm text-slate-400 sm:block">
            Focus signals for healthier digital habits
          </p>
        </div>
        <div className="flex items-center gap-3">
          <div className="hidden text-right sm:block">
            <p className="text-sm font-bold text-slate-100">{user?.username || 'User'}</p>
            <p className="text-xs text-slate-500">{user?.email}</p>
          </div>
          <button className="secondary-button" type="button" onClick={logout}>
            Logout
          </button>
        </div>
      </div>
    </header>
  )
}
