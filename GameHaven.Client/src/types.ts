export interface Game {
  id: string
  title: string
  description: string
  basePrice: number
  discountPercentage: number
  currentPrice: number
  coverImageUrl: string
  developer: string
  publisher: string
  genre: string
  ratingPercentage: number
  releaseDate: string
}

export interface CartItem {
  gameId: string
  priceAtAddedTime: number
}

export interface CartResponse {
  userId: string
  totalAmount: number
  items: CartItem[]
}

export interface OwnedGame {
  gameId: string
  purchaseDate: string
  playTimeMinutes: number
}

export interface LibraryResponse {
  userId: string
  ownedGames: OwnedGame[]
}
