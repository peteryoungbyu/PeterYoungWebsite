import { useEffect, useState } from 'react'
import './Myfuncomponent.css'

type TableRow = Record<string, unknown>

function formatCell(value: unknown) {
  if (value === null || value === undefined) return '—'
  if (typeof value === 'object') return JSON.stringify(value)
  return String(value)
}

export default function Myfuncomponent() {
  const [rows, setRows] = useState<TableRow[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    async function loadRows() {
      try {
        const response = await fetch('/api/testtable', { signal: controller.signal })
        if (!response.ok) {
          throw new Error(`Request failed (${response.status})`)
        }

        const data: TableRow[] = await response.json()
        setRows(data)
      } catch (requestError) {
        if (!controller.signal.aborted) {
          setError(requestError instanceof Error ? requestError.message : 'Could not load table data.')
        }
      } finally {
        if (!controller.signal.aborted) setLoading(false)
      }
    }

    void loadRows()
    return () => controller.abort()
  }, [])

  const columns = rows.length > 0 ? Object.keys(rows[0]) : []

  return (
    <section className="table-view" aria-labelledby="testtable-heading">
      <h2 id="testtable-heading">Test table</h2>
      {loading && <p role="status">Loading table…</p>}
      {error && <p className="table-error" role="alert">{error}</p>}
      {!loading && !error && rows.length === 0 && <p>The table has no rows.</p>}
      {!loading && !error && rows.length > 0 && (
        <div className="table-scroll">
          <table>
            <thead>
              <tr>{columns.map((column) => <th key={column} scope="col">{column}</th>)}</tr>
            </thead>
            <tbody>
              {rows.map((row, rowIndex) => (
                <tr key={rowIndex}>
                  {columns.map((column) => <td key={column}>{formatCell(row[column])}</td>)}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}
