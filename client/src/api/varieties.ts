import { apiClient } from './client'
import type { Variety } from '../types'

export interface AddFromPacketPayload {
  existingPlantId?: string
  newPlantCommonName?: string
  newPlantFamily?: string
  newPlantCategory?: string
  varietyName: string
  supplier?: string
  packetYear?: number
  daysToMaturity?: number
  daysToGerminate?: number
  sowingDepthInches?: number
  spacingInches?: number
  diseaseResistance?: string
  flavorNotes?: string
  colorNotes?: string
  notes?: string
  createSeedLot?: boolean
  packetSeedCount?: number
  purchaseDate?: string
}

export interface AddFromPacketResult {
  variety: Variety
  seedLotId?: string
}

export const varietiesApi = {
  addFromPacket: (data: AddFromPacketPayload) =>
    apiClient.post<AddFromPacketResult>('/api/varieties/from-packet', data).then(r => r.data),
}
