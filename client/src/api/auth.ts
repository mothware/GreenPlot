import { apiClient } from './client'

export const authApi = {
  register: (email: string, password: string, displayName: string) =>
    apiClient.post('/api/auth/register', { email, password, displayName }).then(r => r.data),

  login: (email: string, password: string) =>
    apiClient.post<{ token: string; userId: string; displayName: string }>(
      '/api/auth/login', { email, password }
    ).then(r => r.data),

  me: () => apiClient.get('/api/auth/me').then(r => r.data),
}
