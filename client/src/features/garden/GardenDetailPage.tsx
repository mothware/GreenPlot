import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { gardensApi } from '../../api/gardens'
import toast from 'react-hot-toast'

export default function GardenDetailPage() {
  const { gardenId } = useParams<{ gardenId: string }>()
  const [showCreateBed, setShowCreateBed] = useState(false)
  const [bedName, setBedName] = useState('')
  const [widthFeet, setWidthFeet] = useState('4')
  const [lengthFeet, setLengthFeet] = useState('8')
  const qc = useQueryClient()

  const { data: gardens = [] } = useQuery({
    queryKey: ['gardens'],
    queryFn: () => gardensApi.list(),
  })
  const garden = gardens.find((g: { id: string }) => g.id === gardenId)

  const createBed = useMutation({
    mutationFn: () => gardensApi.createBed(gardenId!, {
      name: bedName,
      type: 'Raised' as const,
      shape: 'Rectangle' as const,
      widthFeet: parseFloat(widthFeet),
      lengthFeet: parseFloat(lengthFeet),
      gridCellSizeFeet: 1,
    }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['gardens'] })
      setShowCreateBed(false)
      setBedName('')
      toast.success('Bed created!')
    },
    onError: () => toast.error('Could not create bed.'),
  })

  return (
    <div className="p-8 max-w-4xl">
      <div className="mb-6">
        <Link to="/gardens" className="text-sm text-leaf-600 hover:underline">← All gardens</Link>
        <h2 className="font-display text-3xl font-bold text-gray-900 mt-2">
          {garden?.name ?? 'Garden'}
        </h2>
        {garden?.hardinessZone && (
          <p className="text-sm text-leaf-600 font-medium">
            Zone {garden.hardinessZone}
            {garden.lastFrostDate && ` · Last frost: ${garden.lastFrostDate}`}
          </p>
        )}
      </div>

      <div className="flex items-center justify-between mb-4">
        <h3 className="font-display text-xl font-semibold">Beds</h3>
        <button onClick={() => setShowCreateBed(true)} className="btn-primary">+ Add Bed</button>
      </div>

      {showCreateBed && (
        <div className="card mb-5">
          <h4 className="font-display text-base font-semibold mb-3">New Bed</h4>
          <div className="grid grid-cols-2 gap-3">
            <div className="col-span-2">
              <label className="label">Bed name</label>
              <input className="input" value={bedName} onChange={e => setBedName(e.target.value)} placeholder="Bed 1" />
            </div>
            <div>
              <label className="label">Width (ft)</label>
              <input className="input font-mono" type="number" value={widthFeet} onChange={e => setWidthFeet(e.target.value)} min="1" max="100" />
            </div>
            <div>
              <label className="label">Length (ft)</label>
              <input className="input font-mono" type="number" value={lengthFeet} onChange={e => setLengthFeet(e.target.value)} min="1" max="100" />
            </div>
          </div>
          <div className="flex gap-2 mt-3">
            <button className="btn-primary" onClick={() => createBed.mutate()} disabled={!bedName || createBed.isPending}>
              {createBed.isPending ? 'Creating…' : 'Create'}
            </button>
            <button className="btn-secondary" onClick={() => setShowCreateBed(false)}>Cancel</button>
          </div>
        </div>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {/* Beds will be loaded here once we add per-garden bed listing */}
        <div className="card border-dashed text-center py-8 text-gray-400">
          <p className="text-3xl mb-2">🛖</p>
          <p className="text-sm">Create a bed to start planning</p>
        </div>
      </div>
    </div>
  )
}
