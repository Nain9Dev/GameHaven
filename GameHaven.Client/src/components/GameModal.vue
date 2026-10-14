<script setup lang="ts">
import type { Game } from '../types'

defineProps<{
  game: Game | null
  inCart: boolean
  isOwned: boolean
}>()

defineEmits<{
  (e: 'close'): void
  (e: 'add-cart', gameId: string): void
}>()
</script>

<template>
  <div class="modal-backdrop" v-if="game" @click.self="$emit('close')">
    <div class="modal-card glass-panel">
      <button class="modal-close" @click="$emit('close')" aria-label="Close modal">✕</button>

      <div class="modal-media">
        <img :src="game.coverImageUrl" :alt="game.title" class="modal-img" />
        <div class="modal-overlay"></div>
        <div class="modal-badges">
          <span class="badge badge-genre">{{ game.genre }}</span>
          <span class="badge badge-discount" v-if="game.discountPercentage > 0">-{{ game.discountPercentage }}%</span>
          <span class="badge badge-rating font-mono">★ {{ game.ratingPercentage }}% Positive</span>
        </div>
      </div>

      <div class="modal-content">
        <h2 class="modal-title">{{ game.title }}</h2>

        <div class="meta-row">
          <div class="meta-item">
            <span class="meta-label">Developer</span>
            <span class="meta-val">{{ game.developer }}</span>
          </div>
          <div class="meta-item">
            <span class="meta-label">Publisher</span>
            <span class="meta-val">{{ game.publisher }}</span>
          </div>
          <div class="meta-item">
            <span class="meta-label">Release Date</span>
            <span class="meta-val font-mono">{{ new Date(game.releaseDate).toLocaleDateString() }}</span>
          </div>
        </div>

        <div class="modal-desc">
          <h4>About the Game</h4>
          <p>{{ game.description }}</p>
        </div>

        <div class="modal-footer">
          <div class="modal-price">
            <span class="base-price font-mono" v-if="game.discountPercentage > 0">€{{ game.basePrice.toFixed(2) }}</span>
            <span class="current-price font-mono">€{{ game.currentPrice.toFixed(2) }}</span>
          </div>

          <div class="modal-actions">
            <span class="owned-indicator" v-if="isOwned">✓ In Your Library</span>
            <button
              v-else
              class="btn-primary"
              :disabled="inCart"
              @click="$emit('add-cart', game.id)"
            >
              {{ inCart ? 'In Shopping Cart' : 'Add to Cart' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  z-index: 200;
  background: rgba(4, 5, 8, 0.85);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
  animation: fadeIn 180ms ease-out;
}

.modal-card {
  position: relative;
  width: 100%;
  max-width: 680px;
  background: var(--bg-surface);
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.85);
  animation: slideUp 220ms cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: rgba(0, 0, 0, 0.6);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  font-size: 0.875rem;
}

.modal-close:hover {
  background: rgba(0, 0, 0, 0.9);
}

.modal-media {
  position: relative;
  width: 100%;
  height: 260px;
}

.modal-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.modal-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, transparent 40%, var(--bg-surface) 100%);
}

.modal-badges {
  position: absolute;
  bottom: 1rem;
  left: 1.5rem;
  display: flex;
  gap: 0.5rem;
}

.modal-content {
  padding: 1.5rem;
}

.modal-title {
  font-size: 1.5rem;
  font-weight: 800;
  color: #fff;
  margin-bottom: 1rem;
}

.meta-row {
  display: flex;
  gap: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--line-dim);
  margin-bottom: 1.25rem;
}

.meta-item {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
}

.meta-label {
  font-size: 0.6875rem;
  color: var(--text-dim);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.meta-val {
  font-size: 0.8125rem;
  color: var(--text-main);
  font-weight: 500;
}

.modal-desc h4 {
  font-size: 0.8125rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-dim);
  margin-bottom: 0.5rem;
}

.modal-desc p {
  font-size: 0.875rem;
  line-height: 1.6;
  color: var(--text-muted);
  margin-bottom: 1.5rem;
}

.modal-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid var(--line-dim);
  padding-top: 1.25rem;
}

.modal-price {
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
}

.base-price {
  font-size: 0.8125rem;
  color: var(--text-dim);
  text-decoration: line-through;
}

.current-price {
  font-size: 1.5rem;
  font-weight: 800;
  color: #fff;
}

.owned-indicator {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--accent-emerald);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { transform: translateY(20px) scale(0.98); opacity: 0; }
  to { transform: translateY(0) scale(1); opacity: 1; }
}
</style>
