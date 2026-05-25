import { useCallback, useMemo, useState } from 'react'
import api from '../api/client.js'
import { AuthContext } from './authContext.js'

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('doomscrolling.token'))
  const [user, setUser] = useState(() => {
    const storedUser = localStorage.getItem('doomscrolling.user')
    return storedUser ? JSON.parse(storedUser) : null
  })

  const saveSession = useCallback((authResponse) => {
    localStorage.setItem('doomscrolling.token', authResponse.token)
    localStorage.setItem(
      'doomscrolling.user',
      JSON.stringify({
        userId: authResponse.userId,
        username: authResponse.username,
        email: authResponse.email,
      }),
    )
    setToken(authResponse.token)
    setUser({
      userId: authResponse.userId,
      username: authResponse.username,
      email: authResponse.email,
    })
  }, [])

  const login = useCallback(async (email, password) => {
    const { data } = await api.post('/auth/login', { email, password })
    saveSession(data)
  }, [saveSession])

  const register = useCallback(async (username, email, password) => {
    const { data } = await api.post('/auth/register', { username, email, password })
    saveSession(data)
  }, [saveSession])

  const logout = useCallback(() => {
    localStorage.removeItem('doomscrolling.token')
    localStorage.removeItem('doomscrolling.user')
    setToken(null)
    setUser(null)
  }, [])

  const value = useMemo(
    () => ({
      isAuthenticated: Boolean(token),
      login,
      logout,
      register,
      token,
      user,
    }),
    [login, logout, register, token, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
