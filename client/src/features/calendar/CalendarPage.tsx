import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { calendarApi } from '../../api/calendar'
import { format, startOfMonth, endOfMonth, addMonths, subMonths } from 'date-fns'
import type { CalendarEvent } from '../../types'

const EVENT_COLORS: Record<string, string> = {
  Sown: 'bg-amber-100 text-amber-800 border-amber-200',
  Germinated: 'bg-green-100 text-green-800 border-green-200',
  Transplanted: 'bg-leaf-100 text-leaf-800 border-leaf-200',
  Harvested: 'bg-tomato-100 text-tomato-800 border border-red-200',
  Watered: 'bg-blue-100 text-blue-800 border-blue-200',
  default: 'bg-paper-100 text-gray-700 border-paper-200',
}

export default function CalendarPage() {
  const [currentMonth, setCurrentMonth] = useState(new Date())

  const from = startOfMonth(currentMonth)
  const to = endOfMonth(currentMonth)

  const { data: events = [], isLoading } = useQuery({
    queryKey: ['calendar', format(from, 'yyyy-MM'), 'events'],
    queryFn: () => calendarApi.getEvents({
      from: from.toISOString(),
      to: to.toISOString(),
    }),
  })

  return (
    <div className="p-8 max-w-4xl">
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-display text-3xl font-bold text-gray-900">Calendar</h2>
        <div className="flex items-center gap-2">
          <button className="btn-secondary" onClick={() => setCurrentMonth(m => subMonths(m, 1))}>←</button>
          <span className="font-display text-lg font-semibold min-w-[9rem] text-center">
            {format(currentMonth, 'MMMM yyyy')}
          </span>
          <button className="btn-secondary" onClick={() => setCurrentMonth(m => addMonths(m, 1))}>→</button>
        </div>
      </div>

      {isLoading ? (
        <p className="text-gray-400">Loading events…</p>
      ) : events.length === 0 ? (
        <div className="text-center py-16 text-gray-400">
          <p className="text-4xl mb-3">📅</p>
          <p>No activities logged this month.</p>
          <p className="text-sm mt-1">Start logging activities in the bed designer.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {events.map((event: CalendarEvent) => (
            <div
              key={event.id}
              className={`flex items-start gap-3 p-3 rounded-xl border text-sm ${EVENT_COLORS[event.eventType] ?? EVENT_COLORS.default}`}
            >
              <div className="font-mono text-xs shrink-0 mt-0.5 opacity-70">
                {format(new Date(event.startDate), 'MMM d')}
              </div>
              <div className="flex-1 min-w-0">
                <p className="font-medium truncate">{event.title}</p>
                {event.gardenName && (
                  <p className="text-[11px] opacity-70 truncate">
                    {event.gardenName} {event.bedName && `· ${event.bedName}`}
                  </p>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
