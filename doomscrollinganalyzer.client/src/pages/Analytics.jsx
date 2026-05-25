import { useEffect, useState } from 'react'
import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import api from '../api/client.js'
import AnalyticsCard from '../components/AnalyticsCard.jsx'
import EmptyState from '../components/EmptyState.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'

export default function Analytics() {
  const [summary, setSummary] = useState(null)
  const [weeklyReport, setWeeklyReport] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    async function loadAnalytics() {
      const [summaryResponse, weeklyResponse] = await Promise.all([
        api.get('/analytics/summary'),
        api.get('/analytics/weekly-report'),
      ])
      setSummary(summaryResponse.data)
      setWeeklyReport(weeklyResponse.data)
      setIsLoading(false)
    }

    loadAnalytics().catch(() => setIsLoading(false))
  }, [])

  if (isLoading) {
    return <LoadingSpinner label="Loading analytics" />
  }

  if (!summary) {
    return <EmptyState title="Analytics unavailable" message="The API did not return analytics data." />
  }

  const chartData = weeklyReport.map((week) => ({
    week: new Date(week.weekStartDate).toLocaleDateString(undefined, { month: 'short', day: 'numeric' }),
    hours: week.totalHoursSpent,
    score: week.doomscrollingScore,
    entries: week.entryCount,
  }))

  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-title">Analytics</h1>
        <p className="mt-3 max-w-2xl text-slate-400">
          Score drivers, weekly volume, and recommendations based on your tracked entries.
        </p>
      </div>

      <section className="grid gap-4 md:grid-cols-3">
        <AnalyticsCard accent="rose" label="Risk score" value={summary.doomscrollingScore} />
        <AnalyticsCard accent="amber" label="Status" value={summary.productivityStatus} />
        <AnalyticsCard accent="cyan" label="Average daily" value={`${summary.averageDailyUsage}h`} />
      </section>

      <section className="glass-card rounded-3xl p-5">
        <h2 className="text-xl font-black text-slate-50">Weekly report</h2>
        <p className="mt-1 text-sm text-slate-400">{summary.recommendationMessage}</p>
        <div className="mt-6 h-96">
          {chartData.length ? (
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={chartData}>
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
                <Legend />
                <Bar dataKey="hours" fill="#22d3ee" name="Hours" radius={[8, 8, 0, 0]} />
                <Bar dataKey="score" fill="#fb7185" name="Score" radius={[8, 8, 0, 0]} />
                <Bar dataKey="entries" fill="#fbbf24" name="Entries" radius={[8, 8, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          ) : (
            <EmptyState title="No report data" message="Add entries to generate weekly statistics." />
          )}
        </div>
      </section>
    </div>
  )
}
