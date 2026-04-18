import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { gardensApi } from '../../api/gardens'
import toast from 'react-hot-toast'
import type { Garden } from '../../types'

export default function GardenListPage() {
  const [showCreate, setShowCreate] = useState(false)
  const [name, setName] = useState('')
  const [zipCode, setZipCode] = useState('')
  const qc = useQueryClient()

  const { data: gardens = [], isLoading } = useQuery({
    queryKey: ['gardens'],
    queryFn: () => gardensApi.list(),
  })

  const create = useMutation({
    mutationFn: () => gardensApi.create({ name, zipCode: zipCode || undefined }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['gardens'] })
      setShowCreate(false)
      setName('')
      setZipCode('')
      toast.success('Garden created!')
    },
    onError: () => toast.error('Could not create garden.'),
  })

  return (
    <div className="p-8 max-w-4xl">
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-display text-3xl font-bold text-gray-900">My Gardens</h2>
        <button onClick={() => setShowCreate(true)} className="btn-primary">
          + New Garden
        </button>
      </div>

      {showCreate && (
        <div className="card mb-6">
          <h3 className="font-display text-lg font-semibold mb-4">Create Garden</h3>
          <div className="space-y-3">
            <div>
              <label className="label">Garden name</label>
              <input className="input" value={name} onChange={e => setName(e.target.value)} placeholder="Backyard raised beds" />
            </div>
            <div>
              <label className="label">Zip code (for frost dates)</label>
              <input className="input" value={zipCode} onChange={e => setZipCode(e.target.value)} placeholder="37601" />
            </div>
            <div className="flex gap-2 pt-1">
              <button className="btn-primary" onClick={() => create.mutate()} disabled={!name || create.isPending}>
                {create.isPending ? 'Creating…' : 'Create'}
              </button>
              <button className="btn-secondary" onClick={() => setShowCreate(false)}>Cancel</button>
            </div>
          </div>
        </div>
      )}

      {isLoading ? (
        <p className="text-gray-400">Loading…</p>
      ) : gardens.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          <p className="text-4xl mb-3">🌱</p>
          <p className="text-lg font-medium text-gray-600">No gardens yet</p>
          <p className="text-sm mt-1">Create your first garden to get started</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {gardens.map((g: Garden) => (
            <Link key={g.id} to={`/gardens/${g.id}`} className="card hover:shadow-md transition-shadow block">
              <h3 className="font-display text-xl font-semibold text-gray-900">{g.name}</h3>
              {g.hardinessZone && (
                <p className="text-sm text-leaf-600 font-medium mt-1">Zone {g.hardinessZone}</p>
              )}
              {g.lastFrostDate && (
                <p className="text-xs text-gray-500 mt-1">Last frost: {g.lastFrostDate}</p>
              )}
              <p className="text-sm text-gray-500 mt-2">
                {g.bedCount} {g.bedCount === 1 ? 'bed' : 'beds'}
              </p>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}
