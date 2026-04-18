import { apiClient } from './client'
import type { Garden, Bed, BedDetail } from '../types'

export const gardensApi = {
  list: () => apiClient.get<Garden[]>('/api/gardens').then(r => r.data),

  create: (data: { name: string; zipCode?: string; notes?: string }) =>
    apiClient.post<Garden>('/api/gardens', data).then(r => r.data),

  getBed: (gardenId: string, bedId: string) =>
    apiClient.get<BedDetail>(`/api/gardens/${gardenId}/beds/${bedId}`).then(r => r.data),

  createBed: (gardenId: string, data: Partial<Bed>) =>
    apiClient.post<Bed>(`/api/gardens/${gardenId}/beds`, data).then(r => r.data),
}
