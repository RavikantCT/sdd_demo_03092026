import { useState, useEffect } from 'react'
import { api } from '../api/client'
import type { AuthorizationHistoryEntry } from '../types'
import { STATUS_COLORS, STATUS_BG } from '../types'

interface HistoryPanelProps {
  authorizationId: number
}

export default function HistoryPanel({ authorizationId }: HistoryPanelProps) {
  const [entries, setEntries] = useState<AuthorizationHistoryEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    setLoading(true); setError(null)
    api.authorizations.getHistory(authorizationId)
      .then(setEntries)
      .catch(() => setError('Failed to load status history'))
      .finally(() => setLoading(false))
  }, [authorizationId])

  return (
    <div className="card mt-4">
      <div className="card-header"><span className="card-title">Status History</span></div>
      <div className="card-body">
        {loading ? (
          <div className="loading">Loading…</div>
        ) : error ? (
          <div className="error-msg">{error}</div>
        ) : entries.length === 0 ? (
          <p className="text-muted">No history available</p>
        ) : (
          entries.map((entry, i) => (
            <div key={entry.historyId}>
              <div className="detail-row">
                <span className="detail-label">
                  <span
                    className="badge"
                    style={{
                      color: STATUS_COLORS[entry.status],
                      background: STATUS_BG[entry.status],
                      fontSize: 12,
                      padding: '4px 12px',
                    }}
                  >
                    {entry.status}
                  </span>
                </span>
                <span className="detail-value">
                  {new Date(entry.changedAt).toLocaleString()}
                </span>
              </div>
              <div className="detail-row">
                <span className="detail-label">Changed By</span>
                <span className="detail-value">{entry.changedBy ?? '—'}</span>
              </div>
              {entry.notes && (
                <div className="detail-row">
                  <span className="detail-label">Notes</span>
                  <span className="detail-value">{entry.notes}</span>
                </div>
              )}
              {i < entries.length - 1 && <div className="divider" />}
            </div>
          ))
        )}
      </div>
    </div>
  )
}
