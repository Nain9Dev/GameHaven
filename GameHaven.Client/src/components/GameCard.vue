<script setup lang="ts">
import type { Game } from '../types'

defineProps<{
  game: Game
  inCart: boolean
  isOwned: boolean
}>()

defineEmits<{
  (e: 'view-game', game: Game): void
  (e: 'add-cart', gameId: string): void
}>()
</script>

<template>
  <div class="game-card" @click="$emit('view-game', game)">
    <div class="card-media">
      <img :src="game.coverImageUrl" :alt="game.title" loading="lazy" class="card-img" />
      <div class="card-badges">
        <span class="badge badge-discount" v-if="game.discountPercentage > 0">-{{ game.discountPercentage }}%</span>
        <span class="badge badge-genre">{{ game.genre }}</span>
      </div>
      <div class="card-owned-tag" v-if="isOwned">
        <span>In Library</span>
      </div>
    </div>

    <div class="card-body">
      <div class="card-header">
        <h3 class="card-title" :title="game.title">{{ game.title }}</h3>
        <span class="rating-pill font-mono">★ {{ game.ratingPercentage }}%</span>
      </div>

      <div class="card-developer">{{ game.developer }}</div>

      <div class="card-footer">
        <div class="price-container">
          <span class="base-price font-mono" v-if="game.discountPercentage > 0">€{{ game.basePrice.toFixed(2) }}</span>
          <span class="current-price font-mono">€{{ game.currentPrice.toFixed(2) }}</span>
        </div>

        <button
          v-if="!isOwned"
          class="btn-cart"
          :class="{ added: inCart }"
          @click.stop="$emit('add-cart', game.id)"
          :disabled="inCart"
          :title="inCart ? 'Already in cart' : 'Add to cart'"
        >
          <span v-if="inCart">Added</span>
          <span v-else>+ Cart</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.game-card {
  display: flex;
  flex-direction: column;
  background: var(--bg-surface);
  border: 1px solid var(--line-dim);
  border-radius: var(--radius-lg);
  overflow: hidden;
  cursor: pointer;
  transition: transform var(--transition-smooth), border-color var(--transition-fast), box-shadow var(--transition-smooth);
}

.game-card:hover {
  transform: translateY(-4px);
  border-color: var(--line-bright);
  box-shadow: 0 12px 30px -4px rgba(0, 0, 0, 0.6);
}

.card-media {
  position: relative;
  width: 100%;
  aspect-ratio: 16 / 9;
  background: var(--bg-elevated);
  overflow: hidden;
}

.card-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 500ms ease;
}

.game-card:hover .card-img {
  transform: scale(1.06);
}

.card-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.35rem;
}

.card-owned-tag {
  position: absolute;
  bottom: 0.75rem;
  right: 0.75rem;
  background: rgba(16, 185, 129, 0.9);
  color: #fff;
  font-size: 0.6875rem;
  font-weight: 700;
  padding: 0.2rem 0.6rem;
  border-radius: var(--radius-sm);
  backdrop-filter: blur(8px);
}

.card-body {
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.card-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
}

.card-title {
  font-size: 1rem;
  font-weight: 700;
  color: #fff;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.rating-pill {
  font-size: 0.6875rem;
  font-weight: 700;
  color: var(--accent-emerald);
  flex-shrink: 0;
}

.card-developer {
  font-size: 0.75rem;
  color: var(--text-dim);
  margin-bottom: 1.25rem;
}

.card-footer {
  margin-top: auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid var(--line-dim);
  padding-top: 0.875rem;
}

.price-container {
  display: flex;
  flex-direction: column;
}

.base-price {
  font-size: 0.6875rem;
  color: var(--text-dim);
  text-decoration: line-through;
}

.current-price {
  font-size: 1.125rem;
  font-weight: 700;
  color: #fff;
}

.btn-cart {
  padding: 0.4rem 0.85rem;
  font-size: 0.75rem;
  font-weight: 600;
  border-radius: var(--radius-sm);
  background: var(--bg-elevated);
  color: var(--text-main);
  border: 1px solid var(--line-dim);
}

.btn-cart:hover:not(:disabled) {
  background: var(--accent-primary);
  border-color: var(--accent-primary);
  color: #fff;
}

.btn-cart.added {
  background: rgba(16, 185, 129, 0.15);
  border-color: rgba(16, 185, 129, 0.4);
  color: var(--accent-emerald);
  cursor: default;
}
</style>
