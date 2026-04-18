import { useState, FormEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { varietiesApi } from '../../api/varieties'
import type { Plant } from '../../types'
import toast from 'react-hot-toast'
import clsx from 'clsx'

interface Props {
  plants: Plant[]
  onClose: () => void
}

export default function AddFromPacketModal({ plants, onClose }: Props) {
  const qc = useQueryClient()

  // Plant selection
  const [useExisting, setUseExisting] = useState(true)
  const [existingPlantId, setExistingPlantId] = useState('')
  const [newPlantName, setNewPlantName] = useState('')
  const [newPlantFamily, setNewPlantFamily] = useState('')
  const [newPlantCategory, setNewPlantCategory] = useState('Vegetable')

  // Variety / packet fields
  const [varietyName, setVarietyName] = useState('')
  const [supplier, setSupplier] = useState('')
  const [packetYear, setPacketYear] = useState(String(new Date().getFullYear()))
  const [daysToMaturity, setDaysToMaturity] = useState('')
  const [spacingInches, setSpacingInches] = useState('')
  const [packetSeedCount, setPacketSeedCount] = useState('')
  const [flavorNotes, setFlavorNotes] = useState('')
  const [colorNotes, setColorNotes] = useState('')
  const [notes, setNotes] = useState('')

  const mutation = useMutation({
    mutationFn: () => varietiesApi.addFromPacket({
      existingPlantId: useExisting ? existingPlantId || undefined : undefined,
      newPlantCommonName: !useExisting ? newPlantName : undefined,
      newPlantFamily: !useExisting ? newPlantFamily : undefined,
      newPlantCategory: !useExisting ? newPlantCategory : undefined,
      varietyName,
      supplier: supplier || undefined,
      packetYear: packetYear ? parseInt(packetYear) : undefined,
      daysToMaturity: daysToMaturity ? parseInt(daysToMaturity) : undefined,
      spacingInches: spacingInches ? parseFloat(spacingInches) : undefined,
      flavorNotes: flavorNotes || undefined,
      colorNotes: colorNotes || undefined,
      notes: notes || undefined,
      createSeedLot: true,
      packetSeedCount: packetSeedCount ? parseInt(packetSeedCount) : undefined,
    }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['plants'] })
      toast.success('Variety added from packet!')
      onClose()
    },
    onError: () => toast.error('Could not save variety.'),
  })

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    if (!varietyName.trim()) return toast.error('Variety name is required.')
    if (useExisting && !existingPlantId) return toast.error('Select a plant.')
    if (!useExisting && !newPlantName.trim()) return toast.error('Plant name is required.')
    mutation.mutate()
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg max-h-[90vh] flex flex-col">
        {/* Header */}
        <div className="p-5 border-b border-paper-200 flex items-center justify-between shrink-0">
          <div>
            <h2 className="font-display text-xl font-semibold text-gray-900">Log from seed packet</h2>
            <p className="text-xs text-gray-400 mt-0.5">Add a variety you own and track your inventory</p>
          </div>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-2xl leading-none">×</button>
        </div>

        <form onSubmit={handleSubmit} className="overflow-y-auto flex-1 p-5 space-y-5">
          {/* Plant section */}
          <section>
            <h3 className="text-sm font-semibold text-gray-700 mb-3">Plant</h3>
            <div className="flex gap-2 mb-3">
              <button
                type="button"
                onClick={() => setUseExisting(true)}
                className={clsx('flex-1 py-1.5 rounded-lg text-sm font-medium transition-colors',
                  useExisting ? 'bg-leaf-600 text-white' : 'bg-paper-100 text-gray-600 hover:bg-paper-200')}
              >
                From catalog
              </button>
              <button
                type="button"
                onClick={() => setUseExisting(false)}
                className={clsx('flex-1 py-1.5 rounded-lg text-sm font-medium transition-colors',
                  !useExisting ? 'bg-leaf-600 text-white' : 'bg-paper-100 text-gray-600 hover:bg-paper-200')}
              >
                New plant
              </button>
            </div>

            {useExisting ? (
              <div>
                <label className="label">Select plant</label>
                <select
                  className="input"
                  value={existingPlantId}
                  onChange={e => setExistingPlantId(e.target.value)}
                >
                  <option value="">— choose —</option>
                  {plants.map(p => (
                    <option key={p.id} value={p.id}>{p.commonName}</option>
                  ))}
                </select>
              </div>
            ) : (
              <div className="grid grid-cols-2 gap-3">
                <div className="col-span-2">
                  <label className="label">Plant common name</label>
                  <input className="input" value={newPlantName} onChange={e => setNewPlantName(e.target.value)} placeholder="e.g. Tomato" />
                </div>
                <div>
                  <label className="label">Family</label>
                  <input className="input" value={newPlantFamily} onChange={e => setNewPlantFamily(e.target.value)} placeholder="e.g. Solanaceae" />
                </div>
                <div>
                  <label className="label">Category</label>
                  <select className="input" value={newPlantCategory} onChange={e => setNewPlantCategory(e.target.value)}>
                    {['Vegetable','Herb','Fruit','Flower','CoverCrop'].map(c => (
                      <option key={c} value={c}>{c}</option>
                    ))}
                  </select>
                </div>
              </div>
            )}
          </section>

          {/* Variety / packet section */}
          <section>
            <h3 className="text-sm font-semibold text-gray-700 mb-3">Variety & packet details</h3>
            <div className="grid grid-cols-2 gap-3">
              <div className="col-span-2">
                <label className="label">Variety name <span className="text-tomato-500">*</span></label>
                <input className="input" value={varietyName} onChange={e => setVarietyName(e.target.value)} placeholder="e.g. Cherokee Purple" />
              </div>
              <div>
                <label className="label">Supplier</label>
                <input className="input" value={supplier} onChange={e => setSupplier(e.target.value)} placeholder="Baker Creek" />
              </div>
              <div>
                <label className="label">Packet year</label>
                <input className="input font-mono" type="number" value={packetYear} onChange={e => setPacketYear(e.target.value)} min="2000" max="2030" />
              </div>
              <div>
                <label className="label">Days to maturity</label>
                <input className="input font-mono" type="number" value={daysToMaturity} onChange={e => setDaysToMaturity(e.target.value)} placeholder="80" />
              </div>
              <div>
                <label className="label">Spacing (inches)</label>
                <input className="input font-mono" type="number" value={spacingInches} onChange={e => setSpacingInches(e.target.value)} placeholder="24" />
              </div>
              <div>
                <label className="label">Seed count in packet</label>
                <input className="input font-mono" type="number" value={packetSeedCount} onChange={e => setPacketSeedCount(e.target.value)} placeholder="25" />
              </div>
              <div>
                <label className="label">Color notes</label>
                <input className="input" value={colorNotes} onChange={e => setColorNotes(e.target.value)} placeholder="Deep purple" />
              </div>
              <div className="col-span-2">
                <label className="label">Flavor notes</label>
                <input className="input" value={flavorNotes} onChange={e => setFlavorNotes(e.target.value)} placeholder="Rich, complex, low acid" />
              </div>
              <div className="col-span-2">
                <label className="label">Additional notes</label>
                <textarea className="input" rows={2} value={notes} onChange={e => setNotes(e.target.value)} placeholder="Heirloom, open-pollinated…" />
              </div>
            </div>
          </section>
        </form>

        <div className="p-5 border-t border-paper-200 flex gap-2 justify-end shrink-0">
          <button type="button" className="btn-secondary" onClick={onClose}>Cancel</button>
          <button
            type="submit"
            className="btn-primary"
            disabled={mutation.isPending}
            onClick={handleSubmit as unknown as React.MouseEventHandler}
          >
            {mutation.isPending ? 'Saving…' : 'Save to my collection'}
          </button>
        </div>
      </div>
    </div>
  )
}
