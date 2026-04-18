import { apiClient } from './client'
import type { Plant, Variety, PagedResult } from '../types'

export const plantsApi = {
  list: (params?: {
    search?: string
    category?: string
    lifecycle?: string
    page?: number
    pageSize?: number
  }) => apiClient.get<PagedResult<Plant>>('/api/plants', { params }).then(r => r.data),

  get: (id: string) =>
    apiClient.get<Plant>(`/api/plants/${id}`).then(r => r.data),

  getVarieties: (plantId: string) =>
    apiClient.get<Variety[]>(`/api/plants/${plantId}/varieties`).then(r => r.data),

  create: (data: Partial<Plant>) =>
    apiClient.post<Plant>('/api/plants', data).then(r => r.data),
}
