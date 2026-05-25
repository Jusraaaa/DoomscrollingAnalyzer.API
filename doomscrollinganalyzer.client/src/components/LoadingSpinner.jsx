export default function LoadingSpinner({ label = 'Loading' }) {
  return (
    <div className="flex min-h-48 flex-col items-center justify-center gap-4 text-slate-300">
      <div className="h-11 w-11 animate-spin rounded-full border-4 border-slate-700 border-t-cyan-300" />
      <span className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-500">
        {label}
      </span>
    </div>
  )
}
