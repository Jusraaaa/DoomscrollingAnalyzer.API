export default function EmptyState({ title, message }) {
  return (
    <div className="glass-panel rounded-2xl px-6 py-10 text-center">
      <h3 className="text-lg font-bold text-slate-100">{title}</h3>
      <p className="mx-auto mt-2 max-w-md text-sm text-slate-400">{message}</p>
    </div>
  )
}
