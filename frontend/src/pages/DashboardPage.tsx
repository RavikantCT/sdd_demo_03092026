import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../api/client'
import type { AuthorizationSummary } from '../types'
import { STATUS_COLORS, STATUS_BG } from '../types'

const PAGE_SIZE = 20

export default function DashboardPage() {
  const [authorizations, setAuthorizations] = useState<AuthorizationSummary[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [statusFilter, setStatusFilter] = useState('')
  const navigate = useNavigate()

  const load = async () => {
    setLoading(true); setError(null)
    try {
      const data = await api.authorizations.getAll(statusFilter || undefined, page, PAGE_SIZE)
      setAuthorizations(data.items)
      setTotalCount(data.totalCount)
    } catch {
      setError('Failed to load authorizations. Is the API running?')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [statusFilter, page])

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE))

  const statuses = ['', 'PENDING', 'IN_REVIEW', 'APPROVED', 'DENIED', 'CANCELLED']

  return (
    <>
      <div className="page-header flex justify-between items-center">
        <div>
          <h1 className="page-title">Prior Authorization Requests</h1>
          <p className="page-sub">Review and manage all authorization requests</p>
        </div>
        <button className="btn btn-primary" onClick={() => navigate('/new')}>
          + New Request
        </button>
      </div>

      <div className="card">
        <div className="card-header">
          <span className="card-title">
            All Requests {totalCount > 0 && `(${totalCount})`}
          </span>
          <div className="flex gap-2">
            <select
              className="form-select"
              style={{ width: 160 }}
              value={statusFilter}
              onChange={e => { setStatusFilter(e.target.value); setPage(1) }}
            >
              {statuses.map(s => (
                <option key={s} value={s}>{s || 'All Statuses'}</option>
              ))}
            </select>
            <button className="btn btn-secondary btn-sm" onClick={load}>Refresh</button>
          </div>
        </div>

        {error && <div className="card-body"><div className="error-msg">{error}</div></div>}

        {loading ? (
          <div className="loading">Loading…</div>
        ) : authorizations.length === 0 ? (
          <div className="empty-state">
            <div className="empty-state-icon">📋</div>
            <h3>No authorizations found</h3>
            <p>
              {statusFilter
                ? `No ${statusFilter} requests.`
                : 'Create your first prior authorization request to get started.'}
            </p>
          </div>
        ) : (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Reference #</th>
                  <th>Member</th>
                  <th>Provider</th>
                  <th>Health Plan</th>
                  <th>Diagnosis</th>
                  <th>Status</th>
                  <th>Created</th>
                </tr>
              </thead>
              <tbody>
                {authorizations.map(a => (
                  <tr
                    key={a.authorizationId}
                    className="row-link"
                    onClick={() => navigate(`/authorization/${a.authorizationId}`)}
                  >
                    <td>
                      <span style={{ fontFamily: 'monospace', fontWeight: 600, color: '#1e40af' }}>
                        {a.referenceNumber}
                      </span>
                    </td>
                    <td>{a.memberName ?? '—'}</td>
                    <td>{a.providerName ?? '—'}</td>
                    <td>{a.healthPlanName ?? '—'}</td>
                    <td>
                      <span style={{ fontFamily: 'monospace', fontSize: 12 }}>
                        {a.primaryDiagnosis ?? '—'}
                      </span>
                    </td>
                    <td>
                      <span
                        className="badge"
                        style={{
                          color: STATUS_COLORS[a.status] ?? '#374151',
                          background: STATUS_BG[a.status] ?? '#f3f4f6',
                        }}
                      >
                        {a.status}
                      </span>
                    </td>
                    <td className="text-muted text-sm">
                      {new Date(a.createdAt).toLocaleDateString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {!loading && totalCount > 0 && (
          <div className="card-header" style={{ justifyContent: 'flex-end', gap: 12 }}>
            <span className="text-muted text-sm">Page {page} of {totalPages}</span>
            <div className="flex gap-2">
              <button
                className="btn btn-secondary btn-sm"
                disabled={page <= 1}
                onClick={() => setPage(p => Math.max(1, p - 1))}
              >
                ← Prev
              </button>
              <button
                className="btn btn-secondary btn-sm"
                disabled={page >= totalPages}
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              >
                Next →
              </button>
            </div>
          </div>
        )}
      </div>
    </>
  )
}
