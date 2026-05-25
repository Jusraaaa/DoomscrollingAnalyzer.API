export default function AnalyticsCard({ accent = 'cyan', label, value, detail }) {
  const accentClasses = {
    amber: 'from-amber-300/22 to-orange-500/8 text-amber-200',
    cyan: 'from-cyan-300/22 to-teal-500/8 text-cyan-200',
    rose: 'from-rose-300/22 to-pink-500/8 text-rose-200',
    violet: 'from-violet-300/22 to-indigo-500/8 text-violet-200',
  }

  return (
    <article className="glass-card rounded-2xl p-5 transition duration-200 hover:-translate-y-1 hover:border-cyan-300/30">
      <div
        className={`mb-5 h-1.5 w-16 rounded-full bg-gradient-to-r ${accentClasses[accent] || accentClasses.cyan}`}
      />
      <p className="text-sm font-semibold uppercase tracking-[0.18em] text-slate-500">{label}</p>
      <div className="mt-3 text-3xl font-black text-slate-50">{value}</div>
      {detail && <p className="mt-2 text-sm text-slate-400">{detail}</p>}
    </article>
  )
}
