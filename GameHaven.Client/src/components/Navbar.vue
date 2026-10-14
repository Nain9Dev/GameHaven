<script setup lang="ts">
defineProps<{
  activeTab: 'store' | 'library'
  cartCount: number
  isOnline: boolean
}>()

defineEmits<{
  (e: 'set-tab', tab: 'store' | 'library'): void
  (e: 'open-cart'): void
}>()
</script>

<template>
  <header class="navbar">
    <div class="container nav-content">
      <div class="brand" @click="$emit('set-tab', 'store')">
        <div class="brand-icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round">
            <rect x="2" y="6" width="20" height="12" rx="4" />
            <line x1="6" y1="12" x2="10" y2="12" />
            <line x1="8" y1="10" x2="8" y2="14" />
            <circle cx="15" cy="11" r="1" fill="currentColor" />
            <circle cx="18" cy="13" r="1" fill="currentColor" />
          </svg>
        </div>
        <div class="brand-info">
          <span class="brand-title">GAME<span class="brand-accent">HAVEN</span></span>
          <span class="brand-subtitle font-mono">.NET 10 & VUE 3</span>
        </div>
      </div>

      <nav class="nav-links">
        <button
          class="nav-tab"
          :class="{ active: activeTab === 'store' }"
          @click="$emit('set-tab', 'store')"
        >
          Storefront
        </button>
        <button
          class="nav-tab"
          :class="{ active: activeTab === 'library' }"
          @click="$emit('set-tab', 'library')"
        >
          My Library
        </button>
      </nav>

      <div class="nav-actions">
        <!-- API Status Indicator -->
        <div class="status-pill" :class="{ online: isOnline, offline: !isOnline }" :title="isOnline ? 'Connected to local .NET 10 API (port 8180)' : 'API Offline / Using resilient local snapshot'">
          <span class="status-dot"></span>
          <span class="status-label font-mono">{{ isOnline ? 'API .NET 10' : 'OFFLINE SNAPSHOT' }}</span>
        </div>

        <!-- Cart Button -->
        <button class="cart-btn" @click="$emit('open-cart')" aria-label="Open Shopping Cart">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="9" cy="21" r="1" />
            <circle cx="20" cy="21" r="1" />
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6" />
          </svg>
          <span class="cart-badge font-mono" v-if="cartCount > 0">{{ cartCount }}</span>
        </button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.navbar {
  position: sticky;
  top: 0;
  z-index: 100;
  background: rgba(9, 10, 15, 0.85);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid var(--line-dim);
}

.nav-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 4.25rem;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.brand-icon {
  width: 2.25rem;
  height: 2.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--accent-primary), var(--accent-hover));
  border-radius: var(--radius-md);
  color: #fff;
  box-shadow: 0 4px 12px var(--accent-glow);
}

.brand-info {
  display: flex;
  flex-direction: column;
}

.brand-title {
  font-weight: 800;
  font-size: 1.125rem;
  letter-spacing: -0.02em;
  color: #fff;
}

.brand-accent {
  color: var(--accent-cyan);
}

.brand-subtitle {
  font-size: 0.625rem;
  color: var(--text-dim);
  letter-spacing: 0.05em;
}

.nav-links {
  display: flex;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.03);
  padding: 0.25rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--line-dim);
}

.nav-tab {
  padding: 0.4rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--text-muted);
  border-radius: var(--radius-sm);
}

.nav-tab:hover {
  color: var(--text-main);
}

.nav-tab.active {
  background: var(--bg-elevated);
  color: #fff;
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.nav-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.status-pill {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.3rem 0.6rem;
  border-radius: var(--radius-full);
  font-size: 0.6875rem;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid var(--line-dim);
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
}

.status-pill.online {
  border-color: rgba(16, 185, 129, 0.3);
  color: #6ee7b7;
}

.status-pill.online .status-dot {
  background: #10b981;
  box-shadow: 0 0 8px #10b981;
}

.status-pill.offline {
  border-color: rgba(245, 158, 11, 0.3);
  color: #fcd34d;
}

.status-pill.offline .status-dot {
  background: #f59e0b;
}

.cart-btn {
  position: relative;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-elevated);
  border: 1px solid var(--line-dim);
  border-radius: var(--radius-md);
  color: var(--text-main);
}

.cart-btn:hover {
  background: rgba(255, 255, 255, 0.08);
  border-color: var(--line-bright);
}

.cart-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  min-width: 1.125rem;
  height: 1.125rem;
  padding: 0 3px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--accent-rose);
  color: #fff;
  border-radius: var(--radius-full);
  font-size: 0.625rem;
  font-weight: 700;
  border: 2px solid var(--bg-deep);
}
</style>
