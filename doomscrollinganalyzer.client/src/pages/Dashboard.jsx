import { useEffect, useMemo, useState } from 'react'
import {
  Area,
  AreaChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import api from '../api/client.js'
import AnalyticsCard from '../components/AnalyticsCard.jsx'
import EmptyState from '../components/EmptyState.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'

export default function Dashboard() {
  const [summary, setSummary] = useState(null)
  const [weeklyReport, setWeeklyReport] = useState([])
  const [entries, setEntries] = useState([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    async function loadDashboard() {
      try {
        const [summaryResponse, weeklyResponse, entriesResponse] = await Promise.all([
          api.get('/analytics/summary'),
          api.get('/analytics/weekly-report'),
          api.get('/screentime', { params: { pageNumber: 1, pageSize: 5, sortBy: 'date', sortDirection: 'desc' } }),
        ])
        setSummary(summaryResponse.data)
        setWeeklyReport(weeklyResponse.data)
        setEntries(entriesResponse.data.items || [])
      } catch {
        setError('Unable to load dashboard data. Make sure the API is running.')
      } finally {
        setIsLoading(false)
      }
    }

    loadDashboard()
  }, [])

  const chartData = useMemo(
    () =>
      [...weeklyReport]
        .reverse()
        .map((week) => ({
          week: new Date(week.weekStartDate).toLocaleDateString(undefined, {
            month: 'short',
            day: 'numeric',
          }),
          hours: week.totalHoursSpent,
          score: week.doomscrollingScore,
        })),
    [weeklyReport],
  )

  if (isLoading) {
    return <LoadingSpinner label="Loading dashboard" />
  }

  if (error) {
    return <EmptyState title="Dashboard unavailable" message={error} />
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 xl:flex-row xl:items-end">
        <div>
          <h1 className="page-title">Dashboard</h1>
          <p className="mt-3 max-w-2xl text-slate-400">
            A clean view of your screen-time load, risk score, and recent digital patterns.
          </p>
        </div>
        <div className="glass-panel rounded-2xl px-5 py-3 text-sm text-slate-300">
          API: <span className="font-bold text-cyan-200">{import.meta.env.VITE_API_BASE_URL}</span>
        </div>
      </div>

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <AnalyticsCard
          accent="cyan"
          label="Total usage"
          value={`${summary.totalHoursSpent}h`}
          detail="Tracked across all platforms"
        />
        <AnalyticsCard
          accent="rose"
          label="Doomscrolling score"
          value={summary.doomscrollingScore}
          detail="Higher means more risk"
        />
        <AnalyticsCard
          accent="amber"
          label="Productivity status"
          value={summary.productivityStatus}
          detail="Based on score thresholds"
        />
        <AnalyticsCard
          accent="violet"
          label="Top platform"
          value={summary.mostUsedPlatform}
          detail={`${summary.averageDailyUsage}h average daily usage`}
        />
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.45fr_0.8fr]">
        <div className="glass-card rounded-3xl p-5">
          <div className="mb-5 flex items-center justify-between">
            <div>
              <h2 className="text-xl font-black text-slate-50">Weekly usage trend</h2>
              <p className="mt-1 text-sm text-slate-400">Hours and risk score by week</p>
            </div>
          </div>
          <div className="h-80">
            {chartData.length ? (
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={chartData}>
                  <defs>
                    <linearGradient id="hoursGradient" x1="0" x2="0" y1="0" y2="1">
                      <stop offset="5%" stopColor="#22d3ee" stopOpacity={0.75} />
                      <stop offset="95%" stopColor="#22d3ee" stopOpacity={0.05} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid stroke="#334155" strokeDasharray="3 3" />
                  <XAxis dataKey="week" stroke="#94a3b8" />
                  <YAxis stroke="#94a3b8" />
                  <Tooltip
                    contentStyle={{
                      background: '#0f172a',
                      border: '1px solid rgba(148, 163, 184, 0.25)',
                      borderRadius: '14px',
                      color: '#e2e8f0',
                    }}
                  />
                  <Area
                    dataKey="hours"
                    fill="url(#hoursGradient)"
                    name="Hours"
                    stroke="#22d3ee"
                    strokeWidth={3}
                    type="monotone"
                  />
                  <Area
                    dataKey="score"
                    fill="rgba(251, 113, 133, 0.06)"
                    name="Score"
                    stroke="#fb7185"
                    strokeWidth={2}
                    type="monotone"
                  />
                </AreaChart>
              </ResponsiveContainer>
            ) : (
              <EmptyState title="No weekly data" message="Add entries to populate the trend chart." />
            )}
          </div>
        </div>

        <div className="glass-card rounded-3xl p-5">
          <h2 className="text-xl font-black text-slate-50">Recent entries</h2>
          <div className="mt-5 space-y-3">
            {entries.length ? (
              entries.map((entry) => (
                <div
                  className="rounded-2xl border border-slate-700/60 bg-slate-950/42 p-4 transition hover:border-cyan-300/30"
                  key={entry.id}
                >
                  <div className="flex items-center justify-between gap-4">
                    <div>
                      <p className="font-bold text-slate-100">{entry.platformName}</p>
                      <p className="mt-1 text-xs text-slate-500">
                        {new Date(entry.usageDate).toLocaleString()}
                      </p>
                    </div>
                    <span className="rounded-full bg-cyan-300/10 px-3 py-1 text-sm font-black text-cyan-200">
                      {entry.hoursSpent}h
                    </span>
                  </div>
                </div>
              ))
            ) : (
              <EmptyState title="No entries yet" message="Create entries or seed demo data." />
            )}
          </div>
        </div>
      </section>
    </div>
  )
}
