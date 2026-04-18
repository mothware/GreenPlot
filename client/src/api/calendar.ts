import { apiClient } from './client'
import type { CalendarEvent } from '../types'

export const calendarApi = {
  getEvents: (params: {
    from: string
    to: string
    gardenId?: string
    bedId?: string
  }) => apiClient.get<CalendarEvent[]>('/api/calendar/events', { params }).then(r => r.data),
}
