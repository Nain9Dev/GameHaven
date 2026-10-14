<script setup lang="ts">
import type { Game } from '../types'

defineProps<{
  game: Game | undefined
}>()

defineEmits<{
  (e: 'view-game', game: Game): void
  (e: 'add-cart', gameId: string): void
}>()
</script>

<template>
  <section class="hero-banner" v-if="game">
    <div class="hero-backdrop" :style="{ backgroundImage: `url(${game.coverImageUrl})` }"></div>
    <div class="hero-overlay"></div>
    
    <div class="hero-content">
      <div class="hero-tags">
        <span class="badge badge-genre">{{ game.genre }}</span>
        <span class="badge badge-discount" v-if="game.discountPercentage > 0">-{{ game.discountPercentage }}%</span>
        <span class="badge badge-rating font-mono">★ {{ game.ratingPercentage }}% Rating</span>
      </div>

      <h1 class="hero-title">{{ game.title }}</h1>
      <p class="hero-desc">{{ game.description }}</p>

      <div class="hero-footer">
        <div class="price-block">
          <span class="base-price font-mono" v-if="game.discountPercentage > 0">€{{ game.basePrice.toFixed(2) }}</span>
          <span class="current-price font-mono">€{{ game.currentPrice.toFixed(2) }}</span>
        </div>

        <div class="hero-actions">
          <button class="btn-primary" @click="$emit('add-cart', game.id)">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Add to Cart
          </button>
          <button class="btn-secondary" @click="$emit('view-game', game)">
            View Details
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.hero-banner {
  position: relative;
  border-radius: var(--radius-lg);
  overflow: hidden;
  margin: 1.5rem 0 2.5rem 0;
  min-height: 380px;
  display: flex;
  align-items: flex-end;
  border: 1px solid var(--line-dim);
  box-shadow: var(--shadow-card);
}

.hero-backdrop {
  position: absolute;
  inset: 0;
  background-size: cover;
  background-position: center;
  filter: brightness(0.65) saturate(1.2);
  transform: scale(1.02);
  transition: transform 600ms ease;
}

.hero-banner:hover .hero-backdrop {
  transform: scale(1.05);
}

.hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(9, 10, 15, 0.2) 0%, rgba(9, 10, 15, 0.95) 100%),
              linear-gradient(90deg, rgba(9, 10, 15, 0.9) 0%, transparent 60%);
}

.hero-content {
  position: relative;
  z-index: 2;
  padding: 2.5rem;
  max-width: 680px;
}

.hero-tags {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.hero-title {
  font-size: 2.25rem;
  font-weight: 800;
  letter-spacing: -0.03em;
  line-height: 1.15;
  margin-bottom: 0.75rem;
  color: #fff;
}

.hero-desc {
  font-size: 0.9375rem;
  color: var(--text-muted);
  line-height: 1.6;
  margin-bottom: 1.5rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.hero-footer {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.price-block {
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
}

.base-price {
  font-size: 0.875rem;
  color: var(--text-dim);
  text-decoration: line-through;
}

.current-price {
  font-size: 1.75rem;
  font-weight: 800;
  color: #fff;
}

.hero-actions {
  display: flex;
  gap: 0.75rem;
}
</style>
