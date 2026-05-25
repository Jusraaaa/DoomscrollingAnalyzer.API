import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth.js'

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await login(form.email, form.password)
      navigate('/dashboard')
    } catch (requestError) {
      setError(requestError.response?.data?.message || 'Invalid email or password.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="app-shell flex min-h-screen items-center justify-center px-4 py-10">
      <section className="grid w-full max-w-5xl gap-8 lg:grid-cols-[1.05fr_0.95fr]">
        <div className="flex flex-col justify-center">
          <p className="text-sm font-bold uppercase tracking-[0.26em] text-cyan-300">
            Doomscrolling Analyzer
          </p>
          <h1 className="mt-5 max-w-2xl text-5xl font-black leading-tight text-slate-50 sm:text-6xl">
            Turn screen-time noise into focus signals.
          </h1>
          <p className="mt-5 max-w-xl text-lg leading-8 text-slate-400">
            Track social usage, monitor late-night patterns, and catch digital drift before it
            becomes a habit.
          </p>
        </div>

        <form className="glass-card rounded-3xl p-6 sm:p-8" onSubmit={handleSubmit}>
          <h2 className="text-2xl font-black text-slate-50">Welcome back</h2>
          <p className="mt-2 text-sm text-slate-400">Log in to open your dashboard.</p>

          {error && (
            <div className="mt-5 rounded-2xl border border-rose-400/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
              {error}
            </div>
          )}

          <div className="mt-6 space-y-4">
            <label className="block">
              <span className="mb-2 block text-sm font-bold text-slate-300">Email</span>
              <input
                className="field"
                type="email"
                value={form.email}
                onChange={(event) => setForm({ ...form, email: event.target.value })}
                placeholder="you@example.com"
                required
              />
            </label>
            <label className="block">
              <span className="mb-2 block text-sm font-bold text-slate-300">Password</span>
              <input
                className="field"
                type="password"
                value={form.password}
                onChange={(event) => setForm({ ...form, password: event.target.value })}
                placeholder="Enter your password"
                required
              />
            </label>
          </div>

          <button className="primary-button mt-6 w-full" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Signing in...' : 'Sign in'}
          </button>
          <p className="mt-5 text-center text-sm text-slate-400">
            New here?{' '}
            <Link className="font-bold text-cyan-300 hover:text-cyan-200" to="/register">
              Create an account
            </Link>
          </p>
        </form>
      </section>
    </main>
  )
}
