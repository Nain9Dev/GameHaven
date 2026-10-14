<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Navbar from './components/Navbar.vue'
import HeroBanner from './components/HeroBanner.vue'
import GameCard from './components/GameCard.vue'
import GameModal from './components/GameModal.vue'
import CartDrawer from './components/CartDrawer.vue'
import LibraryView from './components/LibraryView.vue'
import { api } from './api'
import type { Game, OwnedGame } from './types'

// State
const activeTab = ref<'store' | 'library'>('store')
const isOnline = ref(false)
const games = ref<Game[]>([])
const cartIds = ref<string[]>([])
const ownedEntries = ref<OwnedGame[]>([])

const searchQuery = ref('')
const selectedGenre = ref('All')
const sortBy = ref<'featured' | 'rating' | 'price-asc' | 'price-desc'>('featured')

const selectedGame = ref<Game | null>(null)
const isCartOpen = ref(false)
const isCheckingOut = ref(false)
const toastMessage = ref<string | null>(null)

// Computed
const genres = computed(() => {
  const set = new Set(games.value.map((g) => g.genre))
  return ['All', ...Array.from(set)]
})

const featuredGame = computed(() => {
  return games.value.find((g) => g.ratingPercentage >= 95) || games.value[0]
})

const filteredGames = computed(() => {
  let list = games.value.filter((g) => {
    const matchesSearch = g.title.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                          g.developer.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesGenre = selectedGenre.value === 'All' || g.genre === selectedGenre.value
    return matchesSearch && matchesGenre
  })

  if (sortBy.value === 'rating') {
    list = [...list].sort((a, b) => b.ratingPercentage - a.ratingPercentage)
  } else if (sortBy.value === 'price-asc') {
    list = [...list].sort((a, b) => a.currentPrice - b.currentPrice)
  } else if (sortBy.value === 'price-desc') {
    list = [...list].sort((a, b) => b.currentPrice - a.currentPrice)
  }

  return list
})

const cartGames = computed(() => {
  return games.value.filter((g) => cartIds.value.includes(g.id))
})

const cartTotal = computed(() => {
  return cartGames.value.reduce((sum, g) => sum + g.currentPrice, 0)
})

const ownedGameIds = computed(() => {
  return ownedEntries.value.map((e) => e.gameId)
})

// Methods
const showToast = (msg: string) => {
  toastMessage.value = msg
  setTimeout(() => {
    toastMessage.value = null
  }, 3000)
}

const loadData = async () => {
  isOnline.value = await api.checkHealth()
  games.value = await api.getGames()

  const cart = await api.getCart()
  cartIds.value = cart.items.map((i) => i.gameId)

  const lib = await api.getLibrary()
  ownedEntries.value = lib.ownedGames
}

const handleAddToCart = async (gameId: string) => {
  if (cartIds.value.includes(gameId)) return
  await api.addToCart(gameId)
  cartIds.value.push(gameId)
  const game = games.value.find((g) => g.id === gameId)
  showToast(`Added "${game?.title || 'Game'}" to cart.`)
}

const handleRemoveFromCart = async (gameId: string) => {
  await api.removeFromCart(gameId)
  cartIds.value = cartIds.value.filter((id) => id !== gameId)
}

