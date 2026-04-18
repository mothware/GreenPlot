import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { DndContext, DragEndEvent, DragOverlay, useDraggable, useDroppable } from '@dnd-kit/core'
import { gardensApi } from '../../api/gardens'
import { plantingsApi } from '../../api/plantings'
import { plantsApi } from '../../api/plants'
import type { BedCell, Plant, Variety, PlantingSummary } from '../../types'
import clsx from 'clsx'
import toast from 'react-hot-toast'

// Family → color mapping for visual differentiation
const FAMILY_COLORS: Record<string, string> = {
  Solanaceae: 'bg-red-100 border-red-300 text-red-800',
  Brassicaceae: 'bg-purple-100 border-purple-300 text-purple-800',
  Cucurbitaceae: 'bg-green-100 border-green-300 text-green-800',
  Fabaceae: 'bg-yellow-100 border-yellow-300 text-yellow-800',
  Apiaceae: 'bg-orange-100 border-orange-300 text-orange-800',
  Alliaceae: 'bg-pink-100 border-pink-300 text-pink-800',
  Asteraceae: 'bg-amber-100 border-amber-300 text-amber-800',
  default: 'bg-leaf-100 border-leaf-300 text-leaf-800',
}

function familyColor(family: string) {
  return FAMILY_COLORS[family] ?? FAMILY_COLORS.default
}

function PlantCard({ plant }: { plant: Plant }) {
  const { attributes, listeners, setNodeRef, isDragging } = useDraggable({
    id: `plant-${plant.id}`,
    data: { plantId: plant.id, family: plant.family },
  })

  return (
    <div
      ref={setNodeRef}
      {...listeners}
      {...attributes}
      className={clsx(
        'cursor-grab active:cursor-grabbing select-none',
        'rounded-xl border px-3 py-2 text-xs font-medium transition-shadow',
        familyColor(plant.family),
        isDragging && 'opacity-40 shadow-lg'
      )}
    >
      <p className="font-semibold truncate">{plant.commonName}</p>
      <p className="text-[10px] opacity-70 truncate">{plant.family}</p>
      {plant.daysToMaturityMin && (
        <p className="font-mono text-[10px] mt-0.5 opacity-60">{plant.daysToMaturityMin}d</p>
      )}
    </div>
  )
}

function GridCell({
  cell,
  onSelect,
  isSelected,
}: {
  cell: BedCell
  onSelect: (cell: BedCell) => void
  isSelected: boolean
}) {
  const { setNodeRef, isOver } = useDroppable({ id: `cell-${cell.id}` })
  const p = cell.planting

  return (
    <div
      ref={setNodeRef}
      onClick={() => onSelect(cell)}
      className={clsx(
        'w-12 h-12 border rounded-lg flex items-center justify-center transition-all cursor-pointer',
        'text-[9px] font-medium text-center leading-tight overflow-hidden p-0.5',
        isOver && 'ring-2 ring-leaf-500 bg-leaf-50',
        isSelected && 'ring-2 ring-honey-400',
        p ? [familyColor(p.plantFamily), 'border'] : 'border-paper-200 bg-white hover:bg-paper-50'
      )}
      title={p ? `${p.varietyName} (${p.plantCommonName})` : `Cell (${cell.x}, ${cell.y})`}
    >
      {p && (
        <span className="truncate px-0.5">{p.varietyName.slice(0, 6)}</span>
      )}
    </div>
  )
}

function DetailsPanel({ cell, onClose }: { cell: BedCell | null; onClose: () => void }) {
  if (!cell?.planting) return null
  const p = cell.planting
  return (
    <aside className="w-64 bg-white border-l border-paper-200 p-4 shrink-0 overflow-y-auto">
      <div className="flex items-start justify-between mb-3">
        <div>
          <h3 className="font-display font-semibold text-gray-900">{p.varietyName}</h3>
          <p className="text-xs text-gray-500">{p.plantCommonName} · {p.plantFamily}</p>
        </div>
        <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-lg leading-none">×</button>
      </div>
      <div className="space-y-2 text-sm">
        <div className="flex justify-between">
          <span className="text-gray-500">State</span>
          <span className="font-medium">{p.state}</span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-500">Method</span>
          <span className="font-medium">{p.method}</span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-500">Start date</span>
          <span className="font-mono text-xs">{p.startDate}</span>
        </div>
      </div>
    </aside>
  )
}

