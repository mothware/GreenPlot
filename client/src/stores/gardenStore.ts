import { create } from 'zustand'
import type { Garden, BedDetail } from '../types'

interface GardenState {
  selectedGardenId: string | null
  selectedBedId: string | null
  activeBedDetail: BedDetail | null
  setSelectedGarden: (id: string | null) => void
  setSelectedBed: (id: string | null) => void
  setActiveBedDetail: (bed: BedDetail | null) => void
}

export const useGardenStore = create<GardenState>((set) => ({
  selectedGardenId: null,
  selectedBedId: null,
  activeBedDetail: null,

  setSelectedGarden: (id) => set({ selectedGardenId: id }),
  setSelectedBed: (id) => set({ selectedBedId: id }),
  setActiveBedDetail: (bed) => set({ activeBedDetail: bed }),
}))