const handleCheckout = async () => {
  isCheckingOut.value = true
  await api.checkout()
  const lib = await api.getLibrary()
  ownedEntries.value = lib.ownedGames
  cartIds.value = []
  isCheckingOut.value = false
  isCartOpen.value = false
  showToast('Checkout successful! Games added to your library.')
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="app-shell">
    <div class="ambient-glow"></div>

    <Navbar
      :active-tab="activeTab"
      :cart-count="cartIds.length"
      :is-online="isOnline"
      @set-tab="activeTab = $event"
      @open-cart="isCartOpen = true"
    />

    <main class="container main-layout">
      <!-- Storefront Tab -->
      <div v-if="activeTab === 'store'">
        <HeroBanner
          :game="featuredGame"
          @view-game="selectedGame = $event"
          @add-cart="handleAddToCart"
        />

        <!-- Filter Controls -->
        <div class="filter-bar glass-panel">
          <div class="search-box">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by title or developer..."
              class="search-input"
            />
          </div>

          <div class="filter-actions">
            <div class="genre-pills">
              <button
                v-for="genre in genres"
                :key="genre"
                class="genre-btn"
                :class="{ active: selectedGenre === genre }"
                @click="selectedGenre = genre"
              >
                {{ genre }}
              </button>
            </div>

            <div class="sort-select-wrapper">
              <select v-model="sortBy" class="sort-select font-mono">
                <option value="featured">Featured</option>
                <option value="rating">Top Rated</option>
                <option value="price-asc">Price: Low to High</option>
                <option value="price-desc">Price: High to Low</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Catalog Grid -->
        <section class="catalog-section">
          <div class="section-title-row">
            <h2 class="section-title">Indie Catalog</h2>
            <span class="catalog-count font-mono">{{ filteredGames.length }} Titles</span>
          </div>

          <div v-if="filteredGames.length === 0" class="no-results">
            <p>No games matched your search criteria.</p>
          </div>

          <div v-else class="games-grid">
            <GameCard
              v-for="game in filteredGames"
              :key="game.id"
              :game="game"
              :in-cart="cartIds.includes(game.id)"
              :is-owned="ownedGameIds.includes(game.id)"
              @view-game="selectedGame = $event"
              @add-cart="handleAddToCart"
            />
          </div>
        </section>
      </div>

      <!-- Library Tab -->
      <div v-else-if="activeTab === 'library'">
        <LibraryView :owned-entries="ownedEntries" :catalog="games" />
      </div>
    </main>

    <!-- Modals and Drawers -->
    <GameModal
      :game="selectedGame"
      :in-cart="selectedGame ? cartIds.includes(selectedGame.id) : false"
      :is-owned="selectedGame ? ownedGameIds.includes(selectedGame.id) : false"
      @close="selectedGame = null"
      @add-cart="handleAddToCart"
    />

    <CartDrawer
      :is-open="isCartOpen"
      :cart-games="cartGames"
      :total-amount="cartTotal"
      :is-checking-out="isCheckingOut"
      @close="isCartOpen = false"
      @remove="handleRemoveFromCart"
      @checkout="handleCheckout"
    />

    <!-- Toast Notification -->
    <div class="toast-popup" v-if="toastMessage">
      <span>{{ toastMessage }}</span>
    </div>

    <!-- Architectural Footer Badge -->
    <footer class="app-footer">
      <div class="container footer-content font-mono">
        <span>GameHaven Case Study · .NET 10 LTS & Vue 3 Clean Architecture</span>
        <a href="https://github.com/Nain9Dev/GameHaven" target="_blank" rel="noopener noreferrer" class="footer-link">
          GitHub Repository ↗
        </a>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.app-shell {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-layout {
  flex: 1;
}

.filter-bar {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  padding: 0.875rem 1.25rem;
  margin-bottom: 2rem;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: var(--bg-deep);
  border: 1px solid var(--line-dim);
  border-radius: var(--radius-md);
  padding: 0.5rem 0.875rem;
  flex: 1;
  min-width: 240px;
  max-width: 380px;
  color: var(--text-dim);
}

.search-input {
  background: none;
  border: none;
  color: #fff;
  font-size: 0.875rem;
  width: 100%;
}

.search-input::placeholder {
  color: var(--text-dim);
}

.filter-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.genre-pills {
  display: flex;
  gap: 0.4rem;
  overflow-x: auto;
}

.genre-btn {
  padding: 0.35rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: var(--text-muted);
  border-radius: var(--radius-sm);
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid var(--line-dim);
}

.genre-btn:hover {
  color: #fff;
  border-color: var(--line-bright);
}

.genre-btn.active {
  background: var(--accent-primary);
  border-color: var(--accent-primary);
  color: #fff;
  font-weight: 600;
}

.sort-select {
  background: var(--bg-deep);
  border: 1px solid var(--line-dim);
  border-radius: var(--radius-md);
  padding: 0.45rem 0.75rem;
  font-size: 0.75rem;
  color: var(--text-muted);
}

.section-title-row {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 800;
  color: #fff;
}

.catalog-count {
  font-size: 0.75rem;
  color: var(--text-dim);
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
  gap: 1.5rem;
  margin-bottom: 4rem;
}

.no-results {
  text-align: center;
  padding: 4rem 1rem;
  color: var(--text-dim);
}

.toast-popup {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  z-index: 300;
  background: rgba(16, 18, 26, 0.95);
  border: 1px solid var(--accent-primary);
  color: #fff;
  padding: 0.75rem 1.25rem;
  border-radius: var(--radius-md);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.7);
  font-size: 0.875rem;
  animation: slideToast 200ms ease-out;
}

.app-footer {
  border-top: 1px solid var(--line-dim);
  background: var(--bg-surface);
  padding: 1.5rem 0;
  margin-top: auto;
}

.footer-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 0.75rem;
  color: var(--text-dim);
}

.footer-link {
  color: var(--accent-cyan);
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}

@keyframes slideToast {
  from { transform: translateY(20px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
</style>
