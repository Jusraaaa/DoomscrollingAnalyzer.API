import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth.js'

export default function Register() {
  const { register } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ username: '', email: '', password: '' })
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (event) => {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await register(form.username, form.email, form.password)
      navigate('/dashboard')
    } catch (requestError) {
      setError(requestError.response?.data?.message || 'Registration failed. Check your details.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="app-shell flex min-h-screen items-center justify-center px-4 py-10">
      <form className="glass-card w-full max-w-xl rounded-3xl p-6 sm:p-8" onSubmit={handleSubmit}>
        <p className="text-sm font-bold uppercase tracking-[0.26em] text-cyan-300">
          Start tracking
        </p>
        <h1 className="mt-4 text-4xl font-black text-slate-50">Create your workspace</h1>
        <p className="mt-3 text-sm leading-6 text-slate-400">
          Your first session opens a private analytics dashboard for screen-time patterns.
        </p>

        {error && (
          <div className="mt-5 rounded-2xl border border-rose-400/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-200">
            {error}
          </div>
        )}

        <div className="mt-6 space-y-4">
          <label className="block">
            <span className="mb-2 block text-sm font-bold text-slate-300">Username</span>
            <input
              className="field"
              value={form.username}
              onChange={(event) => setForm({ ...form, username: event.target.value })}
              placeholder="focuspilot"
              required
            />
          </label>
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
              minLength={8}
              value={form.password}
              onChange={(event) => setForm({ ...form, password: event.target.value })}
              placeholder="At least 8 characters"
              required
            />
          </label>
        </div>

        <button className="primary-button mt-6 w-full" type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Creating account...' : 'Create account'}
        </button>
        <p className="mt-5 text-center text-sm text-slate-400">
          Already registered?{' '}
          <Link className="font-bold text-cyan-300 hover:text-cyan-200" to="/login">
            Sign in
          </Link>
        </p>
      </form>
    </main>
  )
}
