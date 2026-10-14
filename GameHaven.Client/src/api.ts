import type { Game, CartResponse, LibraryResponse } from './types'
import { MOCK_GAMES } from './data/mockCatalog'

const DEMO_USER_ID = 'e9f4c3a2-1111-4000-8000-000000000001'

export class GameHavenApiClient {
  private isOnline = false
  private localCart: string[] = []
  private localLibrary: string[] = ['a1b2c3d4-0005-4000-8000-000000000005']

  constructor() {
    this.checkHealth()
  }

  async checkHealth(): Promise<boolean> {
    try {
      const res = await fetch('/api/health', { method: 'GET', signal: AbortSignal.timeout(2000) })
      this.isOnline = res.ok
    } catch {
      this.isOnline = false
    }
    return this.isOnline
  }

  getStatus(): boolean {
    return this.isOnline
  }

  getDemoUserId(): string {
    return DEMO_USER_ID
  }

  async getGames(): Promise<Game[]> {
    try {
      const res = await fetch('/api/games', { signal: AbortSignal.timeout(3000) })
      if (res.ok) {
        this.isOnline = true
        const data = await res.json()
        if (Array.isArray(data) && data.length > 0) return data
      }
    } catch {
      this.isOnline = false
    }
    return MOCK_GAMES
  }

  async getGameById(id: string): Promise<Game | null> {
    try {
      const res = await fetch(`/api/games/${id}`, { signal: AbortSignal.timeout(2000) })
      if (res.ok) {
        this.isOnline = true
        return await res.json()
      }
    } catch {
      this.isOnline = false
    }
    return MOCK_GAMES.find((g) => g.id === id) || null
  }

  async getCart(): Promise<CartResponse> {
    if (this.isOnline) {
      try {
        const res = await fetch(`/api/cart/${DEMO_USER_ID}`, { signal: AbortSignal.timeout(2000) })
        if (res.ok) return await res.json()
      } catch {
        this.isOnline = false
      }
    }

    // Local in-memory cart fallback
    const games = MOCK_GAMES.filter((g) => this.localCart.includes(g.id))
    return {
      userId: DEMO_USER_ID,
      totalAmount: games.reduce((sum, g) => sum + g.currentPrice, 0),
      items: games.map((g) => ({ gameId: g.id, priceAtAddedTime: g.currentPrice })),
    }
  }

  async addToCart(gameId: string): Promise<boolean> {
    if (this.isOnline) {
      try {
        const res = await fetch(`/api/cart/${DEMO_USER_ID}/add`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ gameId }),
          signal: AbortSignal.timeout(2000),
        })
        if (res.ok) return true
      } catch {
        this.isOnline = false
      }
    }

    if (!this.localCart.includes(gameId)) {
      this.localCart.push(gameId)
    }
    return true
  }

  async removeFromCart(gameId: string): Promise<boolean> {
    if (this.isOnline) {
      try {
        const res = await fetch(`/api/cart/${DEMO_USER_ID}/remove`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ gameId }),
          signal: AbortSignal.timeout(2000),
        })
        if (res.ok) return true
      } catch {
        this.isOnline = false
      }
    }

    this.localCart = this.localCart.filter((id) => id !== gameId)
    return true
  }

  async checkout(): Promise<boolean> {
    if (this.isOnline) {
      try {
        const res = await fetch(`/api/cart/${DEMO_USER_ID}/checkout`, {
          method: 'POST',
          signal: AbortSignal.timeout(2000),
        })
        if (res.ok) return true
      } catch {
        this.isOnline = false
      }
    }

    for (const id of this.localCart) {
      if (!this.localLibrary.includes(id)) {
        this.localLibrary.push(id)
      }
    }
    this.localCart = []
    return true
  }

  async getLibrary(): Promise<LibraryResponse> {
    if (this.isOnline) {
      try {
        const res = await fetch(`/api/library/${DEMO_USER_ID}`, { signal: AbortSignal.timeout(2000) })
        if (res.ok) return await res.json()
      } catch {
        this.isOnline = false
      }
    }

    return {
      userId: DEMO_USER_ID,
      ownedGames: this.localLibrary.map((id, index) => ({
        gameId: id,
        purchaseDate: new Date(Date.now() - (index + 1) * 86400000 * 4).toISOString(),
        playTimeMinutes: (index + 1) * 145 + 32,
      })),
    }
  }
}

export const api = new GameHavenApiClient()
