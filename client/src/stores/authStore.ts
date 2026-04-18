import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface AuthState {
  token: string | null
  userId: string | null
  displayName: string | null
  isAuthenticated: boolean
  setAuth: (token: string, userId: string, displayName: string) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      userId: null,
      displayName: null,
      isAuthenticated: false,

      setAuth: (token, userId, displayName) => {
        localStorage.setItem('gp_token', token)
        set({ token, userId, displayName, isAuthenticated: true })
      },

      clearAuth: () => {
        localStorage.removeItem('gp_token')
        set({ token: null, userId: null, displayName: null, isAuthenticated: false })
      },
    }),
    { name: 'gp-auth' }
  )
)
