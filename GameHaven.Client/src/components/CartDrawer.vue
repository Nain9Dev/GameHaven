<script setup lang="ts">
import type { Game } from '../types'

defineProps<{
  isOpen: boolean
  cartGames: Game[]
  totalAmount: number
  isCheckingOut: boolean
}>()

defineEmits<{
  (e: 'close'): void
  (e: 'remove', gameId: string): void
  (e: 'checkout'): void
}>()
</script>

<template>
  <div class="drawer-backdrop" v-if="isOpen" @click.self="$emit('close')">
    <div class="drawer-panel">
      <div class="drawer-header">
        <div class="drawer-title-group">
          <h3>Shopping Cart</h3>
          <span class="count-badge font-mono">{{ cartGames.length }}</span>
        </div>
        <button class="close-btn" @click="$emit('close')">✕</button>
      </div>

      <div class="drawer-content">
        <div v-if="cartGames.length === 0" class="empty-state">
          <div class="empty-icon">🛒</div>
          <h4>Your cart is empty</h4>
          <p>Discover hand-crafted indie titles in the storefront.</p>
        </div>

        <div v-else class="cart-list">
          <div v-for="game in cartGames" :key="game.id" class="cart-item">
            <img :src="game.coverImageUrl" :alt="game.title" class="item-img" />
            <div class="item-info">
              <span class="item-title">{{ game.title }}</span>
              <span class="item-genre">{{ game.genre }}</span>
              <span class="item-price font-mono">€{{ game.currentPrice.toFixed(2) }}</span>
            </div>
            <button class="item-remove" @click="$emit('remove', game.id)" title="Remove item">
              🗑
            </button>
          </div>
        </div>
      </div>

      <div class="drawer-footer" v-if="cartGames.length > 0">
        <div class="total-row">
          <span>Estimated Total</span>
          <span class="total-val font-mono">€{{ totalAmount.toFixed(2) }}</span>
        </div>
        <button class="btn-primary checkout-btn" :disabled="isCheckingOut" @click="$emit('checkout')">
          <span v-if="isCheckingOut">Processing Transaction...</span>
          <span v-else>Complete Checkout</span>
        </button>
        <span class="checkout-hint">Mock sandbox payment processed via Clean Architecture domain rules.</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.drawer-backdrop {
  position: fixed;
  inset: 0;
  z-index: 250;
  background: rgba(4, 5, 8, 0.75);
  backdrop-filter: blur(4px);
  -webkit-backdrop-filter: blur(4px);
  display: flex;
  justify-content: flex-end;
}

.drawer-panel {
  width: 100%;
  max-width: 440px;
  height: 100%;
  background: var(--bg-surface);
  border-left: 1px solid var(--line-dim);
  display: flex;
  flex-direction: column;
  animation: slideLeft 250ms cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: -10px 0 40px rgba(0, 0, 0, 0.7);
}

.drawer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem;
  border-bottom: 1px solid var(--line-dim);
}

.drawer-title-group {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.drawer-title-group h3 {
  font-size: 1.125rem;
  font-weight: 700;
  color: #fff;
}

.count-badge {
  background: rgba(99, 102, 241, 0.2);
  color: #a5b4fc;
  padding: 0.15rem 0.5rem;
  border-radius: var(--radius-full);
  font-size: 0.75rem;
  font-weight: 600;
}

.close-btn {
  color: var(--text-muted);
  font-size: 1rem;
}

.close-btn:hover {
  color: #fff;
}

.drawer-content {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  text-align: center;
  color: var(--text-dim);
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.6;
}

.empty-state h4 {
  font-size: 1.125rem;
  color: #fff;
  margin-bottom: 0.25rem;
}

.cart-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cart-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: var(--bg-elevated);
  padding: 0.75rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--line-dim);
}

.item-img {
  width: 60px;
  height: 60px;
  border-radius: var(--radius-sm);
  object-fit: cover;
}

.item-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
}

.item-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #fff;
}

.item-genre {
  font-size: 0.6875rem;
  color: var(--text-dim);
}

.item-price {
  font-size: 0.875rem;
  font-weight: 700;
  color: var(--accent-cyan);
}

.item-remove {
  color: var(--text-dim);
  padding: 0.4rem;
  font-size: 0.875rem;
}

.item-remove:hover {
  color: var(--accent-rose);
}

.drawer-footer {
  padding: 1.5rem;
  border-top: 1px solid var(--line-dim);
  background: var(--bg-deep);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.total-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-muted);
}

.total-val {
  font-size: 1.5rem;
  font-weight: 800;
  color: #fff;
}

.checkout-btn {
  width: 100%;
  padding: 0.875rem;
  font-size: 1rem;
}

.checkout-hint {
  font-size: 0.6875rem;
  color: var(--text-dim);
  text-align: center;
}

@keyframes slideLeft {
  from { transform: translateX(100%); }
  to { transform: translateX(0); }
}
</style>
