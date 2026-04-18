import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { plantsApi } from '../../api/plants'
import type { Plant, PlantCategory } from '../../types'
import clsx from 'clsx'
import AddFromPacketModal from './AddFromPacketModal'

const CATEGORIES: Array<{ value: PlantCategory | ''; label: string }> = [
  { value: '', label: 'All' },
  { value: 'Vegetable', label: 'Vegetables' },
  { value: 'Herb', label: 'Herbs' },
  { value: 'Fruit', label: 'Fruits' },
  { value: 'Flower', label: 'Flowers' },
  { value: 'CoverCrop', label: 'Cover Crops' },
]

const SUN_LABELS: Record<string, string> = {
  FullSun: '☀️ Full sun',
  PartialSun: '⛅ Partial',
  Shade: '🌥️ Shade',
}

export default function CatalogPage() {
  const [search, setSearch] = useState('')
  const [category, setCategory] = useState<PlantCategory | ''>('')
  const [page, setPage] = useState(1)
  const [showPacketModal, setShowPacketModal] = useState(false)

  const { data, isLoading } = useQuery({
    queryKey: ['plants', search, category, page],
    queryFn: () => plantsApi.list({
      search: search || undefined,
      category: category || undefined,
      page,
      pageSize: 24,
    }),
  })

  const plants = data?.items ?? []
  const total = data?.totalCount ?? 0

  return (
    <div className="p-8">
      {showPacketModal && (
        <AddFromPacketModal plants={plants} onClose={() => setShowPacketModal(false)} />
      )}

      <div className="flex items-center justify-between mb-6">
        <h2 className="font-display text-3xl font-bold text-gray-900">Plant Catalog</h2>
        <button className="btn-primary" onClick={() => setShowPacketModal(true)}>
          + Log from seed packet
        </button>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-2 mb-4">
        <input
          className="input max-w-xs"
          placeholder="Search plants…"
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(1) }}
        />
        <div className="flex gap-1">
          {CATEGORIES.map(({ value, label }) => (
            <button
              key={value}
              onClick={() => { setCategory(value); setPage(1) }}
              className={clsx(
                'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
                category === value
                  ? 'bg-leaf-600 text-white'
                  : 'bg-white border border-paper-200 text-gray-600 hover:bg-paper-100'
              )}
            >
              {label}
            </button>
          ))}
        </div>
      </div>

      {isLoading ? (
        <div className="text-gray-400 py-8">Loading catalog…</div>
      ) : plants.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          <p className="text-4xl mb-3">🔍</p>
          <p>No plants found matching your search.</p>
        </div>
      ) : (
        <>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3 mb-6">
            {plants.map((plant: Plant) => (
              <div key={plant.id} className="card hover:shadow-md transition-shadow cursor-default">
                <p className="font-display font-semibold text-gray-900 truncate">{plant.commonName}</p>
                <p className="text-xs text-gray-400 italic truncate">{plant.scientificName}</p>
                <div className="flex flex-wrap gap-1 mt-2">
                  <span className="text-[10px] bg-paper-100 text-gray-600 rounded px-1.5 py-0.5">{plant.category}</span>
                  <span className="text-[10px] bg-paper-100 text-gray-600 rounded px-1.5 py-0.5">{SUN_LABELS[plant.sunRequirement]}</span>
                </div>
                {plant.daysToMaturityMin && (
                  <p className="text-xs text-gray-400 mt-1.5 font-mono">
                    {plant.daysToMaturityMin}
                    {plant.daysToMaturityMax && plant.daysToMaturityMax !== plant.daysToMaturityMin
                      ? `–${plant.daysToMaturityMax}`
                      : ''} days to maturity
                  </p>
                )}
                <p className="text-xs text-leaf-600 mt-1">{plant.varietyCount} {plant.varietyCount === 1 ? 'variety' : 'varieties'}</p>
              </div>
            ))}
          </div>

          {/* Pagination */}
          {total > 24 && (
            <div className="flex items-center justify-between text-sm text-gray-500">
              <span>{total} plants total</span>
              <div className="flex gap-2">
                <button className="btn-secondary" disabled={page === 1} onClick={() => setPage(p => p - 1)}>← Prev</button>
                <span className="px-3 py-2">Page {page}</span>
                <button className="btn-secondary" disabled={page * 24 >= total} onClick={() => setPage(p => p + 1)}>Next →</button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  )
}
