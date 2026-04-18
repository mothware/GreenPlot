import { apiClient } from './client'
import type { Planting, Activity } from '../types'

export const plantingsApi = {
  create: (data: {
    bedId: string
    varietyId: string
    seasonId: string
    seedLotId?: string
    startDate: string
    method: string
    cellIds: string[]
    notes?: string
  }) => apiClient.post<Planting>('/api/plantings', data).then(r => r.data),

  logActivity: (plantingId: string, data: {
    type: string
    occurredAt: string
    quantity?: number
    weightGrams?: number
    notes?: string
    photoUrls?: string[]
  }) => apiClient.post<Activity>(`/api/plantings/${plantingId}/activities`, data).then(r => r.data),
}
