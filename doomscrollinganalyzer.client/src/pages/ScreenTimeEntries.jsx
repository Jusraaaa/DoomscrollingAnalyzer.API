import { useCallback, useEffect, useState } from 'react'
import api from '../api/client.js'
import EmptyState from '../components/EmptyState.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'

const initialForm = {
  platformName: '',
  hoursSpent: '',
  usageDate: '',
}

export default function ScreenTimeEntries() {
  const [entries, setEntries] = useState([])
  const [form, setForm] = useState(initialForm)
  const [filters, setFilters] = useState({ search: '', platform: '', sortBy: 'date', sortDirection: 'desc' })
  const [page, setPage] = useState({ pageNumber: 1, totalPages: 1, totalCount: 0 })
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  const loadEntries = useCallback(async (pageNumber = 1) => {
    setIsLoading(true)
    const { data } = await api.get('/screentime', {
      params: {
        pageNumber,
        pageSize: 8,
        search: filters.search || undefined,
        platform: filters.platform || undefined,
        sortBy: filters.sortBy,
        sortDirection: filters.sortDirection,
      },
    })
    setEntries(data.items || [])
    setPage({
      pageNumber: data.pageNumber,
      totalPages: data.totalPages,
      totalCount: data.totalCount,
    })
    setIsLoading(false)
  }, [filters.platform, filters.search, filters.sortBy, filters.sortDirection])

  useEffect(() => {
    async function fetchEntries() {
      await loadEntries(1)
    }

    fetchEntries()
  }, [loadEntries])

  const handleSubmit = async (event) => {
    event.preventDefault()
    setIsSaving(true)
    await api.post('/screentime', {
      platformName: form.platformName,
      hoursSpent: Number(form.hoursSpent),
      usageDate: new Date(form.usageDate).toISOString(),
    })
    setForm(initialForm)
    await loadEntries(1)
    setIsSaving(false)
  }

  const deleteEntry = async (id) => {
    await api.delete(`/screentime/${id}`)
    await loadEntries(page.pageNumber)
  }

  const seedDemoData = async () => {
    await api.post('/dev/seed-demo-data')
    await loadEntries(1)
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 xl:flex-row xl:items-end">
        <div>
          <h1 className="page-title">Screen Time Entries</h1>
          <p className="mt-3 max-w-2xl text-slate-400">
            Add usage events, filter by platform, and inspect recent behavioral signals.
          </p>
        </div>
        <button className="secondary-button" type="button" onClick={seedDemoData}>
          Seed demo data
        </button>
      </div>

      <section className="grid gap-6 xl:grid-cols-[0.8fr_1.4fr]">
        <form className="glass-card rounded-3xl p-5" onSubmit={handleSubmit}>
          <h2 className="text-xl font-black text-slate-50">Create entry</h2>
          <div className="mt-5 space-y-4">
            <label className="block">
              <span className="mb-2 block text-sm font-bold text-slate-300">Platform</span>
              <input
                className="field"
                value={form.platformName}
                onChange={(event) => setForm({ ...form, platformName: event.target.value })}
                placeholder="TikTok"
                required
              />
            </label>
            <label className="block">
              <span className="mb-2 block text-sm font-bold text-slate-300">Hours spent</span>
              <input
                className="field"
                max="24"
                min="0.01"
                step="0.1"
                type="number"
                value={form.hoursSpent}
                onChange={(event) => setForm({ ...form, hoursSpent: event.target.value })}
                required
              />
            </label>
            <label className="block">
              <span className="mb-2 block text-sm font-bold text-slate-300">Usage date</span>
              <input
                className="field"
                type="datetime-local"
                value={form.usageDate}
                onChange={(event) => setForm({ ...form, usageDate: event.target.value })}
                required
              />
            </label>
          </div>
          <button className="primary-button mt-5 w-full" disabled={isSaving} type="submit">
            {isSaving ? 'Saving...' : 'Save entry'}
          </button>
        </form>

        <div className="glass-card rounded-3xl p-5">
          <div className="grid gap-3 md:grid-cols-4">
            <input
              className="field md:col-span-2"
              value={filters.search}
              onChange={(event) => setFilters({ ...filters, search: event.target.value })}
              onKeyDown={(event) => {
                if (event.key === 'Enter') {
                  loadEntries(1)
                }
              }}
              placeholder="Search platform"
            />
            <input
              className="field"
              value={filters.platform}
              onChange={(event) => setFilters({ ...filters, platform: event.target.value })}
              placeholder="Filter platform"
            />
            <select
              className="field"
              value={`${filters.sortBy}:${filters.sortDirection}`}
              onChange={(event) => {
                const [sortBy, sortDirection] = event.target.value.split(':')
                setFilters({ ...filters, sortBy, sortDirection })
              }}
            >
              <option value="date:desc">Newest</option>
              <option value="date:asc">Oldest</option>
              <option value="hours:desc">Highest hours</option>
              <option value="hours:asc">Lowest hours</option>
            </select>
          </div>

          <div className="mt-5">
            {isLoading ? (
              <LoadingSpinner label="Loading entries" />
            ) : entries.length ? (
              <div className="overflow-hidden rounded-2xl border border-slate-700/50">
                <table className="w-full min-w-[640px] text-left text-sm">
                  <thead className="bg-slate-950/70 text-xs uppercase tracking-[0.18em] text-slate-500">
                    <tr>
                      <th className="px-4 py-3">Platform</th>
                      <th className="px-4 py-3">Hours</th>
                      <th className="px-4 py-3">Usage date</th>
                      <th className="px-4 py-3 text-right">Action</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-700/50">
                    {entries.map((entry) => (
                      <tr className="bg-slate-950/25 transition hover:bg-cyan-300/5" key={entry.id}>
                        <td className="px-4 py-4 font-bold text-slate-100">{entry.platformName}</td>
                        <td className="px-4 py-4 text-cyan-200">{entry.hoursSpent}h</td>
                        <td className="px-4 py-4 text-slate-400">
                          {new Date(entry.usageDate).toLocaleString()}
                        </td>
                        <td className="px-4 py-4 text-right">
                          <button
                            className="rounded-full border border-rose-300/30 px-3 py-1 text-sm font-bold text-rose-200 transition hover:bg-rose-500/10"
                            type="button"
                            onClick={() => deleteEntry(entry.id)}
                          >
                            Delete
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <EmptyState title="No entries found" message="Try another filter or add a new entry." />
            )}
          </div>

          <div className="mt-5 flex items-center justify-between text-sm text-slate-400">
            <span>{page.totalCount} total entries</span>
            <div className="flex items-center gap-2">
              <button
                className="secondary-button px-3 py-2"
                disabled={page.pageNumber <= 1}
                type="button"
                onClick={() => loadEntries(page.pageNumber - 1)}
              >
                Previous
              </button>
              <span>
                Page {page.pageNumber} of {Math.max(page.totalPages, 1)}
              </span>
              <button
                className="secondary-button px-3 py-2"
                disabled={page.pageNumber >= page.totalPages}
                type="button"
                onClick={() => loadEntries(page.pageNumber + 1)}
              >
                Next
              </button>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}