export default function BedDesignerPage() {
  const { gardenId, bedId } = useParams<{ gardenId: string; bedId: string }>()
  const [selectedCell, setSelectedCell] = useState<BedCell | null>(null)
  const [searchQuery, setSearchQuery] = useState('')
  const qc = useQueryClient()

  const { data: bed, isLoading } = useQuery({
    queryKey: ['bed', gardenId, bedId],
    queryFn: () => gardensApi.getBed(gardenId!, bedId!),
    enabled: !!gardenId && !!bedId,
  })

  const { data: plantsResult } = useQuery({
    queryKey: ['plants', searchQuery],
    queryFn: () => plantsApi.list({ search: searchQuery || undefined, pageSize: 50 }),
  })

  const plants = plantsResult?.items ?? []

  const placePlanting = useMutation({
    mutationFn: ({ cellId, plantId }: { cellId: string; plantId: string }) =>
      plantingsApi.create({
        bedId: bedId!,
        varietyId: plantId, // simplified — in full implementation uses variety selection
        seasonId: 'default-season', // would be selected by user
        startDate: new Date().toISOString().slice(0, 10),
        method: 'DirectSow',
        cellIds: [cellId],
      }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['bed', gardenId, bedId] })
      toast.success('Planting added!')
    },
    onError: () => toast.error('Could not place planting.'),
  })

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over) return

    const cellId = (over.id as string).replace('cell-', '')
    const plantId = (active.id as string).replace('plant-', '')

    placePlanting.mutate({ cellId, plantId })
  }

  if (isLoading) {
    return <div className="p-8 text-gray-400">Loading bed…</div>
  }

  if (!bed) {
    return <div className="p-8 text-red-500">Bed not found.</div>
  }

  const cols = Math.round(bed.widthFeet / bed.gridCellSizeFeet)
  const rows = Math.round(bed.lengthFeet / bed.gridCellSizeFeet)

  const cellGrid: (BedCell | null)[][] = Array.from({ length: rows }, (_, r) =>
    Array.from({ length: cols }, (_, c) =>
      bed.cells.find(cell => cell.x === c && cell.y === r) ?? null
    )
  )

  return (
    <DndContext onDragEnd={handleDragEnd}>
      <div className="flex h-screen overflow-hidden">
        {/* Left: Plant library */}
        <aside className="w-52 bg-white border-r border-paper-200 flex flex-col shrink-0">
          <div className="p-3 border-b border-paper-200">
            <h3 className="font-display text-sm font-semibold text-gray-700 mb-2">Plant Library</h3>
            <input
              className="input text-xs"
              placeholder="Search plants…"
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
            />
          </div>
          <div className="flex-1 overflow-y-auto p-2 space-y-1.5">
            {plants.map((plant: Plant) => (
              <PlantCard key={plant.id} plant={plant} />
            ))}
            {plants.length === 0 && (
              <p className="text-xs text-gray-400 text-center py-4">No plants found</p>
            )}
          </div>
        </aside>

        {/* Center: Canvas */}
        <div className="flex-1 overflow-auto bg-paper-50 p-6">
          <div className="mb-4 flex items-center gap-3">
            <Link to={`/gardens/${gardenId}`} className="text-sm text-leaf-600 hover:underline">
              ← Garden
            </Link>
            <h2 className="font-display text-2xl font-bold text-gray-900">{bed.name}</h2>
            <span className="text-sm text-gray-400 font-mono">
              {bed.widthFeet} × {bed.lengthFeet} ft
            </span>
          </div>

          {/* Bed grid with wood-frame border */}
          <div
            className="inline-block p-3 rounded-2xl border-4 border-soil-400 bg-soil-300 shadow-lg"
            style={{ borderStyle: 'ridge' }}
          >
            <div
              className="grid gap-0.5"
              style={{ gridTemplateColumns: `repeat(${cols}, 3rem)` }}
            >
              {cellGrid.map((row, r) =>
                row.map((cell, c) =>
                  cell ? (
                    <GridCell
                      key={cell.id}
                      cell={cell}
                      isSelected={selectedCell?.id === cell.id}
                      onSelect={setSelectedCell}
                    />
                  ) : (
                    <div key={`${r}-${c}`} className="w-12 h-12 border border-dashed border-soil-500 rounded-lg bg-soil-300 opacity-30" />
                  )
                )
              )}
            </div>
          </div>

          <div className="mt-4 text-xs text-gray-400 font-mono">
            Drag a plant from the library onto a cell to place it
          </div>
        </div>

        {/* Right: Details */}
        <DetailsPanel cell={selectedCell} onClose={() => setSelectedCell(null)} />
      </div>
    </DndContext>
  )
}
